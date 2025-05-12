using Drawie.Backend.Core.Numerics;
using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public class ComplexCollider : Collider<ComplexCollider>
{
    public List<ConvexCollider> ConvexColliders { get; }
    public override VectorPath LocalPath { get; }

    public ComplexCollider(List<ConvexCollider> convexShapes)
    {
        ConvexColliders = new List<ConvexCollider>(convexShapes);
        LocalPath = BuildVectorPath();
    }

    public override bool Intersects(VecD point)
    {
        return WorldPath.Contains((float)point.X, (float)point.Y);
    }

    public override VecD GetCollisionCentroid(VecD intersectionCenter)
    {
        ConvexCollider closestCollider = FindClosestCollider(intersectionCenter);
        return closestCollider.GetCollisionCentroid(intersectionCenter);
    }

    public override VecD GetClosestPointTo(VecD point)
    {
        var closestCollider = FindClosestCollider(point);
        return closestCollider.GetClosestPointTo(point);
    }


    private VectorPath BuildVectorPath()
    {
        var path = new VectorPath();
        foreach (var collider in ConvexColliders)
        {
            path.AddPath(collider.LocalPath, AddPathMode.Append);
        }

        return path;
    }

    private ConvexCollider FindClosestCollider(VecD point)
    {
        ConvexCollider closestCollider = null;
        double closestDistance = double.MaxValue;

        foreach (var collider in ConvexColliders)
        {
            VecD closestPathPoint = collider.GetClosestPointTo(point);
            double distance = VecD.Distance(closestPathPoint, point);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }

        return closestCollider!;
    }
}