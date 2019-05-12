Shader "Unlit/DepthPrepass"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent-10" }
			ColorMask 0
			ZWrite On
			ZTest Lequal
		}
	}
}
