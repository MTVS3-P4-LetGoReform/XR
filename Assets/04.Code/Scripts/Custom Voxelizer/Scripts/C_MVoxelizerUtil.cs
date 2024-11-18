using System.Collections.Generic;
using MVoxelizer.Util;
using UnityEngine;

public static class C_MVMathLib
{
    /* 3D 공간에서 선과 삼각형이 교차하는지를 확인하는 충돌 감지 알고리즘 */
    public static bool LineIntersectsTriangle(Vector3 linePoint, Vector3 lineDir, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 intersection)
    {
        intersection = new Vector3();

        // Find the normal of the triangle
        Vector3 u = v1 - v0;
        Vector3 v = v2 - v0;
        // 법선 벡터 계산 : 삼각형 두 변으로부터 평면의 법선 벡터를 계산.
        Vector3 normal = Vector3.Cross(u, v);

        // Check if line and plane are parallel
        // 평행 여부 확인을 위한
        float dot = Vector3.Dot(normal, lineDir);
        // 선과 평면이 평행하면 교차하지 않으므로 false 반환
        if (Mathf.Abs(dot) < 1e-8) return false;

        // Compute the distance along the line to the intersection point
        // 교차점까지의 거리 계산 : 선이 평면과 교차하는 경우, 선을 따라 이동했을 때 교차점까지의 거리를 계산.
        // 삼각형의 첫 번째 v0와 선의 시작점 linepoint간의 벡터를 사용하여 선이 평면과 교차하는 지점까지의 거리.
        float d = Vector3.Dot(normal, v0 - linePoint) / dot;
        // 교차점 계산: 위에서 구한 거리 d를 선의 방향 벡터 lineDir에 곱하여 교차점(intersection)의 위치를 구합니다. 교차점은 linePoint에서 d만큼 이동한 위치에 있습니다.
        intersection = linePoint + d * lineDir;

        // Check if the intersection point lies inside the triangle
        // 삼각형 내부에 있는지 확인
        return IsPointInTriangle(intersection, v0, v1, v2);
    }

    public static bool LineIntersectsTriangle(Vector3 linePoint, Vector3 lineDir, Vector3 normal, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 intersection)
    {
        intersection = new Vector3();

        // Check if line and plane are parallel
        float dot = Vector3.Dot(normal, lineDir);
        if (Mathf.Abs(dot) < 1e-8) return false;

        // Compute the distance along the line to the intersection point
        float d = Vector3.Dot(normal, v0 - linePoint) / dot;
        intersection = linePoint + d * lineDir;

        // Check if the intersection point lies inside the triangle
        return IsPointInTriangle(intersection, v0, v1, v2);
    }

    /* 한 점 p가 주어진 삼각형 내부에 있는지 확인하는 바리센트릭 좌표 기반의 알고리즘.*/
    // 사용 경우 : 1. 충돌 감지
    // 2. 광선과 메쉬 교차 검사(레이 트레이싱)
    // 3. 텍스처 매핑
    // 4. 물리 시뮬레이션
    // 5. 모델링 및 애니메이션
    // 6. 프로그래밍 최적화( 그리드 기반 처리 등 LOD) 
    public static bool IsPointInTriangle(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Compute vectors
        Vector3 v0v1 = v1 - v0;
        Vector3 v0v2 = v2 - v0;
        Vector3 v0p = p - v0;

        // Compute dot products
        float dot00 = Vector3.Dot(v0v2, v0v2);
        float dot01 = Vector3.Dot(v0v2, v0v1);
        float dot02 = Vector3.Dot(v0v2, v0p);
        float dot11 = Vector3.Dot(v0v1, v0v1);
        float dot12 = Vector3.Dot(v0v1, v0p);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
}
    
public static class C_MVHelper
{
    /* Default Mesh 가져오기 */
    public static Mesh GetDefaultVoxelMesh()
    {
        Mesh m = (Mesh)Resources.Load("DefaultVoxelCube", typeof(Mesh));
        return m;
    }

