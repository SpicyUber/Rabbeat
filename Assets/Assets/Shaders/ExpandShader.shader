Shader "Unlit/ExpandShader"
{
    Properties
    {
        _PlayerPosition("PlayerPosition", Vector) = (0, 0, 0, 0)
        _MainTex("MainTexture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
       
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

        float4 _PlayerPosition;

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
            float4 _MainTex_ST;
            

            v2f vert (appdata v)
            {

                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                
                
                float3 temp = float3(worldPos.x - _PlayerPosition.x, worldPos.y - _PlayerPosition.y, worldPos.z - _PlayerPosition.z);
                if (length(temp) <= 1.8 && _PlayerPosition.w == 1) {
                    worldPos = worldPos + normalize(temp) * 0.35;
                    o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, float4(worldPos.xyz,1.0)));
                }
                else {
                    o.vertex = UnityObjectToClipPos(v.vertex);
                }
               
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                return tex2D( _MainTex , i.uv);
            }
            ENDCG
        }
    }
}
