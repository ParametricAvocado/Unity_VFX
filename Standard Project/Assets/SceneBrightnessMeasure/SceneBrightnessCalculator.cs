using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneBrightnessCalculator : MonoBehaviour {
    [SerializeField] private ComputeShader m_Shader;

    [SerializeField] [Range(0f,1f)] private float m_AverageBrightness;
    
    private ComputeBuffer frameDataBuffer;

    private struct FrameData {
        public float brightness;
    };

    private const int Resolution = 8;
    
    private readonly FrameData[] frameData = new FrameData[Resolution * Resolution];
    public float AverageBrightness => m_AverageBrightness;

    private void Start() {
        frameDataBuffer = new ComputeBuffer(frameData.Length, sizeof(float), ComputeBufferType.Default);
        
        m_Shader.SetFloat("resolution", Resolution);
    }


    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        m_Shader.SetTexture(0,"source_texture", src);
            
        frameDataBuffer.SetData(frameData);
        m_Shader.SetBuffer(0, "frame_data", frameDataBuffer);
            
        m_Shader.Dispatch(0, 1, 1, 1);
        frameDataBuffer.GetData(frameData);
        
        m_AverageBrightness = frameData.Sum(data => data.brightness)/ frameData.Length;
        Graphics.Blit(src,dest);
    }
}