    /* Default material 가져오기*/
    public static Material GetDefaultVoxelMaterial()
    {
        // 기본 프리미티브 객체(1x1 크기 2D평면 사각하여)를 생성
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        // 사각형에 할당된 공유 머티리얼 가져옴.
        Material m = go.GetComponent<MeshRenderer>().sharedMaterial;
        // go 객체 즉시 삭제. 머티리얼 m 은 남아있음.
        GameObject.DestroyImmediate(go);
        return m;
    }
    
    
    public static Texture2D GetTexture2D(Texture tex)
    {
        // tex와 동일한 크기 새로운 Texture2D 객체 생성. 변환된 텍스처 저장될 공간.
        Texture2D texture2D = new Texture2D(tex.width, tex.height);
        // tex오 ㅏ같은 크기 임시 렌더 텍스처 생성.
        // RenderTexture : GPU 상에서 텍스처를 다룰 수 있는 메모리 버퍼
        // 텍스처를 렌더링하거나 복사할때 사용.
        // 32 : 32bit 깊이 버퍼 사용.
        RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height, 32);
        // 주어진 텍스처를 렌더 텍스처로 복사
        // Blit은 그래픽스 프로세싱을 통해 한 텍스처를 다른 텍스처로 빠르게 전송하는 기능.
        Graphics.Blit(tex, rt);
        // 현재 활성화된 렌더 텍스처를 가져옴.
        RenderTexture curr = RenderTexture.active;
        RenderTexture.active = rt;
        // ReadPixels 함수는 현재 활성화된 RenderTexture로부터 픽셀 데이터를 일거와 Texture2D 객체에 저장.
        // 첫번째 인자 : Rect 범위 내의 픽셀을 읽어 옴.
        // 두 번째, 세번째 ㅇ니자 : Texture2D의 어느 위치에 데이터를 복사할지 나타냄. (0, 0)은 시작좌표
        texture2D.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        // 텍스처에 적용된 모든 변경사항을 .l5ure.active = curr;
        RenderTexture.ReleaseTemporary(rt);
        return texture2D;
    }
}

public  class C_MVInt3
{
    public int x, y, z;

    public C_MVInt3() : this(0, 0, 0) { }

    public C_MVInt3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
        C_MVInt3 p = (C_MVInt3)obj;
        return (x == p.x) && (y == p.y) && (z == p.z);
    }

    /* 객체 고유 해쉬코드 생성 */
    public override int GetHashCode()
    {
        return ShiftAndWrap(x.GetHashCode(), 4) ^ ShiftAndWrap(y.GetHashCode(), 2) ^ z.GetHashCode();
    }

    /* 비트 시프트 연산 */
    public int ShiftAndWrap(int value, int positions)
    {
        // postions 값을 0~31 범위 내에서 제한하기 위한 연산
        positions = positions & 0x1F;

        uint number = System.BitConverter.ToUInt32(System.BitConverter.GetBytes(value), 0);
        uint wrapped = number >> (32 - positions);
        return System.BitConverter.ToInt32(System.BitConverter.GetBytes((number << positions) | wrapped), 0);
    }

    /* 객체를 문자열로 변호나 */
    public override string ToString()
    {
        return System.String.Format("({0}, {1}, {2})", x, y, z);
    }
}

public class C_MVVoxel
{
    // Voxel의 중심 좌표
    public Vector3 centerPos;
    // Voxel의 정점 좌표
    public Vector3 vertPos;
    // 이 Voxel이 포함된 메쉬의 삼각형 index
    public int index;
    // subMesh 인덱스
    public int subMesh;
    // Voxel이 차지하는 공간의 비율을 나타내는 벡터
    public Vector3 ratio;
    // 정점 수를 나타내는 변수
    public int verticeCount = 0;
    // 샘플링된 횟수를 나타내는 ㅂ녀수
    public int sampleCount = 0;
    // 다른 Voxel과의 연결 정보를 저장하는 변수들
    public C_MVVoxel v_forward = null;
    public C_MVVoxel v_up = null;
    public C_MVVoxel v_back = null;
    public C_MVVoxel v_down = null;
    public C_MVVoxel v_left = null;
    public C_MVVoxel v_right = null;
    // 각 방향에 대해 해당 Voxel이 존재하는지 여부를 나타내는 불리언 값.
    public bool forward = false;
    public bool up = false;
    public bool back = false;
    public bool down = false;
    public bool left = false;
    public bool right = false;

    // 입력받은 normal 벡터를 기반으로, 해당 Voxel이 어느 방향으로 면이 있는지 업데이트
    public void UpdateNormal(Vector3 normal)
    {
        back = back || normal.z <= 0.0f;
        forward = forward || normal.z >= 0.0f;
        left = left || normal.x <= 0.0f;
        right = right || normal.x >= 0.0f;
        up = up || normal.y >= 0.0f;
        down = down || normal.y <= 0.0f;
    }
}

