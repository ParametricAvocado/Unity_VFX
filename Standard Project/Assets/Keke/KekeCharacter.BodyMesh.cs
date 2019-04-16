using UnityEngine;

public partial class KekeCharacter
{
    public class BodyMesh : IDrawable
    {
        private float size = 1.0f;
        private float taper = 0.0f;
        private float flatten = 0.0f;
        private float squashStretch = 0.0f;
        private Matrix4x4 squashMatrix = Matrix4x4.identity;

        private Vector3 localPosition = Vector3.zero;
        private Vector3 scale = Vector3.one;
        private Transform parent;
        private bool needsUpdateMesh;
        private Mesh baseMesh;
        private Mesh mesh;


        public Vector3 LocalPosition
        {
            get
            {
                return localPosition;
            }

            set
            {
                localPosition = value;
                UpdateTransform();
            }
        }

        public Vector3 Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
                UpdateTransform();
            }
        }

        public float Size
        {
            get
            {
                return size;
            }

            set
            {
                if (size == value) { return; }

                size = value;
                needsUpdateMesh = true;
            }
        }


        public Matrix4x4 LocalMatrix { get; set; }

        public Material Material { get; set; }

        public float Taper
        {
            get
            {
                return taper;
            }

            set
            {
                if (taper == value) { return; }
                taper = value;
                needsUpdateMesh = true;
            }
        }

        public float Flatten
        {
            get
            {
                return flatten;
            }

            set
            {
                if (flatten == value) { return; }
                flatten = value;
                needsUpdateMesh = true;
            }
        }

        public float SquashStretch
        {
            get
            {
                return squashStretch;
            }

            set
            {
                squashStretch = value;
                squashMatrix = Matrix4x4.Scale(new Vector3(1.0f - squashStretch, 1.0f + squashStretch, 1.0f - squashStretch));
                UpdateTransform();
            }
        }

        public BodyMesh(Mesh baseMesh, Transform parent)
        {
            this.baseMesh = baseMesh;

            mesh = new Mesh();
            mesh.vertices = baseMesh.vertices;
            mesh.normals = baseMesh.normals;
            mesh.tangents = baseMesh.tangents;
            mesh.uv = baseMesh.uv;
            mesh.triangles = baseMesh.triangles;
            mesh.indexFormat = baseMesh.indexFormat;

            this.parent = parent;
        }

        private void UpdateMesh()
        {
            if (!needsUpdateMesh) { return; }

            needsUpdateMesh = false;
            var base_bounds = baseMesh.bounds;
            var base_vertices = baseMesh.vertices;
            var base_normals = baseMesh.normals;
            var vertices = mesh.vertices;
            var normals = mesh.normals;

            for (int i = 0; i < base_vertices.Length; i++)
            {
                var vertex = base_vertices[i];

                var boundsSpacePos = new Vector3()
                {
                    x = (vertex.x - base_bounds.min.x) / base_bounds.size.x,
                    y = (vertex.y - base_bounds.min.y) / base_bounds.size.y,
                    z = (vertex.z - base_bounds.min.z) / base_bounds.size.z
                };

                if (Flatten != 0)
                {
                    var flatten_scale = Flatten * Mathf.Pow(Mathf.Abs(vertex.z / base_bounds.extents.z), 2.0f);
                    vertex.z *= 1.0f - flatten_scale;
                }

                if (taper != 0)
                {
                    var taper_scale = Mathf.Pow(1.0f - (Mathf.Abs(taper) * (taper > 0 ? boundsSpacePos.y : (1.0f - boundsSpacePos.y))), 2);

                    vertex.x *= taper_scale;
                    vertex.z *= taper_scale;
                }


                vertex *= Size;
                vertices[i] = vertex;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.UploadMeshData(false);
        }

        public void Draw()
        {
            UpdateMesh();
            Matrix4x4 matrix = parent.localToWorldMatrix * LocalMatrix;

            Graphics.DrawMesh(mesh, matrix, Material, parent.gameObject.layer, null);
        }

        private void UpdateTransform()
        {
            LocalMatrix = Matrix4x4.Translate(localPosition) * squashMatrix * Matrix4x4.Scale(scale);
        }
    }
}
