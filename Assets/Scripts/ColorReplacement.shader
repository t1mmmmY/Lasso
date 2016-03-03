// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/ColorReplacement" {
Properties {
	_BaseColor ("Base Color", Color) = (1,1,1,1)
	_Sensitivity ("Sensitivity", Range(0,1)) = 0.5
	_TargetColor ("Target Color", Color) = (1,1,1,0)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
//			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
                fixed4 color    : COLOR;
				half2 texcoord : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed4 _BaseColor;
			fixed4 _TargetColor;
			
			fixed _Sensitivity;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
//				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
			
				fixed4 col = tex2D(_MainTex, i.texcoord);
				
//				clip(col.a - _Sensitivity);
//				clip(_BaseColor);
				
				if (col.r >= _BaseColor.r - _Sensitivity && col.r <= _BaseColor.r + _Sensitivity &&
		    		col.g >= _BaseColor.g - _Sensitivity && col.g <= _BaseColor.g + _Sensitivity &&
		    		col.b >= _BaseColor.b - _Sensitivity && col.b <= _BaseColor.b + _Sensitivity)
				{
					col = _TargetColor;
				}
				
				
//				if(any(i.color.rgb == _BaseColor))
//				{
//					col = _TargetColor;
//				}
				
//				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}
