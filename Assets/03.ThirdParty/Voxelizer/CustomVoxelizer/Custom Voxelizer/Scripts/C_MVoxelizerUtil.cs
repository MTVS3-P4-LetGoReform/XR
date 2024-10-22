using UnityEngine;

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