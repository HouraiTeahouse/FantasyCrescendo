// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SkyBoxBlend" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	_YOffset ("Y Offset", Range(-0.99,1)) = 0.0
	_SkyBlend ("Blend", Range(0.0, 1.0)) = 0.0

	[NoScaleOffset] _FrontTex_01 ("Front_01 [+Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _BackTex_01 ("Back_01 [-Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _LeftTex_01 ("Left_01 [+X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _RightTex_01 ("Right_01 [-X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _UpTex_01 ("Up_01 [+Y]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _DownTex_01 ("Down_01 [-Y]   (HDR)", 2D) = "grey" {}

	[NoScaleOffset] _FrontTex_02 ("Front_02 [+Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _BackTex_02 ("Back_02 [-Z]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _LeftTex_02 ("Left_02 [+X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _RightTex_02 ("Right_02 [-X]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _UpTex_02 ("Up_02 [+Y]   (HDR)", 2D) = "grey" {}
	[NoScaleOffset] _DownTex_02 ("Down_02 [-Y]   (HDR)", 2D) = "grey" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off
	
	CGINCLUDE
	#include "UnityCG.cginc"

	half4 _Tint;
	half _Exposure;
	float _Rotation;
	float _SkyBlend;
	float _YOffset;

	float4 RotateAroundZInDegrees (float4 vertex, float degrees, float yOffset)
	{
		float alpha = degrees * UNITY_PI / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);

		vertex =  float4(mul(m, vertex.xy), vertex.zw).xyzw;

		return float4(vertex.x, vertex.y + yOffset, vertex.zw).xyzw;
	}
	
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
	};
	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(RotateAroundZInDegrees(v.vertex, _Rotation, _YOffset));
		o.texcoord = v.texcoord;
		return o;
	}

	half4 skybox_frag (v2f i, sampler2D smp1, half4 smpDecode1, sampler2D smp2, half4 smpDecode2)
	{
		half4 tex1 = tex2D (smp1, i.texcoord);
		half4 tex2 = tex2D (smp2, i.texcoord);

		half3 c1 = DecodeHDR (tex1, smpDecode1);
		half3 c2 = DecodeHDR (tex2, smpDecode2);

		half3 c = lerp(c1, c2, _SkyBlend);

		c = c * _Tint.rgb * unity_ColorSpaceDouble;
		c *= _Exposure;
		return half4(c, 1);
	}
	ENDCG
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _FrontTex_01;
		sampler2D _FrontTex_02;
		half4 _FrontTex_01_HDR;
		half4 _FrontTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex_01, _FrontTex_01_HDR, _FrontTex_02, _FrontTex_02_HDR); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _BackTex_01;
		sampler2D _BackTex_02;
		half4 _BackTex_01_HDR;
		half4 _BackTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex_01, _BackTex_01_HDR, _BackTex_02, _BackTex_02_HDR); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _LeftTex_01;
		sampler2D _LeftTex_02;
		half4 _LeftTex_01_HDR;
		half4 _LeftTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex_01, _LeftTex_01_HDR, _LeftTex_02, _LeftTex_02_HDR); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _RightTex_01;
		sampler2D _RightTex_02;
		half4 _RightTex_01_HDR;
		half4 _RightTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex_01, _RightTex_01_HDR, _RightTex_02, _RightTex_02_HDR); }
		ENDCG
	}	
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _UpTex_01;
		sampler2D _UpTex_02;
		half4 _UpTex_01_HDR;
		half4 _UpTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex_01, _UpTex_01_HDR, _UpTex_02, _UpTex_02_HDR); }
		ENDCG
	}	
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _DownTex_01;
		sampler2D _DownTex_02;
		half4 _DownTex_01_HDR;
		half4 _DownTex_02_HDR;
		half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex_01, _DownTex_01_HDR, _DownTex_02, _DownTex_02_HDR); }
		ENDCG
	}
}
}
