using System;
using UnityEngine;

public class SkinnedMeshTrail : MonoBehaviour
{
    public enum MeshTrailDrawOrder
    {
        None,
        OldestFirst,
        NewestFirst
    }
    public class SkinnedMeshRendererSnapshot
    {
        private static int generatedMeshIndex = 0;
        private float snapshotTime;
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private Mesh mesh;
        private Material[] sharedMaterials;
        private Material overrideMaterial;
        private MaterialPropertyBlock materialPropertyBlock;
        private Matrix4x4 transform;

        public float LifeTime
        {
            get
            {
                return Time.time - snapshotTime;
            }
        }

        public SkinnedMeshRendererSnapshot(SkinnedMeshRenderer meshRenderer, Material overrideMaterial)
        {
            skinnedMeshRenderer = meshRenderer;
            mesh = new Mesh();
            mesh.name = "Trail Mesh #" + generatedMeshIndex;
            generatedMeshIndex++;

            sharedMaterials = new Material[skinnedMeshRenderer.sharedMaterials.Length];
            for (int m = 0; m < sharedMaterials.Length; m++)
            {
                sharedMaterials[m] = overrideMaterial != null ? overrideMaterial : skinnedMeshRenderer.sharedMaterials[m];
            }

            this.overrideMaterial = overrideMaterial;
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void TakeSnapshot()
        {
            skinnedMeshRenderer.BakeMesh(mesh);
            transform = skinnedMeshRenderer.transform.localToWorldMatrix;
            snapshotTime = Time.time;
            materialPropertyBlock.SetFloat("_SpawnTime", snapshotTime);
        }

        public void SetLifetime(float lifetime)
        {
            materialPropertyBlock.SetFloat("_LifeSpan", lifetime);
        }

        public void Render()
        {
            if (skinnedMeshRenderer.HasPropertyBlock())
            {
                skinnedMeshRenderer.GetPropertyBlock(materialPropertyBlock);
            }

            materialPropertyBlock.SetFloat("_Age", Time.time - snapshotTime);

            for (int subIndex = 0; subIndex < mesh.subMeshCount; subIndex++)
            {
                Graphics.DrawMesh(mesh, transform, sharedMaterials[subIndex], skinnedMeshRenderer.gameObject.layer, null, subIndex, materialPropertyBlock);
            }
        }

        public void Destroy()
        {
            skinnedMeshRenderer = null;
            UnityEngine.Object.Destroy(mesh);
            sharedMaterials = null;
            overrideMaterial = null;
            materialPropertyBlock = null;
        }
    }
    [SerializeField]
    private int m_TrailLength = 12;
    [SerializeField]
    private float m_LifeTime = 0;
    [SerializeField]
    private int m_FrameSkip = 2;

    [SerializeField]
    private Material m_OverrideMaterial = null;

    [SerializeField]
    private MeshTrailDrawOrder m_TrailDrawOrder;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private SkinnedMeshRendererSnapshot[] snapshotBuffer;
    private int bufferIndex = 0;
    private int lastFrameSkip = 0;

    public bool IsEmitting
    {
        get;
        set;
    }

    private void Awake()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void PopulateNextSnapshot(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        if (snapshotBuffer[bufferIndex] == null)
        {
            snapshotBuffer[bufferIndex] = new SkinnedMeshRendererSnapshot(skinnedMeshRenderer, m_OverrideMaterial);
        }
        snapshotBuffer[bufferIndex].TakeSnapshot();
        snapshotBuffer[bufferIndex].SetLifetime(m_LifeTime);
        if (bufferIndex < snapshotBuffer.Length - 1)
        {
            bufferIndex++;
        }
        else
        {
            bufferIndex = 0;
        }
    }

    private void TakeSnapshot()
    {
        if (lastFrameSkip == 0)
        {
            lastFrameSkip = m_FrameSkip;
        }
        else
        {
            lastFrameSkip--;
            return;
        }

        if (snapshotBuffer == null || snapshotBuffer.Length != m_TrailLength * skinnedMeshRenderers.Length)
        {
            bool transferBuffer = snapshotBuffer != null;

            SkinnedMeshRendererSnapshot[] oldBuffer = snapshotBuffer;

            snapshotBuffer = new SkinnedMeshRendererSnapshot[m_TrailLength * skinnedMeshRenderers.Length];

            if(transferBuffer)
            {
                for (int i = 0; i < oldBuffer.Length; i++)
                {
                    if(i < snapshotBuffer.Length)
                    {
                        snapshotBuffer[i] = oldBuffer[i];
                    }else
                    {
                        oldBuffer[i].Destroy();
                    }
                }
            }


            bufferIndex = 0;
        }

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            PopulateNextSnapshot(skinnedMeshRenderers[i]);
        }
    }

    private void RenderSnapshots()
    {
        if (snapshotBuffer == null)
        {
            return;
        }

        int offsetIndex;
        for (int i = 0; i < snapshotBuffer.Length; i++)
        {
            switch (m_TrailDrawOrder)
            {
                case MeshTrailDrawOrder.OldestFirst:
                    offsetIndex = (bufferIndex + i) % snapshotBuffer.Length;
                    break;
                case MeshTrailDrawOrder.NewestFirst:
                    offsetIndex = (bufferIndex - i - 1) % snapshotBuffer.Length;
                    break;
                case MeshTrailDrawOrder.None:
                    offsetIndex = i;
                    break;
            }

            if (snapshotBuffer[i] == null) continue;

            if (m_LifeTime > 0)
            {
                if (snapshotBuffer[i].LifeTime > m_LifeTime) continue;

                snapshotBuffer[i].Render();
            }
            else
            {
                snapshotBuffer[i].Render();
            }
        }
    }

    private void LateUpdate()
    {
        TakeSnapshot();
        RenderSnapshots();
    }
}
