using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public class CircleCollider : Collider<CircleCollider>
{
    public VecD LocalCenter { get; }
    public VecD WorldCenter => PhysicsBody.Position + LocalCenter;
    public double Radius { get; }

    public override VectorPath LocalPath { get; }

    public CircleCollider(VecD localCenter, double radius, IPhysicsBody body) : base(body)
    {
        LocalCenter = localCenter;
        Radius = radius;

        var path = new VectorPath();

        path.AddOval(new RectD(
            localCenter.X - radius,
            localCenter.Y - radius,
            radius * 2,
            radius * 2
        ));
        LocalPath = path;
    }

    protected override bool CollidesWith(CircleCollider other)
    {
        double distance = VecD.Distance(WorldCenter, other.WorldCenter);
        return distance < Radius + other.Radius;
    }


    public override bool Intersects(VecD point)
    {
        double distance = VecD.Distance(WorldCenter, point);
        return distance < Radius;
    }
}