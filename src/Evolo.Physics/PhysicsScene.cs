using System.Numerics;
using Drawie.Backend.Core.Numerics;
using Drawie.Numerics;
using Evolo.Physics.Math;

namespace Evolo.Physics;

public class PhysicsScene
{
    public VecD Gravity { get; set; } = new VecD(0, -9.807);

    public List<IPhysicsBody> PhysicsBodies = new List<IPhysicsBody>();
    public void Simulate(double fixedStep)
    {
        foreach (var body in PhysicsBodies)
        {
            var acceleration = (body.Force * (1f / body.Mass) + Gravity);

            body.LinearVelocity += acceleration * fixedStep;
            VecD positionChange = body.LinearVelocity * fixedStep;

            body.TrsMatrix *= Matrix3x3.CreateTranslation(positionChange);

            if (body.Position.Y < -35)
            {
                body.Position = new VecD(body.Position.X, -35);
            }

            body.Force = VecD.Zero;
        }
    }

    public void AddBody(IPhysicsBody physicsBody)
    {
        PhysicsBodies.Add(physicsBody);
    }
}