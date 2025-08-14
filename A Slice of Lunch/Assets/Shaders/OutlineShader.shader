Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
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
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Convert thickness to UV space (texture-relative)
                float2 texel = _OutlineThickness / float2(tex2D(_MainTex, float2(0,0)).x, tex2D(_MainTex, float2(0,0)).y);

                float alpha = 0;
                // 8-direction sampling
                alpha += tex2D(_MainTex, uv + float2(texel.x, 0)).a;
                alpha += tex2D(_MainTex, uv + float2(-texel.x, 0)).a;
                alpha += tex2D(_MainTex, uv + float2(0, texel.y)).a;
                alpha += tex2D(_MainTex, uv + float2(0, -texel.y)).a;
                alpha += tex2D(_MainTex, uv + float2(texel.x, texel.y)).a;
                alpha += tex2D(_MainTex, uv + float2(-texel.x, texel.y)).a;
                alpha += tex2D(_MainTex, uv + float2(texel.x, -texel.y)).a;
                alpha += tex2D(_MainTex, uv + float2(-texel.x, -texel.y)).a;

                float currAlpha = tex2D(_MainTex, uv).a;

                // Draw sprite normally, but overlay outline where neighbor pixels exist
                float outline = step(0.01, alpha) * step(0.0, 1.0 - currAlpha);

                fixed4 col = tex2D(_MainTex, uv);
                col.rgb = lerp(_OutlineColor.rgb, col.rgb, currAlpha); 
                col.a = max(currAlpha, outline);

                return col;
            }
            ENDCG
        }
    }
}