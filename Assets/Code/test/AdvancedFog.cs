using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(AdvancedFogRenderer), PostProcessEvent.AfterStack, "Gabo7/AdvancedFog")]


/*public class GradientParameter : ParameterOverride<Gradient>
{
}*/

public sealed class AdvancedFog : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Distance from the camera.")]
    public FloatParameter distance = new FloatParameter { value = 0.05f };
    /*[Tooltip("Vignette color. Use the alpha channel for transparency.")]
    public ColorParameter color1 = new ColorParameter { value = new Color(0f, 0f, 0f, 1f) };*/
    [Tooltip("Colors the fog will use."), DisplayName("Gradient")]
    public TextureParameter gradientTex = new TextureParameter { };
    /*[Tooltip("Vignette color. Use the alpha channel for transparency.")]
    public GradientParameter color4 = new GradientParameter { value = new Gradient() };*/
}



public sealed class AdvancedFogRenderer : PostProcessEffectRenderer<AdvancedFog>
{

    Texture2D m_InternalSpectralLut;

    public override void Render(PostProcessRenderContext context)
    {

        var spectralLut = settings.gradientTex.value;
        if (spectralLut == null)
        {
            if (m_InternalSpectralLut == null)
            {
                m_InternalSpectralLut = new Texture2D(3, 1, TextureFormat.RGB24, false)
                {
                    name = "Chromatic Aberration Spectrum Lookup",
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    anisoLevel = 0,
                    hideFlags = HideFlags.DontSave
                };

                m_InternalSpectralLut.SetPixels(new[]
                {
                        new Color(1f, 0f, 0f),
                        new Color(0f, 1f, 0f),
                        new Color(0f, 0f, 1f)
                    });

                m_InternalSpectralLut.Apply();
            }

            spectralLut = m_InternalSpectralLut;
        }


        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Gabo7/AdvancedFog"));
        sheet.properties.SetFloat("_Blend", settings.distance);
        //sheet.properties.SetVector("_Color1", settings.color1);
        sheet.properties.SetTexture("_tex", spectralLut);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
