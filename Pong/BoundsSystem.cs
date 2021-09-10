using Microsoft.Xna.Framework;
using Undine.Core;
using Undine.MonoGame.Primitives2D;
using Undine.VelcroPhysics.MonoGame;

namespace Pong
{
    public class BoundsSystem : UnifiedSystem<VelcroPhysicsComponent, Primitives2DComponent>
    {
        private float _upperBound;
        private float _bottomBound;
        private float _halfHeight;

        public BoundsSystem(float upperBound, float bottomBound)
        {
            _upperBound = VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(upperBound);
            _bottomBound = VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(bottomBound);
        }

        public override void ProcessSingleEntity(int entityId, ref VelcroPhysicsComponent a, ref Primitives2DComponent b)
        {
            _halfHeight = VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(b.Size.Y / 2f);
            if (a.Body.Position.Y - _halfHeight < _upperBound)
            {
                a.Body.Position = new Vector2(a.Body.Position.X, _upperBound + _halfHeight);
                a.Body.LinearVelocity = new Vector2(a.Body.LinearVelocity.X, -a.Body.LinearVelocity.Y);
            }
            else if (a.Body.Position.Y + _halfHeight > _bottomBound)
            {
                a.Body.Position = new Vector2(a.Body.Position.X, _bottomBound - _halfHeight);
                a.Body.LinearVelocity = new Vector2(a.Body.LinearVelocity.X, -a.Body.LinearVelocity.Y);
            }
        }
    }
}