Shader "Unlit/Jumpwave"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0.,0.,0.,0.)
        _Color2("Color2", Color) = (0.,0.,0.,0.)
        _Value("Value", Range(0.,1.)) = 0.1
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
                 float3 normal : NORMAL;
                 
             };

             struct v2f
             {
                 float2 uv : TEXCOORD0;
                 UNITY_FOG_COORDS(1)
                 float4 vertex : SV_POSITION;
                 float3 worldVertex : TEXCOORD2;
                 float3 worldNormal : NORMAL;
             };

             sampler2D _MainTex;
             float4 _MainTex_ST;
            
             fixed4 _Color;
             fixed4 _Color2;
             float _Value;
            

             v2f vert(appdata v)
             {
                 v2f o;

                 o.worldNormal =  normalize(mul(unity_ObjectToWorld,v.normal));

                 o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                 UNITY_TRANSFER_FOG(o,o.vertex);
                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                 // sample the texture
                 fixed4 col = _Color ;
             float f =  saturate(1 + dot(i.worldNormal, normalize(i.worldVertex - _WorldSpaceCameraPos.xyz)));
             if (f<0.7 )col.a = col.a * 0.25f;
              
          //   col.g = saturate(col.g + 0.1);
            // col.r = saturate(col.r + 0.2);
             // apply fog
              
             return col;
         }
         ENDCG
     }
        }
}
