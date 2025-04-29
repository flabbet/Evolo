using Drawie.Backend.Core;
using Drawie.Numerics;
using Drawie.Windowing;
using Drawie.Windowing.Input;
using DrawiEngine;
using Evolo.Renderer;
using Evolo.Simulation.Core;
using Evolo.Simulation.Engine;

namespace Evolo;

public class EvoloApp : DrawieApp
{
    private IWindow window;
    private SceneRenderer sceneRenderer;

    public override IWindow CreateMainWindow()
    {
        window = Engine.WindowingPlatform.CreateWindow("Evolo", new VecI(1280, 720));
        return window;
    }

    protected override void OnInitialize()
    {
        SimulationScene scene = new SimulationScene();

        Cell cell = new Cell();
        scene.AddEntity(cell);

        sceneRenderer = new SceneRenderer(scene);
        window.Render += WindowOnRender;
        window.Update += WindowOnUpdate;

        scene.Run();
    }

    private void WindowOnUpdate(double deltaTime)
    {
        const double speed = 100;
        if (window.InputController.DefaultKeyboard == null)
        {
            return;
        }

        VecD pan = new VecD(0, 0);
        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.W))
        {
            pan += new VecD(0, 1);
        }
        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.S))
        {
            pan += new VecD(0, -1);
        }
        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.A))
        {
            pan += new VecD(1, 0);
        }
        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.D))
        {
            pan += new VecD(-1, 0);
        }


        double scale = 0;
        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.Q))
        {
            scale += 0.1;
        }

        if (window.InputController.DefaultKeyboard.IsKeyPressed(Key.E))
        {
            scale += -0.1;
        }

        sceneRenderer.ViewportScale += scale * speed * deltaTime;

        double panSpeed = speed * sceneRenderer.ViewportScale;
        sceneRenderer.ViewportPosition += pan * panSpeed * deltaTime;
    }

    private void WindowOnRender(Texture renderTexture, double deltaTime)
    {
        sceneRenderer.Render(renderTexture, deltaTime);
    }
}