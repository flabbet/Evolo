using Drawie.Backend.Core.Vector;
using Drawie.Numerics;

namespace Evolo.Physics;

internal static class VectorMath
{
    public static VecD? GetClosestPointOnSegment(VecD point, Verb verb)
    {
        if (verb == null || verb.IsEmptyVerb()) return null;

        switch (verb.VerbType)
        {
            case PathVerb.Move:
                return (VecD)verb.From;
            case PathVerb.Line:
                return ClosestPointOnLine((VecD)verb.From, (VecD)verb.To, point);
            case PathVerb.Quad:
                return GetClosestPointOnQuad(point, (VecD)verb.From, (VecD)(verb.ControlPoint1 ?? verb.From),
                    (VecD)verb.To);
            case PathVerb.Conic:
                return GetClosestPointOnConic(point, (VecD)verb.From, (VecD)(verb.ControlPoint1 ?? verb.From),
                    (VecD)verb.To,
                    verb.ConicWeight);
            case PathVerb.Cubic:
                return GetClosestPointOnCubic(point, (VecD)verb.From, (VecD)(verb.ControlPoint1 ?? verb.From),
                    (VecD)(verb.ControlPoint2 ?? verb.To), (VecD)verb.To);
            case PathVerb.Close:
                return (VecD)verb.From;
            case PathVerb.Done:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    public static bool IsPointOnSegment(VecD point, Verb shapePointVerb)
    {
        if (shapePointVerb.IsEmptyVerb()) return false;

        switch (shapePointVerb.VerbType)
        {
            case PathVerb.Move:
                return System.Math.Abs(point.X - shapePointVerb.From.X) < 0.0001 &&
                       System.Math.Abs(point.Y - shapePointVerb.From.Y) < 0.0001;
            case PathVerb.Line:
                return IsPointOnLine(point, (VecD)shapePointVerb.From, (VecD)shapePointVerb.To);
            case PathVerb.Quad:
                return IsPointOnQuad(point, (VecD)shapePointVerb.From,
                    (VecD)(shapePointVerb.ControlPoint1 ?? shapePointVerb.From),
                    (VecD)shapePointVerb.To);
            case PathVerb.Conic:
                return IsPointOnConic(point, (VecD)shapePointVerb.From,
                    (VecD)(shapePointVerb.ControlPoint1 ?? shapePointVerb.From),
                    (VecD)shapePointVerb.To, shapePointVerb.ConicWeight);
            case PathVerb.Cubic:
                return IsPointOnCubic(point, (VecD)shapePointVerb.From,
                    (VecD)(shapePointVerb.ControlPoint1 ?? shapePointVerb.From),
                    (VecD)(shapePointVerb.ControlPoint2 ?? shapePointVerb.To), (VecD)shapePointVerb.To);
            case PathVerb.Close:
                break;
            case PathVerb.Done:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    public static VecD ClosestPointOnLine(VecD start, VecD end, VecD point)
    {
        VecD startToPoint = point - start;
        VecD startToEnd = end - start;

        double sqrtMagnitudeToEnd = System.Math.Pow(startToEnd.X, 2) + System.Math.Pow(startToEnd.Y, 2);

        double dot = startToPoint.X * startToEnd.X + startToPoint.Y * startToEnd.Y;
        var t = dot / sqrtMagnitudeToEnd;

        if (t < 0) return start;
        if (t > 1) return end;

        return start + startToEnd * t;
    }

    public static bool IsPointOnLine(VecD point, VecD start, VecD end)
    {
        return System.Math.Abs(VecD.Distance(start, point) + VecD.Distance(end, point) - VecD.Distance(start, end)) <
               0.001f;
    }

    public static VecD GetClosestPointOnQuad(VecD point, VecD start, VecD controlPoint, VecD end)
    {
        return FindClosestPointBruteForce(point, (t) => QuadraticBezier(start, controlPoint, end, t));
    }

    public static VecD GetClosestPointOnCubic(VecD point, VecD start, VecD controlPoint1, VecD controlPoint2, VecD end)
    {
        return FindClosestPointBruteForce(point, (t) => CubicBezier(start, controlPoint1, controlPoint2, end, t));
    }

    public static VecD GetClosestPointOnConic(VecD point, VecD start, VecD controlPoint, VecD end, float weight)
    {
        return FindClosestPointBruteForce(point, (t) => ConicBezier(start, controlPoint, end, weight, t));
    }

    public static bool IsPointOnQuad(VecD point, VecD start, VecD controlPoint, VecD end)
    {
        return IsPointOnPath(point, (t) => QuadraticBezier(start, controlPoint, end, t));
    }

    public static bool IsPointOnCubic(VecD point, VecD start, VecD controlPoint1, VecD controlPoint2, VecD end)
    {
        return IsPointOnPath(point, (t) => CubicBezier(start, controlPoint1, controlPoint2, end, t));
    }

    public static bool IsPointOnConic(VecD point, VecD start, VecD controlPoint, VecD end, float weight)
    {
        return IsPointOnPath(point, (t) => ConicBezier(start, controlPoint, end, weight, t));
    }

    /// <summary>
    ///     Finds value from 0 to 1 that represents the position of point on the segment.
    /// </summary>
    /// <param name="onVerb">Verb that represents the segment.</param>
    /// <param name="point">Point that is on the segment.</param>
    /// <returns>Value from 0 to 1 that represents the position of point on the segment.</returns>
    public static float GetNormalizedSegmentPosition(Verb onVerb, VecF point)
    {
        if (onVerb.IsEmptyVerb()) return 0;

        if (onVerb.VerbType == PathVerb.Cubic)
        {
            return (float)FindNormalizedSegmentPositionBruteForce(point, (t) =>
                CubicBezier((VecD)onVerb.From, (VecD)(onVerb.ControlPoint1 ?? onVerb.From),
                    (VecD)(onVerb.ControlPoint2 ?? onVerb.To), (VecD)onVerb.To, t));
        }

        throw new NotImplementedException();
    }

    private static VecD FindClosestPointBruteForce(VecD point, Func<double, VecD> func, double step = 0.001)
    {
        double minDistance = double.MaxValue;
        VecD closestPoint = new VecD();
        for (double t = 0; t <= 1; t += step)
        {
            VecD currentPoint = func(t);
            double distance = VecD.Distance(point, currentPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = currentPoint;
            }
        }

        return closestPoint;
    }

    private static double FindNormalizedSegmentPositionBruteForce(VecF point, Func<double, VecD> func,
        double step = 0.001)
    {
        double minDistance = float.MaxValue;
        double closestT = 0;
        for (double t = 0; t <= 1; t += step)
        {
            VecD currentPoint = func(t);
            float distance = (point - currentPoint).Length;
            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = t;
            }
        }

        return closestT;
    }

    private static bool IsPointOnPath(VecD point, Func<double, VecD> func, double step = 0.001)
    {
        for (double t = 0; t <= 1; t += step)
        {
            VecD currentPoint = func(t);
            if (VecD.Distance(point, currentPoint) < 0.1)
            {
                return true;
            }
        }

        return false;
    }

    private static VecD QuadraticBezier(VecD start, VecD control, VecD end, double t)
    {
        double x = System.Math.Pow(1 - t, 2) * start.X + 2 * (1 - t) * t * control.X + System.Math.Pow(t, 2) * end.X;
        double y = System.Math.Pow(1 - t, 2) * start.Y + 2 * (1 - t) * t * control.Y + System.Math.Pow(t, 2) * end.Y;
        return new VecD(x, y);
    }

    private static VecD CubicBezier(VecD start, VecD control1, VecD control2, VecD end, double t)
    {
        double x = System.Math.Pow(1 - t, 3) * start.X + 3 * System.Math.Pow(1 - t, 2) * t * control1.X +
                   3 * (1 - t) * System.Math.Pow(t, 2) * control2.X + System.Math.Pow(t, 3) * end.X;
        double y = System.Math.Pow(1 - t, 3) * start.Y + 3 * System.Math.Pow(1 - t, 2) * t * control1.Y +
                   3 * (1 - t) * System.Math.Pow(t, 2) * control2.Y + System.Math.Pow(t, 3) * end.Y;
        return new VecD(x, y);
    }

    private static VecD ConicBezier(VecD start, VecD control, VecD end, float weight, double t)
    {
        double b0 = (1 - t) * (1 - t);
        double b1 = 2 * t * (1 - t);
        double b2 = t * t;

        VecD numerator = (start * b0) + (control * b1 * weight) + (end * b2);

        double denominator = b0 + (b1 * weight) + b2;

        return numerator / denominator;
    }

    public static VecD? GetLineIntersection(VecD from, VecD to, VecD from1, VecD to1)
    {
        double x1 = from.X, y1 = from.Y;
        double x2 = to.X, y2 = to.Y;
        double x3 = from1.X, y3 = from1.Y;
        double x4 = to1.X, y4 = to1.Y;

        double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (System.Math.Abs(denom) < 1e-10)
            return null; // Lines are parallel or coincident

        double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
        double u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / denom;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            double ix = x1 + t * (x2 - x1);
            double iy = y1 + t * (y2 - y1);
            return new VecD(ix, iy);
        }

        return null; // Intersection is outside the bounds of the segments
    }

    public static VecD GetLineNormal(VecF from, VecF to)
    {
        VecD dir = new VecD(to.X - from.X, to.Y - from.Y);
        VecD normal = new VecD(-dir.Y, dir.X);

        double length = MathF.Sqrt((float)(normal.X * normal.X + normal.Y * normal.Y));
        if (length > 1e-6f)
        {
            normal = new VecD(normal.X / length, normal.Y / length);
        }

        return normal;
    }

    public static VecD? GetConicIntersection(VecD from, VecD to, VecD conicFrom, VecD controlPoint, VecD conicTo,
        float conicWeight)
    {
        var d = to - from;
        var n = new VecD(-d.Y, d.X); // Normal of the line

        // Line equation: dot((p - from), n) = 0 → dot(p, n) - dot(from, n) = 0
        double c = from.Dot(n);

        // Conic Bézier:
        // Numerator: (1 - t)^2 * P0 + 2(1 - t)t * w * P1 + t^2 * P2
        // Denominator: (1 - t)^2 + 2(1 - t)t * w + t^2

        // Let’s expand numerator and denominator
        // Then plug x(t), y(t) into the line equation: dot(B(t), n) = c * w(t)
        // Leads to quadratic in t

        double w = conicWeight;

        VecD A = conicFrom;
        VecD B = controlPoint;
        VecD C = conicTo;

        // Compute coefficients for the rational Bézier curve
        VecD p0 = A * 1;
        VecD p1 = B * (2 * w);
        VecD p2 = C * 1;

        // Coefficients of the numerator polynomial
        VecD N0 = p0;
        VecD N1 = (B * (2 * w)) - (A * 2);
        VecD N2 = A - (B * (2 * w)) + C;

        // Coefficients of the denominator polynomial: D(t) = a*t^2 + b*t + c
        double D0 = 1;
        double D1 = 2 * w - 2;
        double D2 = 1 - 2 * w + w * w;

        // Now build the implicit form:
        // dot(N(t), n) - c * D(t) = 0
        // where N(t) = N0 + N1*t + N2*t^2

        double a = N2.Dot(n) - c * D2;
        double b = N1.Dot(n) - c * D1;
        double cc = N0.Dot(n) - c * D0;

        // Solve a*t^2 + b*t + cc = 0
        double discriminant = b * b - 4 * a * cc;
        if (discriminant < 0)
            return null;

        List<double> roots = new();
        if (System.Math.Abs(a) < 1e-8)
        {
            if (System.Math.Abs(b) < 1e-8)
                return null;
            double t = -cc / b;
            roots.Add(t);
        }
        else
        {
            double sqrtD = System.Math.Sqrt(discriminant);
            roots.Add((-b + sqrtD) / (2 * a));
            roots.Add((-b - sqrtD) / (2 * a));
        }

        foreach (var t in roots)
        {
            if (t < 0 || t > 1)
                continue;

            double u = 1 - t;
            double denom = u * u + 2 * u * t * w + t * t;
            VecD point =
                (A * (u * u) + B * (2 * u * t * w) + C * (t * t)) * (1.0 / denom);

            // Now check if point is on the line segment
            VecD ap = point - from;
            double lineLengthSq = d.Dot(d);
            double proj = ap.Dot(d) / lineLengthSq;
            if (proj >= 0 && proj <= 1)
                return point;
        }

        return null;
    }

    public static VecD? GetConicNormal(VecD from, VecD controlPoint1, VecD to, float conicWeight, double t)
    {
        // t = 0.5
        double u = 1.0 - t;

        // De Casteljau-style intermediate points (not strictly needed here but useful for visualization)
        VecD A = from.Lerp(controlPoint1, t);
        VecD B = controlPoint1.Lerp(to, t);

        // Evaluate the point on the conic at t = 0.5
        VecD numerator = from * (u * u) +
                         controlPoint1 * (2.0 * conicWeight * u * t) +
                         to * (t * t);

        double denominator = u * u + 2.0 * conicWeight * u * t + t * t;

        if (System.Math.Abs(denominator) < 1e-8)
            return null; // Degenerate

        VecD pointOnCurve = numerator / denominator;

        // Derivative (tangent vector) at t = 0.5 for rational quadratic Bézier
        VecD dp0 = controlPoint1 - from;
        VecD dp1 = to - controlPoint1;

        VecD dNumerator = dp0 * (2.0 * conicWeight * u) +
                          dp1 * (2.0 * conicWeight * t);

        double dDenominator = 2.0 * (1.0 - conicWeight) * (u - t);

        VecD tangent = (dNumerator * denominator - numerator * dDenominator) / (denominator * denominator);

        if (tangent.Length < 1e-8)
            return null;

        // Normal is perpendicular to tangent: (-dy, dx)
        VecD normal = new VecD(-tangent.Y, tangent.X).Normalize();

        return normal;
    }

    const double Tolerance = 0.5; // Adjust for accuracy/performance tradeoff

    public static VecD? CubicLineIntersection(
        VecD p0, VecD p1, VecD p2, VecD p3, // Cubic Bézier
        VecD lineA, VecD lineB)             // Line segment
    {
        return CubicLineRecursive(p0, p1, p2, p3, lineA, lineB, 0);
    }

    private static VecD? CubicLineRecursive(
        VecD p0, VecD p1, VecD p2, VecD p3,
        VecD lineA, VecD lineB,
        int depth)
    {
        // Flat enough? Treat Bézier as line
        if (IsFlatEnough(p0, p1, p2, p3) || depth > 10)
        {
            VecD? isect = GetLineIntersection(p0, p3, lineA, lineB);
            if (isect != null && IsOnSegment(isect.Value, lineA, lineB) && IsOnSegment(isect.Value, p0, p3))
                return isect;
            return null;
        }

        // Subdivide cubic
        (VecD l0, VecD l1, VecD l2, VecD l3, VecD r0, VecD r1, VecD r2, VecD r3) = SubdivideCubic(p0, p1, p2, p3);

        // Check each half
        VecD? left = CubicLineRecursive(l0, l1, l2, l3, lineA, lineB, depth + 1);
        if (left != null) return left;

        VecD? right = CubicLineRecursive(r0, r1, r2, r3, lineA, lineB, depth + 1);
        return right;
    }

    private static bool IsFlatEnough(VecD p0, VecD p1, VecD p2, VecD p3)
    {
        double d1 = DistancePointToLine(p1, p0, p3);
        double d2 = DistancePointToLine(p2, p0, p3);
        return d1 < Tolerance && d2 < Tolerance;
    }

    private static double DistancePointToLine(VecD p, VecD a, VecD b)
    {
        VecD ab = b - a;
        VecD ap = p - a;
        double t = ap.Dot(ab) / ab.Dot(ab);
        VecD closest = a + ab * t;
        return VecD.Distance(p, closest);
    }

    private static bool IsOnSegment(VecD p, VecD a, VecD b)
    {
        double minX = System.Math.Min(a.X, b.X) - 1e-6;
        double maxX = System.Math.Max(a.X, b.X) + 1e-6;
        double minY = System.Math.Min(a.Y, b.Y) - 1e-6;
        double maxY = System.Math.Max(a.Y, b.Y) + 1e-6;
        return p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY;
    }

    private static (VecD, VecD, VecD, VecD, VecD, VecD, VecD, VecD) SubdivideCubic(VecD p0, VecD p1, VecD p2, VecD p3)
    {
        VecD p01 = p0.Lerp(p1, 0.5);
        VecD p12 = p1.Lerp(p2, 0.5);
        VecD p23 = p2.Lerp(p3, 0.5);
        VecD p012 = p01.Lerp(p12, 0.5);
        VecD p123 = p12.Lerp(p23, 0.5);
        VecD p0123 = p012.Lerp(p123, 0.5);

        return (p0, p01, p012, p0123, p0123, p123, p23, p3);
    }

    public static VecD? GetCubicNormal(VecD from, VecD controlPoint1, VecD controlPoint2, VecD to, double t, bool clockwise = false)
    {
        VecD tangent = GetCubicTangent(from, controlPoint1, controlPoint2, to, t);

        if (tangent.Length < 1e-8)
            return new VecD(0, 0); // or throw

        VecD normal = clockwise
            ? new VecD(tangent.Y, -tangent.X)  // CW normal
            : new VecD(-tangent.Y, tangent.X); // CCW normal

        return normal.Normalize();
    }

    public static VecD GetCubicTangent(VecD p0, VecD p1, VecD p2, VecD p3, double t)
    {
        double u = 1.0 - t;

        VecD a = (p1 - p0) * (3 * u * u);
        VecD b = (p2 - p1) * (6 * u * t);
        VecD c = (p3 - p2) * (3 * t * t);

        return a + b + c;
    }

    public static bool IsClockWise(VecD from, VecD to, VecD vecD, VecD to1)
    {
        VecD line = to - from;
        VecD line1 = to1 - vecD;

        double cross = line.X * line1.Y - line.Y * line1.X;

        return cross < 0;
    }
}