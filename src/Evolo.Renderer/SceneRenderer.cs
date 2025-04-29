using Drawie.Backend.Core;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.ColorsImpl.Paintables;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Backend.Core.Text;
using Drawie.Numerics;
using Evolo.Simulation.Engine;

namespace Evolo.Renderer;

public class SceneRenderer
{
    private double scale = 1.0;
    public SimulationScene Scene { get; set; }
    public VecD ViewportPosition { get; set; }

    private Font debugFont;

    private double debugTextUpdateRate = 0;
    private string debugText = "";

    public double ViewportScale
    {
        get => scale;
        set { scale = Math.Max(0.1, value); }
    }

    public SceneRenderer(SimulationScene scene)
    {
        Scene = scene;
        debugFont = Font.CreateDefault();
        debugFont.Size = 16;
    }

    public void Render(Texture renderTexture, double deltaTime)
    {
        renderTexture.DrawingSurface.Canvas.Clear(Colors.Black);

        DrawDebugText(renderTexture, deltaTime);

        int savedWidth = renderTexture.DrawingSurface.Canvas.Save();
        renderTexture.DrawingSurface.Canvas.Scale((float)ViewportScale, (float)ViewportScale,
            renderTexture.Size.X / 2f, renderTexture.Size.Y / 2f);
        renderTexture.DrawingSurface.Canvas.Translate(ViewportPosition);

        foreach (var cell in Scene.SimulableEntities)
        {
            RenderCell(cell, renderTexture);
        }

        renderTexture.DrawingSurface.Canvas.RestoreToCount(savedWidth);
    }

    private void RenderCell(ISimulableEntity cell, Texture texture)
    {
        var position = cell.Position;

        position += texture.Size / 2;

        const double size = 10;

        var color = Colors.White;
        var radius = size / 2;
        var center = new VecD(position.X, position.Y);

        using Paint paint = new Paint()
        {
            Color = color,
            Style = PaintStyle.Fill,
            IsAntiAliased = true
        };

        texture.DrawingSurface.Canvas.DrawCircle(center, (float)radius, paint);
    }

    private void DrawDebugText(Texture renderTexture, double deltaTime)
    {
        if (debugTextUpdateRate > 0)
        {
            debugTextUpdateRate -= deltaTime;
        }
        else
        {
            var msPerFrameText = $"ms/f: {deltaTime * 1000:F2} ms ({1 / deltaTime:F2} fps)";
            var tpsText = $"ms/t: {Scene.LastTps:F2} ms ({1 / Scene.LastTps:F2} tps)";
            debugText = msPerFrameText + "\n" + tpsText;
            debugTextUpdateRate = 0.5f;
        }

        using Paint paint = new Paint();

        RichText richText = new RichText(debugText);
        richText.FillPaintable = new ColorPaintable(Colors.White);
        richText.Fill = true;
        richText.Paint(renderTexture.DrawingSurface.Canvas, new VecD(0, debugFont.Size), debugFont, paint, null);

        string viewportPos = $"Viewport: {ViewportPosition.X:F2}, {ViewportPosition.Y:F2}";
        string viewportScale = $"Scale: {ViewportScale:F2}";
        RichText richText2 = new RichText(viewportPos + "\n" + viewportScale);
        richText2.FillPaintable = new ColorPaintable(Colors.White);
        richText2.Fill = true;
        richText2.Paint(renderTexture.DrawingSurface.Canvas, new VecD(0, richText.MeasureBounds(debugFont).Height + debugFont.Size), debugFont, paint, null);
    }
}