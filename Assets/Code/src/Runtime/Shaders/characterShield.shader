Shader "Gabo7/Characters/Character Shields" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_BumpMap ("Normal Map", 2D) = "bump" {}
		//_BumpScale("Normal Intensity", Range(0.01,7)) = 1.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
		_Glossiness("Glossiness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_ScrollXSpeed("X scroll speed", Range(-10, 10)) = 0
		_ScrollYSpeed("Y scroll speed", Range(-10, 10)) = -0.4 
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		Blend One One
		//ZTest Always
		CGPROGRAM
//#pragma surface surf Lambert
#pragma surface surf Standard
			struct Input {
				float4 color : COLOR;
				float2 uv_MainTex;
				//float2 uv_BumpMap;
				float3 viewDir;
};
			float4 _Color;
			sampler2D _MainTex;
			//sampler2D _BumpMap;
			float4 _RimColor;
			float _RimPower;
			//float _BumpScale;
			half _Glossiness;
			half _Metallic;
			fixed _ScrollXSpeed;
			fixed _ScrollYSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed offsetX = _ScrollXSpeed * _Time;
			fixed offsetY = _ScrollYSpeed * _Time;
			fixed2 offsetUV = fixed2(offsetX, offsetY);

			//fixed2 normalUV = IN.uv_NormalTex + offsetUV;
			fixed2 mainUV = IN.uv_MainTex + offsetUV;

			IN.color = _Color;
			o.Albedo = (tex2D(_MainTex, mainUV) * IN.color).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			//normal.z = normal.z / _BumpScale;
			//o.Normal = normalize(normal);

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
