using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics;

public interface ICollider
{
    public VectorPath LocalPath { get; }
    public VectorPath WorldPath { get; }
    public RectD AABB { get; }
    public bool IsColliding(ICollider other, out CollisionData[] collisions);
    public bool Intersects(VecD point);
    public IPhysicsBody PhysicsBody { get; }
    public VecD GetCollisionCentroid(VecD intersectionCenter);
    public void Attach(IPhysicsBody physicsBody);
}