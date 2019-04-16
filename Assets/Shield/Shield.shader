// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Shield" {
	Properties{
		_Color("Base Color",Color) = (1,1,1,1)
		_AccentColor("Accent Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_TexWeight("Texture Weight", Range(0,1)) = 0.2
		_InvFade("Depth Sensitivity",Range(0.01,3.0)) = 1.0
		[PowerSlider(3)]_Fresnel("Fresnel",Range(0,64)) = 1
		[KeywordEnum(Off, Full, Vertical, Spiral, Sphere, Fresnel)] _Pulse("Pulse", Float) = 0
	}
		SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
			Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off ZWrite Off
			CGINCLUDE
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
				float3 normal : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD4;
				#endif

#ifdef _PULSE_VERTICAL
				float3 wPos : TEXCOORD5;
#elif _PULSE_SPIRAL
				float3 lPos : TEXCOORD5;
#elif _PULSE_SPHERE
				float radius : TEXCOORD5;
#endif

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D_float _CameraDepthTexture;
			fixed4 _Color;
			fixed4 _AccentColor;
			float _TexWeight;
			float _InvFade;
			float _Fresnel;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef _PULSE_VERTICAL
				o.wPos = mul(unity_ObjectToWorld,v.vertex);
#elif _PULSE_SPIRAL
				o.lPos = v.vertex;
#elif _PULSE_SPHERE
				o.radius = length(v.vertex);
#endif
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.normal = normalize(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 col;
				float fade = saturate(pow(abs(dot(i.viewDir,i.normal)),_Fresnel));

#ifdef _PULSE_OFF
				fade -= tex2D(_MainTex, i.uv).r*_TexWeight;
#elif _PULSE_FULL
				fade -= (tex2D(_MainTex, i.uv).r + pow(sin(_Time.w * 2),4))*_TexWeight;
#elif _PULSE_VERTICAL
				fade -= (tex2D(_MainTex, i.uv).r - sin(_Time.w * 3 + i.wPos.y * 10) / 3) *_TexWeight;
#elif _PULSE_SPIRAL
				fade -= (tex2D(_MainTex, i.uv).r + pow(saturate(sin(_Time.w * 2 + atan2(i.lPos.x, i.lPos.z) * 2 + i.lPos.y * 6)),7))*_TexWeight;
#elif _PULSE_SPHERE
				fade -= (tex2D(_MainTex, i.uv).r + pow(saturate(sin(_Time.w * 2 - i.radius)),7))*_TexWeight;
#elif _PULSE_FRESNEL
				fade -= (tex2D(_MainTex, i.uv).r - sin(_Time.w * 3 + fade * 20) / 2)*_TexWeight;
#endif

				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				fade = min(fade,saturate(_InvFade * (sceneZ - partZ)));
				#else
				col = _Color;
				#endif
				col = lerp(_AccentColor, _Color, fade);
				//col.rgb = i.normal;
				// sample the texture
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}

			ENDCG

				/*Pass {
					Name "Inner Faces"
					Cull Front
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_particles
					#pragma multi_compile_fog
					ENDCG
				}*/

				Pass {
					Name "Outer Faces"
						//Cull Back
						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag
						#pragma multi_compile_particles
						#pragma multi_compile_fog
						#pragma multi_compile _PULSE_OFF _PULSE_FULL _PULSE_VERTICAL _PULSE_SPIRAL _PULSE_SPHERE _PULSE_FRESNEL

						ENDCG
					}
		}
}
