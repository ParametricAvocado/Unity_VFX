Shader "Blood Orb/Blood Orb Displacement"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Albedo (RGB)", 2D) = "white" {}
		_DispTile("Displace Density", Range(0,1)) = 1
		_DispIntensity("Displace Intensity", Range(0,1)) = 1
		_DispScroll("Displace Scroll", Float) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Tess("Tessellation",Range(0,3)) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard vertex:disp addshadow fullforwardshadows tessellate:tessFixed

			#pragma target 3.0

			sampler2D _MainTex;

			struct VertexInput
			{
				float4 vertex: POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float2 texcoord2 : TEXCOORD2;
				float4 color : COLOR;
			};

			struct Input
			{
				half4 color : COLOR;
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float _DispIntensity;
			float _DispTile;
			float _DispScroll;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)
			

			float _Tess;

			float4 tessFixed()
			{
				return _Tess;
			}

			void disp(inout VertexInput v)
			{
				float2 scroll = float2(_DispScroll*_Time.y, 0);

				float3 triplanar = v.normal.x* (tex2Dlod(_MainTex, float4(v.vertex.zy*_DispTile + scroll.yx, 0, 0)).rgb * 2 - 1);
				triplanar += v.normal.y* (tex2Dlod(_MainTex, float4(v.vertex.xz*_DispTile + scroll.yx, 0, 0)).rgb * 2 - 1);
				triplanar += v.normal.z* (tex2Dlod(_MainTex, float4(v.vertex.xy*_DispTile + scroll.yx, 0, 0)).rgb * 2 - 1);


				float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;
				v.normal = normalize(v.normal + v.tangent.xyz*triplanar.x + binormal * triplanar.y);

				v.vertex.xyz += v.normal*triplanar.z*_DispIntensity;
#ifdef BLOODORB_DEBUG_COLOR
				v.color.rgb = triplanar/2+0.5;
#else
				v.color.rgb = 1-pow(1-(triplanar.b/2+0.5),3);
#endif
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c =  _Color;
#ifdef BLOODORB_DEBUG_COLOR
				o.Albedo = IN.color;
#else
				o.Albedo = c.rgb*IN.color;
#endif
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
		FallBack "Standard"
}
