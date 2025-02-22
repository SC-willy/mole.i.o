Shader "Supercent/Rendering/Planar Shadow" 
{
    Properties
    {
        _LightDirection ("Light Direction", Vector) = (0, 0, 0, 0)
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0)
        _ShadowPivotOffset ("Position Offset (X, Y, Z)", Vector) = (0, 0, 0, 0)
    }
    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        Pass {
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
            #pragma shader_feature _USE_OFFSET
            
            #include "UnityCG.cginc"
            
            float3 _LightDirection;
            float4 _ShadowColor;
            float3 _ShadowPivotOffset;
            
            #ifdef _USE_OFFSET
            #define APPLY_OFFSET
            #endif
            
            struct vsOut
            {
                float4 pos : SV_POSITION;
            };
            
            vsOut vert(appdata_base v)
            {
                vsOut o;
                
                float4 vPosWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 vPos = vPosWorld.xyz + (_LightDirection * (vPosWorld.y - _ShadowPivotOffset.y));
            
                #ifdef APPLY_OFFSET
                vPos.x += _ShadowPivotOffset.x;
                vPos.z += _ShadowPivotOffset.z;
                #endif
            
                o.pos = mul(UNITY_MATRIX_VP, float4(vPos.x, _ShadowPivotOffset.y, vPos.z, 1));
                
                return o;
            }
            
            fixed4 frag(vsOut i) : COLOR
            {
                return _ShadowColor;
            }
            
            ENDCG
        }
    }
}
