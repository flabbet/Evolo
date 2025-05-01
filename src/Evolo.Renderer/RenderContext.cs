using Drawie.Backend.Core;
using Drawie.Backend.Core.Numerics;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Backend.Core.Vector;
using Drawie.Numerics;
using Evolo.Simulation.Engine;

namespace Evolo.Renderer;

public class RenderContext
{
    private Texture targetTexture;

    public RenderContext(Texture targetTexture)
    {
        this.targetTexture = targetTexture;
    }

    public void DrawRect(RectD rect, Paint paint)
    {
        var canvas = targetTexture.DrawingSurface.Canvas;

        VecD position = WorldToViewport(rect.Center);
        VecD size = rect.Size * SimulationScene.PixelsPerMeter;

        canvas.DrawRect(RectD.FromCenterAndSize(position, size), paint);
    }

    public void DrawCircle(VecD center, double radius, Paint paint)
    {
        var canvas = targetTexture.DrawingSurface.Canvas;

        VecD position = WorldToViewport(center);
        double size = radius * SimulationScene.PixelsPerMeter;

        canvas.DrawCircle(position, (float)size, paint);
    }

    public void DrawPath(VectorPath path, Paint paint)
    {
        var canvas = targetTexture.DrawingSurface.Canvas;

        Matrix3X3 scaleMatrix = Matrix3X3.CreateScale((float)SimulationScene.PixelsPerMeter, (float)SimulationScene.PixelsPerMeter);

        using VectorPath scaledPath = new VectorPath();
        scaledPath.AddPath(path, scaleMatrix, AddPathMode.Append);

        canvas.DrawPath(scaledPath, paint);
    }

    public VecD WorldToViewport(VecD position)
    {
        return new VecD(position.X, -position.Y) * SimulationScene.PixelsPerMeter;
    }
}