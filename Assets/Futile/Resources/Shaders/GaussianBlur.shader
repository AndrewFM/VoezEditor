Shader "Futile/GaussianBlur"
{
    Properties 
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        radius ("Radius", Range(0,30)) = 15
        resolution ("Resolution", float) = 320  
        hstep("HorizontalStep", Range(0,1)) = 0.1
        vstep("VerticalStep", Range(0,1)) = 0.1  
    }
    
    Category 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        //Alphatest Greater 0
        Blend SrcAlpha OneMinusSrcAlpha 
        Fog { Color(0,0,0,0) }
        Lighting Off
        Cull Off //we can turn backface culling off because we know nothing will be facing backwards

        BindChannels 
        {
            Bind "Vertex", vertex
            Bind "texcoord", texcoord 
            Bind "Color", color 
        }

        SubShader   
        {
            Pass 
            {
                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastes
                #include "UnityCG.cginc"
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float radius;
                float resolution;
                float hstep;
                float vstep;

                //normpdf function gives us a Guassian distribution for each blur iteration; 
                //this is equivalent of multiplying by hard #s 0.16,0.15,0.12,0.09, etc. in code above
                float normpdf(float x, float sigma)
                {
                    return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
                }

                //this is the blur function... pass in standard col derived from tex2d(_MainTex,i.uv)
                half4 blur(sampler2D tex, float2 uv, float blurAmount) {
                    //get our base color...
                    half4 col = tex2D(tex, uv);
                    //total width/height of our blur "grid":
                    const int mSize = 11;
                    //this gives the number of times we'll iterate our blur on each side 
                    //(up,down,left,right) of our uv coordinate;
                    //NOTE that this needs to be a const or you'll get errors about unrolling for loops
                    const int iter = (mSize - 1) / 2;
                    //run loops to do the equivalent of what's written out line by line above
                    //(number of blur iterations can be easily sized up and down this way)
                    for (int i = -iter; i <= iter; ++i) {
                        for (int j = -iter; j <= iter; ++j) {
                            col += tex2D(tex, float2(uv.x + i * blurAmount, uv.y + j * blurAmount)) * normpdf(float(i), 7);
                        }
                    }
                    //return blurred color
                    return col/mSize;
                }

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };    
                struct v2f
                {
                    half2 texcoord  : TEXCOORD0;
                    float4 vertex   : SV_POSITION;
                    fixed4 color    : COLOR;
                };

                v2f vert (appdata_base v)
                {
                    v2f o;
                    o.vertex = mul (UNITY_MATRIX_MVP, v.vertex);
                    o.texcoord = TRANSFORM_TEX (v.texcoord, _MainTex);
                    return o;
                }

                half4 frag (v2f i) : COLOR
                {
                    float2 uv = i.texcoord.xy;
                    float4 sum = float4(0.0, 0.0, 0.0, 0.0);
                    float2 tc = uv;

                    //blur radius in pixels
                    float blur = radius/resolution/4;     

                    sum += tex2D(_MainTex, float2(tc.x - 4.0*blur*hstep, tc.y - 4.0*blur*vstep)) * 0.0162162162;
                    sum += tex2D(_MainTex, float2(tc.x - 3.0*blur*hstep, tc.y - 3.0*blur*vstep)) * 0.0540540541;
                    sum += tex2D(_MainTex, float2(tc.x - 2.0*blur*hstep, tc.y - 2.0*blur*vstep)) * 0.1216216216;
                    sum += tex2D(_MainTex, float2(tc.x - 1.0*blur*hstep, tc.y - 1.0*blur*vstep)) * 0.1945945946;

                    sum += tex2D(_MainTex, float2(tc.x, tc.y)) * 0.2270270270;

                    sum += tex2D(_MainTex, float2(tc.x + 1.0*blur*hstep, tc.y + 1.0*blur*vstep)) * 0.1945945946;
                    sum += tex2D(_MainTex, float2(tc.x + 2.0*blur*hstep, tc.y + 2.0*blur*vstep)) * 0.1216216216;
                    sum += tex2D(_MainTex, float2(tc.x + 3.0*blur*hstep, tc.y + 3.0*blur*vstep)) * 0.0540540541;
                    sum += tex2D(_MainTex, float2(tc.x + 4.0*blur*hstep, tc.y + 4.0*blur*vstep)) * 0.0162162162;
                    return float4(sum.rgb, 1);
                }
                ENDCG
            }
        } 
    }
}