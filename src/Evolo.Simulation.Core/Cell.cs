using System.Numerics;
using Drawie.Backend.Core.Numerics;
using Drawie.Numerics;
using Evolo.Physics;
using Evolo.Physics.Math;

namespace Evolo.Simulation.Core;

public class Cell : ICell, IPhysicsBody
{
    public VecD Position => new VecD(TrsMatrix.Translation.X, TrsMatrix.Translation.Y);
    public Matrix3x3 TrsMatrix { get; set; } = Matrix3x3.Identity;
    public VecD LinearVelocity { get; set; }
    public VecD Force { get; set; }
    public double Mass { get; set; } = 1;
    public ICollider Collider { get; }

    public PhysicalProperties PhysicalProperties { get; }
    public ChemicalProperties ChemicalProperties { get; }


    public void Simulate()
    {
    }
}