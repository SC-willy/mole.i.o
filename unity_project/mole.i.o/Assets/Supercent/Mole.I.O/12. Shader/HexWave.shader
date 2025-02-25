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
        float wave = sin(elapsedTime * _WaveSpeed) * _WaveHeight * (1.0 - elapsedTime / _WaveDuration);

        // 🔥 로컬 좌표 → 월드 좌표 변환
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        worldPos.y += wave;

        // 🔥 월드 좌표 → 로컬 좌표 변환
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