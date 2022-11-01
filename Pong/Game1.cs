using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using Undine.Core;
using Undine.DefaultEcs;

using Undine.MonoGame;
using Undine.MonoGame.Primitives2D;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Utilities;

//using Undine.LeopotamEcs;
//using Undine.MinEcs;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; }
        private SpriteBatch _spriteBatch;
        private readonly int _preferredBackBufferWidth;
        private readonly int _preferredBackBufferHeight;
        private Matrix _scale;
        private EcsContainer _ecsContainer;
        private List<SoundEffect> _soundEffects;
        private Camera2D _camera;
        private ISystem _primitives2DSystem;

        private VelcroPhysicsSystem _velcroPhysicsSystem;

        private VelcroPhysicsComponent _ballPhysics;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this)
            {
                //PreferredBackBufferWidth = 1280,
                //PreferredBackBufferHeight = 720
            };
            _preferredBackBufferHeight = Graphics.PreferredBackBufferHeight;
            _preferredBackBufferWidth = Graphics.PreferredBackBufferWidth;
            Content.RootDirectory = "Content";
            SetScore();
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            _scale = Matrix.CreateScale((float)Graphics.GraphicsDevice.Viewport.Width / (float)_preferredBackBufferWidth,
                (float)Graphics.GraphicsDevice.Viewport.Height / (float)_preferredBackBufferHeight, 1f);
        }

        protected override void Initialize()
        {
            float meterInPixels = 16;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(meterInPixels);
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
            base.Initialize();
            Window_ClientSizeChanged(null, null);
            var screenCenter = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2f, Graphics.GraphicsDevice.Viewport.Height / 2f);

            _ecsContainer = new DefaultEcsContainer();
            //_ecsContainer = new MinEcsContainer();
            //_ecsContainer = new LeopotamEcsContainer();
            _ecsContainer.AddSystem(new KeyboardSystem());
            _ecsContainer.AddSystem(new PaddleControlSystem(16));
            _velcroPhysicsSystem = new VelcroPhysicsSystem();
            _ecsContainer.AddSystem(_velcroPhysicsSystem);
            _ecsContainer.AddSystem(new BoundsSystem(8, Graphics.GraphicsDevice.Viewport.Height - 8));
            _ecsContainer.AddSystem(new VelocityCorrectionSystem());
            _ecsContainer.AddSystem(new VelcroPhysicsTransformSystem());
            _camera = new Camera2D();
            _camera.Initialize(this);
            var vpbcs = new VelcroPhysicsBodyContactSoundSystem();
            vpbcs.Camera = _camera;
            _ecsContainer.AddSystem(vpbcs);
            _primitives2DSystem = _ecsContainer.GetSystem(new Primitives2DSystem(_spriteBatch));

            _ecsContainer.Init();

            var physicsEntity = _ecsContainer.CreateNewEntity();
            var physicsWorld = new VelcroPhysics.Dynamics.World(Vector2.Zero);
            physicsEntity.AddComponent(new VelcroWorldComponent()
            {
                World = physicsWorld
            });

            var factory = new PongUnifiedObjectsFactory(physicsWorld, _ecsContainer);

            var paddleSize = new Vector2(8, 64);
            var leftPlayer = factory.GetPaddle(new Vector2(32, Graphics.GraphicsDevice.Viewport.Height / 2f),
                paddleSize,
                Color.Green);
            leftPlayer.AddComponent(new SoundEffectComponent() { SoundEffect = _soundEffects.Find(x => x.Name == "Ping") });
            var lpkb = new KeyboardComponent
            {
                Bindings = new List<KeyBinding>()
                {
                    new KeyBinding() {Key = Keys.W, KeyAction = "up"},
                    new KeyBinding() {Key = Keys.S, KeyAction = "down"}
                }
            };
            leftPlayer.AddComponent(lpkb);

            var rightPlayer = factory.GetPaddle(new Vector2(Graphics.GraphicsDevice.Viewport.Width - 32, Graphics.GraphicsDevice.Viewport.Height / 2f),
                paddleSize,
                Color.Orange);
            rightPlayer.AddComponent(new SoundEffectComponent() { SoundEffect = _soundEffects.Find(x => x.Name == "Pong") });
            var rpkb = new KeyboardComponent
            {
                Bindings = new List<KeyBinding>()
                {
                    new KeyBinding() {Key = Keys.O, KeyAction = "up"},
                    new KeyBinding() {Key = Keys.L, KeyAction = "down"}
                }
            };
            rightPlayer.AddComponent(rpkb);

            factory.GetWall(new Vector2(0, Graphics.GraphicsDevice.Viewport.Height / 2f),
                new Vector2(16, Graphics.GraphicsDevice.Viewport.Height),
                 Color.White);
            factory.GetWall(new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2f, 0),
                new Vector2(Graphics.GraphicsDevice.Viewport.Width, 16),
                 Color.White);
            factory.GetWall(new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2f, Graphics.GraphicsDevice.Viewport.Height),
                new Vector2(Graphics.GraphicsDevice.Viewport.Width, 16),
                 Color.White);
            factory.GetWall(new Vector2(Graphics.GraphicsDevice.Viewport.Width, Graphics.GraphicsDevice.Viewport.Height / 2f),
                new Vector2(16, Graphics.GraphicsDevice.Viewport.Height),
                 Color.White);
            var ball = factory.GetBall(new Vector2(Graphics.GraphicsDevice.Viewport.Width, Graphics.GraphicsDevice.Viewport.Height) / 2,
                new Vector2(16), Color.White);
            _ballPhysics = ball.Item2;
            _camera.Focus = ball.Item1;

            this.Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _soundEffects = new List<SoundEffect>();
            var pingSound = Content.Load<SoundEffect>("Ping");
            _soundEffects.Add(pingSound);
            var pongSound = Content.Load<SoundEffect>("Pong");
            _soundEffects.Add(pongSound);
            _font = Content.Load<SpriteFont>("qc_console");
        }

        private bool enabled;

        protected override void Update(GameTime gameTime)
        {
            _velcroPhysicsSystem.ElapsedGameTimeTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Any())
                enabled = true;
            var reset = false;
            if (_ballPhysics.Body.Position.X >
                VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(
                    Graphics.GraphicsDevice.Viewport.Width))
            {
                scoreRight += 1;
                reset = true;
            }
            if (_ballPhysics.Body.Position.X < 0)
            {
                scoreLeft += 1;
                reset = true;
            }

            var transform = _camera.Focus.GetComponent<TransformComponent>();
            if (reset)
            {
                _ballPhysics.Body.SetTransform(
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(
                        new Vector2(Graphics.GraphicsDevice.Viewport.Width,
                            Graphics.GraphicsDevice.Viewport.Height) / 2
                    ), 0);
                _ballPhysics.Body.LinearVelocity = new Vector2(_ballPhysics.Body.LinearVelocity.X, 0);

                enabled = false;
                SetScore();
                _ecsContainer.Run();
            }
            if (enabled)
                _ecsContainer.Run();
            base.Update(gameTime);
        }

        private SpriteFont _font;
        private static int scoreLeft = 0;
        private static int scoreRight = 0;
        private static string score;

        private static void SetScore()
        {
            score = string.Format(format: "{0}:{1}", arg0: scoreLeft, arg1: scoreRight);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _scale);

            _primitives2DSystem.ProcessAll();

            if (!enabled)
            {
                var scale = 1920f / Graphics.PreferredBackBufferWidth;
                var size = _font.MeasureString(score);
                _spriteBatch.DrawString(_font, score
                    , new Vector2(Graphics.PreferredBackBufferWidth / 2f, size.Y * scale), Color.White, 0, size / 2f, scale, SpriteEffects.None, 1);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}