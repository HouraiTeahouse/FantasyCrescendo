Shader "Custom/Anime Eye" {
	Properties {
		_Color ("Eye Color", Color) = (1,1,1,1)
		_BackTint("Back Tint", Color) = (1,1,1,1)
		_Top("Front Top Colorize", Color) = (1,1,1,1)
		_Bottom("Front Bottom Colorize", Color) = (0,0,0,1)
		_MainTex("Front", 2D) = "white" {}
		_Back("Back", 2D) = "white" {}
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Emission("Emission", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Back;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _Emission;
		fixed4 _Color;
		fixed4 _Top;
		fixed4 _Bottom;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 b = tex2D(_Back, IN.uv_MainTex);
			fixed greyScale = (c.r + c.g + c.b) / 3;
			fixed lerpVal = c.a;
			if (greyScale > 0.5)
				c = lerp(_Color, _Top, 2 * (greyScale - 0.5));
			else
				c = lerp(_Bottom, _Color, 2 * greyScale);
			c = lerp(b, c, lerpVal);
			o.Albedo = c.rgb;
			o.Emission = c.rgb * _Emission;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
