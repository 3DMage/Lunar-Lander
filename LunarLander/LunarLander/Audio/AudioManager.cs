using Microsoft.Xna.Framework.Audio;
using LunarLander.Utilities;

namespace Audio
{
    // Manages audio assets and their state.
    public class AudioManager
    {
        // Music.
        public SoundEffect src_gameMusic;
        public SoundEffect src_menuMusic;

        public SoundEffectInstance gameMusic { get; private set; }
        public SoundEffectInstance menuMusic { get; private set; }

        // Sounds Effects
        public SoundEffect src_moveSelectBeep;
        public SoundEffect src_selectBeep;
        public SoundEffect src_rocketRumble;
        public SoundEffect src_explosion;
        public SoundEffect src_alert;
        public SoundEffect src_messageBeep;
        public SoundEffect src_success;

        public SoundEffectInstance rocketRumble { get; private set; }
        public SoundEffectInstance alert { get; private set; }

        // Configures sound and music assets.
        public void ConfigureAudio()
        {
            //Music setup
            gameMusic.IsLooped = true;
            gameMusic.Volume = 0.86f;

            menuMusic.IsLooped = true;

            // Sound setup
            rocketRumble.IsLooped = true;
            rocketRumble.Volume = 0.96f;

            alert.IsLooped = true;
        }

        // Load music and sound assets.
        public void LoadAudio()
        {
            // Music
            src_gameMusic = ContentManagerHandle.Content.Load<SoundEffect>("audio/gameMusic");
            src_menuMusic = ContentManagerHandle.Content.Load<SoundEffect>("audio/menuMusic");

            gameMusic = src_gameMusic.CreateInstance();
            menuMusic = src_menuMusic.CreateInstance();

            // Sound effects.
            src_moveSelectBeep = ContentManagerHandle.Content.Load<SoundEffect>("audio/moveSelection");
            src_selectBeep = ContentManagerHandle.Content.Load<SoundEffect>("audio/selection");
            src_rocketRumble = ContentManagerHandle.Content.Load<SoundEffect>("audio/rocket");
            src_explosion = ContentManagerHandle.Content.Load<SoundEffect>("audio/explosion");
            src_alert = ContentManagerHandle.Content.Load<SoundEffect>("audio/alarm");
            src_messageBeep = ContentManagerHandle.Content.Load<SoundEffect>("audio/messageBeep");
            src_success = ContentManagerHandle.Content.Load<SoundEffect>("audio/success");


            rocketRumble = src_rocketRumble.CreateInstance();
            alert = src_alert.CreateInstance();
        }
    }
}