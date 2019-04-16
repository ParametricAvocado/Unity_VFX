Shader "Unlit/TrailShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_ColorOverLifetime("ColorOverLifetime",2D) = "white"{}
		_MainTex("Texture", 2D) = "white" {}
		_FadeOverAge("FadeOverAge",Range(0,1)) = 1

		_DispVelocity("Velocity", Vector) = (0,0,0,0)
		_DispAcceleration("Gravity", Vector) = (0,0,0,0)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

			Pass
			{
				Stencil
				{
					ref 10
					comp notequal
					pass keep
					fail keep
					zfail keep
				}

				Zwrite off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
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
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _ColorOverLifetime;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				half4 _Color;
				float _FadeOverAge;
				float _SpawnTime;
				float _LifeSpan;
				float _Age;
				float4 _DispVelocity;
				float4 _DispAcceleration;


				void Displace(inout float4 vertex, float3 normal)
				{
					vertex.xyz += mul(unity_WorldToObject, _DispVelocity*_Age + _DispAcceleration * pow(_Age, 2));
				}

				v2f vert(appdata v)
				{
					v2f o;

					Displace(v.vertex,v.normal);

					o.vertex = UnityObjectToClipPos(v.vertex);

					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv)*tex2D(_ColorOverLifetime,_Age)*_Color;
					col.a *= 1 - (_FadeOverAge*_Age / _LifeSpan);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}
