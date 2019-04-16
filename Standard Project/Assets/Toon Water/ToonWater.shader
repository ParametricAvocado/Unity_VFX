// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/ToonWater" {
	Properties{
		[Header(Body)]
		_Color("Shallow Color", Color) = (1,1,1,1)
		_DeepColor("Deep Color", Color) = (1,1,1,1)
		_DepthTransition("Depth Transition", Float) = 3
		_DepthFade("Depth Fade", Float) = 1

		[Header(Waves)]
		_WaveDirection("Direction", Vector) = (0,0,1,0)
		_WaveHeight("Maximum Height", Range(0.01,5)) = 1
		_WaveTip("Tip", Range(1,16)) = 3
		_WaveLength("Length", Range(0.1,10)) = 0.1
		_WaveSpeed("Speed", Range(0,2)) = 0.1
		_WaveNormalStrength("Normal Strength", Range(0.001,0.1)) = 1
		[Toggle(DEBUG_NORMALS)]_DebugNormals("Debug Normals",Float) = 0


		[Header(Foam)]
		_FoamDistance("Foam Distance", Range(0.01,1)) = 1

		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_FoamTex("Foam",2D) = "white"{}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		[Header(Tesselation)]
		_Tess("Amount", Range(1,32)) = 4
		_TessMinDist("Min Distance", Range(0, 200)) = 20
		_TessMaxDist("Max Distance", Range(1, 200)) = 50
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+10" "ForceNoShadowCasting" = "true" }
			LOD 200

			GrabPass
			{
				"_RefractionTex"
			}
			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tessDistance nolightmap
			#pragma target 4.6
			#include "Tessellation.cginc"
			#pragma shader_feature DEBUG_NORMALS
			fixed4 _MainTex_ST;

			UNITY_DECLARE_TEX2D(_MainTex);
			UNITY_DECLARE_TEX2D_NOSAMPLER(_FoamTex);
			sampler2D _RefractionTex;
			sampler2D _CameraDepthTexture;

			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct Input {
				float2 uv_MainTex;
				fixed4 color : COLOR;
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;

			fixed4 _Color;
			fixed4 _DeepColor;
			float _FoamDistance;
			float _DepthTransition;
			float _DepthFade;

			float4 _WaveDirection;
			float _WaveHeight;
			float _WaveLength;
			float _WaveTip;
			float _WaveSpeed;
			float _WaveNormalStrength;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)


			float _Tess;
			float _TessMinDist;
			float _TessMaxDist;
			float _WaterTime;

			float4 tessDistance(appdata v0, appdata v1, appdata v2) {
				return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TessMinDist, _TessMaxDist, _Tess);
			}

			float subwavegen(float3 pos, float3 dir, float length, float speed, float magnitude)
			{
				pos = _WaterTime * speed + (pos * dir) / length;
				return pow(sin(pos.x + pos.y + pos.z)* 0.5 + 0.5, _WaveTip) * magnitude;
			}

			float wavegen(float3 pos)
			{
				float3 wPos = mul(unity_ObjectToWorld, pos).xyz;
				float4 dir = _WaveDirection;

				float wave = subwavegen(wPos, dir.xwz, _WaveLength, _WaveSpeed, _WaveHeight);
				wave += subwavegen(wPos, dir.zwx, _WaveLength*0.7, _WaveSpeed, _WaveHeight);
				wave += subwavegen(wPos, dir.xwy, _WaveLength*1.3, _WaveSpeed, _WaveHeight);
				wave += subwavegen(wPos, dir.zwy, _WaveLength *0.9, _WaveSpeed, _WaveHeight);
				return wave / 4;
			}


			void vert(inout appdata v) {

				float4 wsVert = mul(unity_ObjectToWorld, v.vertex);
				float3 wsNorm = mul(unity_ObjectToWorld, v.normal);
				float4 wsTan = mul(unity_ObjectToWorld, v.tangent);
				float3 wsBin = cross(wsNorm, wsTan.xyz) / wsTan.w;
				float3 tanDist = wsTan.xyz*_WaveNormalStrength;
				float3 binDist = wsBin.xyz*_WaveNormalStrength;


				float3 offset = wsNorm * wavegen(wsVert.xyz);
				float3 tan = wsNorm * wavegen(wsVert + tanDist);
				float3 bin = wsNorm * wavegen(wsVert + binDist);
				float3 n = cross(normalize(bin + binDist - offset), normalize(tan + tanDist - offset));

				float3 ntan = wsNorm * wavegen(wsVert - tanDist);
				float3 nbin = wsNorm * wavegen(wsVert - binDist);
				n += cross(normalize(nbin - binDist - offset), normalize(ntan - tanDist - offset));

				n = normalize(n / 2);

				wsVert.xyz += offset;
				v.vertex = mul(unity_WorldToObject, wsVert);
				v.normal = mul(unity_WorldToObject, n);
				//v.tangent = mul(unity_WorldToObject, normalize(tan + tanDist - offset));
#ifdef DEBUG_NORMALS
				v.color = half4(v.normal,1);
#endif
			}


			void surf(Input IN, inout SurfaceOutputStandard o) {
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;

#ifdef DEBUG_NORMALS
				o.Albedo = IN.color;
				o.Alpha = 1;
				return;
#endif

				float4 projCoord = UNITY_PROJ_COORD(IN.screenPos);

				float prevDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, projCoord).r);
				float depth = prevDepth - IN.screenPos.w;
				float foam = 1 - (prevDepth - IN.screenPos.w) / _FoamDistance;

				//float edgeBlend = 1-saturate((prevZ-IN.packedData.z)/_Depth);

				//float surfaceDistance = length(_WorldSpaceCameraPos - IN.worldPos);
				//float prevDistance = length(_WorldSpaceCameraPos - prevProj.xyz);

				o.Albedo = lerp(tex2Dproj(_RefractionTex, IN.screenPos), lerp(_Color, _DeepColor, saturate(depth / _DepthTransition)), saturate(depth / _DepthFade));
				//o.Alpha = saturate(depth / _DepthTransition);
				return;

				//float2 foamCoords = edgeBlend-_Time.x;
				//float2 foamCoords2 = edgeBlend+_Time.x;

				//fixed4 wTex = tex2D(_MainTex, IN.packedData.xy) *_Color;
				//fixed4 fTex = tex2D(_FoamTex, foamCoords);
				//fTex = max(fTex,tex2D(_FoamTex, foamCoords2)/2) * _Color*2;

				//fixed4 c = IN.eyeDepth *_ProjectionParams.w;
				//o.Albedo =  lerp(wTex.rgb,fTex.rgb, fTex.a*edgeBlend);
				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				//o.Alpha = 1;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
