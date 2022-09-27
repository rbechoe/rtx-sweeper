Shader "CustomRenderTexture/Caustics"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _CausticsTex ("Caustics Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", float) = 1
        _Tiling ("Tiling", float) = 1
        
        _NoiseMap ("Noise Map", 2D) = "white" {}
        _NoiseScale ("Noise Scale", float) = 0.05
        _NoiseStrength ("Noise Strength", float) = 10
    }

    SubShader
    {
        Blend One Zero

        Pass
        {
            Name "Caustics"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _Tint;
            sampler2D _CausticsTex;
            float _FlowSpeed;
            float _Tiling;
            
            sampler2D _NoiseMap;
            float _NoiseScale;
            float _NoiseStrength;
            
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float2 uv = IN.globalTexcoord.xy * _Tiling;
                uv.x += _Time * _FlowSpeed;
                uv += tex2D(_NoiseMap, IN.globalTexcoord.xy * _NoiseScale) * _NoiseStrength;
                float4 layer1 = tex2D(_CausticsTex, uv);

                float2 uv2 = IN.globalTexcoord.xy * _Tiling;
                uv2.y += _Time * _FlowSpeed;
                uv2 += tex2D(_NoiseMap, IN.globalTexcoord.xy * _NoiseScale) * _NoiseStrength;
                float4 layer2 = tex2D(_CausticsTex, uv2);

                float4 col = min(layer1, layer2); // Can be changed to other blend methods (like min() or lerp() whatever)
                col = float4(1, 1, 1, col.r); // Only use opacity
                
                return col * _Tint;
            }
            ENDCG
        }
    }
}
