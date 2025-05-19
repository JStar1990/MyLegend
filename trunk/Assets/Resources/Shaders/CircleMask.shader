Shader "Custom/CircleMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0,1)) = 0.5
        _FillAmount ("Fill Amount", Range(0,1)) = 1
        _StartAngle ("Start Angle", Range(0, 360)) = 0
        _IsCDMask ("Is CD Mask", Float) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
        _Smoothness ("Edge Smoothness", Range(0.0001, 0.05)) = 0.01  // 添加边缘平滑度参数
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Radius;
            float _FillAmount;
            float _StartAngle;
            float _IsCDMask;
            float _Alpha;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float2 center = float2(0.5, 0.5);
                float distance = length(i.uv - center);
                
                // 使用平滑步长函数替代硬边界
                float smoothedAlpha = smoothstep(_Radius, _Radius - _Smoothness, distance);
                
                if (_IsCDMask > 0.5)
                {
                    float angle = atan2(i.uv.y - center.y, i.uv.x - center.x);
                    angle = degrees(angle);
                    angle = angle < 0 ? angle + 360 : angle;
                    angle = (angle - _StartAngle + 360) % 360;
                    float normalizedAngle = angle / 360;
                    
                    // 为 CD 遮罩边缘也添加平滑过渡
                    float cdEdge = smoothstep(_FillAmount, _FillAmount - _Smoothness, normalizedAngle);
                    
                    if (distance <= _Radius)
                    {
                        col.a *= cdEdge;
                    }
                    else
                    {
                        col.a *= smoothedAlpha;
                    }
                }
                else
                {
                    col.a *= smoothedAlpha;
                }
                
                col.a *= _Alpha;
                return col;
            }
            ENDCG
        }
    }
}