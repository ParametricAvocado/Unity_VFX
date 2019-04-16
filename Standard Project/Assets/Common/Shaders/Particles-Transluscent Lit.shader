Shader "Particles/Transluscent Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" }

		
        CGPROGRAM
        #pragma surface surf Transluscent nolightmap nometa keepalpha vertex:vert alpha:blend
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:vertInstancingSetup
		#pragma target 3.0

		#include "UnityStandardParticles.cginc"

		inline fixed4 TransluscentLight(SurfaceOutputStandard s, UnityLight light)
		{
			fixed4 c;
			c.rgb = s.Albedo * light.color;
			c.a = s.Alpha;
			return c;
		}
		
		inline half4 LightingTransluscent(SurfaceOutputStandard s, float3 viewDir, UnityGI gi)
		{
			fixed4 c;
			c = TransluscentLight(s, gi.light);

			#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
			#endif
			return c;
		}

		inline void LightingTransluscent_GI(
			SurfaceOutputStandard s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, s.Normal);
		}
		ENDCG
	}
    FallBack "VertexLit"
}
