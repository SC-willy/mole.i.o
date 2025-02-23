Shader "Custom/HexWave"
{
    Properties
    {
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveHeight ("Wave Height", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        
        float _WaveSpeed;
        float _WaveHeight;

        struct Input {
            float3 worldPos;
        };

        void vert(inout appdata_full v) {
            float waveTime = _Time.y * _WaveSpeed - v.vertex.y * 0.1;
            float wave = sin(waveTime) * _WaveHeight;
            v.vertex.y += wave;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = float3(0, 1, 0); // 초록색 기본 타일
        }
        ENDCG
    }
}