// 메쉬 소스와 관련된 정보를 담고 있음.
// 주로 3D 모델의 메쉬와 메터리얼(재질)을 다루는 클래스
public class C_MVSource
{
    // 소스 오브젝트의 Transform 컴포넌트
    public Transform transform = null;
    // 소스 오브젝트의 mesh(3D 모델의 메쉬데이터)
    public Mesh mesh = null;
    // 소스 오브젝트의 Material 배열
    public Material[] materials = null;
    // 소스 오브젝트의 MeshFilter 컴포넌트
    public MeshFilter meshFilter = null;
    // 소스 오브젝트의 MeshRenderer 컴포넌트
    public MeshRenderer meshRenderer = null;
    // 스키닝된 메쉬일 경우 컴포넌트
    public SkinnedMeshRenderer skinnedMeshRenderer = null;
    
    // 소스 오브젝트 초기화
    public void Init(GameObject sourceGameObject)
    {
        transform = sourceGameObject.transform;
        skinnedMeshRenderer = sourceGameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            mesh = sourceGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            materials = sourceGameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
        }
        else
        {
            meshFilter = sourceGameObject.GetComponent<MeshFilter>();
            meshRenderer = sourceGameObject.GetComponent<MeshRenderer>();
            if (meshFilter != null) mesh = meshFilter.sharedMesh;
            if (meshRenderer != null) materials = meshRenderer.sharedMaterials;
        }
    }

    /* Voxel에 대한 UV좌표 계산하여 반환 */
    // 메쉬의 각 삼각형에 속하는 정점들의 UV값을 가중치로 계산해 반환
    public Vector2 GetUVCoord(C_MVVoxel voxel)
    {
        Vector2 p0 = mesh.uv[mesh.triangles[voxel.index]];
        Vector2 p1 = mesh.uv[mesh.triangles[voxel.index + 1]];
        Vector2 p2 = mesh.uv[mesh.triangles[voxel.index + 2]];
        float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;
        Vector2 uv = p0 * (voxel.ratio.x / sum) + p1 * (voxel.ratio.y / sum) + p2 * (voxel.ratio.z / sum);
        return uv;
    }

    public Vector2 GetUV2Coord(C_MVVoxel voxel)
    {
        Vector2 p0 = mesh.uv2[mesh.triangles[voxel.index]];
        Vector2 p1 = mesh.uv2[mesh.triangles[voxel.index + 1]];
        Vector2 p2 = mesh.uv2[mesh.triangles[voxel.index + 2]];
        float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;
        Vector2 uv = p0 * (voxel.ratio.x / sum) + p1 * (voxel.ratio.y / sum) + p2 * (voxel.ratio.z / sum);
        return uv;
    }

    public Vector2 GetUV3Coord(C_MVVoxel voxel)
    {
        Vector2 p0 = mesh.uv3[mesh.triangles[voxel.index]];
        Vector2 p1 = mesh.uv3[mesh.triangles[voxel.index + 1]];
        Vector2 p2 = mesh.uv3[mesh.triangles[voxel.index + 2]];
        float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;
        Vector2 uv = p0 * (voxel.ratio.x / sum) + p1 * (voxel.ratio.y / sum) + p2 * (voxel.ratio.z / sum);
        return uv;
    }

    public Vector2 GetUV4Coord(C_MVVoxel voxel)
    {
        Vector2 p0 = mesh.uv4[mesh.triangles[voxel.index]];
        Vector2 p1 = mesh.uv4[mesh.triangles[voxel.index + 1]];
        Vector2 p2 = mesh.uv4[mesh.triangles[voxel.index + 2]];
        float sum = voxel.ratio.x + voxel.ratio.y + voxel.ratio.z;
        Vector2 uv = p0 * (voxel.ratio.x / sum) + p1 * (voxel.ratio.y / sum) + p2 * (voxel.ratio.z / sum);
        return uv;
    }
}

/* 그리드 기반 좌표계 관련 클래스 */
public class C_MVGrid
{
    // 그리드의 한 셀(단위)의 크기
    public float unitSize = 1.0f;

    // 그리드의 X, Y, Z 축에 대한 셀 개수를 저장.
    public C_MVInt3 unitCount = new C_MVInt3();

