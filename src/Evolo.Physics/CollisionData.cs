using Drawie.Numerics;

namespace Evolo.Physics;

public struct CollisionData
{
    public ICollider ColliderA { get; }
    public ICollider ColliderB { get; }
    public VecD Normal { get; }
    public double PenetrationDepth { get; }

    public CollisionData(ICollider colliderA, ICollider colliderB, VecD collision, double penetration)
    {
        ColliderA = colliderA;
        ColliderB = colliderB;
        Normal = collision;
        PenetrationDepth = penetration;
    }
}