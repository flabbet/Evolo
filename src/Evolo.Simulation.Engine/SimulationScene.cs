using System.Diagnostics;
using Evolo.Physics;

namespace Evolo.Simulation.Engine;

public class SimulationScene
{
    public const double PixelsPerMeter = 10;
    public IReadOnlyList<ISimulableEntity> SimulableEntities => entities;
    public PhysicsScene PhysicsScene { get; private set; } = new PhysicsScene();
    public double LastTps { get; private set; }

    public double PhysicsTimeStep { get; set; } = 16; // ms
    public double PhysicsTimeStepInSeconds => PhysicsTimeStep / 1000d;

    private List<ISimulableEntity> entities = new List<ISimulableEntity>();
    private bool isRunning;

    private double accumulatedTime = 0;

    private Queue<ISimulableEntity> entitiesToAdd = new Queue<ISimulableEntity>();

    public void Run()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void AddEntity(ISimulableEntity entity)
    {
        if (isRunning)
        {
            entitiesToAdd.Enqueue(entity);
            return;
        }

        AddEntityInternal(entity);
    }

    private void AddEntityInternal(ISimulableEntity entity)
    {
        if (entity is IPhysicsBody physicsBody)
        {
            PhysicsScene.AddBody(physicsBody);
        }

        entities.Add(entity);
    }

    public void TickSimulation(double deltaTime)
    {
        double ms = deltaTime * 1000;
        SimulatePhysics(ref accumulatedTime);
        SimulateStep();
        accumulatedTime += ms;
        LastTps = ms;
    }

    private void SimulateStep()
    {
        foreach (var entity in SimulableEntities)
        {
            entity.Simulate();
        }

        while (entitiesToAdd.Count > 0)
        {
            var entity = entitiesToAdd.Dequeue();
            AddEntityInternal(entity);
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