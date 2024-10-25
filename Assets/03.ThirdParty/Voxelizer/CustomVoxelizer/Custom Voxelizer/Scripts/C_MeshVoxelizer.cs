using System.Collections.Generic;
using System.Linq;
//using MVoxelizer;
//using MVoxelizer.Util;
//using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.PlayerLoop;


public class C_MeshVoxelizer : MonoBehaviour
{
    public enum Precision
    {
        Low, Standard, High
    }

    public enum VoxelSizeType
    {
        Subdivision, AbsoluteSize
    }

    public enum GenerationType
    {
        SingleMesh, SeparateVoxels
    }

    public enum UVConversion
    {
        None, SourceMesh, VoxelMesh
    }

    public enum FillCenterMethod
    {
        ScanlineXAxis, ScanlineYAxis, ScanlineZAxis
    }
    
    public const int MAX_SUBDIVISION = 500;
    public const float SAMPLE_THRESHOLD = 0.05f;
    public const float EDGE_SMOOTHING_THRESHOLD = 0.90f;
    
    //settings
    public GameObject sourceGameObject = null;
    public VoxelSizeType voxelSizeType = VoxelSizeType.Subdivision;
    public Precision precision = Precision.Standard;
    public UVConversion uvConversion = UVConversion.SourceMesh;
    public bool edgeSmoothing = false;
    public bool applyScaling = false;
    public bool alphaCutoff = false;
    public float cutoffThreshold = 0.5f;
    // 몇 개의 서브 디비전으로 나눌 것인지
    // 수정
    public int subdivisionLevel = 30;
    public float absoluteVoxelSize = 10000;
    
    //voxel
    public bool modifyVoxel = false;
    public Mesh voxelMesh = null;
    public Vector3 voxelScale = Vector3.one;
    public Vector3 voxelRotation = Vector3.zero;
    
    //single mesh
    private bool instantiateResult = true;
    
    //seperate voxels
    // 수정
    public bool fillCenter = true;
    public FillCenterMethod fillMethod = FillCenterMethod.ScanlineYAxis;
    public Material centerMaterial = null;
    
    //voxelization data
    protected C_MVSource m_source;
    protected C_MVGrid m_grid;
    protected C_MVResult m_result;
    //protected C_MVOptimization m_opt;
    protected Dictionary<C_MVInt3, C_MVVoxel> voxelDict = new Dictionary<C_MVInt3, C_MVVoxel>();

    public GameObject VoxelizeMesh(bool instantiateResult)
    {
        this.instantiateResult = instantiateResult;
        GameObject go = VoxelizeMesh();
        this.instantiateResult = true;
        return go;
    }

    public virtual GameObject VoxelizeMesh()
    {
        if (sourceGameObject == null) return null;

        GameObject result = null;
        Clear();
        if (!Initialization())
        {
            Clear();
            return null;
        }
        if (!AnalyzeMesh())
        {
            Clear();
            return null;
        }
        if (!ProcessVoxelData())
        {
            Clear();
            return null;
        }

        List<Vector3> centerVoxels = new List<Vector3>();
        if (!FillCenterSpace(centerVoxels))
        {
            Clear();
            return null;
        }

        result = GenerateVoxels(centerVoxels);
        Clear();
        
        DestroyImmediate(sourceGameObject);
        return result;
    }

    protected virtual bool Initialization()
    {
        if (CancelProgress("Initializing... ", 0)) { return false; }

        m_source = new C_MVSource();
        m_source.Init(sourceGameObject);
        if (m_source.mesh == null) return false;

        m_grid = new C_MVGrid();
        m_grid.Init(this, m_source);

        if (voxelMesh == null) voxelMesh = C_MVHelper.GetDefaultVoxelMesh();
        if (centerMaterial == null) centerMaterial = C_MVHelper.GetDefaultVoxelMaterial();

        return true;
    }
    
