Shader "Hidden/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
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
            float4 _MainTex_ST;
            float _Intensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Simple motion blur effect
                float2 blurOffset = float2(0.01, 0.01) * _Intensity;
                
                fixed4 blur1 = tex2D(_MainTex, i.uv + blurOffset);
                fixed4 blur2 = tex2D(_MainTex, i.uv - blurOffset);
                fixed4 blur3 = tex2D(_MainTex, i.uv + float2(blurOffset.x, -blurOffset.y));
                fixed4 blur4 = tex2D(_MainTex, i.uv + float2(-blurOffset.x, blurOffset.y));
                
                col = (col + blur1 + blur2 + blur3 + blur4) / 5.0;
                
                return col;
            }
            ENDCG
        }
    }
}
