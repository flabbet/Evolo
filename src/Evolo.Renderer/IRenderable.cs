using Drawie.Backend.Core.Surfaces;

namespace Evolo.Renderer;

public interface IRenderable
{
    public void Render(RenderContext renderContext);
}