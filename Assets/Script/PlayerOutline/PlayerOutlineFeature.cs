using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerOutlineFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PlayerOutlineSettings
    {
        public Material outlineMaterial;
        public LayerMask outlineLayer;
    }

    public PlayerOutlineSettings settings = new PlayerOutlineSettings();
    private PlayerOutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new PlayerOutlinePass(settings.outlineMaterial, settings.outlineLayer.value);
        outlinePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material. {0} pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }

        outlinePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(outlinePass);
    }
}
