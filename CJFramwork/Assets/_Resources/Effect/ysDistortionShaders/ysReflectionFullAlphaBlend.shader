Shader "ysShader/Reflection/Reflection full effect alphablend" 
{
Properties {
	_TintColor("颜色修正",Color) = (1,1,1,1)
	_MainTex ("主贴图", 2D) = "black" {}
	_NoiseTex_v ("纵向扭曲噪波 (RG)", 2D) = "white" {}
	_NoiseTex_u("横向扭曲噪波 (RG)", 2D) = "white" {}
	_MaskTex ("遮罩(R)",2D) = "white" {}
	_RenderTex("相机视角图像(设置层使得该物体不被渲染)",2D) = "black" {}
	_HeatTime_v("纵向扭曲速度", range (-1.5,1.5)) = 1
	_HeatTime_u("横向扭曲速度", range(-1.5,1.5)) = 1
	_HeatForce  ("扭曲力度", range (-1.5,1.5)) = 0.1
}

Category {
	Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
	Blend srcalpha oneminussrcalpha
	AlphaTest Greater .01
	Cull Off Lighting Off ZWrite Off
	SubShader {
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t {
	half4 vertex : POSITION;
	half4 color : COLOR;
	half2 texcoord: TEXCOORD0;
};

struct v2f {
	half4 vertex : POSITION;
	half4 uvgrab : TEXCOORD0;
	half2 uvmain : TEXCOORD1;
	half2 uvmask : TEXCOORD2;
};

half _HeatForce;
half _HeatTime_v;
half _HeatTime_u;
half4 _MainTex_ST;
half4 _NoiseTex_V_ST;
half4 _NoiseTex_u_ST;
half4 _MaskTex_ST;
sampler2D _NoiseTex_v;
sampler2D _NoiseTex_u;
sampler2D _MainTex;
sampler2D _MaskTex;
sampler2D _RenderTex;
sampler2D _RenderTex_ST;
half4 _TintColor;
v2f vert (appdata_t v)
{
	v2f o;
	UNITY_INITIALIZE_OUTPUT(v2f,o);
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.uvmask = TRANSFORM_TEX(v.texcoord, _MaskTex );

	//android真机表现正确,pc_android纵向相反
	//o.uvgrab = ComputeGrabScreenPos(o.vertex);

	//////////////////////pc_andriod平台下使用
	float scale = 1.0;
	o.uvgrab.xy = (float2(o.vertex.x*-1, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	//////////////////////

	return o;
}

half4 frag( v2f i ) : COLOR
{
	//noise effect
	half4 offsetColor1 = tex2D(_NoiseTex_v, i.uvmain + _Time.xz*_HeatTime_v);
    half4 offsetColor2 = tex2D(_NoiseTex_u, i.uvmain - _Time.zx*_HeatTime_u);
	i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce;
	i.uvmain.y += ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce;
	i.uvgrab.x += ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce;
	i.uvgrab.y += ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce;
	half4 tint = tex2D( _MainTex, i.uvmain);
	half4 render = tex2Dproj(_RenderTex,UNITY_PROJ_COORD(i.uvgrab));
	half4 mask = tex2D(_MaskTex,i.uvmask);
	tint.a = tint.a* mask.r;
	half4 final = tint * tint.a + render*(1 - tint.a);
	final *= _TintColor;
	final.a *= mask.r;
	return final;
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
