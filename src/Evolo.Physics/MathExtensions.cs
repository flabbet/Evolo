using Drawie.Backend.Core.Numerics;
using Evolo.Physics.Math;

namespace Evolo.Physics;

public static class MathExtensions
{
    public static Matrix3X3 ToDrawieMatrix(this Matrix3x3 matrix)
    {
        return new Matrix3X3(
            (float)matrix.M00, (float)matrix.M01, (float)matrix.M02,
            (float)matrix.M10, (float)matrix.M11, (float)matrix.M12,
            (float)matrix.M20, (float)matrix.M21, (float)matrix.M22
        );
    }
}