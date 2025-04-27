using System.Diagnostics;

namespace Evolo.Simulation.Engine;

public class SimulationScene
{
    public List<ISimulableEntity> SimulableEntities { get; private set; } = new List<ISimulableEntity>();
    private bool isRunning;
    private Task simulationTask;

    private Stopwatch stopwatch = new Stopwatch();

    public void Run()
    {
        if(simulationTask is { IsCompleted: false })
        {
            return;
        }

        isRunning = true;
        simulationTask = Task.Run(Simulate);
    }

    public void Stop()
    {
        isRunning = false;
    }

    private void Simulate()
    {
        while (isRunning)
        {
            SimulateStep();
        }
    }

    private void SimulateStep()
    {
        stopwatch.Restart();
        foreach (var entity in SimulableEntities)
        {
            entity.Simulate();
        }
        Console.WriteLine($"Simulated in: {stopwatch.ElapsedMilliseconds} ms");
    }
}