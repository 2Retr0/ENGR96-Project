using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyan;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/**
 * Manages post processing effects and their parameters
 */
public class PostManager : Singleton<PostManager>
{
    private static readonly int Strength = Shader.PropertyToID("_Strength");

    private readonly HashSet<int> editors = new();

    private Material impactLineMaterial;
    private float currentStrength;

    private void Start()
    {
        // --- Get impact line material ---
        var urpRenderer = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset)?.GetRenderer(0);
        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);

        if (property == null || property.GetValue(urpRenderer) is not List<ScriptableRendererFeature> features)
            return;

        foreach (var feature in features.Where(feature => feature.GetType() == typeof(Blit)))
        {
            impactLineMaterial = (feature as Blit)?.settings.blitMaterial;
            impactLineMaterial!.SetFloat(Strength, 0.0f);
        }
    }

    public void SetImpactStrength(float strength, GameObject who)
    {
        var _ = 0.0f; // Dummy ref
        if (strength <= 0.0005f) {
            editors.Remove(who.GetInstanceID());

            if (editors.Count != 0) return;
            impactLineMaterial.SetFloat(Strength, 0);
            currentStrength = 0;
        }
        else
        {
            editors.Add(who.GetInstanceID());

            var maxi = Mathf.Max(currentStrength, strength);
            var mini = Mathf.Min(currentStrength, strength);
            currentStrength = Mathf.SmoothDamp(mini, maxi, ref _, 0.1f);

            impactLineMaterial.SetFloat(Strength, currentStrength);

        }
    }
}
