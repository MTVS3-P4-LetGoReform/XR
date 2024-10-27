using System.Collections.Generic;
using UnityEngine;

public class OutlineRenderer
{
    //private MeshFilter meshFilter;
    //private Mesh outlineMesh;
    
    // void Start()
    // {
    //     //meshFilter = GetComponent<MeshFilter>();
    //     if (meshFilter != null)
    //     {
    //         // Mesh 데이터를 기반으로 외곽선을 그리는 메쉬 생성
    //         outlineMesh = CreateOutlineMesh(meshFilter.mesh);
    //         
    //         // 외곽선 Mesh를 사용해 라인을 렌더링합니다.
    //         DrawOutline(outlineMesh);
    //     }
    // }

    public Mesh CreateOutlineMesh(Mesh mesh)
    {
        Dictionary<Edge, int> edgeDict = new Dictionary<Edge, int>();

        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v1 = triangles[i];
            int v2 = triangles[i + 1];
            int v3 = triangles[i + 2];

            AddEdge(edgeDict, v1, v2);
            AddEdge(edgeDict, v2, v3);
            AddEdge(edgeDict, v3, v1);
        }

        List<Vector3> lineVertices = new List<Vector3>();

        // 외곽선에 해당하는 Edge만 저장
        foreach (var edge in edgeDict)
        {
            if (edge.Value == 1) // 한 번만 사용된 Edge가 외곽선
            {
                lineVertices.Add(vertices[edge.Key.VertexIndex1]);
                lineVertices.Add(vertices[edge.Key.VertexIndex2]);
            }
        }

        Mesh outline = new Mesh();
        outline.vertices = lineVertices.ToArray();

        // 인덱스 배열을 생성하여 외곽선 렌더링에 사용할 라인 세그먼트
        int[] indices = new int[lineVertices.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        outline.SetIndices(indices, MeshTopology.Lines, 0);
        return outline;
    }

    void AddEdge(Dictionary<Edge, int> edgeDict, int v1, int v2)
    {
        Edge edge = new Edge(v1, v2);
        if (edgeDict.ContainsKey(edge))
        {
            edgeDict[edge]++;
        }
        else
        {
            edgeDict[edge] = 1;
        }
    }

    public void DrawOutline(Mesh outline, GameObject voxel, Material mat)
    {
        GameObject outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(voxel.transform, false);

        MeshFilter mf = outlineObject.AddComponent<MeshFilter>();
        mf.mesh = outline;

        MeshRenderer mr = outlineObject.AddComponent<MeshRenderer>();
        mr.material = mat;
    }

    // Edge 구조체: 두 버텍스를 저장하는 간단한 구조체
    struct Edge
    {
        public int VertexIndex1;
        public int VertexIndex2;

        public Edge(int v1, int v2)
        {
            VertexIndex1 = Mathf.Min(v1, v2);
            VertexIndex2 = Mathf.Max(v1, v2);
        }

        // Dictionary에서 동일한 Edge를 찾을 수 있도록 HashCode와 Equals를 오버라이드
        public override int GetHashCode()
        {
            unchecked
            {
                return VertexIndex1 * 31 + VertexIndex2;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;

            Edge other = (Edge)obj;
            return VertexIndex1 == other.VertexIndex1 && VertexIndex2 == other.VertexIndex2;
        }
    }
}
