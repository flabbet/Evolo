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

    public IPhysicsBody PhysicsBody { get; }

    public VectorPath WorldPath
    {
        get
        {
            var path = new VectorPath();
            path.AddPath(LocalPath, PhysicsBody.TrsMatrix.ToDrawieMatrix(), AddPathMode.Append);
            return path;
        }
    }

    public Collider(IPhysicsBody body)
    {
        PhysicsBody = body;
    }

    public bool IsColliding(ICollider other, out VecD collisionNormal, out double penetration)
    {
        /*if (other is T otherCollider)
        {
            return CollidesWith(otherCollider);
        }*/

        using var worldPath = WorldPath;

        using var otherOffsetedPath = other.WorldPath;
        using var intersectedPath = otherOffsetedPath.Op(worldPath, VectorPathOp.Intersect);

        bool collides = !intersectedPath.IsEmpty;
        collisionNormal = VecD.Zero;
        penetration = 0;
        if (collides)
        {
            // Calculate the collision normal
            var intersection = intersectedPath.TightBounds;
            var center = intersection.Center;
            var normal = (center - PhysicsBody.Position).Normalize();

            collisionNormal = normal;

            var bounds = worldPath.TightBounds;
            var otherBounds = other.WorldPath.TightBounds;

            // Project the two bounding boxes onto the normal
            double aMin = bounds.BottomLeft.Dot(normal);
            double aMax = bounds.TopRight.Dot(normal);
            double bMin = otherBounds.BottomLeft.Dot(normal);
            double bMax = otherBounds.TopRight.Dot(normal);

            double overlap = System.Math.Min(aMax, bMax) - System.Math.Max(aMin, bMin);
            penetration = System.Math.Max(overlap, 0);
        }

        return collides;
    }

    protected abstract bool CollidesWith(T other);

    public abstract bool Intersects(VecD point);
}