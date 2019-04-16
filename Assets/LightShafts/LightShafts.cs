using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class LightShafts : MonoBehaviour
{
    private struct Edge
    {
        public int indexA;
        public int indexB;

        public Edge(int a, int b) : this()
        {
            indexA = a;
            indexB = b;
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                Edge other = (Edge)obj;

                return (indexA == other.indexA && indexB == other.indexB)
                    || (indexA == other.indexB && indexB == other.indexA);
            }
            return base.Equals(obj);
        }
    }

    private class ExtrusionData
    {
        private List<Edge> edges = new List<Edge>();
        private List<Vector3> vertices;
        private List<Vector3> normals;
        private int[] indices;
        private Vector2[] uvs;
        private Color[] colors;

        private int sourceVertexCount;
        private Vector3[] sourceVertices;
        private Vector3[] sourceNormals;
        private Vector3 extrusionVector;
        private Vector3 extrusionPoint;
        private float extrusionLength;
        private bool directional = true;

        public ExtrusionData(Mesh sourceMesh, Mesh serializedMesh, Mesh tempMesh)
        {
            sourceVertexCount = sourceMesh.vertexCount;
            vertices = new List<Vector3>(sourceVertexCount * 2);
            serializedMesh.GetVertices(vertices);

            tempMesh.vertices = serializedMesh.vertices;
            tempMesh.triangles = serializedMesh.triangles;
            tempMesh.normals = serializedMesh.normals;
            tempMesh.uv = serializedMesh.uv;
            tempMesh.colors = serializedMesh.colors;
            tempMesh.UploadMeshData(false);
        }

        public ExtrusionData(Mesh sourceMesh, Mesh serializedMesh, Mesh tempMesh, bool allowDuplicateEdges, bool onlyOuterEdges, int maxEdges = 0, float edgeRatio = 1, int randomSeed = 0)
        {
            if (!sourceMesh)
            {
                Debug.LogError("No Source Mesh provided."); return;
            }

            if (!serializedMesh)
            {
                Debug.LogError("No Target Mesh provided."); return;
            }

            var tempRandomState = Random.state;

            Random.InitState(randomSeed);

            sourceVertexCount = sourceMesh.vertexCount;
            sourceVertices = sourceMesh.vertices;
            sourceNormals = sourceMesh.normals;
            var sourceIndices = sourceMesh.GetIndices(0);

            for (int i = 0; i < sourceIndices.Length; i += 3)
            {
                AddEdge(edges, new Edge(sourceIndices[i], sourceIndices[i + 1]), allowDuplicateEdges, onlyOuterEdges);
                AddEdge(edges, new Edge(sourceIndices[i + 1], sourceIndices[i + 2]), allowDuplicateEdges, onlyOuterEdges);
                AddEdge(edges, new Edge(sourceIndices[i + 2], sourceIndices[i]), allowDuplicateEdges, onlyOuterEdges);
            }

            int edgeCount = edges.Count;
            int limitEdges = Mathf.CeilToInt((maxEdges > 0 ? Mathf.Min(maxEdges, edgeCount) : edgeCount) * Mathf.Clamp01(edgeRatio));

            for (int i = 0; i < Mathf.Max(0, edgeCount - limitEdges); i++)
            {
                edges.RemoveAt(Random.Range(0, edges.Count));
            }

            indices = new int[edges.Count * 4];

            for (int i = 0; i < edges.Count; i++)
            {
                indices[i * 4] = edges[i].indexA;
                indices[i * 4 + 1] = edges[i].indexB;
                indices[i * 4 + 2] = edges[i].indexB + sourceVertexCount;
                indices[i * 4 + 3] = edges[i].indexA + sourceVertexCount;
            }

            vertices = new List<Vector3>(sourceVertexCount * 2);
            vertices.AddRange(sourceVertices);
            vertices.AddRange(sourceVertices);

            normals = new List<Vector3>(sourceVertexCount * 2);
            normals.AddRange(sourceNormals);
            normals.AddRange(sourceNormals);


            uvs = new Vector2[sourceVertexCount * 2];
            colors = new Color[sourceVertexCount * 2];
            for (int i = 0; i < sourceVertexCount; i++)
            {
                uvs[i] = Vector2.one;
                uvs[i + sourceVertexCount] = Vector2.zero;
                colors[i] = Color.white;
                colors[i + sourceVertexCount] = new Color(1, 1, 1, 0);
            }

            Random.state = tempRandomState;

            serializedMesh.SetVertices(vertices);
            serializedMesh.SetNormals(normals);
            serializedMesh.SetIndices(indices, MeshTopology.Quads, 0);
            serializedMesh.colors = colors;
            serializedMesh.uv = uvs;
            serializedMesh.UploadMeshData(false);

            tempMesh.vertices = serializedMesh.vertices;
            tempMesh.triangles = serializedMesh.triangles;
            tempMesh.normals = serializedMesh.normals;
            tempMesh.uv = serializedMesh.uv;
            tempMesh.colors = serializedMesh.colors;
            tempMesh.UploadMeshData(false);
        }

        public void SetExtrusionPoint(Vector3 point, float length)
        {
            extrusionPoint = point;
            extrusionLength = length;
            directional = false;
        }

        public void SetExtrusionDirectional(Vector3 direction)
        {
            extrusionVector = direction;
            directional = true;
        }

        public void GetExtrudedMesh(Mesh mesh)
        {
            if (!mesh)
            {
                return;
            }

            for (int v = 0; v < sourceVertexCount; v++)
            {
                if (directional)
                {
                    vertices[v + sourceVertexCount] = vertices[v] + extrusionVector;
                }
                else
                {
                    vertices[v + sourceVertexCount] = vertices[v] + (vertices[v] - extrusionPoint).normalized * extrusionLength;
                }
            }

            mesh.SetVertices(vertices);
            mesh.UploadMeshData(false);
        }

        private static void AddEdge(List<Edge> list, Edge edge, bool allowDuplicates, bool onlyOuterEdges)
        {
            if (onlyOuterEdges && list.Contains(edge))
            {
                list.Remove(edge);
            }
            else if (allowDuplicates || !list.Contains(edge))
            {
                list.Add(edge);
            }
        }
    }

    [SerializeField]
    private Mesh m_SerializedMesh;

    [SerializeField]
    private Material m_Material;

    [SerializeField]
    private float m_ExtrusionLength = 1f;

    [SerializeField]
    private bool m_AllowDuplicateEdges = false;

    [SerializeField]
    private bool m_OnlyOuterEdges = false;

    [SerializeField]
    private int m_MaxEdges = 0;

    [Range(0, 1)]
    [SerializeField]
    private float m_EdgeMultiplier = 1;

    [Range(0, 100)]
    [SerializeField]
    private int m_EdgeRandomSeed = 0;

    [SerializeField]
    private Light m_LightSource = null;

    [SerializeField]
    private Vector3 m_DefaultDirection = Vector3.down;

    //Serialized Values

    [SerializeField]
    [HideInInspector]
    private bool s_AllowDuplicateEdges;

    [SerializeField]
    [HideInInspector]
    private bool s_OnlyOuterEdges;

    [SerializeField]
    [HideInInspector]
    private int s_MaxEdges;

    [SerializeField]
    [HideInInspector]
    private float s_EdgeMultiplier;

    [SerializeField]
    [HideInInspector]
    private int s_EdgeRandomSeed;


    private ExtrusionData extrusionData;

    private MeshFilter sourceMeshFilter = null;
    private MeshRenderer sourceMeshRenderer = null;
    private Mesh sourceMesh = null;

    private Mesh mesh = null;

    private MaterialPropertyBlock propertyBlock;

    private bool isDirty = false;
    private Matrix4x4 lastTransform;
    private Matrix4x4 lastLightTransform;

    private Vector3 lastDefaultDirection;

    private int lightCoordsID = Shader.PropertyToID("_LightCoords");
    private int lightAttenID = Shader.PropertyToID("_LightAtten");

    public Vector3 DefaultDirection
    {
        get
        {
            return m_DefaultDirection;
        }

        set
        {
            m_DefaultDirection = value;
        }
    }

    private bool IsTransformDirty { get { return lastTransform != transform.localToWorldMatrix; } }
    private bool IsLightTransformDirty { get { return (m_LightSource && lastLightTransform != m_LightSource.transform.localToWorldMatrix); } }
    private bool IsDefaultDirectionDirty { get { return m_DefaultDirection != lastDefaultDirection; } }


    private void InitSourceMesh()
    {
        sourceMeshRenderer = GetComponent<MeshRenderer>();
        if (!sourceMeshRenderer)
        {
            enabled = false;
            return;
        }

        sourceMeshFilter = GetComponent<MeshFilter>();
        if (!sourceMeshFilter)
        {
            enabled = false;
            return;
        }

        sourceMesh = sourceMeshFilter.sharedMesh;
    }

    private void OnEnable()
    {
        InitSourceMesh();
        propertyBlock = new MaterialPropertyBlock();

        if (m_SerializedMesh)
        {
            mesh = new Mesh();
            extrusionData = new ExtrusionData(sourceMesh, m_SerializedMesh, mesh);
        }

        isDirty = true;
    }

    private void LateUpdate()
    {
        isDirty |= IsTransformDirty || IsLightTransformDirty || IsDefaultDirectionDirty;

        if (isDirty)
        {

            lastTransform = transform.localToWorldMatrix;
            if (m_LightSource)
            {
                lastLightTransform = m_LightSource.transform.localToWorldMatrix;
            }
        }

        Refresh();

        DrawMesh();
    }


    private void Refresh()
    {
        if (!isDirty || extrusionData == null)
        {
            return;
        }

        bool isDirectional = !m_LightSource || m_LightSource.type == LightType.Directional;
        Vector4 lightCoords = Vector4.zero;
        float lightAtten = 0.0f;
        if (isDirectional)
        {
            Vector3 dir = m_LightSource ? transform.InverseTransformDirection(m_LightSource.transform.forward) : DefaultDirection.normalized;

            lightCoords = dir;
            lightAtten = m_LightSource.intensity;

            extrusionData.SetExtrusionDirectional(dir * m_ExtrusionLength);
        }
        else
        {
            extrusionData.SetExtrusionPoint(transform.InverseTransformPoint(m_LightSource.transform.position), m_ExtrusionLength / transform.localScale.x);
            lightCoords = transform.InverseTransformPoint(m_LightSource.transform.position);
            lightCoords.w = 1;
            lightAtten = m_LightSource.intensity * (1 - Vector3.Distance(m_LightSource.transform.position, transform.position) / m_LightSource.range);
        }

        propertyBlock.SetVector(lightCoordsID, lightCoords);
        propertyBlock.SetFloat(lightAttenID, Mathf.Clamp01(lightAtten));

        extrusionData.GetExtrudedMesh(mesh);
        isDirty = false;
    }

    private void DrawMesh()
    {
        if (!mesh)
        {
            return;
        }

        for (int m = 0; m < mesh.subMeshCount; m++)
        {
            Graphics.DrawMesh(mesh, transform.localToWorldMatrix, m_Material, gameObject.layer, null, m, propertyBlock, false, false, false);
        }
    }

    private void RegenerateMesh()
    {
        InitSourceMesh();
        m_SerializedMesh = new Mesh();
        m_SerializedMesh.name = name + "_Lightshafts";
        mesh = new Mesh();
        extrusionData = new ExtrusionData(sourceMesh, m_SerializedMesh, mesh, m_AllowDuplicateEdges, m_OnlyOuterEdges, m_MaxEdges, m_EdgeMultiplier, m_EdgeRandomSeed);
    }

    public void SetPropertyBlock(MaterialPropertyBlock mpb)
    {
        propertyBlock = mpb;
    }


    private void OnValidate()
    {
        bool paramsDirty = m_AllowDuplicateEdges != s_AllowDuplicateEdges
            || m_OnlyOuterEdges != s_OnlyOuterEdges
            || m_MaxEdges != s_MaxEdges
            || m_EdgeMultiplier != s_EdgeMultiplier
            || m_EdgeRandomSeed != s_EdgeRandomSeed;


        if (enabled && !m_SerializedMesh || paramsDirty)
        {
            s_AllowDuplicateEdges = m_AllowDuplicateEdges;
            s_OnlyOuterEdges = m_OnlyOuterEdges;
            s_MaxEdges = m_MaxEdges;
            s_EdgeMultiplier = m_EdgeMultiplier;
            s_EdgeRandomSeed = m_EdgeRandomSeed;

            RegenerateMesh();
        }
        isDirty = true;
    }
}
