namespace Evolo.Simulation.Core;

public class Cell : ICell
{
    public PhysicalProperties PhysicalProperties { get; }
    public ChemicalProperties ChemicalProperties { get; }

    public void Simulate()
    {
         Task.Delay(Random.Shared.Next(1000)).Wait();
    }
}