using UnityEngine;

public partial class KekeCharacter
{
    public class SplineMesh : IDrawable
    {
        private bool needsUpdateMesh;
        private Mesh mesh;

        private int[] splineIndices;

        private int resU, resV, quadCount;

        private Vector3 positionFrom, tangentFrom, positionTo, tangentTo;
        private float _thickness;
        private float _resolutionSpread;

        public int ResolutionU
        {
            get { return resU; }
            set
            {
                if (value != resU)
                {
                    resU = value;
                    RebuildMesh();
                }
            }
        }

        public int ResolutionV
        {
            get { return resV; }
            set
            {
                if (value != resV)
                {
                    resV = value;
                    RebuildMesh();
                }
            }
        }


        public float ResolutionSpread
        {
            get
            {
                return _resolutionSpread;
            }
            set
            {
                if (_resolutionSpread == value)
                {
                    return;
                }

                _resolutionSpread = value;
                needsUpdateMesh = true;
            }
        }
        public float Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                if (_thickness == value)
                {
                    return;
                }

                _thickness = value;
                needsUpdateMesh = true;
            }
        }
        public AnimationCurve ThicknessCurve { get; set; }
        public Material Material { get; set; }
        public Transform Parent { get; set; }

        public Vector3 PositionFrom
        {
            get { return positionFrom; }

            set
            {
                if (positionFrom == value)
                {
                    return;
                }

                positionFrom = value;
                needsUpdateMesh = true;
            }
        }

        public Vector3 TangentFrom
        {
            get { return tangentFrom; }

            set
            {
                if (tangentFrom == value)
                {
                    return;
                }

                tangentFrom = value;
                needsUpdateMesh = true;
            }
        }

        public Vector3 PositionTo
        {
            get { return positionTo; }

            set
            {
                if (positionTo == value)
                {
                    return;
                }

                positionTo = value;
                needsUpdateMesh = true;
            }
        }

        public Vector3 TangentTo
        {
            get { return tangentTo; }

            set
            {
                if (tangentTo == value)
                {
                    return;
                }

                tangentTo = value;
                needsUpdateMesh = true;
            }
        }

        public SplineMesh(int resolutionU, int resolutionV)
        {
            resU = resolutionU;
            resV = resolutionV;
            RebuildMesh();
        }

        private void RebuildMesh()
        {
            quadCount = resU * resV;
            splineIndices = new int[4 * quadCount];

            for (int i = 0; i < quadCount; i++)
            {
                int quadU = i % resU;
                int quadV = i / resU;
                splineIndices[i * 4] = i;
                splineIndices[i * 4 + 1] = resU * quadV + ((quadU + 1) % resU);
                splineIndices[i * 4 + 2] = resU * (quadV + 1) + ((quadU + 1) % resU);
                splineIndices[i * 4 + 3] = i + resU;
            }

            mesh = new Mesh
            {
                vertices = new Vector3[resU * (resV + 1)],
                normals = new Vector3[resU * (resV + 1)]
            };
            mesh.SetIndices(splineIndices, MeshTopology.Quads, 0);
            needsUpdateMesh = true;
        }

        private void GetSplinePoint(float ratio, Vector3 a, Vector3 aTan, Vector3 b, Vector3 bTan, out Vector3 position, out Vector3 tangent, out Vector3 normal, out Vector3 binormal)
        {
            ratio = Mathf.Clamp01(ratio);
            position = Vector3.Lerp(a + aTan * ratio, b + bTan * (1 - ratio), ratio);
            tangent = Vector3.Lerp(aTan, -bTan, ratio).normalized;
            normal = Vector3.Lerp(bTan, aTan, ratio).normalized;
            binormal = Vector3.zero;
            Vector3.OrthoNormalize(ref tangent, ref normal, ref binormal);
        }

        private Vector3 GetSplinePoint(float ratio, Vector3 a, Vector3 aTan, Vector3 b, Vector3 bTan)
        {
            ratio = Mathf.Clamp01(ratio);

            return Vector3.Lerp(a + aTan * ratio, b + bTan * (1 - ratio), ratio);
        }

        public void Update(Vector3 from, Vector3 fromTangent, Vector3 to, Vector3 toTangent)
        {
            PositionFrom = from;
            TangentFrom = fromTangent;
            PositionTo = to;
            TangentTo = toTangent;
        }

        private void UpdateMesh()
        {
            if (!needsUpdateMesh)
            {
                return;
            }

            needsUpdateMesh = false;

            Vector3[] positions = mesh.vertices;
            Vector3[] normals = mesh.normals;
            for (int v = 0; v <= resV; v++)
            {
                float tV = (float)v / resV;

                tV = Mathf.Lerp(tV, tV * tV * tV * (tV * (tV * 6 - 15) + 10), ResolutionSpread);

                Vector3 pos, tan, norm, binorm;

                GetSplinePoint(tV, PositionFrom, TangentFrom, PositionTo, TangentTo, out pos, out tan, out norm, out binorm);

                Matrix4x4 tr = Matrix4x4.TRS(pos, Quaternion.LookRotation(tan, norm), Vector3.one);

                for (int u = 0; u < resU; u++)
                {
                    int idx = v * resU + u;
                    float tU = (float)u / resU;
                    float angle0 = tU * Mathf.PI * 2;
                    float radius = ThicknessCurve != null ? Thickness * ThicknessCurve.Evaluate(tV) : Thickness;
                    float x0 = Mathf.Sin(-angle0);
                    float y0 = Mathf.Cos(-angle0);

                    positions[idx] = tr.MultiplyPoint(new Vector3(x0, y0, 0) * radius);
                    normals[idx] = tr.MultiplyVector(new Vector3(x0, y0, 0));
                }
            }

            mesh.vertices = positions;
            mesh.normals = normals;
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
        }

        public void DrawDebug()
        {
            Matrix4x4 matrix = Parent ? Parent.localToWorldMatrix : Matrix4x4.identity;
            for (int v = 0; v <= resV; v++)
            {
                float tV = (float)v / resV;

                tV = Mathf.Lerp(tV, tV * tV * tV * (tV * (tV * 6 - 15) + 10), ResolutionSpread);

                Vector3 pos, tan, norm, binorm;

                GetSplinePoint(tV, PositionFrom, TangentFrom, PositionTo, TangentTo, out pos, out tan, out norm, out binorm);

                Debug.DrawRay(matrix.MultiplyPoint(pos), matrix.MultiplyVector(norm) / 10, Color.blue, 0, false);
                Debug.DrawRay(matrix.MultiplyPoint(pos), matrix.MultiplyVector(tan) / 10, Color.green, 0, false);
                Debug.DrawRay(matrix.MultiplyPoint(pos), matrix.MultiplyVector(binorm) / 10, Color.red, 0, false);
            }
        }

        public void Draw()
        {
            Matrix4x4 matrix = Parent ? Parent.localToWorldMatrix : Matrix4x4.identity;
            UpdateMesh();
            Graphics.DrawMesh(mesh, matrix, Material, 0);
        }
    }
}
