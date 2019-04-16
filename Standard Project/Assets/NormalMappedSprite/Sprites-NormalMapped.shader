// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Normal Mapped"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_BumpTex("Normal Map", 2D) = "bump" {}
		_Color("Tint", Color) = (1,1,1,1)

		[MaterialToggle] UseToonShading("Use Toon Shading", Float) = 0
		_ToonLightRange("Toon Light Range", Range(0,1)) = 0.5

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[HideInInspector] _SpriteFlip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma surface surf Sprite vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ USETOONSHADING_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
			#include "UnityCG.cginc"
			#include "UnityGlobalIllumination.cginc"

			sampler2D _BumpTex;
			fixed2 _SpriteFlip;
			struct Input
			{
				float2 uv_MainTex;
				fixed4 color;
			};

			void vert(inout appdata_full v, out Input o)
			{
				v.vertex = UnityFlipSprite(v.vertex, _Flip);

				#if defined(PIXELSNAP_ON)
				v.vertex = UnityPixelSnap(v.vertex);
				#endif

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = v.color * _Color * _RendererColor;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
				o.Albedo = c.rgb * c.a;

				o.Alpha = c.a;
				o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_MainTex));
				o.Normal.xy *= _SpriteFlip;
			}

			fixed _ToonLightRange;

			inline fixed4 ToonLight(SurfaceOutput s, UnityLight light)
			{
				fixed diff = max(0, dot(s.Normal, light.dir));

				diff = ceil(diff - _ToonLightRange);

				fixed4 c;
				c.rgb = s.Albedo * light.color * diff;
				c.a = s.Alpha;
				return c;
			}

			inline fixed4 LightingSprite(SurfaceOutput s, UnityGI gi)
			{
				fixed4 c;

#ifdef USETOONSHADING_ON
				c = ToonLight(s, gi.light);
#else
				c = UnityLambertLight(s, gi.light);
#endif

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
				c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

				return c;
			}


			inline void LightingSprite_GI(
				SurfaceOutput s,
				UnityGIInput data,
				inout UnityGI gi)
			{
				gi = UnityGlobalIllumination(data, 1.0, s.Normal);
			}
			ENDCG
		}

			Fallback "Sprites/Diffuse"
}
