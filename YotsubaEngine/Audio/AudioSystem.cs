using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Audio
{
    /// <summary>
    /// Main audio system for managing sound effects and music playback.
    /// Sistema principal de audio para gestionar efectos de sonido y reproducción de música.
    /// </summary>
    public static class AudioSystem
    {
        private static readonly List<SoundEffectInstance> _activeSoundInstances = new List<SoundEffectInstance>();
        private static readonly Dictionary<string, SoundEffect> _cachedSoundEffects = new Dictionary<string, SoundEffect>();
        private static readonly Dictionary<string, Song> _cachedSongs = new Dictionary<string, Song>();

        private static float _masterVolume = 1.0f;
        private static float _musicVolume = 1.0f;
        private static float _sfxVolume = 1.0f;
        private static bool _isMuted = false;

        private static IAudioRegistry _audioRegistry;

        #region Properties

        /// <summary>
        /// Master volume for all audio (0.0 to 1.0).
        /// Volumen maestro para todo el audio (0.0 a 1.0).
        /// </summary>
        public static float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = Math.Clamp(value, 0.0f, 1.0f);
                UpdateMusicVolume();
            }
        }

        /// <summary>
        /// Volume for music playback (0.0 to 1.0).
        /// Volumen para reproducción de música (0.0 a 1.0).
        /// </summary>
        public static float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Clamp(value, 0.0f, 1.0f);
                UpdateMusicVolume();
            }
        }

        /// <summary>
        /// Volume for sound effects (0.0 to 1.0).
        /// Volumen para efectos de sonido (0.0 a 1.0).
        /// </summary>
        public static float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Math.Clamp(value, 0.0f, 1.0f);
        }

        /// <summary>
        /// Indicates if all audio is muted.
        /// Indica si todo el audio está silenciado.
        /// </summary>
        public static bool IsMuted
        {
            get => _isMuted;
            set
            {
                _isMuted = value;
                MediaPlayer.IsMuted = value;
            }
        }

        /// <summary>
        /// Indicates if music is currently playing.
        /// Indica si la música está reproduciéndose actualmente.
        /// </summary>
        public static bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        /// <summary>
        /// Indicates if music is paused.
        /// Indica si la música está pausada.
        /// </summary>
        public static bool IsMusicPaused => MediaPlayer.State == MediaState.Paused;

        /// <summary>
        /// Gets or sets whether music should loop.
        /// Obtiene o establece si la música debe repetirse.
        /// </summary>
        public static bool IsRepeating
        {
            get => MediaPlayer.IsRepeating;
            set => MediaPlayer.IsRepeating = value;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Sets the audio registry to use for loading audio assets.
        /// Establece el registro de audio a usar para cargar activos de audio.
        /// </summary>
        public static void SetAudioRegistry(IAudioRegistry registry)
        {
            _audioRegistry = registry;
        }

        /// <summary>
        /// Initializes the audio system.
        /// Inicializa el sistema de audio.
        /// </summary>
        public static void Initialize()
        {
            AudioAssets.Initialize();
            MediaPlayer.Volume = _masterVolume * _musicVolume;
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Plays a sound effect by name with default settings.
        /// Reproduce un efecto de sonido por nombre con configuración predeterminada.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <returns>True if the sound was played successfully.</returns>
        public static bool PlaySound(string name)
        {
            return PlaySound(name, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays a sound effect by name with specified volume.
        /// Reproduce un efecto de sonido por nombre con volumen especificado.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <returns>True if the sound was played successfully.</returns>
        public static bool PlaySound(string name, float volume)
        {
            return PlaySound(name, volume, 0.0f, 0.0f);
        }

        /// <summary>
        /// Plays a sound effect by name with full control.
        /// Reproduce un efecto de sonido por nombre con control total.
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">Pan position (-1.0 left to 1.0 right).</param>
        /// <returns>True if the sound was played successfully.</returns>
        public static bool PlaySound(string name, float volume, float pitch, float pan)
        {
            if (_isMuted) return false;

            var soundEffect = GetSoundEffect(name);
            if (soundEffect == null) return false;

            float adjustedVolume = CalculateAdjustedSfxVolume(volume);
            soundEffect.Play(adjustedVolume, pitch, pan);
            return true;
        }

        /// <summary>
        /// Calculates the final adjusted volume for sound effects.
        /// Calcula el volumen final ajustado para efectos de sonido.
        /// </summary>
        /// <param name="baseVolume">Base volume (0.0 to 1.0).</param>
        /// <returns>Adjusted volume considering master and SFX volume settings.</returns>
        private static float CalculateAdjustedSfxVolume(float baseVolume)
        {
            return Math.Clamp(baseVolume * _sfxVolume * _masterVolume, 0.0f, 1.0f);
        }

        /// <summary>
        /// Creates a sound effect instance for advanced control (loop, pause, etc.).
        /// Crea una instancia de efecto de sonido para control avanzado (loop, pausa, etc.).
        /// </summary>
        /// <param name="name">The name of the sound effect asset.</param>
        /// <returns>A SoundEffectInstance or null if not found.</returns>
        public static SoundEffectInstance CreateSoundInstance(string name)
        {
            var soundEffect = GetSoundEffect(name);
            if (soundEffect == null) return null;

            var instance = soundEffect.CreateInstance();
            _activeSoundInstances.Add(instance);
            return instance;
        }

        /// <summary>
        /// Gets a SoundEffect by name (cached).
        /// Obtiene un SoundEffect por nombre (cacheado).
        /// </summary>
        private static SoundEffect GetSoundEffect(string name)
        {
            if (_cachedSoundEffects.TryGetValue(name, out var cached))
            {
                return cached;
            }

            // Try from registry
            if (_audioRegistry != null)
            {
                var soundEffect = _audioRegistry.GetSoundEffect(name);
                if (soundEffect != null)
                {
                    _cachedSoundEffects[name] = soundEffect;
                    return soundEffect;
                }
            }

            // Try from static registry
            if (IAudioRegistry.SoundEffects.TryGetValue(name, out var factory))
            {
                var soundEffect = factory();
                _cachedSoundEffects[name] = soundEffect;
                return soundEffect;
            }

            // Try to load directly from content
            if (YTBGlobalState.ContentManager != null)
            {
                try
                {
                    var soundEffect = YTBGlobalState.ContentManager.Load<SoundEffect>(name);
                    _cachedSoundEffects[name] = soundEffect;
                    return soundEffect;
                }
                catch
                {
                    // Asset not found
                }
            }

            Console.WriteLine($"[AudioSystem] SoundEffect not found: {name}");
            return null;
        }

        #endregion

        #region Music

        /// <summary>
        /// Plays a song by name.
        /// Reproduce una canción por nombre.
        /// </summary>
        /// <param name="name">The name of the song asset.</param>
        /// <param name="loop">Whether to loop the song.</param>
        /// <returns>True if the song started playing successfully.</returns>
        public static bool PlayMusic(string name, bool loop = true)
        {
            var song = GetSong(name);
            if (song == null) return false;

            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Volume = _masterVolume * _musicVolume;
            MediaPlayer.Play(song);
            return true;
        }

        /// <summary>
        /// Stops the currently playing music.
        /// Detiene la música que se está reproduciendo actualmente.
        /// </summary>
        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Pauses the currently playing music.
        /// Pausa la música que se está reproduciendo actualmente.
        /// </summary>
        public static void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes paused music.
        /// Reanuda la música pausada.
        /// </summary>
        public static void ResumeMusic()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Gets a Song by name (cached).
        /// Obtiene una canción por nombre (cacheada).
        /// </summary>
        private static Song GetSong(string name)
        {
            if (_cachedSongs.TryGetValue(name, out var cached))
            {
                return cached;
            }

            // Try from registry
            if (_audioRegistry != null)
            {
                var song = _audioRegistry.GetSong(name);
                if (song != null)
                {
                    _cachedSongs[name] = song;
                    return song;
                }
            }

            // Try from static registry
            if (IAudioRegistry.Songs.TryGetValue(name, out var factory))
            {
                var song = factory();
                _cachedSongs[name] = song;
                return song;
            }

            // Try to load directly from content
            if (YTBGlobalState.ContentManager != null)
            {
                try
                {
                    var song = YTBGlobalState.ContentManager.Load<Song>(name);
                    _cachedSongs[name] = song;
                    return song;
                }
                catch
                {
                    // Asset not found
                }
            }

            Console.WriteLine($"[AudioSystem] Song not found: {name}");
            return null;
        }

        #endregion

        #region Global Control

        /// <summary>
        /// Pauses all active sound instances and music.
        /// Pausa todas las instancias de sonido activas y la música.
        /// </summary>
        public static void PauseAll()
        {
            foreach (var instance in _activeSoundInstances)
            {
                if (instance.State == SoundState.Playing)
                {
                    instance.Pause();
                }
            }
            if (IsMusicPlaying)
            {
                PauseMusic();
            }
        }

        /// <summary>
        /// Resumes all paused sound instances and music.
        /// Reanuda todas las instancias de sonido pausadas y la música.
        /// </summary>
        public static void ResumeAll()
        {
            foreach (var instance in _activeSoundInstances)
            {
                if (instance.State == SoundState.Paused)
                {
                    instance.Resume();
                }
            }
            if (IsMusicPaused)
            {
                ResumeMusic();
            }
        }

        /// <summary>
        /// Stops all active sound instances and music.
        /// Detiene todas las instancias de sonido activas y la música.
        /// </summary>
        public static void StopAll()
        {
            foreach (var instance in _activeSoundInstances)
            {
                instance.Stop();
            }
            StopMusic();
        }

        /// <summary>
        /// Cleans up stopped sound instances.
        /// Limpia las instancias de sonido detenidas.
        /// </summary>
        public static void CleanupStoppedInstances()
        {
            for (int i = _activeSoundInstances.Count - 1; i >= 0; i--)
            {
                if (_activeSoundInstances[i].State == SoundState.Stopped)
                {
                    _activeSoundInstances[i].Dispose();
                    _activeSoundInstances.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Disposes all cached audio resources.
        /// Libera todos los recursos de audio cacheados.
        /// </summary>
        public static void Dispose()
        {
            StopAll();

            foreach (var instance in _activeSoundInstances)
            {
                instance.Dispose();
            }
            _activeSoundInstances.Clear();

            foreach (var soundEffect in _cachedSoundEffects.Values)
            {
                soundEffect.Dispose();
            }
            _cachedSoundEffects.Clear();

            // Songs are managed by the ContentManager, don't dispose them directly
            _cachedSongs.Clear();
        }

        private static void UpdateMusicVolume()
        {
            MediaPlayer.Volume = _masterVolume * _musicVolume;
        }

        #endregion
    }
}
