
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ysShader/Rim/Rim Character Mobile"
{
	Properties
	{
			_RimColor("Rim Color",Color)=(1,1,1,1)
			_RimWidth("Rim Width",Range(0.4,2))=0.9
			_RimIntensity("Rim Intensity",Range(1,10)) = 0.0
	}
			SubShader{
			Pass{
			Tags{"RenderType"="Transparent" "RenderQueue"="Transparent"}
			Lighting off
			fog{mode off}
			blend one one
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f
		{
			float4 pos :SV_POSITION;
			fixed3 color : COLOR;
		};
		fixed4 _RimColor;
		float _RimWidth;
		float _RimIntensity;
	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		float dotProduct = 1 - dot(v.normal, viewDir);
		o.color = smoothstep(1 - _RimWidth, 1.0, dotProduct);
		o.color *= _RimColor;
		o.color *= _RimIntensity;
		return o;
	}
	fixed4 frag(v2f i):COLOR
	{
		fixed4 texcol = fixed4(i.color,1);
		return texcol;
	}
	ENDCG
	}
	}
}
