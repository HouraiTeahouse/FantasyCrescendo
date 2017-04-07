Shader "Custom/mushroomrimlight" {
	Properties {
		_ColorTint ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Intensity", Range(0.01,7)) = 1.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
#pragma surface surf Lambert
			struct Input {
				float4 color : COLOR;
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 viewDir;
};
			float4 _ColorTint;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			float4 _RimColor;
			float _RimPower;
			float _BumpScale;

		void surf (Input IN, inout SurfaceOutput o) {
			
			IN.color = _ColorTint;
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
			fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			normal.z = normal.z / _BumpScale;
			o.Normal = normalize(normal);

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
