Shader "Unlit/Keke_Master"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_ShadeColor("Shade Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex("Base", 2D) = "white" {}
		_Outline("Outline", Range(0,1)) = 0.1
		_DitherTex("Dither Tex", 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 SSPos : TEXCOORD6;
				float3 WSNormal : TEXCOORD7;
				float3 WSViewDir : TEXCOORD8;
			};

			sampler2D _MainTex;
			sampler2D _DitherTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			fixed4 _ShadeColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.SSPos = ComputeScreenPos(o.vertex);
				o.WSNormal = mul(unity_ObjectToWorld,v.normal);
				o.WSViewDir = WorldSpaceViewDir(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed shade = dot(i.WSNormal, i.WSViewDir);

				fixed2 aspect_ratio = fixed2(1, _ScreenParams.y / _ScreenParams.x);
				fixed d = tex2D(_DitherTex, (i.SSPos.xy / i.SSPos.w)*aspect_ratio*(_ScreenParams.x / 8)).r;


				return col * lerp(_ShadeColor,_Color,  step(d, shade));
			}
			ENDCG
		}
	}
}
