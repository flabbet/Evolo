using Drawie.Backend.Core.Numerics;
using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics.Colliders;

public class RectangleCollider : ConvexCollider
{
    public double RotationAngle { get; set; }
    public override VectorPath LocalPath { get; }
    public RectD Rectangle { get; set; }

    public RectangleCollider(VecD position, VecD size, double rotation = 0)
    {
        Rectangle = new RectD(position, size);
        var path = new VectorPath();
        path.AddRect(Rectangle);
        if (rotation != 0)
        {
            path.Transform(Matrix3X3.CreateRotationDegrees((float)rotation, (float)Rectangle.Center.X, (float)Rectangle.Center.Y));
        }

        LocalPath = path;
    }

    public override bool Intersects(VecD point)
    {
        return AABB.ContainsInclusive(point);
    }

    public override VecD GetClosestPointTo(VecD point)
    {
        VecD closestPoint = point;

        if (point.X < Rectangle.Left)
            closestPoint.X = Rectangle.Left;
        else if (point.X > Rectangle.Right)
            closestPoint.X = Rectangle.Right;

        if (point.Y < Rectangle.Top)
            closestPoint.Y = Rectangle.Top;
        else if (point.Y > Rectangle.Bottom)
            closestPoint.Y = Rectangle.Bottom;

        return closestPoint;
    }

    public override VecD GetCollisionCentroid(VecD intersectionCenter)
    {
        return WorldPath.TightBounds.Center;
    }
}