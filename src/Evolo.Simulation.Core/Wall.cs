using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.Surfaces;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Numerics;
using Evolo.Physics.Colliders;
using Evolo.Renderer;
using Evolo.Simulation.Engine;

namespace Evolo.Simulation.Core;

public class Wall : PhysicsObject, ISimulableEntity, IRenderable
{
    public Wall(VecD position, VecD size)
    {
        IsStatic = true;
        Position = position;
        Scale = size;

        Collider = new RectangleCollider(new VecD(0, 0), new VecD(1, 1), this);
    }

    public void Simulate()
    {

    }

    public void Render(RenderContext context)
    {
        using var paint = new Paint()
        {
            Color = Colors.Gray,
            Style = PaintStyle.Fill,
            IsAntiAliased = true
        };

        context.DrawRect(new RectD(Position, Scale), paint);
    }
}