using Microsoft.Xna.Framework;
using System.Linq;
using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;

namespace Pong
{
    public class PaddleControlSystem : UnifiedSystem<KeyboardComponent, VelcroPhysicsComponent>
    {
        private Vector2 _up = new Vector2(0, -1);
        private Vector2 _down = new Vector2(0, 1);
        private Vector2 _stop = new Vector2(0, 0);

        public PaddleControlSystem(float speed)
        {
            _up *= speed;
            _down *= speed;
        }
        public override void ProcessSingleEntity(int entityId, ref KeyboardComponent a, ref VelcroPhysicsComponent b)
        {
            if (a.Bindings.Any(x => x.IsDown && x.KeyAction == "up"))
            {
                b.Body.LinearVelocity = _up;
            }
            else if (a.Bindings.Any(x => x.IsDown && x.KeyAction == "down"))
            {
                b.Body.LinearVelocity = _down;
            }
            else
            {
                b.Body.LinearVelocity = _stop;
            }
        }
    }
}