    // 각 셀에 대한 복셀 크기 비율.
    public Vector3 unitVoxelRatio = Vector3.one;

    // 복셀 회전 정보 저장
    public Quaternion voxelRotation = Quaternion.identity;

    // 그리드 시작 좌표.
    public Vector3 origin = Vector3.zero;

    // 3D 좌표 p를 그리드 좌표계로 변환
    public C_MVInt3 GetGridCoordinate(Vector3 p)
    {
        C_MVInt3 pos = new C_MVInt3();
        // 그리드 셀 좌표 변환
        pos.x = Mathf.FloorToInt(p.x / unitSize);
        pos.y = Mathf.FloorToInt(p.y / unitSize);
        pos.z = Mathf.FloorToInt(p.z / unitSize);
        return pos;
    }

    // 실제 3D 공간상의 좌표계 변환
    public Vector3 GetPosition(C_MVInt3 coord)
    {
        // 셀의 중심 좌표 반환
        Vector3 pos = new Vector3();
        pos.x = unitSize * (coord.x + 0.5f);
        pos.y = unitSize * (coord.y + 0.5f);
        pos.z = unitSize * (coord.z + 0.5f);
        return pos;
    }

    // 주어진 정점 좌표 v에 복셀 크기 비율 적용 및 회전 변환 거친 후 오프셋 더함.
    public Vector3 GetVertex(Vector3 v, Vector3 offset)
    {
        Vector3 vert = v;
        vert.x *= unitVoxelRatio.x;
        vert.y *= unitVoxelRatio.y;
        vert.z *= unitVoxelRatio.z;
        vert = voxelRotation * vert;
        return vert + offset;
    }

    // 그리드 초기화 함수
    public void Init(C_MeshVoxelizer voxelizer, C_MVSource source)
    {
        // 메쉬의 경계 박스 크기를 나타내는 Vector3 메쉬가 차지하는 3D 공간에서 X, Y, Z축 방향으로 얼마나 큰지.
        Vector3 sourceMeshSize = source.mesh.bounds.size;
        Vector3 sourceMeshMin = source.mesh.bounds.min;
        // if (voxelizer.applyScaling)
        // {
        //     sourceMeshSize.x *= source.transform.lossyScale.x;
        //     sourceMeshSize.y *= source.transform.lossyScale.y;
        //     sourceMeshSize.z *= source.transform.lossyScale.z;
        //     sourceMeshMin.x  *= source.transform.lossyScale.x;
        //     sourceMeshMin.y  *= source.transform.lossyScale.y;
        //     sourceMeshMin.z  *= source.transform.lossyScale.z;
        // }

        // 주어진 메쉬의 가장 큰 경계 박스 크기를 구함. 세 축 중 가장 큰 값을 선택하여 maxBBoxSize에 저장.
        float maxBBoxSize = Mathf.Max(sourceMeshSize.x, sourceMeshSize.y, sourceMeshSize.z);
        // Subdivision 방식 : 메쉬의 크기를 일정한 서브디비전 레벨로 나누어 그리드의 셀 크기를 결정.
        if (voxelizer.voxelSizeType == C_MeshVoxelizer.VoxelSizeType.Subdivision)
        {
            // 최대, 최소 usbdivisonLevel 제한
            voxelizer.subdivisionLevel = Mathf.Clamp(voxelizer.subdivisionLevel, 1, C_MeshVoxelizer.MAX_SUBDIVISION);
            // 한 그리드의 셀 크기 계싼
            unitSize = maxBBoxSize / voxelizer.subdivisionLevel;
        }
        else
        {
            // 절대 복셀 크기 방식
            voxelizer.absoluteVoxelSize = Mathf.Clamp(voxelizer.absoluteVoxelSize,
                maxBBoxSize / C_MeshVoxelizer.MAX_SUBDIVISION, maxBBoxSize);
            unitSize = voxelizer.absoluteVoxelSize;
        }

        // 미세 조정 : 부동소수점 오차로 인해 그리드 셀 크기 계산에서 발생할 수 있는 문제 방지
        unitSize *= 1.00001f;

        // 올림. 나눈 값이 소수점일 경우 더큰 정수로 변환하여 그리드 셀의 개수 보장
        unitCount.x = Mathf.CeilToInt(sourceMeshSize.x / unitSize);
        unitCount.y = Mathf.CeilToInt(sourceMeshSize.y / unitSize);
        unitCount.z = Mathf.CeilToInt(sourceMeshSize.z / unitSize);
        // 매우 작은 메쉬 크기에서도 그리드 셀이 최소 하나는 존재하도록 보장
        if (unitCount.x == 0) unitCount.x = 1;
        if (unitCount.y == 0) unitCount.y = 1;
        if (unitCount.z == 0) unitCount.z = 1;

        // if (voxelizer.modifyVoxel)
        // {
        //     unitVoxelRatio.x = unitSize * voxelizer.voxelScale.x / voxelizer.voxelMesh.bounds.size.x;
        //     unitVoxelRatio.y = unitSize * voxelizer.voxelScale.y / voxelizer.voxelMesh.bounds.size.y;
        //     unitVoxelRatio.z = unitSize * voxelizer.voxelScale.z / voxelizer.voxelMesh.bounds.size.z;
        //     voxelRotation = Quaternion.Euler(voxelizer.voxelRotation);
        // }
        // else
        //{
        voxelizer.voxelMesh = C_MVHelper.GetDefaultVoxelMesh();
        // 복셀 스케일 적용X라서 셀 크기를 복셀 메쉬 크기로 나눔.
        unitVoxelRatio.x = unitSize / voxelizer.voxelMesh.bounds.size.x;
        unitVoxelRatio.y = unitSize / voxelizer.voxelMesh.bounds.size.y;
        unitVoxelRatio.z = unitSize / voxelizer.voxelMesh.bounds.size.z;
        //}

        // offset :메쉬 크기와 그리드 셀 크기 간 차이 보정하기 위한 값.
        Vector3 offset = new Vector3();
        offset.x = sourceMeshSize.x > unitSize ? 0.0f : (unitSize - sourceMeshSize.x) * 0.5f;
        offset.y = sourceMeshSize.y > unitSize ? 0.0f : (unitSize - sourceMeshSize.y) * 0.5f;
        offset.z = sourceMeshSize.z > unitSize ? 0.0f : (unitSize - sourceMeshSize.z) * 0.5f;
        origin = sourceMeshMin - offset;
    }
}

