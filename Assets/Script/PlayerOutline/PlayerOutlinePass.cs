using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerOutlinePass : ScriptableRenderPass
{
    private readonly Material outlineMaterial;
    private RenderTargetIdentifier currentTarget;
    private RenderTargetHandle temporaryColorTexture;
    private readonly int outlineLayer;

    public PlayerOutlinePass(Material outlineMaterial, int outlineLayer)
    {
        this.outlineMaterial = outlineMaterial;
        this.outlineLayer = outlineLayer;
        temporaryColorTexture.Init("_TemporaryColorTexture");
    }

    public void Setup(RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (outlineMaterial == null) return;

        CommandBuffer cmd = CommandBufferPool.Get("PlayerOutlinePass");
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDesc, FilterMode.Bilinear);

        // Render the outline layer to a temporary texture
        cmd.SetRenderTarget(temporaryColorTexture.Identifier(), RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        cmd.ClearRenderTarget(true, true, Color.clear);
        var drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
        var filteringSettings = new FilteringSettings(RenderQueueRange.all, outlineLayer);
        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

        // Blit the outline texture onto the main color target
        cmd.SetRenderTarget(currentTarget, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
        cmd.Blit(temporaryColorTexture.Identifier(), currentTarget, outlineMaterial);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
    }
}
