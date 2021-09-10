using Undine.Core;
using Undine.VelcroPhysics.MonoGame;

namespace Pong
{
    public class VelocityCorrectionSystem : UnifiedSystem<VelocityCorrectionComponent, VelcroPhysicsComponent>
    {
        public override void ProcessSingleEntity(int entityId, ref VelocityCorrectionComponent a, ref VelcroPhysicsComponent b)
        {
            var length = b.Body.LinearVelocity.Length();
            if (length != 0f)
            {
                b.Body.LinearVelocity = b.Body.LinearVelocity
                    / b.Body.LinearVelocity.Length()
                    * a.Velocity;
                if (b.Body.ContactList == null)
                {
                    //pair.ComponentB.Body.AngularVelocity = 0;
                }
            }
        }
    }
}