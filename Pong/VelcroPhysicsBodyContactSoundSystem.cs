using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;

namespace Pong
{
    public class VelcroPhysicsBodyContactSoundSystem : UnifiedSystem<SoundEffectComponent, VelcroPhysicsComponent>
    {
        public Camera2D Camera { get; set; }
        public override void ProcessSingleEntity(int entityId, ref SoundEffectComponent a, ref VelcroPhysicsComponent b)
        {
            if (b.Body.ContactList != null)
            {
                var pan = (b.Body.Position.X - Camera.ScreenCenter.X) / Camera.ViewportWidth * 2.0f;
                a.SoundEffect.Play(0.01f, 0, pan);
            }
            //pair.Value.ComponentA.SoundEffect.Play();
            //var instance = pair.Value.ComponentA.SoundEffect.CreateInstance();
            //instance.Pan = pan;
            //instance.Play();
        }
    }
}