Shader "Custom/HexWave_Movement"
{
    Properties
    {
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveHeight ("Wave Height", Float) = 0.3
        _WaveDuration ("Wave Duration", Float) = 1.5
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1) // 기존 재질의 색상 유지
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            ZWrite Off   // 깊이 버퍼를 변경하지 않음 → 원래 재질 유지
            ZTest Always // 항상 위치 변경 적용
            ColorMask 0  // 렌더링 안 하고 위치만 변경
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _WaveSpeed;
            float _WaveHeight;
            float _WaveDuration;
            float _GlobalTime;
            float _WaveStartTime;
            fixed4 _BaseColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR; // 기존 색상 정보 유지
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float CalculateHexWave(float3 worldPos, float globalTime, float waveStartTime, float waveDuration, float waveHeight)
            {
                float elapsedTime = globalTime - waveStartTime;
                if (elapsedTime > 0 && elapsedTime < waveDuration)
                {
                    float t = elapsedTime / waveDuration;
                    float easeInOut = sin(t * 2.0 * 3.14159265359) * (1.0 - t); // UNITY_PI 대신 직접 입력
                    return easeInOut * waveHeight;
                }
                return 0.0;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float waveOffset = CalculateHexWave(worldPos, _GlobalTime, _WaveStartTime, _WaveDuration, _WaveHeight);

                worldPos.y += waveOffset;
                v.vertex = mul(unity_WorldToObject, float4(worldPos, 1.0));
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _BaseColor; // 기존 색상 유지
            }
            ENDCG
        }
    }
}