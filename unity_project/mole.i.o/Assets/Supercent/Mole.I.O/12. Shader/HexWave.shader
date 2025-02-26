Shader "Custom/HexWaveShader_HLSL"
{
    Properties
    {
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveHeight ("Wave Height", Float) = 0.3
        _WaveDuration ("Wave Duration", Float) = 1.5
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert

        // Unity 내장 라이브러리 포함
        #include "UnityCG.cginc"

        // 📌 Properties (Material에서 조절 가능)
        float _WaveSpeed;
        float _WaveHeight;
        float _WaveDuration;
        float _GlobalTime;
        float _WaveStartTime;
        fixed4 _BaseColor;

        // 📌 Input 구조체 (픽셀 쉐이더에서 사용할 값)
        struct Input
        {
            float3 worldPos;
        };

        // 📌 웨이브 애니메이션 계산 함수
        float CalculateWave(float3 worldPos, float globalTime, float waveStartTime, float waveDuration, float waveHeight)
        {
            float elapsedTime = globalTime - waveStartTime;
            if (elapsedTime > 0 && elapsedTime < waveDuration)
            {
                float t = elapsedTime / waveDuration;
                float easeInOut = sin(t * 6.28318530718) * (1.0 - t); // 2 * PI 사용
                return easeInOut * waveHeight;
            }
            return 0.0;
        }

        // 📌 Vertex Shader (버텍스 변형)
        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            // Y축 웨이브 애니메이션 적용
            worldPos.y += CalculateWave(worldPos, _GlobalTime, _WaveStartTime, _WaveDuration, _WaveHeight);

            // 🔥 **크기 문제 해결**: `UNITY_MATRIX_MVP`로 좌표 변환
            v.vertex = mul(unity_WorldToObject, float4(worldPos, 1.0));
        }

        // 📌 Surface Shader (픽셀 처리)
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _BaseColor.rgb; // 머티리얼에서 설정한 기본 색상 적용
        }

        ENDCG
    }
}