public struct C_MVTriangleData
    {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 normal;
        public Vector3 normalABS;
        public Vector3 v01;
        public Vector3 v12;
        public Vector3 v20;
        public Bounds bound;
        public int index;
        public int subMesh;
        float triangleStep;

        C_MVSource m_source;
        C_MVGrid m_grid;
        Dictionary<C_MVInt3, C_MVVoxel> voxelDict;

        public C_MVTriangleData(C_MVSource source, C_MVGrid grid, Dictionary<C_MVInt3, C_MVVoxel> vDict, int index, int subMesh, float triangleStep, bool applyScaling)
        {
            // 메시 정보
            m_source = source;
            // 그리드 정보
            m_grid = grid;
            // 분석 결과를 저장하는
            voxelDict = vDict;
            // 삼각형 인덱스
            this.index = index;
            this.subMesh = subMesh;
            this.triangleStep = triangleStep;

            // 삼각형 세 정 점 좌표
            p0 = m_source.mesh.vertices[m_source.mesh.triangles[index]];
            p1 = m_source.mesh.vertices[m_source.mesh.triangles[index + 1]];
            p2 = m_source.mesh.vertices[m_source.mesh.triangles[index + 2]];
            // if (applyScaling)
            // {
            //     p0.x *= m_source.transform.lossyScale.x;
            //     p0.y *= m_source.transform.lossyScale.y;
            //     p0.z *= m_source.transform.lossyScale.z;
            //     p1.x *= m_source.transform.lossyScale.x;
            //     p1.y *= m_source.transform.lossyScale.y;
            //     p1.z *= m_source.transform.lossyScale.z;
            //     p2.x *= m_source.transform.lossyScale.x;
            //     p2.y *= m_source.transform.lossyScale.y;
            //     p2.z *= m_source.transform.lossyScale.z;
            // }

            v01 = p1 - p0;
            v12 = p2 - p1;
            v20 = p0 - p2;

            normal = Vector3.Cross(v01, -v20).normalized;
            normalABS = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
            if (normalABS.x < 0.00001f) normal.x = 0.0f;
            if (normalABS.y < 0.00001f) normal.y = 0.0f;
            if (normalABS.z < 0.00001f) normal.z = 0.0f;

            Vector3 min = new Vector3();
            Vector3 max = new Vector3();
            min.x = Mathf.Min(p0.x, p1.x, p2.x);
            min.y = Mathf.Min(p0.y, p1.y, p2.y);
            min.z = Mathf.Min(p0.z, p1.z, p2.z);
            max.x = Mathf.Max(p0.x, p1.x, p2.x);
            max.y = Mathf.Max(p0.y, p1.y, p2.y);
            max.z = Mathf.Max(p0.z, p1.z, p2.z);
            bound = new Bounds((min + max) * 0.5f, max - min);
        }

        public void Scan()
        {
            if (normalABS.x > normalABS.y && normalABS.x > normalABS.z)
            {
                Vector3 center = bound.center - new Vector3(bound.extents.x + 1.0f, 0.0f, 0.0f);
                Vector3 rayDir = Vector3.right;
                float w = 0.0f;
                while (w <= bound.extents.z)
                {
                    float h = 0.0f;
                    while (h <= bound.extents.y)
                    {
                        Vector3 rayPoint1 = center + new Vector3(0.0f,  h,  w);
                        Vector3 rayPoint2 = center + new Vector3(0.0f, -h,  w);
                        Vector3 rayPoint3 = center + new Vector3(0.0f, -h, -w);
                        Vector3 rayPoint4 = center + new Vector3(0.0f,  h, -w);

                        CheckRay(rayPoint1, rayDir);
                        CheckRay(rayPoint2, rayDir);
                        CheckRay(rayPoint3, rayDir);
                        CheckRay(rayPoint4, rayDir);

                        h += triangleStep;
                    }
                    w += triangleStep;
                }
            }
            else if (normalABS.y > normalABS.x && normalABS.y > normalABS.z)
            {
                Vector3 center = bound.center - new Vector3(0.0f, bound.extents.y + 1.0f, 0.0f);
                Vector3 rayDir = Vector3.up;
                float w = 0.0f;
                while (w <= bound.extents.x)
                {
                    float h = 0.0f;
                    while (h <= bound.extents.z)
                    {
                        Vector3 rayPoint1 = center + new Vector3( w, 0.0f,  h);
                        Vector3 rayPoint2 = center + new Vector3(-w, 0.0f,  h);
                        Vector3 rayPoint3 = center + new Vector3(-w, 0.0f, -h);
                        Vector3 rayPoint4 = center + new Vector3( w, 0.0f, -h);

                        CheckRay(rayPoint1, rayDir);
                        CheckRay(rayPoint2, rayDir);
                        CheckRay(rayPoint3, rayDir);
                        CheckRay(rayPoint4, rayDir);

                        h += triangleStep;
                    }
                    w += triangleStep;
                }
            }
            else
            {
                Vector3 center = bound.center - new Vector3(0.0f, 0.0f, bound.extents.z + 1.0f);
                Vector3 rayDir = Vector3.forward;
                float w = 0.0f;
                while (w <= bound.extents.x)
                {
                    float h = 0.0f;
                    while (h <= bound.extents.y)
                    {
                        Vector3 rayPoint1 = center + new Vector3( w,  h, 0.0f);
                        Vector3 rayPoint2 = center + new Vector3(-w,  h, 0.0f);
                        Vector3 rayPoint3 = center + new Vector3(-w, -h, 0.0f);
                        Vector3 rayPoint4 = center + new Vector3( w, -h, 0.0f);

                        CheckRay(rayPoint1, rayDir);
                        CheckRay(rayPoint2, rayDir);
                        CheckRay(rayPoint3, rayDir);
                        CheckRay(rayPoint4, rayDir);

                        h += triangleStep;
                    }
                    w += triangleStep;
                }
            }
        }

        // 특정 점에서 시작하는 광선이 삼각형과 교차ㅎ하는지 확인
        void CheckRay(Vector3 rayPoint, Vector3 rayDir)
        {
            //  교차 여부 판단
            if (C_MVMathLib.LineIntersectsTriangle(rayPoint, rayDir, normal, p0, p1, p2, out Vector3 p))
            {
                // 교차하는 경우 교차점 p와 그 비율 ratio 계산
                Vector3 ratio = new Vector3(Vector3.Cross((p - p1), v12).magnitude, Vector3.Cross((p - p2), v20).magnitude, Vector3.Cross((p - p0), v01).magnitude);
                // 교차점 p가 어느 그리드 좌표에 해당하는지 확인 후, 맞는 voxel을 찾음.
                CheckPoint(p, ratio);
            }
        }

        void CheckPoint(Vector3 p, Vector3 ratio)
        {
            p -= m_grid.origin;
            C_MVVoxel voxel;
            C_MVInt3 pos = m_grid.GetGridCoordinate(p);
            //만약 voxelDict에 이미 존재하는 voxel이 있으면 그 값 업데이트 없으면 새로운 voxel 생성해서 딕셔너리에 추가
            if (voxelDict.TryGetValue(pos, out voxel))
            {
                Vector3 v = p - voxelDict[pos].centerPos;
                if (v.sqrMagnitude < voxelDict[pos].vertPos.sqrMagnitude)
                {
                    voxelDict[pos].vertPos = v;
                    voxelDict[pos].ratio = ratio;
                    voxelDict[pos].index = index;
                    voxelDict[pos].subMesh = subMesh;
                }
                voxelDict[pos].sampleCount++;
                voxelDict[pos].UpdateNormal(normal);
            }
            else
            {
                voxel = new C_MVVoxel();
                voxel.centerPos = m_grid.GetPosition(pos);
                voxel.vertPos = p - voxel.centerPos;
                voxel.ratio = ratio;
                voxel.index = index;
                voxel.subMesh = subMesh;
                voxel.sampleCount = 1;
                voxel.UpdateNormal(normal);
                voxelDict.Add(pos, voxel);
            }
        }
    }

