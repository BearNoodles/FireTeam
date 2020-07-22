Shader "Custom/Glow" {

    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Multiply ("Multiplier", Int) = 25
    }
    
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off
        Blend One One
 
        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0
 
        float4 _Color;
        int _Multiply;
 
        struct Input {
            float3 viewDir;
            float3 worldNormal;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            o.Alpha = _Color.a * pow(abs(dot(normalize(IN.viewDir),
                normalize(IN.worldNormal))), 4.0);
            o.Emission = _Color.rgb * _Multiply;
        }
        
        ENDCG
    }
    
    FallBack "Diffuse"
}