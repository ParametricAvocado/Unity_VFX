Shader "Blood Orb/Blood Orb Particle"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		[NoScaleOffset]_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" }

			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert addshadow alphatest:_Cutoff

			#pragma target 3.0

			sampler2D _MainTex;

			struct VertexInput {
				float4 vertex: POSITION;
				float4 texcoord: TEXCOORD0;
				float4 texcoord1: TEXCOORD1;
				float4 texcoord2: TEXCOORD2;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct Input
			{
				float4 texcoord;//xy for flipbook, zw for actual quad coords
				float4 color;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout VertexInput v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.texcoord = v.texcoord;
				o.color = v.color;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 t = tex2D(_MainTex, IN.texcoord.xy);
				float radius = pow(length(IN.texcoord.zw * 2 - 1),3);
				o.Albedo = _Color * IN.color.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Normal = UnpackNormal(t);
				o.Alpha = t.z - radius;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
