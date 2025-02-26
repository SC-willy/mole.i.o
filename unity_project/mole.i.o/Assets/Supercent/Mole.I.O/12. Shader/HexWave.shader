Shader "Custom/HexWaveShader"
{
    Properties
    {
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveHeight ("Wave Height", Float) = 0.3
        _WaveDuration ("Wave Duration", Float) = 1.5 // 웨이브 지속 시간
        _Color ("Tile Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard vertex:vert

        float _WaveSpeed;
        float _WaveHeight;
        float _WaveDuration;  // 추가: 웨이브 지속 시간
        float _GlobalTime;  
        float _WaveStartTime; 
        fixed4 _Color;  

        struct Input {
            float3 worldPos;
        };

        void vert(inout appdata_full v)
        {
           float elapsedTime = _GlobalTime - _WaveStartTime;

    if (elapsedTime > 0 && elapsedTime < _WaveDuration)
    {
         float t = elapsedTime / _WaveDuration; // 0~1 정규화
        float easeInOut = sin(t * 2.0 * UNITY_PI) * (1.0 - t); // 이즈인-이즈아웃 보간 (0 -> 1 -> -0.3 -> 0)

        float wave = easeInOut * _WaveHeight;
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        worldPos.y += wave;

        v.vertex = mul(unity_WorldToObject, float4(worldPos, 1.0));
    }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
        }
        ENDCG
    }
}