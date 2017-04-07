Shader "Gabo7/Enviroment/Lunar MoBlam!" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Normal", 2D) = "bump" {}
		_BumpScale("Normal Intensity", Range(-7,7)) = 1.0
		_Glossiness("Glossiness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_TintColorA("Tint Color A", Color) = (1, 1, 1, 1)
		_TintColorB("Tint Color B", Color) = (1, 1, 1, 1)
		_TintMask("Tint Mask", 2D) = "black"
		_Occlusion("Occlusion Map", 2D) = "white" {}
		
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
//#pragma surface surf Lambert
#pragma surface surf Standard fullforwardshadows
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_TintMask;
	};
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _TintMask;
	sampler2D _Occlusion;

	fixed4 _TintColorA;
	fixed4 _TintColorB;
	float _BumpScale;

	half _Glossiness;
	half _Metallic;


	void surf(Input IN, inout /*SurfaceOutput*/ SurfaceOutputStandard o) {
		
		fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 mask = tex2D(_TintMask, IN.uv_TintMask);
		fixed4 Occ = tex2D(_Occlusion, IN.uv_MainTex);
		fixed maskA = mask.r; // red channel is the first mask
		fixed maskB = mask.g; // green channel is the second mask
		fixed3 tint = fixed3(1.0, 1.0, 1.0);
		tint = lerp(tint, _TintColorB, maskB);
		tint = lerp(tint, _TintColorA, maskA);
		albedo.rgb *= tint;

		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
	
		fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		normal.z = normal.z/_BumpScale;
		//o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		o.Albedo = albedo.rgb * Occ.rgb;

		//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		o.Normal = normalize(normal);

		
	}
	ENDCG
	}
		Fallback "Diffuse"
}