using Drawie.Numerics;
using Evolo.Physics.Colliders;
using Evolo.Simulation.Engine;

namespace Evolo.Simulation.Core;

public class PolyObject : PhysicsObject, ISimulableEntity
{
    public PolyObject(List<ConvexCollider> colliders)
    {
        Collider = new ComplexCollider(colliders);
        Position = new VecD(0, 0);

        foreach (var collider in colliders)
        {
            collider.Attach(this);
        }
    }

    public void Simulate()
    {

    }
}