Shader "Supercent/Rendering/Planar Shadow Baked" 
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0)
    }
    SubShader 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        Pass
        {   
            ZWrite Off
            ZTest LEqual 
            Blend SrcAlpha OneMinusSrcAlpha
            
            Stencil {
                Ref 0
                Comp Equal
                Pass IncrWrap
                ZFail Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float4 _ShadowColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _ShadowColor;
            }
            ENDCG
        }
    }
}
