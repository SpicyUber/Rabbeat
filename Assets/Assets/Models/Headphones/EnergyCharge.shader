Shader "Unlit/EnergyCharge"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0.,0.,0.,0.)
        _Color2("Color2", Color) = (0.,0.,0.,0.)
        _Value("Value", Range(0.,1.)) = 0.1
            _Value2("Value2", Range(-10,10)) = 0.1
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
                 float3 objectVertex : TEXCOORD2;
                 float3 worldNormal : NORMAL;
             };

             sampler2D _MainTex;
             float4 _MainTex_ST;

             fixed4 _Color;
             fixed4 _Color2;
             float _Value;
             float _Value2;

             v2f vert(appdata v)
             {
                 v2f o;

                 o.worldNormal = normalize(mul(unity_ObjectToWorld,v.normal));
                 o.objectVertex = v.vertex;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv = v.uv;
                o.uv.y += _Time.y * 1;  

                
                o.uv.y = sin(o.uv.y);  
                
                
                 return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                 // sample the texture
                 fixed4 col = _Color;
             i.uv.y += frac(_Time.x * 0.02);
             fixed4 noise = tex2D(_MainTex, i.uv);
             if (sin((i.uv.x  * 32 +0.2 + (noise.r + noise.g - noise.b) * 2) * 2) > 0 || i.objectVertex.z * 100 > _Value) { col = _Color2; }
             if (sin((i.uv.x * 32 - 0.2 + (noise.r + noise.g - noise.b) * 2) * 2) > 0 || i.objectVertex.z * 100 > _Value) { col = _Color2; }
             if (     sin((i.uv.x * 32+ (noise.r + noise.g - noise.b)*2)*2)>0 || i.objectVertex.z * 100 > _Value || i.objectVertex.z * 100 < _Value2) { col.a = 0; }
             //if ( (i.uv.y) > 0.99) { col= normalize(_Color*2);  }
            // if (i.objectVertex.z * 100 < _Value && i.objectVertex.z * 100 + 0.9 > _Value) { col.a = col.a * (_Value - i.objectVertex.z * 100 );  }
             
             // apply fog

             return col;
         }
         ENDCG
     }
        }
}
