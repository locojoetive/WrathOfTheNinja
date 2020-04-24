Shader "Camera/GrayScaleCamera"
{
	Properties
	{
		_BLACK_AND_WHITE("Black And White", Float) = 0
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float _BLACK_AND_WHITE;
			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);

				if (_BLACK_AND_WHITE > 0)
				{
					//Pixer luminance / Grayscale formula here
					float lum = 0.3 * color.r + 0.59 * color.g + 0.11 * color.b;
					float4 grayscale = float4(lum, lum, lum, color.a);
					return grayscale;
				}
				else {
					return color;
				}
				
			}
			ENDCG
		}
	}
}