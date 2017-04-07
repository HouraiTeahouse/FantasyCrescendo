// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Unlit/Transparent Color" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
		LOD 100

		Pass{
			Cull Off 
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			struct appdata_t {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			fixed4 _Color;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				return o;
			}

			fixed4 frag(v2f i) : COLOR {
				return _Color;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Color"
}
