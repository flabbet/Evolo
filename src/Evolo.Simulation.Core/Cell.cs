using System.Numerics;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.Numerics;
using Drawie.Backend.Core.Surfaces;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Numerics;
using Evolo.Physics;
using Evolo.Physics.Colliders;
using Evolo.Physics.Math;
using Evolo.Renderer;

namespace Evolo.Simulation.Core;

public class Cell : PhysicsObject, ICell, IRenderable
{


    public PhysicalProperties PhysicalProperties { get; }
    public ChemicalProperties ChemicalProperties { get; }

    public Cell()
    {
        Collider = new CircleCollider(new VecD(0, 0), 1);
    }

    public void Simulate()
    {
    }

    public void Render(RenderContext context)
    {
        using var paint = new Paint()
        {
            Color = Colors.White,
            Style = PaintStyle.Fill,
            IsAntiAliased = true
        };

        context.DrawCircle(Position, 1, paint);
    }
}