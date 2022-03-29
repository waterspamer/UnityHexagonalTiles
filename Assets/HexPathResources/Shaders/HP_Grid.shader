// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HP/GridShader"
{
    Properties
    {
        _Color0("Color 0", Color) = (0,0,0,0)
        _Strong("Strong", Float) = 0
        [HideInInspector] __dirty( "", Int ) = 1
    }
    SubShader
    {
        Tags{ "RenderType" = "Overlay"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
        Cull Back
        ZWrite On
        Blend One OneMinusSrcAlpha
        
        CGINCLUDE
        #include "UnityPBSLighting.cginc"
        #include "Lighting.cginc"
        #pragma target 3.0
        struct Input
        {
            float4 vertexColor : COLOR;
        };
        uniform float4 _Color0;
        uniform float _Strong;
        inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
        {
            return half4 ( 0, 0, 0, s.Alpha );
        }
        void surf( Input i , inout SurfaceOutput o )
        {
            o.Emission = _Color0.rgb;
            o.Alpha = ( i.vertexColor * _Strong ).r;
        }
        ENDCG
        CGPROGRAM
        #pragma surface surf Unlit keepalpha fullforwardshadows 
        ENDCG
        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_shadowcaster
            #pragma multi_compile UNITY_PASS_SHADOWCASTER
            #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
            #include "HLSLSupport.cginc"
            #if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
                #define CAN_SKIP_VPOS
            #endif
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            sampler3D _DitherMaskLOD;
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float3 worldPos : TEXCOORD1;
                half4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            v2f vert( appdata_full v )
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_INITIALIZE_OUTPUT( v2f, o );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
                half3 worldNormal = UnityObjectToWorldNormal( v.normal );
                o.worldPos = worldPos;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
                o.color = v.color;
                return o;
            }
            half4 frag( v2f IN
            #if !defined( CAN_SKIP_VPOS )
            , UNITY_VPOS_TYPE vpos : VPOS
            #endif
            ) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID( IN );
                Input surfIN;
                UNITY_INITIALIZE_OUTPUT( Input, surfIN );
                float3 worldPos = IN.worldPos;
                half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
                surfIN.vertexColor = IN.color;
                SurfaceOutput o;
                UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
                surf( surfIN, o );
                #if defined( CAN_SKIP_VPOS )
                float2 vpos = IN.pos;
                #endif
                half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
                clip( alphaRef - 0.01 );
                SHADOW_CASTER_FRAGMENT( IN )
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
    CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18921
2108;158;1675;1019;887.9038;187.227;1;True;True
Node;AmplifyShaderEditor.VertexColorNode;1;-585.1777,167.7611;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-395.2549,335.7808;Inherit;False;Property;_Strong;Strong;2;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-540.2938,-48.69727;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;3;-261.4836,121.545;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-253.4371,196.9802;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;HP/GridShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Overlay;;Overlay;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;3;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;5;0
WireConnection;0;2;2;0
WireConnection;0;9;4;0
ASEEND*/
//CHKSM=7E500EB4EED5BD96C23677D3717C8E04C208FE4B