Shader "ysShader/Distortion/distortion simple" 
{
	Properties{
		_MainTex("主贴图(不设置)", 2D) = "black" {}
		_NoiseTex("扭曲噪波(RG)", 2D) = "white" {}
		_MaskTex("遮罩(R)",2D) = "white" {}
		_RenderTex("相机视角图像(设置层使得该物体不被渲染)",2D) = "black" {}
		_HeatForce("扭曲力度", range(-1.5,1.5)) = 0.1
	}

		Category{
		Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" }
		Blend srcalpha oneminussrcalpha
		AlphaTest Greater .01
		Cull Off Lighting Off ZWrite Off
		SubShader{
		Pass{
		Name "BASE"
		Tags{ "LightMode" = "Always" }

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

	half4 _MainTex_ST;
	half4 _NoiseTex_ST;
	half4 _MaskTex_ST;
	sampler2D _NoiseTex;
	//sampler2D _MainTex;
	sampler2D _MaskTex;
	sampler2D _RenderTex;

	half4 _TintColor;
	v2f vert(appdata_t v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f,o);
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.uvmask = TRANSFORM_TEX(v.texcoord, _MaskTex);

		//android真机表现正确,pc_android纵向相反
		o.uvgrab = ComputeGrabScreenPos(o.vertex);

		////////////////////////pc_andriod平台下使用
		//float scale = 1.0;
		//o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
		//o.uvgrab.zw = o.vertex.zw;
		////////////////////////

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		//noise effect
		half4 offsetColor1 = tex2D(_NoiseTex, i.uvmain );
		//i.uvmain.xy += (offsetColor1.rg *2 - 1) * _HeatForce;
		i.uvgrab.xy += (offsetColor1.rg*2 - 1) * _HeatForce;
		half4 render = tex2Dproj(_RenderTex, UNITY_PROJ_COORD(i.uvgrab));
		half4 mask = tex2D(_MaskTex,i.uvmask);
		render.a *= mask.r;
		return render;
	}
		ENDCG
	}
	}

		// ------------------------------------------------------------------
		// Fallback for older cards and Unity non-Pro

		SubShader{
		Blend DstColor Zero
		Pass{
		Name "BASE"
		SetTexture[_MainTex]{ combine texture }
	}
	}
	}
}
