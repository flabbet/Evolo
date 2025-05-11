using System.Numerics;
using Drawie.Backend.Core.Numerics;
using Drawie.Numerics;
using Evolo.Physics.Math;

namespace Evolo.Physics;

public class PhysicsScene
{
    public VecD Gravity { get; set; } = new VecD(0, -9.807);

    public List<IPhysicsBody> PhysicsBodies = new List<IPhysicsBody>();


    public static CollisionData[] lastCollisions;

    public void Simulate(double fixedStep)
    {
        foreach (var body in PhysicsBodies)
        {
            if (body.IsStatic)
            {
                body.LinearVelocity = VecD.Zero;
                continue;
            }

            var acceleration = (body.Force * (1f / body.Mass) + Gravity);

            body.LinearVelocity += acceleration * fixedStep;
            VecD positionChange = body.LinearVelocity * fixedStep;

            body.TrsMatrix *= Matrix3x3.CreateTranslation(positionChange);

            body.Force = VecD.Zero;

            CheckCollisions(body);
        }
    }

    public void AddBody(IPhysicsBody physicsBody)
    {
        PhysicsBodies.Add(physicsBody);
    }

    private void CheckCollisions(IPhysicsBody body)
    {
        foreach (var otherBody in PhysicsBodies)
        {
            if (body == otherBody) continue;
            if (body.Collider == null || otherBody.Collider == null) continue;

            CollisionData[] collisions;
            if (body.Collider.IsColliding(otherBody.Collider, out collisions))
            {
                foreach (var collision in collisions)
                {
                    ResolveImpulse(body, otherBody, collision.Normal);
                    CorrectPosition(body, otherBody, collision);
                }

                //if(lastCollisions == null)
                    lastCollisions = collisions;
            }
        }
    }

    private static void ResolveImpulse(IPhysicsBody body, IPhysicsBody otherBody, VecD normal)
    {
        double relativeVelocity = (otherBody.LinearVelocity - body.LinearVelocity) * normal;

        if (relativeVelocity < 0)
        {
            double invMass1 = 1 / body.Mass;
            double invMass2 = otherBody.IsStatic ? 0 : 1 / otherBody.Mass;

            var scalar = -(1 + body.Bounciness) * relativeVelocity / (invMass1 + invMass2);
            var impulse = scalar * normal;

            body.LinearVelocity -= impulse / body.Mass;
            if (!otherBody.IsStatic)
            {
                otherBody.LinearVelocity += impulse / otherBody.Mass;
            }
        }
    }

    private static void CorrectPosition(IPhysicsBody bodyA, IPhysicsBody bodyB, CollisionData collision)
    {
        double invMassA = bodyA.IsStatic ? 0 : 1 / bodyA.Mass;
        double invMassB = bodyB.IsStatic ? 0 : 1 / bodyB.Mass;

        const double percent = 0.8;
        const double slop = 0.01;

        double correctionMag = System.Math.Max(collision.PenetrationDepth - slop, 0) / (invMassA + invMassB) * percent;
        VecD correction = correctionMag * collision.Normal;

        if (!bodyA.IsStatic)
            bodyA.Position -= correction * invMassA;
        if (!bodyB.IsStatic)
            bodyB.Position += correction * invMassB;
    }
}