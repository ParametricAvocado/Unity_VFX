Shader "Sprite/Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump"{}
		_SmoothMetalAO("Smooth (R) Metal (G) AO (B)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
		Tags{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}
        LOD 200

		Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:blend
        #pragma target 3.0
		#include "SpriteUtilities.cginc"

		sampler2D _MainTex;
		sampler2D _NormalMap;
        sampler2D _SmoothMetalAO;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			o.Alpha = c.a;

			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			SPRITE_NORMALS_FLIP(o);


			fixed4 sma = tex2D(_SmoothMetalAO, IN.uv_MainTex);
			o.Smoothness = sma.r*_Glossiness;
            o.Metallic = sma.g*_Metallic;
			o.Occlusion = sma.b;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
