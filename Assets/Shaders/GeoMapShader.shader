// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/GeoMapShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _MainTex_ST;

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				//float2 worldXY = mul(unity_ObjectToWorld, v.vertex).xz; 
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//o.uv = TRANSFORM_TEX(worldXY, _MainTex);
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, 1-i.uv);
				//if (i.uv.x > 0.75 || i.uv.y > 0.75 || i.uv.x < 0.25 || i.uv.y < 0.25)
				//	col = (0, 0, 0, 0);
				return col;
			}
			ENDCG
		}
	}
}
