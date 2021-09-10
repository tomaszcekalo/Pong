using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Utilities;

namespace Pong
{
    public class VelcroPhysicsTransformSystem : UnifiedSystem<VelcroPhysicsComponent, TransformComponent>
    {
        public override void ProcessSingleEntity(int entityId,
            ref VelcroPhysicsComponent a,
            ref TransformComponent b)
        {
            b.Position = ConvertUnits.ToDisplayUnits(a.Body.Position);
            b.Rotation = a.Body.Rotation;
        }
    }
}