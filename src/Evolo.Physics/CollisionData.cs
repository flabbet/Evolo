using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics;

public struct CollisionData
{
    public ICollider ColliderA { get; }
    public ICollider ColliderB { get; }
    public VecD CollisionPoint { get; }
    public VecD Normal { get; }
    public double PenetrationDepth { get; }
    public VectorPath IntersectionPath { get; set; }


    public CollisionData(ICollider colliderA, ICollider colliderB, VecD contactPoint, VecD normal, double penetration, VectorPath intersectionPath)
    {
        ColliderA = colliderA;
        ColliderB = colliderB;
        CollisionPoint = contactPoint;
        Normal = normal;
        PenetrationDepth = penetration;
        IntersectionPath = new VectorPath(intersectionPath);
    }
}