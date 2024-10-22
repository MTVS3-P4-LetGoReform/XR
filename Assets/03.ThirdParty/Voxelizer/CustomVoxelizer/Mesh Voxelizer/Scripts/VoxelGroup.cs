using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVoxelizer
{
    [ExecuteInEditMode]
    public class VoxelGroup : MonoBehaviour
    {
        public Mesh voxelMesh = null;
        public Material[] voxelMaterials;
        public Material centerMaterial;
        public MeshVoxelizer.UVConversion uvType = MeshVoxelizer.UVConversion.SourceMesh;
        public Vector3 voxelScale;
        public Vector3 voxelRotation;
        [HideInInspector] public float ratio = 1.0f;
        [HideInInspector] public Voxel[] voxels;
        [HideInInspector] public Vector3[] voxelPosition;
        [HideInInspector] public Vector2[] uvs;
        [HideInInspector] public int[] submesh;
        [HideInInspector] public GameObject[] centerVoxels;
        [HideInInspector] public Vector3[] centerVoxelPosition;

        [HideInInspector] public Mesh m_mesh = null;
        Dictionary<Material, Texture2D> tex2D;

        private void Awake()
        {
            if (m_mesh == null) RebuildVoxels();
        }

        public void RebuildVoxels()
        {
            if (voxelMesh == null) return;
            UpdateVoxel();
            if (uvType == MeshVoxelizer.UVConversion.SourceMesh)
            {
                for (int i = 0; i < voxels.Length; ++i)
                {
                    if (voxels[i] == null) { CreateVoxel(i); }
                    voxels[i].UpdateVoxel(m_mesh, uvs[i]);
                }
            }
            else
            {
                for (int i = 0; i < voxels.Length; ++i)
                {
                    if (voxels[i] == null) { CreateVoxel(i); }
                    voxels[i].GetComponent<MeshFilter>().sharedMesh = m_mesh;
                }
            }
            if (centerVoxels != null)
            {
                for (int i = 0; i < centerVoxels.Length; ++i)
                {
                    if (centerVoxels[i] == null) { CreateCenterVoxel(i); }
                    centerVoxels[i].GetComponent<MeshRenderer>().sharedMaterial = centerMaterial;
                }
            }
        }

        public void ResetVoxels()
        {
            for (int i = 0; i < voxels.Length; ++i)
            {
                if (voxels[i] == null) continue;
                voxels[i].transform.localPosition = voxelPosition[i];
                voxels[i].transform.localScale = Vector3.one;
                voxels[i].transform.localRotation = Quaternion.identity;
            }
            if (centerVoxels != null)
            {
                for (int i = 0; i < centerVoxels.Length; ++i)
                {
                    if (centerVoxels[i] == null) continue;
                    centerVoxels[i].transform.localPosition = centerVoxelPosition[i];
                    centerVoxels[i].transform.localScale = Vector3.one;
                    centerVoxels[i].transform.localRotation = Quaternion.identity;
                }
            }
        }

        public void CreateVoxel(int i)
        {
            GameObject voxelObject = new GameObject("voxel");
            voxelObject.AddComponent<MeshFilter>();
            voxelObject.AddComponent<MeshRenderer>().sharedMaterial = voxelMaterials[submesh[i]];
            voxelObject.transform.parent = transform;
            voxelObject.transform.localPosition = voxelPosition[i];
            voxels[i] = voxelObject.AddComponent<Voxel>();
        }

        public void CreateCenterVoxel(int i)
        {
            GameObject voxelObject = new GameObject("center voxel");
            voxelObject.AddComponent<MeshFilter>().sharedMesh = m_mesh;
            voxelObject.AddComponent<MeshRenderer>().sharedMaterial = centerMaterial;
            voxelObject.transform.parent = transform;
            voxelObject.transform.localPosition = centerVoxelPosition[i];
            centerVoxels[i] = voxelObject;
        }

        void UpdateVoxel()
        {
            if (m_mesh == null) m_mesh = new Mesh();
            // 메쉬의 버텍스, 노멀, UV, 색상, 서브메쉬, 인덱스 등 모든 데이터를 초기화하는 함수.
            m_mesh.Clear();
            Vector3[] vertices = voxelMesh.vertices;
            Vector2[] uvs = voxelMesh.uv;
            // ?
            Vector3 r = ratio * voxelScale;
            // ?
            Quaternion rotation = Quaternion.Euler(voxelRotation);
            
            if (uvType == MeshVoxelizer.UVConversion.None)
            {
                for (int i = 0; i < vertices.Length; ++i)
                {
                    Vector3 v = new Vector3(vertices[i].x * r.x, vertices[i].y * r.y, vertices[i].z * r.z);
                    v = rotation * v;
                    vertices[i] = v;
                    // 텍스처 좌표 원점(0, 0 할당)
                    // 사용 경우?
                    // 1. 텍스처 초기화 : 메쉬의 모든 정점에 (0, 0) UV 좌표를 설정하여 제대로 매핑되지 않도록 비우거나 초기화.
                    // 2. 특정 패턴 텍스처 매핑 : 텍스처 특정영역을 메쉬의 여러 정점에 할당해서 텍스처 해당 부분만 사용하게 하거나 고정된 텍스처 패턴 부여할 때
                    // 3. 단색 텍스처 : 전체 메쉬에 단일 색상 또는 단일 텍스처의 특정 부분을 맵핑하고 싶을 때.
                    uvs[i] = Vector2.zero;
                }
            }
            // 그 외 경우 uv 그대로 적용
            else
            {
                for (int i = 0; i < vertices.Length; ++i)
                {
                    Vector3 v = new Vector3(vertices[i].x * r.x, vertices[i].y * r.y, vertices[i].z * r.z);
                    v = rotation * v;
                    vertices[i] = v;
                }
            }
            m_mesh.vertices = vertices;
            m_mesh.uv = uvs;
            m_mesh.normals = voxelMesh.normals;
            m_mesh.triangles = voxelMesh.triangles;
        }

        /* Voxel 색상 추출 메서드 */
        public Color GetVoxelColor(Voxel v)
        {
            //? : 왜 택스처를 딕셔너리로 관리하지?
            if (tex2D == null) tex2D = new Dictionary<Material, Texture2D>();
            // 메쉬에 해당하는 material 가져옴.
            Material m = v.GetComponent<MeshRenderer>().material;
            // Material.mainTexture는 Texture 클래스 인스턴스를 반환
            // Material.mainTexture에 할당 가능한 형식(Texture2D, Texture3D, CubeMap, RenderTexture)
            // ?대부분의 경우 Texture2D지만 여러종류의 텍스처가 있어 Texture2D로 캐스팅이 필요
            if (!tex2D.ContainsKey(m)) tex2D[m] = Util.MVHelper.GetTexture2D(m.mainTexture);
            Vector2 uv = v.m_mesh.uv[0];
            // 복셀 색상 추출
            Color color = tex2D[m].GetPixel((int)(tex2D[m].width * uv.x), (int)(tex2D[m].height * uv.y));
            return color;
        }

        private void OnDestroy()
        {
            if (m_mesh != null)
            {
                DestroyImmediate(m_mesh);
            }
            if (tex2D != null)
            {
                // DestroyImmediate 는 프레임의 끝을 기다리지 않고 메모리에서 즉시 삭제
                // 주로 에디터 모드나 즉각적인 메모리 해제를 원할때 사용.
                foreach (var v in tex2D.Values) if (v != null) GameObject.DestroyImmediate(v);
                tex2D = null;
            }
        }
    }
}