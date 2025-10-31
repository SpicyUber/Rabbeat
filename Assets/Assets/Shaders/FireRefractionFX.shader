Shader "Unlit/FireRefractionFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Value("Value", Float) = 1.0
        _Ammount("Ammount", Float) = 1.0
    }
        SubShader
        {
            Tags {   "RenderType" = "Background" }
            GrabPass { "_GrabTexture" }
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
            float _Value;
            float _Ammount;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _GrabTexture;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float2 uv2 = i.uv;
                uv2.x = 1.0 - uv2.x;
                if (uv2.x > 0.1 && uv2.x < 0.9) {
                    uv2.x = frac(uv2.x + cos(_Value*16)/100);
                }
                fixed4 col = tex2D(_GrabTexture, uv2);
                float2 center = (0.5, 0.5);
                col.rgba = fixed4(col.r, col.g/2.5, col.b/ 2.5, distance(i.uv, center) * _Ammount);
                return col;
            }
            ENDCG
        }
    }
}
