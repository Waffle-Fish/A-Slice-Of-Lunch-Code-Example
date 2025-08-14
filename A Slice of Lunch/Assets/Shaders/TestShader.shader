Shader "Custom/SimpleSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}  // Main texture
        _Color ("Tint", Color) = (1,1,1,1)            // Tint color
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"                // Draw after opaque objects
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off                                    // Show both sides
        Lighting Off                                // No lighting for sprites
        ZWrite Off                                  // Don't write to depth buffer
        Blend SrcAlpha OneMinusSrcAlpha             // Standard alpha blending

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color; // Multiply vertex color by tint
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.texcoord) * i.color;
                return texCol; // Uses alpha from texture * tint
            }
            ENDCG
        }
    }
}