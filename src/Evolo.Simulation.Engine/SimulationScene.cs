using System.Diagnostics;
using Evolo.Physics;

namespace Evolo.Simulation.Engine;

public class SimulationScene
{
    public IReadOnlyList<ISimulableEntity> SimulableEntities { get; } = new List<ISimulableEntity>();
    public PhysicsScene PhysicsScene { get; private set; } = new PhysicsScene();
    public double LastTps { get; private set; }

    public double PhysicsTimeStep { get; set; } = 16; // ms
    public double PhysicsTimeStepInSeconds => PhysicsTimeStep / 1000d;

    private bool isRunning;
    private Task simulationTask;

    private Stopwatch stopwatch = new Stopwatch();

    public void Run()
    {
        if (simulationTask is { IsCompleted: false })
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

    public void AddEntity(ISimulableEntity entity)
    {
        if (entity is IPhysicsBody physicsBody)
        {
            PhysicsScene.AddBody(physicsBody);
        }

        ((List<ISimulableEntity>)SimulableEntities).Add(entity);
    }

    private void Simulate()
    {
        double accumulatedTime = 0;

        while (isRunning)
        {
            stopwatch.Restart();
            SimulatePhysics(ref accumulatedTime);
            SimulateStep();
            accumulatedTime += stopwatch.Elapsed.TotalMilliseconds;
            LastTps = stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    private void SimulateStep()
    {
        foreach (var entity in SimulableEntities)
        {
            entity.Simulate();
        }
    }

    private void SimulatePhysics(ref double accumulatedTime)
    {
        while (accumulatedTime >= PhysicsTimeStep)
        {
            PhysicsScene.Simulate(PhysicsTimeStepInSeconds);
            accumulatedTime -= PhysicsTimeStep;
        }
    }
}