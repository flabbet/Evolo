using Drawie.Numerics;

namespace Evolo.Simulation.Engine;

public interface ISimulableEntity
{
    public VecD Position { get; }
    public void Simulate();
}