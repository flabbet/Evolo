using Evolo.Simulation.Engine;

namespace Evolo.Simulation.Core;

public interface ICell : ISimulableEntity
{
    public PhysicalProperties PhysicalProperties { get; }
    public ChemicalProperties ChemicalProperties { get; }
}