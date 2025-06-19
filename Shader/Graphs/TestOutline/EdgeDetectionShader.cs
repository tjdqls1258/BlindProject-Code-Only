using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class EdgeDetection : ScriptableRendererFeature
{
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;

        private RTHandle maskTextureHandle;
        private RTHandle maskDataTextureHandle;

        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");

        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial, float count)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
            material.SetColor(OutlineColorProperty, settings.outlineColor);

            if (maskTextureHandle != null)
                material.SetTexture("_maskTex", maskTextureHandle);
            if (maskDataTextureHandle != null)
                material.SetTexture("_maskData", maskDataTextureHandle);

            material.SetFloat("_numMasks", count);
        }

        private class PassData
        {
            public TextureHandle colorAttachment;
            public TextureHandle maskTex;
            public TextureHandle maskDataTex;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>("Edge Detection", out PassData passData);

            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

            if (maskTextureHandle != null)
            {
                var maskTexHandle = renderGraph.ImportTexture(maskTextureHandle);
                builder.UseTexture(maskTexHandle, AccessFlags.Read);
                passData.maskTex = maskTexHandle; // passData에 저장해둘 수는 있어요
            }

            if (maskDataTextureHandle != null)
            {
                var maskDataTexHandle = renderGraph.ImportTexture(maskDataTextureHandle);
                builder.UseTexture(maskDataTexHandle, AccessFlags.Read);
                passData.maskDataTex = maskDataTexHandle;
            }

            builder.UseAllGlobalTextures(true);

            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, RasterGraphContext context) => { Blitter.BlitTexture(context.cmd, Vector2.one, material, 0); });
        }
    }

    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Range(0, 15)] public int outlineThickness = 3;
        public Color outlineColor = Color.black;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    private Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    public Texture maskTexture;
    public Texture maskdataTexture;

    public float MaxSonarCount;

    /// <summary>
    /// Called
    /// - When the Scriptable Renderer Feature loads the first time.
    /// - When you enable or disable the Scriptable Renderer Feature.
    /// - When you change a property in the Inspector window of the Renderer Feature.
    /// </summary>
    public override void Create()
    {
        edgeDetectionPass ??= new EdgeDetectionPass();
    }

    /// <summary>
    /// Called
    /// - Every frame, once for each camera.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Don't render for some views.
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.Reflection
            || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;

        if (edgeDetectionMaterial == null)
        {
            edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Edge Detection"));
            if (edgeDetectionMaterial == null)
            {
                Debug.LogWarning("Not all required materials could be created. Edge Detection will not render.");
                return;
            }
        }

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.requiresIntermediateTexture = true;

        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial, MaxSonarCount);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    /// <summary>
    /// Clean up resources allocated to the Scriptable Renderer Feature such as materials.
    /// </summary>
    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
}