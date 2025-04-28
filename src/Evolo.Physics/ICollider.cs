using Drawie.Numerics;

namespace Evolo.Physics;

public interface ICollider
{
    public bool IsColliding(ICollider other);
    public bool Intersects(VecD point);
}