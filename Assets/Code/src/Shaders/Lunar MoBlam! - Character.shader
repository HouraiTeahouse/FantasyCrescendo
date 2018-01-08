Shader "Gabo7/Characters/Lunar MoBlam! - Character" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", float) = 0.5
		_Cutoff2("Cutoff2", float) = 0.5
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap("Normalmap", 2D) = "bump" {}
	_BumpScale("Normal Intensity", Range(-7,7)) = 1.0
	_SpecMap("Specular map", 2D) = "black" {}
	_EmissionLevels("Emission Levels", 2D) = "white" {}
	_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	_RimConstant("Rim Constant", Range(0,1.5)) = 0

	} SubShader
	{ Tags{  "RenderType" = "Opaque" } Cull Off LOD 400 CGPROGRAM
	#pragma surface surf BlinnPhong alphatest:_Cutoff2
	#pragma target 3.0
	

	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _SpecMap;
	sampler2D _EmissionLevels;
	fixed4 _Color;
	half _Shininess;
	
	float4 _RimColor;
	float _RimPower;
	float _RimConstant;

	float _BumpScale;

	fixed _Cutoff;

	struct Input { 
		float2 uv_MainTex; 
		float2 uv_BumpMap; 
		float2 uv_SpecMap;
		float2 uv_EmissionLevels;
		float3 viewDir;
		fixed facing : VFACE;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 specTex = tex2D(_SpecMap, IN.uv_SpecMap);
		fixed4 emlvl = tex2D(_EmissionLevels, IN.uv_EmissionLevels);
		fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		normal.z = normal.z / _BumpScale;
		o.Albedo = tex.rgb *_Color.rgb;
		o.Gloss = specTex.r;
		o.Alpha = tex.a *_Color.a;
		o.Specular = _Shininess * specTex.g;
		o.Normal = normalize(normal);
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)) + _RimConstant;
		o.Emission = _RimColor.rgb * pow(rim, _RimPower) * emlvl;
		o.Alpha = tex.a;
		clip(o.Alpha - _Cutoff);
		//o.Normal = lerp(IN.facing, o.Normal, -o.Normal);
		if (IN.facing < 0.5)
			o.Normal *= -1.0;

	} ENDCG }

		FallBack "Specular" }
