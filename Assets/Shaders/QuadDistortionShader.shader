Shader "Custom/QuadDistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UpperScale ("Upper Scale", Range(0, 5)) = 1
		_LowerScale ("Lower Scale", Range(0, 5)) = 0.6
		_CustomColor("Custom Color", Color) = (.34, .85, .92, 1)
		_Transparency("Transparency", Range(0, 1)) = 0.8
		_TickColor("Tick Color", Color) = (.75, .79, 0.8, 1)
		_TickTransparency("Tick Transparency", Range(0, 1)) = 1
		_TickThickness("Tick Thickness", Range(0, 1)) = 0.05
		_LevelScale("Scale Level", Range(0, 1)) = 0.5
		_TickStep("Tick Step", Range(0, 1)) = 0.25
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
			float _TickTransparency;
			float _TickThickness;
			float _LevelScale;
			int _OnTop;
			float _TickStep;

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
				fixed4 col;

				float offset, full_range, step, bottom_end, tick_pos, topOffset, bottomOffset;

				offset = 2 * (1 - _LevelScale) / _LevelScale;
				full_range = offset + 2;
				step = _TickStep * full_range;

				bottom_end = offset * _OnTop + 1;
				tick_pos = (i.og_vertex.z + bottom_end) % step;
				topOffset = i.og_vertex.z + bottom_end + _TickThickness;
				bottomOffset = i.og_vertex.z + bottom_end - _TickThickness;

				if ((topOffset < full_range && bottomOffset > 0) &&
					(tick_pos >= (step - _TickThickness) || tick_pos <= _TickThickness))
				{
					col = tex2D(_MainTex, i.uv) * _TickColor;
					col.a = _TickTransparency;
				} else
				{
					col = tex2D(_MainTex, i.uv) * _CustomColor;
					col.a = _Transparency;
				}
				
				return col;
			}
			ENDCG
		}
	}
}
