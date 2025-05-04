using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public abstract class Collider<T> : ICollider
{
    public abstract VectorPath LocalPath { get; }

    public RectD AABB
    {
        get
        {
            using var worldPath = WorldPath;
            return worldPath.TightBounds;
        }
    }

    public IPhysicsBody PhysicsBody { get; set; }
    public abstract VecD GetCollisionCentroid(VecD intersectionCenter);
    public abstract VecD GetClosestPointTo(VecD point);

    public VectorPath WorldPath
    {
        get
        {
            var path = new VectorPath();
            path.AddPath(LocalPath, PhysicsBody.TrsMatrix.ToDrawieMatrix(), AddPathMode.Append);
            return path;
        }
    }

    public void Attach(IPhysicsBody body)
    {
        PhysicsBody = body;
    }

    public bool IsColliding(ICollider other, out CollisionData[] collisions)
    {
        /*if (other is T otherCollider)
        {
            return CollidesWith(otherCollider);
        }*/
        if (other.PhysicsBody == PhysicsBody)
        {
            collisions = [];
            return false;
        }

        return PathCollides(other, out collisions);
    }

    protected bool PathCollides(ICollider other, out CollisionData[] collisions)
    {
        using var worldPath = WorldPath;

        using var otherOffsetedPath = other.WorldPath;
        using var intersectedPath = otherOffsetedPath.Op(worldPath, VectorPathOp.Intersect);

        bool collides = !intersectedPath.IsEmpty;
        collisions = [];
        if (collides)
        {
            EditableVectorPath splittedPath = new EditableVectorPath(intersectedPath);
            collisions = new CollisionData[splittedPath.SubShapes.Count];

            for (var i = 0; i < splittedPath.SubShapes.Count; i++)
            {
                var subShape = splittedPath.SubShapes[i];
                var intersection = intersectedPath.TightBounds;
                var intersectionCenter = intersection.Center;

                var collisionCenter = GetCollisionCentroid(intersection.Center);
                var normal = (intersectionCenter - collisionCenter).Normalize();

                // find the penetration depth by getting contact point, mapping intersection onto a normal and adding size to it

                VecD contactPoint = GetClosestPointTo(intersectionCenter);
                VecD? otherPoint = subShape.GetClosestPointOnPath(contactPoint + normal, float.PositiveInfinity);
                double penetration = 0;
                if (otherPoint.HasValue)
                {
                    penetration = VecD.Distance(contactPoint, otherPoint.Value);
                }

                CollisionData collisionData = new CollisionData(this, other, normal, penetration);
                collisions[i] = collisionData;
            }

            return true;
        }

        return collides;
    }

    //protected abstract bool CollidesWith(T other, out VecD normal, out double penetration);

    public abstract bool Intersects(VecD point);
}