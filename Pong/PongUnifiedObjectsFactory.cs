using Microsoft.Xna.Framework;
using Undine.Core;
using Undine.MonoGame;
using Undine.MonoGame.Primitives2D;
using Undine.VelcroPhysics.MonoGame;

namespace Pong
{
    public class PongUnifiedObjectsFactory
    {
        private VelcroPhysics.Dynamics.World _world;
        private EcsContainer _ecsContainer;

        public PongUnifiedObjectsFactory(VelcroPhysics.Dynamics.World world,
            EcsContainer ecsContainer)
        {
            _world = world;
            _ecsContainer = ecsContainer;
        }

        public IUnifiedEntity GetWall(
            Vector2 position,
            Vector2 size,
            Color color)
        {
            var result = _ecsContainer.CreateNewEntity();
            result.AddComponent(new TransformComponent() { Position = position });
            var primitives2DComponent = new Primitives2DComponent()
            {
                Color = color,
                DrawType = Primitives2DDrawType.FillRectangle,
                Size = size
            };
            result.AddComponent(primitives2DComponent);
            return result;
        }
        public (IUnifiedEntity, VelcroPhysicsComponent) GetBall(
            Vector2 position,
            Vector2 size,
            Color color)
        {
            var result = _ecsContainer.CreateNewEntity();
            var transformComponent = new TransformComponent() { Position = position };
            result.AddComponent(transformComponent);
            var primitives2DComponent = new Primitives2DComponent()
            {
                Color = color,
                DrawType = Primitives2DDrawType.DrawCircle,
                Sides = 64,
                Thickness = size.X,
                Size = size
            };
            result.AddComponent(primitives2DComponent);
            var velcroPhysicsComponent = new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateCircle(
                    _world,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(size.X),
                    //VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(size.Y),
                    0.1f,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(position),
                    VelcroPhysics.Dynamics.BodyType.Dynamic)
            };
            result.AddComponent(velcroPhysicsComponent);
            float velocity = 32;
            velcroPhysicsComponent.Body.LinearVelocity = new Vector2(-velocity, 0);
            velcroPhysicsComponent.Body.FixedRotation = true;
            result.AddComponent(new VelocityCorrectionComponent() { Velocity = velocity });
            velcroPhysicsComponent.Body.Restitution = 1;
            //velcroPhysicsComponent.Body.Awake = false;
            //velcroPhysicsComponent.Body.Enabled = false;

            return (result, velcroPhysicsComponent);
        }

        public IUnifiedEntity GetPaddle(
            Vector2 position,
            Vector2 size,
            Color color)
        {
            var result = _ecsContainer.CreateNewEntity();

            result.AddComponent(new TransformComponent() { Position = position });
            var primitives2DComponent = new Primitives2DComponent()
            {
                Color = color,
                DrawType = Primitives2DDrawType.FillRectangle,
                Size = size
            };
            result.AddComponent(primitives2DComponent);

            var velcroPhysicsComponent = new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    _world,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(size.X),
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(size.Y),
                    1111f,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(position),
                    0,
                    VelcroPhysics.Dynamics.BodyType.Dynamic)
            };
            velcroPhysicsComponent.Body.FixedRotation = true;
            velcroPhysicsComponent.Body.Restitution = 1f;
            result.AddComponent(velcroPhysicsComponent);
            return result;
        }
    }
}