    protected virtual bool AnalyzeMesh()
    {
        // 메시 분석 취소 여부 확인
        if (CancelProgress("Analyzing mesh... ", 0)) { return false; }
        int counter = 0;
        // 삼각형 개수 계산
        // 각 삼각형이 3개의 정점으로 이루어져 있어 전체 triangles 배열 길이를 3으로 나누어 총 삼각형 수를 구함.
        int total = m_source.mesh.triangles.Length / 3;
        // 진행상황 갱신 위해 삼각혛ㅇ 수 5% 해당하는 값 계산
        int rem = Mathf.CeilToInt(total * 0.05f);

        float triangleStep = 0.0f;
        // 정밀도 조정
        switch (precision)
        {
            case Precision.Low:
                triangleStep = m_grid.unitSize * 0.25f;
                break;
            case Precision.High:
                triangleStep = m_grid.unitSize * 0.0625f;
                break;
            default:
                triangleStep = m_grid.unitSize * 0.125f;
                break;
        }
            
        //메시에 여러개의 서브메시가 있을 수 있으므로 독립적인 부분으로 처리
        // 서브메시의 수를 확인해서 각 서브메시에서 삼각형으 ㅣ시작 인덱스와 끝 인덱스를 구함.
        for (int subMesh = 0; subMesh < m_source.mesh.subMeshCount; ++subMesh)
        {
            int start = (int)m_source.mesh.GetIndexStart(subMesh);
            int end = start + (int)m_source.mesh.GetIndexCount(subMesh);
            // 서브 메시 내 삼각형 순회
            for (int i = start; i < end; i += 3)
            {
                // counter가 rem의 배수일때마다 진행 상황 업데이트하고 취소조건 확인
                if (counter % rem == 0 && CancelProgress("Analyzing mesh... ", (float)counter / total)) { return false; }
                else counter++;

                //  삼각형 데이터 처리
                C_MVTriangleData tData = new C_MVTriangleData(m_source, m_grid, voxelDict, i, subMesh, triangleStep, applyScaling);
                tData.Scan();
            }
        }
        return true;
    }
    
