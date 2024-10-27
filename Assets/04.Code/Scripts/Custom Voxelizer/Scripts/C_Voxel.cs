using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스크립트를 에디터 모드에서도 실행되도록하는 속성
[ExecuteInEditMode]
public class C_Voxel : MonoBehaviour
{
    // 인스펙터 창에서 보이지 않도록 숨김.
    [HideInInspector] public Mesh m_mesh = null;

    // 
    public void UpdateVoxel(Mesh mesh, Vector2 uv)
    {
        // m_mesh : 미리 생성된 또는 수정된 메쉬데이터를 담고 있는 변수.
        if (m_mesh == null) m_mesh = new Mesh();
        m_mesh.Clear();
        Vector2[] uvs = new Vector2[mesh.uv.Length]; 
        for (int i = 0; i < uvs.Length; ++i) uvs[i] = uv;
        m_mesh.vertices = mesh.vertices;
        m_mesh.uv = uvs;
        m_mesh.normals = mesh.normals;
        m_mesh.triangles = mesh.triangles;
        // 메쉬 필터 컴포넌트의 sharedMesh 속성에 새로운 메쉬를 할당.
        // sharedMesh는 메쉬필터에 연결된 공유 메쉬를 나타냄.
        // 여러 오브젝트가 동일한 메쉬를 공유할 수 있는 상황에서 사용.
        // 이 메쉬 데이터를 변경하면 이 메쉬를 공유하는 모든 오브젝트의 메쉬가 함께 변경.
        // 만약 개별 오브젝트 메쉬를 변경하고 싶다면 mesh 속성을 사용하는 것이 좋음.
        // mesh는 개별 인스턴스에만 사용되는 메쉬 데이터를 참조하고 변경 시 자동으로 복제되어 다른 오브젝트에는 영향을 미치지 않음.
        GetComponent<MeshFilter>().sharedMesh = m_mesh;
    }

    // OnDestroy : 특정 오브젝트가 파괴될때 호출되는 이벤트 함수.
    private void OnDestroy()
    {
        //m_mesh가 null이 아니라는 것은 메모리에 존재하는 메쉬 오브젝트가 있다는 의미.
        //m_mesh가 이미 삭제되었거나 초기화되지 않았을 때 불필요한 Destroy 호출을 방지하기 위해 사용
        // 메쉬를 메모리에 올리는 타이밍???? 오브젝트 파괴 시 메쉬를 파괴하면 메모리 상에서 내려간다??
        // 유니티 컴파일 및 실행과정에 대해 알 필요가 있음.
        if (m_mesh != null)
        {
            // Unity에서 오브젝트를 즉시 파괴하는 함수
            
        }
        DestroyImmediate(m_mesh);
    }
}