public class C_MVResult
{
    // 메시 정점 저장
    public List<Vector3> vertices = new List<Vector3>();
    // 삼각형 정보 각 삼각형은 세 개의 정점 인덱스를 가짐
    public List<List<int>> triangles = new List<List<int>>();
   // 각 정점에 대한 법선 벡터 저장
    public List<Vector3> normals = new List<Vector3>();
    //  텍스처 좌표, 메시에서 여러 텍스처 레이어
    public List<Vector2> uv = new List<Vector2>();
    public List<Vector2> uv2 = new List<Vector2>();
    public List<Vector2> uv3 = new List<Vector2>();
    public List<Vector2> uv4 = new List<Vector2>();
    public List<BoneWeight> boneWeights = new List<BoneWeight>();
    // 복셀화한 메시 객체를 저장
    public Mesh voxelizedMesh = null;
    // 복셀화된 메시에 사용할 재질들
    public Material[] voxelizedMaterials = null;
    // 그리드 객체
    public C_MVGrid grid = null;
    // 복셀 메시 데이터
    public Mesh voxelMesh = null;

    public void Init(int subMeshCount)
    {
        for (int i = 0; i < subMeshCount; ++i) triangles.Add(new List<int>());
    }

    // 특정 면 정점 데이터 추가
    public void AddFaceVertices(MVVoxel voxel, int vIndex, int index)
    {
        int v = 4 * vIndex;
        int t = 6 * vIndex;
        index = index - v + voxel.verticeCount;
        // voxelMesh에서 정점, 법선 4개를 가져와 그리드 좌표로 변환 후 리스트에 추가
        vertices.Add(grid.GetVertex(voxelMesh.vertices[v + 0], voxel.centerPos));
        vertices.Add(grid.GetVertex(voxelMesh.vertices[v + 1], voxel.centerPos));
        vertices.Add(grid.GetVertex(voxelMesh.vertices[v + 2], voxel.centerPos));
        vertices.Add(grid.GetVertex(voxelMesh.vertices[v + 3], voxel.centerPos));
        normals.Add(voxelMesh.normals[v + 0]);
        normals.Add(voxelMesh.normals[v + 1]);
        normals.Add(voxelMesh.normals[v + 2]);
        normals.Add(voxelMesh.normals[v + 3]);
        // voxelMesh 에서 삼각형 인덱스 6개를 갖ㅕ와 서브 메시의 삼각형 리스트에 추가.
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 0] + index);
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 1] + index);
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 2] + index);
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 3] + index);
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 4] + index);
        triangles[voxel.subMesh].Add(voxelMesh.triangles[t + 5] + index);
        // 추가된 정점의 개수를 더함.
        voxel.verticeCount += 4;
    }

    public void AddFaceUV(int v)
    {
        uv.Add(voxelMesh.uv[v + 0]);
        uv.Add(voxelMesh.uv[v + 1]);
        uv.Add(voxelMesh.uv[v + 2]);
        uv.Add(voxelMesh.uv[v + 3]);
    }
}