    protected virtual bool ProcessVoxelData()
    {
        if (CancelProgress("Processing voxels... ", 0)) { return false; }

        float avg = voxelDict.First().Value.sampleCount;
        foreach (var v in voxelDict)
        {
            //각 복셀의 위치는 그리드의 원점을 기준으로 조정.
            v.Value.centerPos += m_grid.origin;
            avg = (avg + v.Value.sampleCount) * 0.5f;
            // voxel의 주변 voxel 연결 확인
            // 상하좌우 전후의 좌표에 대해 voxelDict에서 탐색하여 존재 여부를 확인, 그 결과를 현재 voxel의 필드(v_back, v_forwrad, v_left)에 저장.
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x,     v.Key.y,     v.Key.z - 1), out v.Value.v_back);
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x,     v.Key.y,     v.Key.z + 1), out v.Value.v_forward);
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x - 1, v.Key.y,     v.Key.z),     out v.Value.v_left);
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x + 1, v.Key.y,     v.Key.z),     out v.Value.v_right);
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x,     v.Key.y + 1, v.Key.z),     out v.Value.v_up);
            voxelDict.TryGetValue(new C_MVInt3(v.Key.x,     v.Key.y - 1, v.Key.z),     out v.Value.v_down);
        }

        Texture2D[] tex2D = null;
        // alphaCutoff가 활성화된 경우 
        if (alphaCutoff)
        {
            // 알파 컷 오프 처리 부분은 텍스처 알파 값을 기준으로 voxel을 필터링하는 과정
            // 특정 텍스처에서 알파 값이 낮아(투명할수록) 해당 부분의 voxel을 제거하거나 무시하도록 함.
            // 소스 오브젝트에 연결된 텍스쳐를 사용해 Texture2D 배열 생성
            tex2D = new Texture2D[m_source.materials.Length];
            for (int i = 0; i < m_source.materials.Length; ++i)
            {
                
                Texture tex = m_source.materials[i].mainTexture;
                tex2D[i] = tex == null ? null : C_MVHelper.GetTexture2D(tex);
            }
        }

        List<C_MVInt3> approxList = new List<C_MVInt3>();
        List<C_MVInt3> discardList = new List<C_MVInt3>();
        int minSample = Mathf.FloorToInt(avg * SAMPLE_THRESHOLD);
        float bound = m_grid.unitSize * 0.5f * EDGE_SMOOTHING_THRESHOLD;
        foreach (var v in voxelDict)
        {
            if (v.Value.sampleCount <= minSample)
            {
                discardList.Add(v.Key);
                continue;
            }

            // 각 voxel이 속한 subMesh의 UV좌표를 가져와 해당 위치의 알파 값을 검사 함.
            // tex2D[v.Value.subMesh] : Voxel 이 속한 subMeshㅇ ㅔ해당하는 텍스처 선택.
            if (tex2D != null && tex2D[v.Value.subMesh] != null)
            {
                // Voxel의 텍스처 좌표(UV 좌표)를 계산. 이 UV 좌표는 2D 텍스처 상에서 위치를 나타냄.
                Vector2 uv = m_source.GetUVCoord(v.Value);
                Color color = tex2D[v.Value.subMesh].GetPixel((int)(tex2D[v.Value.subMesh].width * uv.x), (int)(tex2D[v.Value.subMesh].height * uv.y));
                // 가져온 색상의 알파 값을 확ㅇ니해서 이 알파 값이 cutoffThreshold(임계값)이하인 경우 투명하다고 판단해 Voxel을 제거 대상으로 추가
                if (color.a <= cutoffThreshold)
                {
                    // 삭제할 목록에 추가
                    discardList.Add(v.Key);
                    continue;
                }
            }

            // edgeSmoothing이 활성화된 경우 
            if (edgeSmoothing)
            {
                // 엣지 근처 voxel이 특정 조건을 만족하면 approxlist에 추가
                bool replace = false;
                bool remove = true;
                if (v.Value.vertPos.z > bound && v.Value.back)
                {
                    replace = true;
                    remove = remove && v.Value.v_forward != null;
                }
                if (v.Value.vertPos.z < -bound && v.Value.forward)
                {
                    replace = true;
                    remove = remove && v.Value.v_back != null;
                }
                if (v.Value.vertPos.x > bound && v.Value.left)
                {
                    replace = true;
                    remove = remove && v.Value.v_right != null;
                }
                if (v.Value.vertPos.x < -bound && v.Value.right)
                {
                    replace = true;
                    remove = remove && v.Value.v_left != null;
                }
                if (v.Value.vertPos.y < -bound && v.Value.up)
                {
                    replace = true;
                    remove = remove && v.Value.v_down != null;
                }
                if (v.Value.vertPos.y > bound && v.Value.down)
                {
                    replace = true;
                    remove = remove && v.Value.v_up != null;
                }
                if (replace && remove)
                {
                    approxList.Add(v.Key);
                }
            }
        }

        // 불필요한 voxel 삭제 ?
        foreach (var v in discardList)
        {
            if (voxelDict[v].v_forward != null) { voxelDict[v].v_forward.v_back = null; }
            if (voxelDict[v].v_back != null) { voxelDict[v].v_back.v_forward = null; }
            if (voxelDict[v].v_left != null) { voxelDict[v].v_left.v_right = null; }
            if (voxelDict[v].v_right != null) { voxelDict[v].v_right.v_left = null; }
            if (voxelDict[v].v_up != null) { voxelDict[v].v_up.v_down = null; }
            if (voxelDict[v].v_down != null) { voxelDict[v].v_down.v_up = null; }
            voxelDict.Remove(v);
        }

        foreach (var v in approxList)
        {
            // 엣지 처리 ?
            if (voxelDict[v].v_forward != null) { voxelDict[v].v_forward.back = true; voxelDict[v].v_forward.v_back = null; }
            if (voxelDict[v].v_back != null) { voxelDict[v].v_back.forward = true; voxelDict[v].v_back.v_forward = null; }
            if (voxelDict[v].v_left != null) { voxelDict[v].v_left.right = true; voxelDict[v].v_left.v_right = null; }
            if (voxelDict[v].v_right != null) { voxelDict[v].v_right.left = true; voxelDict[v].v_right.v_left = null; }
            if (voxelDict[v].v_up != null) { voxelDict[v].v_up.down = true; voxelDict[v].v_up.v_down = null; }
            if (voxelDict[v].v_down != null) { voxelDict[v].v_down.up = true; voxelDict[v].v_down.v_up = null; }
            voxelDict.Remove(v);
        }

        discardList.Clear();
        approxList.Clear();
        if (tex2D != null) foreach (var v in tex2D) if (v != null) GameObject.DestroyImmediate(v);

        return true;
    }
    
    protected virtual bool FillCenterSpace(List<Vector3> centerVoxels)
        {
            if (CancelProgress("Filling center space... ", 0)) { return false; }

            switch (fillMethod)
            {
                case FillCenterMethod.ScanlineXAxis:
                    for (int y = 0; y < m_grid.unitCount.y; ++y)
                    {
                        for (int z = 0; z < m_grid.unitCount.z; ++z)
                        {
                            List<int> indice = new List<int>();
                            for (int x = 0; x < m_grid.unitCount.x; ++x)
                            {
                                C_MVInt3 pos = new C_MVInt3(x, y, z);
                                if (voxelDict.ContainsKey(pos)) indice.Add(x);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                C_MVInt3 start = new C_MVInt3(indice[i], y, z);
                                if (voxelDict[start].right) continue;
                                C_MVInt3 end = new C_MVInt3(indice[i + 1], y, z);
                                if (voxelDict[end].left) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = m_grid.unitSize * (j + 0.5f);
                                    pos.y = m_grid.unitSize * (y + 0.5f);
                                    pos.z = m_grid.unitSize * (z + 0.5f);
                                    pos += m_grid.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                case FillCenterMethod.ScanlineYAxis:
                    for (int z = 0; z < m_grid.unitCount.z; ++z)
                    {
                        for (int x = 0; x < m_grid.unitCount.x; ++x)
                        {
                            List<int> indice = new List<int>();
                            for (int y = 0; y < m_grid.unitCount.y; ++y)
                            {
                                C_MVInt3 pos = new C_MVInt3(x, y, z);
                                if (voxelDict.ContainsKey(pos)) indice.Add(y);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                C_MVInt3 start = new C_MVInt3(x, indice[i], z);
                                if (voxelDict[start].up) continue;
                                C_MVInt3 end = new C_MVInt3(x, indice[i + 1], z);
                                if (voxelDict[end].down) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = m_grid.unitSize * (x + 0.5f);
                                    pos.y = m_grid.unitSize * (j + 0.5f);
                                    pos.z = m_grid.unitSize * (z + 0.5f);
                                    pos += m_grid.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                case FillCenterMethod.ScanlineZAxis:
                    for (int x = 0; x < m_grid.unitCount.x; ++x)
                    {
                        for (int y = 0; y < m_grid.unitCount.y; ++y)
                        {
                            List<int> indice = new List<int>();
                            for (int z = 0; z < m_grid.unitCount.z; ++z)
                            {
                                C_MVInt3 pos = new C_MVInt3(x, y, z);
                                if (voxelDict.ContainsKey(pos)) indice.Add(z);
                            }
                            for (int i = 0; i < indice.Count - 1; ++i)
                            {
                                C_MVInt3 start = new C_MVInt3(x, y, indice[i]);
                                if (voxelDict[start].forward) continue;
                                C_MVInt3 end = new C_MVInt3(x, y, indice[i + 1]);
                                if (voxelDict[end].back) continue;
                                for (int j = indice[i] + 1; j < indice[i + 1]; ++j)
                                {
                                    Vector3 pos = new Vector3();
                                    pos.x = m_grid.unitSize * (x + 0.5f);
                                    pos.y = m_grid.unitSize * (y + 0.5f);
                                    pos.z = m_grid.unitSize * (j + 0.5f);
                                    pos += m_grid.origin;
                                    centerVoxels.Add(pos);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return true;
        }
    
    protected virtual GameObject GenerateVoxels(List<Vector3> centerVoxels)
    {
        // 생성 과정 중단할지 확인
        if (CancelProgress("Generating voxels... ", 0)) { return null; }
        // 현재 진행중인 복셀 개수
        int counter = 0;
        // 총 복셀 개수 나타냄.
        int total = voxelDict.Count;
        // 진행 상태 업데이트할 간격 : 전체 복셀 개수 5% 마다 한번씩 상태 확인.
        int rem = Mathf.CeilToInt(total * 0.05f);

        // 새로운 게임 오브젝트 생성 후 VoxelGroup 이라는 컴포넌트 추가
        C_VoxelGroup voxelGroup = new GameObject(sourceGameObject.name + " Voxels").AddComponent<C_VoxelGroup>();
        voxelGroup.voxelMesh = voxelMesh;
        voxelGroup.ratio = modifyVoxel ? m_grid.unitVoxelRatio.x / voxelScale.x : m_grid.unitVoxelRatio.x;
        voxelGroup.voxelScale = modifyVoxel ? voxelScale : Vector3.one;
        voxelGroup.voxelRotation = modifyVoxel ? voxelRotation : Vector3.zero;
        voxelGroup.uvType = uvConversion;
        voxelGroup.voxelMaterials = m_source.materials;
        // voxelDict 만큼 크기 배열을 할당하여 voxel들의 정보를 저장할 공간을 마련.
        voxelGroup.voxels = new C_Voxel[voxelDict.Count];
        voxelGroup.voxelPosition = new Vector3[voxelDict.Count];
        voxelGroup.submesh = new int[voxelDict.Count];
        if (uvConversion == UVConversion.SourceMesh)
        {
            voxelGroup.uvs = new Vector2[voxelDict.Count];
        }
        // fillCenter 옵션이 켜져 있으면
        if (fillCenter)
        {
            // 중앙 voxel 머티리얼과 위치 정보 설정.
            voxelGroup.centerMaterial = centerMaterial;
            // 리스트를 배열로 변경
            voxelGroup.centerVoxelPosition = centerVoxels.ToArray();
            // 각 중앙 복셀에 대한 게임 오브젝트 배열도 초기화.
            voxelGroup.centerVoxels = new GameObject[centerVoxels.Count];
        }

        int temp = 0;
        // voxelDict에 저장된 각 voxel에 대해 반복문 실행
        foreach (C_MVVoxel voxel in voxelDict.Values)
        {
            // 진행 상태를 5%마다 확인해서 중단할지 결정.
            if (counter % rem == 0 && CancelProgress("Generating voxels... ", (float)counter / total)) { return null; }
            else counter++;
            // 각 voxel의 중심 위치와 서브 메쉬 정보를 voxelGroup에 저장.
            voxelGroup.voxelPosition[temp] = voxel.centerPos;
            voxelGroup.submesh[temp] = voxel.subMesh;
            // uv 타입이 SourceMesh이면 각 복셀의 UV 좌표도 계산하여 저장.
            if (uvConversion == UVConversion.SourceMesh) voxelGroup.uvs[temp] = m_source.GetUVCoord(voxel);
            ++temp;
        }

        // 복셀 그룹 재구성
        voxelGroup.RebuildVoxels();
        // 생성된 voxelGroup의 부모 오브젝트를 원래 soruceGameobject의 부모로 설정.
        voxelGroup.transform.parent = sourceGameObject.transform.parent;
        voxelGroup.transform.SetSiblingIndex(sourceGameObject.transform.GetSiblingIndex() + 1);
        // 위치, 스케일, 회전을 원래 오브젝트와 동일하게 맞춤.
        voxelGroup.transform.localPosition = sourceGameObject.transform.localPosition;
        // 스케일 적용 여부는 applyScaling 옵션에 따라 달라지며 꺼져 있으면 원래 스케일을 그대로 사용.
        if (!applyScaling) voxelGroup.transform.localScale = sourceGameObject.transform.localScale;
        voxelGroup.transform.localRotation = sourceGameObject.transform.localRotation;

        return voxelGroup.gameObject;
    }
    void Clear()
    {
        m_source = null;
        m_grid = null;
        m_result = null;
        //m_opt = null;
        voxelDict.Clear();
    }
    
    protected virtual bool CancelProgress(string msg, float value) { return false; }
}
