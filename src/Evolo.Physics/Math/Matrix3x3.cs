using Drawie.Numerics;

namespace Evolo.Physics.Math;

public struct Matrix3x3
{

    public static Matrix3x3 Identity => new Matrix3x3(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1);

    public static Matrix3x3 Zero => new Matrix3x3(
        0, 0, 0,
        0, 0, 0,
        0, 0, 0);

    public double M00 { get; set; }
    public double M01 { get; set; }
    public double M02 { get; set; }
    public double M10 { get; set; }
    public double M11 { get; set; }
    public double M12 { get; set; }
    public double M20 { get; set; }
    public double M21 { get; set; }
    public double M22 { get; set; }

    public VecD Translation => new VecD(M02, M12);

    public VecD Scale => new VecD(
        System.Math.Sqrt(M00 * M00 + M01 * M01),
        System.Math.Sqrt(M10 * M10 + M11 * M11));

    public double RotationAngle
    {
        get
        {
            double angle = System.Math.Atan(M01 / M11);
            return angle;
        }
    }
    public Matrix3x3(double m00, double m01, double m02,
        double m10, double m11, double m12,
        double m20, double m21, double m22)
    {
        M00 = m00;
        M01 = m01;
        M02 = m02;
        M10 = m10;
        M11 = m11;
        M12 = m12;
        M20 = m20;
        M21 = m21;
        M22 = m22;
    }

    public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
    {
        return new Matrix3x3(
            a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20,
            a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21,
            a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22,

            a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20,
            a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21,
            a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22,

            a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20,
            a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21,
            a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22
        );
    }

    public static Matrix3x3 operator +(Matrix3x3 a, Matrix3x3 b)
    {
        return new Matrix3x3(
            a.M00 + b.M00, a.M01 + b.M01, a.M02 + b.M02,
            a.M10 + b.M10, a.M11 + b.M11, a.M12 + b.M12,
            a.M20 + b.M20, a.M21 + b.M21, a.M22 + b.M22
        );
    }

    public static Matrix3x3 CreateTranslation(double x, double y)
    {
        return new Matrix3x3(
            1, 0, x,
            0, 1, y,
            0, 0, 1);
    }

    public static Matrix3x3 CreateScale(double x, double y)
    {
        return new Matrix3x3(
            x, 0, 0,
            0, y, 0,
            0, 0, 1);
    }

    public static Matrix3x3 CreateRotation(double angleRad)
    {
        double cos = System.Math.Cos(angleRad);
        double sin = System.Math.Sin(angleRad);

        return new Matrix3x3(
            cos, sin, 0,
            -sin, cos, 0,
            0, 0, 1);
    }

    public static Matrix3x3 CreateTranslation(VecD by)
    {
        return CreateTranslation(by.X, by.Y);
    }
}