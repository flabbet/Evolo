using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public class RectangleCollider : Collider<RectangleCollider>
{
    public override VectorPath LocalPath { get; }
    public RectD Rectangle { get; set; }

    public RectangleCollider(VecD position, VecD size, IPhysicsBody body) : base(body)
    {
        Rectangle = new RectD(position, size);
        var path = new VectorPath();
        path.AddRect(Rectangle);

        LocalPath = path;
    }

    protected override bool CollidesWith(RectangleCollider other)
    {
        return AABB.IntersectsWithInclusive(other.AABB);
    }

    public override bool Intersects(VecD point)
    {
        return AABB.ContainsInclusive(point);
    }
}