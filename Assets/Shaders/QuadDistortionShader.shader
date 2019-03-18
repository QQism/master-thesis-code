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
		_Angle("Angle", Float) = 0
		_QuadAngle("Quad Angle", Float) = 0
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
				float3 worldPos : TEXCOORD2;
				float3 worldNormal: TEXCOORD3;
			};

			sampler2D _MainTex;
			sampler2D _CustomTex;
			float4 _MainTex_ST;
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
			float _Angle;
			float _QuadAngle;

			v2f vert (appdata v)
			{
				v2f o;

				float avg = (_UpperScale + _LowerScale)/2.0;

				v.vertex.x *=  avg + (v.vertex.z * (_UpperScale - avg));

				o.vertex = UnityObjectToClipPos(v.vertex);
				/*
				float rot = v.uv.x * _Angle + _QuadAngle;
				float r = v.uv.y * 0.5;
				v.uv.x = sin(rot) * r;
				v.uv.y = cos(rot) * r;
				*/

				float2 midpoint = float2(0.5, 0.5);
				//v.uv.y = (v.uv.y / 2);
				//v.uv.y = (v.uv.y  + 1) / 2 ;
				//v.uv += midpoint;
				//if (v.uv.y <= 1.5 && v.uv.y >= 1.0)
				//	v.uv.y = 0;
				//v.uv.y = 1/2 * v.uv.y + 1/4;
				//v.uv.x *= avg + (v.uv.y * (_UpperScale - avg));

				// Get the xy position of the vertex in worldspace
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
				o.uv = v.uv;
				//o.uv = TRANSFORM_TEX(worldXYZ, _MainTex);
				o.og_vertex = v.vertex;
				return o;
			}

			float when_lt(float x, float y)
			{
				return max(sign(y - x), 0.0);
			}

			float when_gt(float x, float y)
			{
				return max(sign(x - y), 0.0);
			}

			float when_ge(float x, float y)
			{
				return 1.0 - when_lt(x, y);
			}

			float when_le(float x, float y)
			{
				return 1.0 - when_gt(x, y);
			}

			float and(float x, float y)
			{
				return x * y;
			}

			float or(float x, float y)
			{
				return min(x + y, 1.0);
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

				
				// 1 if is tick, 0 if not 
				float condition = and(and(
										when_lt(topOffset, full_range),
										when_gt(bottomOffset, 0)),
									or(
										when_ge(tick_pos, step - _TickThickness),
										when_le(tick_pos, _TickThickness)));
				float4 color = (_TickColor * condition + _CustomColor * !condition);
				float transparency = (_TickTransparency * condition + _Transparency * !condition);

				//col = tex2D(_MainTex, i.uv) * _CustomColor;
				col = tex2D(_MainTex, i.uv) * color;
				col.a = transparency;

				/*
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
				*/		

				//col = tex2D(_MainTex, i.uv) * _CustomColor;
				//col.a = _Transparency;

				// Experiment.....
				// Apply texture - Begin
				fixed2 new_uv = i.uv;
				float3 c1 = tex2D(_MainTex, i.worldPos.xy).rgb;
				float3 c2 = tex2D(_MainTex, i.worldPos.zx).rgb;
				float3 c3 = tex2D(_MainTex, i.worldPos.zy).rgb;

				//float alpha21 = abs(i.worldNormal.x);
				//float alpha23 = abs(i.worldNormal.z);

				float3 c21 = lerp(c2, c1, 0.5).rgb;
				float3 c23 = lerp(c21, c3, 0.5).rgb;
				// Apply texture - End
				
				col = tex2D(_MainTex, new_uv);
				col.x = c23.r;
				col.y = c23.g;
				col.z = c23.b;
				col.x = 1;
				return col;
			}
			ENDCG
		}
	}
}
