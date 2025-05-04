using Drawie.Numerics;
using Evolo.Physics;
using Evolo.Physics.Math;

namespace Evolo.Simulation.Core;

public class PhysicsObject : IPhysicsBody
{
    private ICollider? collider;
    public bool IsStatic { get; set; } = false;

    public VecD Position
    {
        get => new VecD(TrsMatrix.Translation.X, TrsMatrix.Translation.Y);
        set => TrsMatrix = Matrix3x3.TRS(new VecD((float)value.X, (float)value.Y), Scale, 0);
    }

    public VecD Scale
    {
        get => new VecD(TrsMatrix.Scale.X, TrsMatrix.Scale.Y);
        set => TrsMatrix = Matrix3x3.TRS(new VecD((float)Position.X, (float)Position.Y), value, 0);
    }

    public Matrix3x3 TrsMatrix { get; set; } = Matrix3x3.Identity;
    public VecD LinearVelocity { get; set; }
    public VecD Force { get; set; }
    public double Mass { get; set; } = 1;
    public double Bounciness { get; set; } = 0.5;

    public ICollider? Collider
    {
        get => collider;
        set
        {
            collider = value;
            collider?.Attach(this);
        }
    }
}