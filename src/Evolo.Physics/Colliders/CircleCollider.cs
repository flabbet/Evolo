using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public class CircleCollider : ConvexCollider
{
    public VecD LocalCenter { get; }
    public VecD WorldCenter => PhysicsBody?.Position ?? VecD.Zero + LocalCenter;
    public double Radius { get; }

    public override VectorPath LocalPath { get; }

    public CircleCollider(VecD localCenter, double radius)
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

    /*protected override bool CollidesWith(CircleCollider other)
    {
        double distance = VecD.Distance(WorldCenter, other.WorldCenter);
        return distance < Radius + other.Radius;
    }*/


    public override bool Intersects(VecD point)
    {
        double distance = VecD.Distance(WorldCenter, point);
        return distance < Radius;
    }

    public override VecD GetClosestPointTo(VecD point)
    {
        VecD direction = point - WorldCenter;
        double distance = direction.Length;

        if (distance == 0)
            return WorldCenter;

        direction /= distance;
        return WorldCenter + direction * Radius;
    }

    public override VecD GetCollisionCentroid(VecD intersectionCenter)
    {
        return WorldCenter;
    }
}