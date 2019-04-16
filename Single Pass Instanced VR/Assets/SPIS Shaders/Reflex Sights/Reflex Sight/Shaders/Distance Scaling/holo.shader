// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Transparent/Holographic Sight" {

	Properties {
		_reticleTex ("Reticle (RGB)", 2D) = "white" {}
		_reticleColour ("Reticle Colour", Color) = (1,1,1,1)
		_reticleBright ("Reticle Brightness", Range(0,1)) = 1
		_reticleOverbrightness("Reticle Overbrightness", Range(0,1)) = 0.3
		_glassColour ("Glass Colour", Color) = (1,1,1,1)
		_glassTrans ("Glass Transparency", Range(0,1)) = 0.1
		_glassSmoothness("Glass Smoothness", Range(0,1)) = 0.9
		_uvScale ("Reticle Scale", Range(0.01,20)) = 1
		_distance ("Reticle Distance", float) = 50
	}

	SubShader {
		Tags {"Queue"="transparent" "RenderType"="transparent" }
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#include "UnityCG.cginc"

		sampler2D _reticleTex;
		float _uvScale;
		float _glassTrans;
		float _reticleBright;
		float _distance;
		float _glassSmoothness;
		float _reticleOverbrightness;
		float4 _reticleColour;
		float4 _glassColour;

		struct Input {
			float3 worldRefl;
			float3 worldPos;
			float3 worldNormal;
		};
		
		void surf (Input IN, inout SurfaceOutputStandard o) {		
			//project (camera - point) vector onto normal of surface to find shortest distance from camera to surface		
			//add distance to reticle to get yComponent of right triangle with reticle and camera connected by hypotenuse
			float yComponent = dot (_WorldSpaceCameraPos - IN.worldPos,IN.worldNormal) + _distance;
			
			float3 normalizedViewVector = normalize(IN.worldPos - _WorldSpaceCameraPos);
			//find length of hypotenuse
			float hypotenuse = yComponent/dot(normalize(-IN.worldNormal), normalizedViewVector);
			
			//extend point - camera along hypotenuse to plane on which reticle lies
			float3 offsetVector = _WorldSpaceCameraPos + normalizedViewVector * hypotenuse;
			float4 offsetPoint;
			offsetPoint.xyz = offsetVector.xyz;
			offsetPoint.w = 1;
			
			//find center of sight window (assume at 0,0 in object space) and offset by distance along -normal
			//take offset point and center of reticle to object space and get difference between them in xy plane
			//multiply by unit scale factor to get uv delta
			float2 uv_Delta = (mul(unity_WorldToObject,offsetPoint) - float4(0,0,_distance,1)).xy / _uvScale;
			
			//sample texture using uv delta
			half reticleBlend = tex2D(_reticleTex,0.5f+ uv_Delta).a* _reticleBright;
			half4 reticleColor = half4(_reticleColour.rgb, 1.0);
			half4 glass = half4(_glassColour.rgb, _glassTrans);

			o.Emission = reticleColor.rgb * reticleBlend + _reticleOverbrightness * pow(reticleBlend,3);
			o.Smoothness = _glassSmoothness;
			o.Albedo = lerp(glass.rgb, reticleColor.rgb, reticleBlend);
			o.Alpha = lerp(glass.a, reticleColor.a, reticleBlend);;
		}
		
		ENDCG
	}
}
