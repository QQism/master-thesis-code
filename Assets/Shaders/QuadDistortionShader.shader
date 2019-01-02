Shader "Custom/QuadDistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_CustomColor("Custom Color", Color) = (.34, .85, .92, 1)
		_Transparency("Transparency", Range(0, 1)) = 0.8
		_TickColor("Tick Color", Color) = (0, 0, 0, 1)
		_TickThickness("Tick Thickness", Range(0, 1)) = 0.01
		_LevelScale("Scale Level", Range(0, 1)) = 0
		_OnTop("On Top?", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 og_vertex : TEXCOORD1;
			};

			sampler2D _MainTex;
			float _UpperScale;
			float _LowerScale;
			float4 _CustomColor; 
			float _Transparency;
			float4 _TickColor;
			float _TickThickness;

			v2f vert (appdata v)
			{
				v2f o;

				float avg = (_UpperScale + _LowerScale)/2.0;

				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.og_vertex = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				//fixed4 col = tex2D(_MainTex, i.uv) * _TickColor;
				float4 color = _CustomColor;
				float og_step = 0.5;
				float step = (og_step * 2);
				float temp = (i.og_vertex.z + 1) % step;
				if (temp >= (og_step - 0.00001))
					color = _TickColor;

				fixed4 col = tex2D(_MainTex, i.uv) * color;

				col.a = _Transparency;
				return col;
			}
			ENDCG
		}
	}
}
