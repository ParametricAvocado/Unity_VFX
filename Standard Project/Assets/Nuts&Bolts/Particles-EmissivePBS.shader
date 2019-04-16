Shader "Particles/EmissivePBS" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map",2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader{
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:blend
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
			fixed4 color : COLOR;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Emission = IN.color*c;
			o.Alpha = c.a*IN.color.a;
		}
		ENDCG
	}
	FallBack "Standard"
}
