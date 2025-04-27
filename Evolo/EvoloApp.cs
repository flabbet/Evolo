using Drawie.Numerics;
using Drawie.Windowing;
using DrawiEngine;
using Evolo.Simulation.Core;
using Evolo.Simulation.Engine;

namespace Evolo;

public class EvoloApp : DrawieApp
{
    public override IWindow CreateMainWindow()
    {
        return Engine.WindowingPlatform.CreateWindow("Evolo", new VecI(1280, 720));
    }

    protected override void OnInitialize()
    {
        SimulationScene scene = new SimulationScene();

        scene.SimulableEntities.Add(new Cell());
        scene.Run();
    }
}