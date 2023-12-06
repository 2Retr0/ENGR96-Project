using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Scripts.Player;
using Cyan;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Code.Scripts
{
    /**
 * Manages post processing effects and their parameters
 */
    public class PostManager : Singleton<PostManager>
    {
        private static readonly int Strength = Shader.PropertyToID("_Strength");

        private readonly HashSet<int> editors = new();

        private Material impactLineMaterial;
        private Material playerOutlineMaterial;
        private Material enemyOutlineMaterial;
        private PlayerController player;

        public float currentStrength;
        public float currentThickness;
        private static readonly int Thickness = Shader.PropertyToID("_Thickness");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        private void ResetImpactStrength()
        {
            impactLineMaterial!.SetFloat(Strength, 0.0f);
            currentStrength = 0f;
        }

        private void ResetOutlineThickness()
        {
            playerOutlineMaterial!.SetFloat(Thickness, 0.04f);
            enemyOutlineMaterial!.SetFloat(Thickness, 0.04f);
            currentThickness = 0.04f;
        }

        private void ResetOutlineColors()
        {
            playerOutlineMaterial!.SetColor(Color1, new Color(180, 192, 204));
            enemyOutlineMaterial!.SetColor(Color1, new Color(181, 51, 59));
        }

        public void Reset()
        {
            ResetImpactStrength();
            ResetOutlineThickness();
            ResetOutlineColors();
        }

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

            foreach (var feature in features.Where(feature => feature.GetType() == typeof(RenderObjects)))
            {
                switch ((feature as RenderObjects)?.name)
                {
                    case "Occluded Outline Player":
                        playerOutlineMaterial = (feature as RenderObjects).settings.overrideMaterial;
                        break;
                    case "Occluded Outline":
                        enemyOutlineMaterial = (feature as RenderObjects).settings.overrideMaterial;
                        break;
                }
            }

            Reset();

            player = FindObjectOfType<PlayerController>();
        }

        public void FixedUpdate()
        {
            if (player.level >= 7)
                enemyOutlineMaterial!.SetColor(Color1, Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1));
        }

        public void SetPlayerOutlineThickness(float thickness)
        {
            playerOutlineMaterial!.SetFloat(Thickness, thickness);
            currentThickness = thickness;
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

        private void OnDestroy()
        {
            ResetImpactStrength();
            ResetOutlineThickness();
            ResetOutlineColors();
        }
    }
}
