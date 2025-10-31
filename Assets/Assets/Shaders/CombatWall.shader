Shader "Unlit/CombatWall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0.,0.,0.,0.)
        _PlayerPosition ("PlayerPosition", Vector) = (0.,0.,0.,0.)
        _Value("Value", Range(0,1)) = 0.1
    }
    SubShader
    {
         Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
        ZWrite Off
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
                float2 dist : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _PlayerPosition;
            fixed4 _Color;
            float _Value;

            v2f vert (appdata v)
            {
                v2f o;
               o.dist.x = distance(_PlayerPosition, mul(unity_ObjectToWorld, v.vertex));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
            col.r = _Color.r;
            col.g = _Color.g;
            col.b = _Color.b;
            if (i.dist.x < 5) {
                col.a = col.a + i.dist.x / 2;
                }
            col.a = col.a * sin(8*((i.uv.y+i.uv.x / 10)  + _Time.y)) * 0.5 * _Value;
                return col + fixed4(0.,0.,0.,0.25 * _Value);
            }
            
            ENDCG
        }
    }
}
