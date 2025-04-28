using System.Numerics;
using Drawie.Numerics;
using Evolo.Physics.Math;

namespace Evolo.Physics;

public interface IPhysicsBody
{
    //public VecD Position { get; set; }
    public Matrix3x3 TrsMatrix { get; set; }
    public VecD Force { get; set; }
    public VecD Velocity { get; set; }
    public double Mass { get; set; }
    public ICollider Collider { get; }
}