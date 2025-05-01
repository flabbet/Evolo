using System.Numerics;
using Drawie.Numerics;
using Evolo.Physics.Math;

namespace Evolo.Physics;

public interface IPhysicsBody
{
    public bool IsStatic { get; }
    public VecD Position { get; set; }
    public VecD Scale { get; set; }
    public Matrix3x3 TrsMatrix { get; set; }
    public VecD Force { get; set; }
    public VecD LinearVelocity { get; set; }
    public double Mass { get; set; }
    public double Bounciness { get; set; }
    public ICollider? Collider { get; }
}