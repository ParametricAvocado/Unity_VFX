Shader "Standard/Concrete Pillar"
{
	Properties
	{
		[KeywordEnum(UV,World)]COORD_SYSTEM("Coordinate System",Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_MainScale("Scale", Float) = 20
		_Glossiness("Base Smoothness", Range(0,1)) = 0.1

		[Header(Detail)]
		_DetailBlend("Detail Blend", Range(0, 1)) = 0.5
		_DetailTex("Detail",2D) = "white" {}

		[NoScaleOffset]_MaskTex("Mask", 2D) = "black" {}

		[Header(Darken Settings)]
		_DarkenScale("Darken Scale", Float) = 1
		_Darken("Darken", Range(0,1)) = 0.3

		[Header(Grime Settings)]
		_GrimeScale("GrimeScale", Float) = 1
		_GrimeIntensity("Grime Intensity", Range(0,1)) = 0.3
		_GrimeColor("Grime Color", Color) = (0,0,0,0)

		[Header(Puddle Settings)]
		_PuddleScale("Puddle Scale", Float) = 1
		_PuddleIntensity("Puddle Intensity", Range(0,1)) = 0.2
		_PuddleSlope("Puddle Slope", Range(0,1)) = 0.4
		_PuddleSmoothness("Puddle Smoothness", Range(0,1)) = 0.98
		_PuddleMetalness("Puddle Metalness", Range(0,1)) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert
			#pragma multi_compile COORD_SYSTEM_UV COORD_SYSTEM_WORLD
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _DetailTex;
			sampler2D _MaskTex;

			half _Glossiness;
			half _Metallic;

			fixed4 _Color;
			float _DetailBlend;
			
			float _GrimeScale;
			fixed4 _GrimeColor;
			fixed _GrimeIntensity;
			
			float _DarkenScale;
			fixed _Darken;

			float _PuddleScale;
			fixed _PuddleIntensity;
			float _PuddleSlope;
			float _PuddleSmoothness;
			float _PuddleMetalness;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
				float2 texcoord:TEXCOORD0;
				float2 texcoord1:TEXCOORD1;
				float2 texcoord2:TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_DetailTex;
				float2 uv_MaskTex;
				float3 worldNormal;
			};

			void vert(inout appdata v)
			{
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
#ifdef COORD_SYSTEM_WORLD
				v.texcoord.xy = worldPos.xz;
#endif
			}

			void SampleBaseAndDetail(Input IN,out fixed4 base, out fixed4 detail)
			{
				base = tex2D(_MainTex, IN.uv_MainTex);
				detail = tex2D(_DetailTex, IN.uv_MainTex);
			}

			void SampleMasked(Input IN, out fixed darken, out fixed grime, out fixed puddle)
			{
				darken = tex2D(_MaskTex, IN.uv_MaskTex/_DarkenScale).r;
				grime = tex2D(_MaskTex, IN.uv_MaskTex / _GrimeScale).g;
				puddle = tex2D(_MaskTex, IN.uv_MaskTex/ _PuddleScale).b;
			}

			float PuddleSlope(Input IN)
			{
				return saturate(IN.worldNormal.y + _PuddleSlope);
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 base, detail;
				fixed darken, grime, puddle;
				
				SampleBaseAndDetail(IN, base, detail);
				SampleMasked(IN, darken, grime, puddle);
				//Base
				o.Albedo = lerp(base,detail, _DetailBlend) *_Color;

				//Darken
				o.Albedo = o.Albedo * (1 - _Darken * darken);
				//Grime
				float grimeBlend = grime*_GrimeIntensity;
				o.Albedo = lerp(o.Albedo, _GrimeColor, grimeBlend);
				o.Smoothness = lerp(_Glossiness, 0, grimeBlend);

				//Puddle
				float puddleBlend = smoothstep(0.8 - _PuddleIntensity, 1 - _PuddleIntensity, puddle*PuddleSlope(IN));
				o.Metallic = lerp(0, _PuddleMetalness, puddleBlend);
				o.Smoothness = lerp(o.Smoothness, _PuddleSmoothness,puddleBlend);
			}
			ENDCG
		}
			FallBack "Diffuse"
}
