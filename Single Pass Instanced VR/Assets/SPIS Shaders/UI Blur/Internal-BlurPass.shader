
Shader "Hidden/UIBlurPass" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;

		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	float2 offsets;
	float _UI_BlurDistance;

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	uniform float4 _MainTex_ST;

	v2f vert(appdata_t v) {
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);

		o.uv01 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		o.uv45 = v.texcoord.xyxy + offsets.xyxy * float4(1, 1, -1, -1) * 3.0;

		return o;
	}

	half4 frag(v2f i) : COLOR{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 color = float4 (0,0,0,0);

		color += 0.40 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
		color += 0.15 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv01.xy);
		color += 0.15 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv01.zw);
		color += 0.10 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv23.xy);
		color += 0.10 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv23.zw);
		color += 0.05 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv45.xy);
		color += 0.05 * UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv45.zw);

		return color;
	}

		ENDCG

		Subshader {
		Pass{
			 ZTest Always Cull Off ZWrite Off
			 Fog { Mode off }

			 CGPROGRAM
			 #pragma fragmentoption ARB_precision_hint_fastest
			 #pragma vertex vert
			 #pragma fragment frag
			 ENDCG
		}
	}
	Fallback off
}
