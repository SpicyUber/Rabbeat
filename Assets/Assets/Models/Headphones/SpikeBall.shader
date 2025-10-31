Shader "Unlit/SpikeBall"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0.,0.,0.,0.)
      _Cutoff("Cutoff", Range(-100,100)) = 0.1
        _Value("Value", Range(-1.,1.)) = 0.1
    }
        SubShader
        {
             Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
            ZWrite Off
            // Blend DstColor Zero
             Blend SrcAlpha OneMinusSrcAlpha

         Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             // make fog work
             #pragma multi_compile_fog

             #include "UnityCG.cginc"

             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
             };

             struct v2f
             {
                 float2 uv : TEXCOORD0;
                 float distance : TEXCOORD2;
                 UNITY_FOG_COORDS(1)
                 float4 vertex : SV_POSITION;
             };

             sampler2D _MainTex;
             float4 _MainTex_ST;
             float4 _PlayerPosition;
             fixed4 _Color;
             float _Value;
             float _Cutoff;

             v2f vert(appdata v)
             {
                 v2f o;
                 o.distance = 0;
                 if (length(v.vertex) > _Cutoff) { v.vertex += _Value * normalize(v.vertex); o.distance = 1; }
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                 // sample the texture
                 fixed4 col = _Color;
           /*  if (i.distance == 0)col.a = col.a * 0.95; else { col.r = saturate(col.r + 0.3f);  col.g = saturate(col.g + 0.3f); col.b = saturate(col.b + 0.3f);
             }*/
             return col;
         }
         ENDCG
     }
        }
}
