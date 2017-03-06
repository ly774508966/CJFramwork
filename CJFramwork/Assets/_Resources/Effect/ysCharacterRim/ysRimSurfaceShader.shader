Shader "ysShader/Rim/rim_basic"
{
	Properties
	{
		_RimColor("【边缘发光颜色】Rim Color", Color) = (0.17,0.36,0.81,0.0)
		_RimPower("【边缘颜色强度】Rim Power", Range(0.0,10.0)) = 8.0
		_RimIntensity("【边缘颜色强度系数】Rim Intensity", Range(0.0,10.0)) = 1.0
	}
	SubShader
	{
		Tags
		{
		"RenderType" = "Transparent"
		"RenderQueue" = "Transparent"
		}
		blend one one
		CGPROGRAM
		#pragma surface surf Lambert  
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
		};
		float4 _RimColor;
		float _RimPower;
		float _RimIntensity;
		void surf(Input IN, inout SurfaceOutput o)
		{
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower)*_RimIntensity;
		}
		ENDCG
	}
	Fallback "Diffuse"
}