using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Undine.Core;
using Undine.MonoGame;

namespace Pong
{
    public interface ICamera2D
    {
        /// <summary>
        /// Gets or sets the position of the camera
        /// </summary>
        /// <value>The position.</value>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the move speed of the camera.
        /// The camera will tween to its destination.
        /// </summary>
        /// <value>The move speed.</value>
        //float MoveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        /// <value>The rotation.</value>
        float Rotation { get; set; }

        /// <summary>
        /// Gets the origin of the viewport (accounts for Scale)
        /// </summary>
        /// <value>The origin.</value>
        Vector2 Origin { get; }

        /// <summary>
        /// Gets or sets the scale of the Camera
        /// </summary>
        /// <value>The scale.</value>
        float Scale { get; set; }

        /// <summary>
        /// Gets the screen center (does not account for Scale)
        /// </summary>
        /// <value>The screen center.</value>
        Vector2 ScreenCenter { get; }

        /// <summary>
        /// Gets the transform that can be applied to
        /// the SpriteBatch Class.
        /// </summary>
        /// <see cref="SpriteBatch"/>
        /// <value>The transform.</value>
        Matrix Transform { get; }

        /// <summary>
        /// Gets or sets the focus of the Camera.
        /// </summary>
        /// <seealso cref="IFocusable"/>
        /// <value>The focus.</value>
        //Entity Focus { get; set; }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
        /// </returns>
        bool IsInView(Vector2 position, Texture2D texture);
    }

    public class Camera2D : UnifiedSystem<CameraFocusComponent, TransformComponent>, ICamera2D
    {
        private Vector2 _position;
        public Vector2 ScreenCenter { get; protected set; }
        public float ViewportHeight { get; set; }
        public float ViewportWidth { get; set; }
        public IUnifiedEntity Focus { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Rotation { get; set; }

        public Vector2 Origin { get; }

        public float Scale { get; set; }

        public Matrix Transform { get; }

        public void Initialize(Game game)
        {
            ViewportWidth = game.GraphicsDevice.Viewport.Width;
            ViewportHeight = game.GraphicsDevice.Viewport.Height;

            ScreenCenter = new Vector2(ViewportWidth / 2, ViewportHeight / 2);
            Scale = 1;
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                //Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetViewMatrix(float parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                //Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) * // ukrylem dla prostokatow Primitives2D
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                    // The next line has a catch. See note below.
                    Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
            }
        }

        public Vector2 ToWorldSpace(Point point)
        {
            var result = Vector2.Transform(point.ToVector2(), Matrix.Invert(TransformMatrix));
            return result;
        }
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen

            if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
                return false;

            // In View
            return true;
        }

        public override void ProcessSingleEntity(int entityId, ref CameraFocusComponent a, ref TransformComponent b)
        {
        }
    }
}