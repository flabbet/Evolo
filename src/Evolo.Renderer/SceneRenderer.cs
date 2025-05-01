using Drawie.Backend.Core;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.ColorsImpl.Paintables;
using Drawie.Backend.Core.Surfaces;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Backend.Core.Text;
using Drawie.Backend.Core.Vector;
using Drawie.Numerics;
using Evolo.Physics;
using Evolo.Simulation.Engine;

namespace Evolo.Renderer;

public class SceneRenderer
{
    public SimulationScene Scene { get; set; }
    public VecD ViewportPosition { get; set; }
    public event Action<Canvas>? DebugDraw;

    private Font debugFont;
    private double scale = 1.0;

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

        RenderContext renderContext = new RenderContext(renderTexture);

        foreach (var entity in Scene.SimulableEntities)
        {
            if (entity is IRenderable renderable)
            {
                renderable.Render(renderContext);
            }

            if (entity is IPhysicsBody physicsBody)
            {
                RenderPathCollider(renderContext, physicsBody);
            }
        }

        DrawInSceneDebugText(renderContext);

        renderTexture.DrawingSurface.Canvas.RestoreToCount(savedWidth);
    }

    private void RenderPathCollider(RenderContext renderContext, IPhysicsBody physicsBody)
    {
        using var paint = new Paint();
        paint.Color = Colors.LightGreen;
        paint.Style = PaintStyle.Stroke;
        paint.StrokeWidth = 2;

        using var path = physicsBody.Collider.WorldPath;
        path.Offset(new VecD(0, -physicsBody.Collider.AABB.Center.Y * 2));

        renderContext.DrawPath(path, paint);
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

        double y = debugFont.Size;

        RichText richText = new RichText(debugText);
        richText.FillPaintable = new ColorPaintable(Colors.White);
        richText.Fill = true;
        richText.Paint(renderTexture.DrawingSurface.Canvas, new VecD(0, y), debugFont, paint, null);

        string viewportPos = $"Viewport: {ViewportPosition.X:F2}, {ViewportPosition.Y:F2}";
        string viewportScale = $"Scale: {ViewportScale:F2}";
        RichText richText2 = new RichText(viewportPos + "\n" + viewportScale);
        richText2.FillPaintable = new ColorPaintable(Colors.White);
        richText2.Fill = true;

        y += richText.MeasureBounds(debugFont).Height + debugFont.Size + 5;

        richText2.Paint(renderTexture.DrawingSurface.Canvas,
            new VecD(0, y), debugFont, paint, null);

        y += richText2.MeasureBounds(debugFont).Height + debugFont.Size + 5;
        VecD pos = new VecD(0, y);

        int savedWidth = renderTexture.DrawingSurface.Canvas.Save();
        renderTexture.DrawingSurface.Canvas.Translate(new VecD(0, pos.Y));
        DebugDraw?.Invoke(renderTexture.DrawingSurface.Canvas);
        renderTexture.DrawingSurface.Canvas.RestoreToCount(savedWidth);
    }

    private void DrawInSceneDebugText(RenderContext renderContext)
    {
        using Paint paint = new Paint();
        foreach (var entity in Scene.SimulableEntities)
        {
            string entityText = $"X: {entity.Position.X:F2}, Y: {entity.Position.Y:F2}";
            RichText richText3 = new RichText(entityText)
            {
                FillPaintable = new ColorPaintable(Colors.White),
                Fill = true
            };

            VecD pos = new VecD(entity.Position.X, entity.Position.Y);

            //richText3.Paint(renderContext.DrawingSurface.Canvas, ToViewportPosition(pos) + new VecD(-richText3.MeasureBounds(debugFont).Width / 2f, -10), debugFont, paint, null);
        }
    }
}