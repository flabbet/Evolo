using Drawie.Backend.Core;
using Drawie.Backend.Core.ColorsImpl;
using Drawie.Backend.Core.ColorsImpl.Paintables;
using Drawie.Backend.Core.Surfaces;
using Drawie.Backend.Core.Surfaces.PaintImpl;
using Drawie.Backend.Core.Text;
using Drawie.Backend.Core.Vector;
using Drawie.Numerics;
using Drawie.Windowing;
using Drawie.Windowing.Input;
using DrawiEngine;
using Evolo.Physics;
using Evolo.Physics.Colliders;
using Evolo.Renderer;
using Evolo.Simulation.Core;
using Evolo.Simulation.Engine;

namespace Evolo;

public class EvoloApp : DrawieApp
{
    private IWindow window;
    private SceneRenderer sceneRenderer;

    public double TimeScale { get; set; } = 1;

    public override IWindow CreateMainWindow()
    {
        window = Engine.WindowingPlatform.CreateWindow("Evolo", new VecI(1280, 720));
        return window;
    }

    protected override void OnInitialize()
    {
        SimulationScene scene = new SimulationScene();

        Wall floor = new Wall(new VecD(-50, -30), new VecD(100, 5));
        scene.AddEntity(floor);

        List<ConvexCollider> colliders = new List<ConvexCollider>();

        colliders.Add(new RectangleCollider(VecD.Zero, new VecD(3, 1), 45));
        colliders.Add(new CircleCollider(new VecD(0, -0.5), 1));

        PolyObject polyObject = new PolyObject(colliders);

        var coliders2 = new List<ConvexCollider>();
        coliders2.Add(new RectangleCollider(new VecD(0, 0), new VecD(1, 1), 0));

        PolyObject polyObject2 = new PolyObject(coliders2);

        polyObject2.Position = new VecD(0, -20);

        sceneRenderer = new SceneRenderer(scene)
        {
            ViewportPosition = window.Size / 2f
        };

        scene.AddEntity(polyObject);
        scene.AddEntity(polyObject2);

        sceneRenderer.DebugDraw += SceneRendererOnDebugDraw;
        window.Render += WindowOnRender;
        window.Update += WindowOnUpdate;

        if (window.InputController.PrimaryPointer != null)
        {
            window.InputController.PrimaryPointer.PointerPressed += PrimaryPointerOnPointerClicked;
        }

        scene.Run();
    }

    private void WindowOnUpdate(double deltaTime)
    {
        deltaTime *= TimeScale;
        sceneRenderer.Scene.TickSimulation(deltaTime);

        const double speed = 100;
        if (window.InputController.PrimaryKeyboard == null)
        {
            return;
        }

        VecD pan = new VecD(0, 0);
        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.W))
        {
            pan += new VecD(0, 1);
        }

        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.S))
        {
            pan += new VecD(0, -1);
        }

        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.A))
        {
            pan += new VecD(1, 0);
        }

        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.D))
        {
            pan += new VecD(-1, 0);
        }


        double scale = 0;
        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.Q))
        {
            scale += 0.025;
        }

        if (window.InputController.PrimaryKeyboard.IsKeyPressed(Key.E))
        {
            scale += -0.025;
        }


        sceneRenderer.ViewportScale += scale * speed * deltaTime;

        double panSpeed = speed * sceneRenderer.ViewportScale;
        sceneRenderer.ViewportPosition += pan * panSpeed * deltaTime;
    }

    private void SceneRendererOnDebugDraw(Canvas canvas)
    {
        if (window.InputController.PrimaryPointer != null)
        {
            string pointerPosition = $"Pointer Position: {window.InputController.PrimaryPointer.Position}";
            var worldPosition = ViewportToWorld(window.InputController.PrimaryPointer.Position);
            string worldPointerPosition = $"World Pointer Position: x: {worldPosition.X:F2} y: {worldPosition.Y:F2}";

            RichText debugText = new RichText(pointerPosition + "\n" + worldPointerPosition);
            var font = Font.CreateDefault();
            font.Size = 16;

            debugText.Fill = true;
            debugText.FillPaintable = new ColorPaintable(Colors.White);
            using var paint = new Paint();
            debugText.Paint(canvas, new VecD(0, 0), font, paint, null);
        }
    }

    private void PrimaryPointerOnPointerClicked(IPointer pointer, PointerButton button, VecD position)
    {
        if (button == PointerButton.Left)
        {
            var cell = new Cell();
            cell.Position = ViewportToWorld(position);
            sceneRenderer.Scene.AddEntity(cell);
        }
    }

    private void WindowOnRender(Texture renderTexture, double deltaTime)
    {
        sceneRenderer.Render(renderTexture, deltaTime);
    }

    private VecD ViewportToWorld(VecD position)
    {
        double metersPerPixel = 1 / SimulationScene.PixelsPerMeter;
        VecD yFixedPosition = new VecD(position.X, -position.Y);
        VecD yFixedViewportPosition = new VecD(sceneRenderer.ViewportPosition.X, -sceneRenderer.ViewportPosition.Y);

        var scaleAdjusted = metersPerPixel / sceneRenderer.ViewportScale;
        return (yFixedPosition - yFixedViewportPosition) * scaleAdjusted;
    }
}