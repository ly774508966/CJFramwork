Shader "ysShader/Distortion/distortion with no render" 
{
Properties {
	_TintColor("颜色叠加",Color) = (1,1,1,1)
	_MainTex ("主贴图", 2D) = "white" {}
	_NoiseTex ("纵向扭曲噪波 (RG)", 2D) = "white" {}
	_NoiseTex_2("横向扭曲噪波 (RG)", 2D) = "white" {}
	_MaskTex ("遮罩(R)",2D) = "white" {}
	_HeatTime  ("纵向扭曲速度", range (-1.5,1.5)) = 1
	_HeatTime_2("横向扭曲速度", range(-1.5,1.5)) = 1
	_HeatForce  ("扭曲力度", range (-1.5,1.5)) = 0.1
}

Category {
	Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
	Blend srcalpha oneminussrcalpha
	AlphaTest Greater .01
	Cull Off Lighting Off ZWrite Off
	

	SubShader {
		//GrabPass {							
		//	Name "BASE"
		//	Tags { "LightMode" = "Always" }
 	//	}

		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord: TEXCOORD0;
};

struct v2f {
	float4 vertex : POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvmain : TEXCOORD1;
	float2 uvmask : TEXCOORD2;
};

float _HeatForce;
float _HeatTime;
float _HeatTime_2;
float4 _MainTex_ST;
float4 _NoiseTex_ST;
float4 _NoiseTex_2_ST;
float4 _MaskTex_ST;
sampler2D _NoiseTex;
sampler2D _NoiseTex_2;
sampler2D _MainTex;
sampler2D _MaskTex;

float4 _TintColor;
v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.uvmask = TRANSFORM_TEX(v.texcoord, _MaskTex );
	return o;
}



half4 frag( v2f i ) : COLOR
{

	//noise effect
	half4 offsetColor1 = tex2D(_NoiseTex, i.uvmain + _Time.xz*_HeatTime);
    half4 offsetColor2 = tex2D(_NoiseTex_2, i.uvmain - _Time.zx*_HeatTime_2);
	i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce;
	i.uvmain.y += ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce;

	half4 tint = tex2D( _MainTex, i.uvmain);
	half4 mask = tex2D(_MaskTex,i.uvmask);
	tint.a = tint.a* mask.r;

	return tint*_TintColor;
}
ENDCG
		}
}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}
}
