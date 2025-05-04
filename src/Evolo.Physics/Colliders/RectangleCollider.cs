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
        RotationAngle = rotation;
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
        VecD unrotatedPoint = point.Rotate(double.DegreesToRadians(RotationAngle), Rectangle.Center);

        if (unrotatedPoint.X < Rectangle.Left)
            unrotatedPoint.X = Rectangle.Left;
        else if (unrotatedPoint.X > Rectangle.Right)
            unrotatedPoint.X = Rectangle.Right;

        if (unrotatedPoint.Y < Rectangle.Top)
            unrotatedPoint.Y = Rectangle.Top;
        else if (unrotatedPoint.Y > Rectangle.Bottom)
            unrotatedPoint.Y = Rectangle.Bottom;

        return unrotatedPoint.Rotate(-double.DegreesToRadians(RotationAngle), Rectangle.Center);
    }

    public override VecD GetCollisionCentroid(VecD intersectionCenter)
    {
        return WorldPath.TightBounds.Center;
    }
}