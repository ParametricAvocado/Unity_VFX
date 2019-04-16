Shader "Unlit/Unlit-Fresnel"
{
	Properties
	{
		_Color("Color",Color) = (1.0,1.0,1.0,1.0)
		[PowerSlider(3)]_Fresnel("Fresnel",Range(0,8)) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		ColorMask RGB
		Lighting Off ZWrite Off
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
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float3 normal : TEXCOORD0;
				float3 viewDir : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			fixed4 _Color;
			float _Fresnel;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(v.normal);
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col =i.color* _Color *  pow(1-dot(i.viewDir,i.normal),_Fresnel);
				// apply fog
				UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0));
				return col;
			}
			ENDCG
		}
	}
}
