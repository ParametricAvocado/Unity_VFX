Shader "Particles/ScrollFlames" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_ScrollTex("ScrollTex",2D) = "white"{}
		_ColorRamp("Color Ramp",2D) = "white"{}
		[KeywordEnum(UV,Polar)]_ScrollType("Scroll Type",Float) = 1
		[Toggle(USESOFT_ON)]_UseSoft("Use Soft Particles", Float) = 1
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
			Blend SrcAlpha One
			ColorMask RGB
			Offset -5,0
			Cull Off Lighting Off ZWrite Off

			SubShader {
				Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
					#pragma multi_compile_particles
					#pragma multi_compile_fog
					#pragma multi_compile _USESOFT_OFF USESOFT_ON 
					#pragma multi_compile _SCROLLTYPE_UV _SCROLLTYPE_POLAR

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					sampler2D _ScrollTex;
					sampler2D _ColorRamp;
					fixed4 _TintColor;

					struct appdata_t {
						float4 vertex : POSITION;
						fixed4 color : COLOR;
						float2 texcoord0 : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						fixed4 color : COLOR;
						float2 texcoord0 : TEXCOORD0;
						float2 texcoord1 : TEXCOORD3;
						UNITY_FOG_COORDS(1)
						#ifdef SOFTPARTICLES_ON
						#ifdef USESOFT_ON
						float4 projPos : TEXCOORD2;
						#endif
						#endif
						UNITY_VERTEX_OUTPUT_STEREO
					};

					float4 _MainTex_ST;
					float4 _ScrollTex_ST;
					float4 _ColorRamp_ST;
					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef SOFTPARTICLES_ON
#ifdef USESOFT_ON
						o.projPos = ComputeScreenPos(o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
#endif
#endif

						o.color = v.color;
						o.texcoord0 = TRANSFORM_TEX(v.texcoord0,_MainTex);
						o.texcoord1 = TRANSFORM_TEX(v.texcoord0,_ScrollTex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						return o;
					}

					sampler2D_float _CameraDepthTexture;
					float _InvFade;

					fixed4 frag(v2f i) : SV_Target
					{
						float2 scroll0 = _ColorRamp_ST.zw*_Time.y;
						float2 scroll1;
						scroll1.x = -scroll0.x;
						scroll1.y = scroll0.y;
#ifdef _SCROLLTYPE_UV
						float2 uv = i.texcoord1;
#elif _SCROLLTYPE_POLAR
						float2 uv = float2(atan2(i.texcoord1.x, i.texcoord1.y) / 3.14, pow(length(i.texcoord1) * 8,0.4));
#endif

						fixed base = i.color.a * tex2D(_MainTex, i.texcoord0).r;

						fixed mask = pow(base,8) + base * tex2D(_ScrollTex, uv + scroll0).r*tex2D(_ScrollTex, -uv - scroll1).r;

						fixed4 col = tex2D(_ColorRamp, mask);

#ifdef SOFTPARTICLES_ON
#ifdef USESOFT_ON
						float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate(_InvFade * (sceneZ - partZ));
						col *= fade;
#endif
#endif
						UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
						return col;
					}
					ENDCG
				}
			}
		}
}
