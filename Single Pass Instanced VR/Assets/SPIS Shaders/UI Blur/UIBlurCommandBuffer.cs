using UnityEngine.Rendering;

namespace UnityEngine.UI.Effects
{
    [ExecuteInEditMode]
    public class UIBlurCommandBuffer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How many times should the framebuffer be blurred.")]
        [Range(1, 3)]
        private int m_Iterations = 1;

        [SerializeField]
        [Tooltip("How far should the blur samples spread per-fragment.")]
        [Range(0, 5)]
        private float m_Distance = 1;


        [SerializeField]
        [Tooltip("The resolution fraction of the blur textures.")]
        [Range(1, 4)]
        private int m_ResolutionFraction = 2;

        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private CameraEvent m_ExecutionEvent = CameraEvent.AfterForwardAlpha;

        private CommandBuffer commandBuffer = null;
        private Material blurMaterial = null;
        private Shader blurShader;
        private Canvas canvas;

        private CameraEvent registeredEvent;

        private void Awake()
        {
            blurShader = Shader.Find("Hidden/UIBlurPass");

            if(m_Camera == null)
                m_Camera = GetComponent<Camera>();
            if (m_Camera == null)
                m_Camera = Camera.main;
        }

        private void CreateCommandBuffer()
        {
            if (!blurShader || !m_Camera)
            {
                return;
            }

            if (commandBuffer == null)
            {
                if (!blurMaterial)
                {
                    blurMaterial = new Material(blurShader);
                    blurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }

                commandBuffer = new CommandBuffer();
                commandBuffer.name = "Grab Screen and Blur";

                int temp0 = Shader.PropertyToID("_Temp0");
                int temp1 = Shader.PropertyToID("_Temp1");
                commandBuffer.GetTemporaryRTArray(temp0, 1080 / m_ResolutionFraction, 1200 / m_ResolutionFraction, 2, 0, FilterMode.Bilinear);
                commandBuffer.GetTemporaryRTArray(temp1, 1080 / m_ResolutionFraction, 1200 / m_ResolutionFraction, 2, 0, FilterMode.Bilinear);
                //Initial Copy
                commandBuffer.BeginSample("Copy and Downsample Screen");
                commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, temp0);
                commandBuffer.EndSample("Copy and Downsample Screen");

                commandBuffer.BeginSample("Blur");
                for (int i = 0; i < m_Iterations; i++)
                {
                    commandBuffer.BeginSample("Blur Iteration " + i);

                    //Horizontal
                    commandBuffer.SetGlobalVector("offsets", new Vector2(((1 + i) * 2 * m_Distance) / Screen.width, 0));
                    commandBuffer.Blit(temp0, temp1, blurMaterial, 0);

                    //Vertical
                    commandBuffer.SetGlobalVector("offsets", new Vector2(0, ((1 + i) * 2 * m_Distance) / Screen.height));
                    commandBuffer.Blit(temp1, temp0, blurMaterial);
                    commandBuffer.EndSample("Blur Iteration " + i);
                }
                commandBuffer.EndSample("Blur");

                commandBuffer.SetGlobalTexture("_GrabBlurTexture", temp0);
                registeredEvent = m_ExecutionEvent;
                m_Camera.AddCommandBuffer(registeredEvent, commandBuffer);
            }
        }

        private void ClearCameraCommandBuffer()
        {
            if (commandBuffer == null || !m_Camera)
            {
                return;
            }

            m_Camera.RemoveCommandBuffer(registeredEvent, commandBuffer);
            commandBuffer = null;
            DestroyImmediate(blurMaterial);
        }

        private void OnEnable()
        {
            CreateCommandBuffer();
        }

        private void OnDisable()
        {
            ClearCameraCommandBuffer();
        }

        private void OnValidate()
        {
            if (!gameObject.activeInHierarchy || !enabled)
            {
                return;
            }

            ClearCameraCommandBuffer();
            CreateCommandBuffer();
        }
    }
}