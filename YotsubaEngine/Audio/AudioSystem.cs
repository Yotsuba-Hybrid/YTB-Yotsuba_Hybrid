using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.Audio
{
    /// <summary>
    /// Sistema principal de audio para gestionar efectos de sonido y reproducción de música.
    /// <para>Main audio system for managing sound effects and music playback.</para>
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
        /// Volumen maestro para todo el audio (0.0 a 1.0).
        /// <para>Master volume for all audio (0.0 to 1.0).</para>
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
        /// Volumen para reproducción de música (0.0 a 1.0).
        /// <para>Volume for music playback (0.0 to 1.0).</para>
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
        /// Volumen para efectos de sonido (0.0 a 1.0).
        /// <para>Volume for sound effects (0.0 to 1.0).</para>
        /// </summary>
        public static float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Math.Clamp(value, 0.0f, 1.0f);
        }

        /// <summary>
        /// Indica si todo el audio está silenciado.
        /// <para>Indicates if all audio is muted.</para>
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
        /// Indica si la música está reproduciéndose actualmente.
        /// <para>Indicates if music is currently playing.</para>
        /// </summary>
        public static bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        /// <summary>
        /// Indica si la música está pausada.
        /// <para>Indicates if music is paused.</para>
        /// </summary>
        public static bool IsMusicPaused => MediaPlayer.State == MediaState.Paused;

        /// <summary>
        /// Obtiene o establece si la música debe repetirse.
        /// <para>Gets or sets whether music should loop.</para>
        /// </summary>
        public static bool IsRepeating
        {
            get => MediaPlayer.IsRepeating;
            set => MediaPlayer.IsRepeating = value;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Establece el registro de audio a usar para cargar activos de audio.
        /// <para>Sets the audio registry to use for loading audio assets.</para>
        /// </summary>
        /// <param name="registry">Registro de audio a usar. <para>Audio registry to use.</para></param>
        public static void SetAudioRegistry(IAudioRegistry registry)
        {
            _audioRegistry = registry;
        }

        /// <summary>
        /// Inicializa el sistema de audio.
        /// <para>Initializes the audio system.</para>
        /// </summary>
        public static void Initialize()
        {
            AudioAssets.Initialize();
            MediaPlayer.Volume = _masterVolume * _musicVolume;
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con configuración predeterminada.
        /// <para>Plays a sound effect by name with default settings.</para>
        /// </summary>
        /// <param name="name">Nombre del efecto de sonido. <para>Sound effect name.</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
        public static bool PlaySound(string name)
        {
            return PlaySound(name, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con volumen especificado.
        /// <para>Plays a sound effect by name with specified volume.</para>
        /// </summary>
        /// <param name="name">Nombre del efecto de sonido. <para>Sound effect name.</para></param>
        /// <param name="volume">Volumen (0.0 a 1.0). <para>Volume (0.0 to 1.0).</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
        public static bool PlaySound(string name, float volume)
        {
            return PlaySound(name, volume, 0.0f, 0.0f);
        }

        /// <summary>
        /// Reproduce un efecto de sonido por nombre con control total.
        /// <para>Plays a sound effect by name with full control.</para>
        /// </summary>
        /// <param name="name">Nombre del efecto de sonido. <para>Sound effect name.</para></param>
        /// <param name="volume">Volumen (0.0 a 1.0). <para>Volume (0.0 to 1.0).</para></param>
        /// <param name="pitch">Ajuste de tono (-1.0 a 1.0). <para>Pitch adjustment (-1.0 to 1.0).</para></param>
        /// <param name="pan">Posición de paneo (-1.0 izquierda a 1.0 derecha). <para>Pan position (-1.0 left to 1.0 right).</para></param>
        /// <returns>True si el sonido se reprodujo correctamente. <para>True if the sound was played successfully.</para></returns>
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
        /// Crea una instancia de efecto de sonido para control avanzado (loop, pausa, etc.).
        /// <para>Creates a sound effect instance for advanced control (loop, pause, etc.).</para>
        /// </summary>
        /// <param name="name">Nombre del efecto de sonido. <para>Sound effect name.</para></param>
        /// <returns>Instancia de SoundEffect o null si no se encuentra. <para>SoundEffect instance or null if not found.</para></returns>
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
        /// Reproduce una canción por nombre.
        /// <para>Plays a song by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la canción. <para>Song name.</para></param>
        /// <param name="loop">Indica si la canción debe repetirse. <para>Whether to loop the song.</para></param>
        /// <returns>True si la canción comenzó a reproducirse. <para>True if the song started playing successfully.</para></returns>
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
        /// Detiene la música que se está reproduciendo actualmente.
        /// <para>Stops the currently playing music.</para>
        /// </summary>
        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Pausa la música que se está reproduciendo actualmente.
        /// <para>Pauses the currently playing music.</para>
        /// </summary>
        public static void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Reanuda la música pausada.
        /// <para>Resumes paused music.</para>
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
        /// Pausa todas las instancias de sonido activas y la música.
        /// <para>Pauses all active sound instances and music.</para>
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
        /// Reanuda todas las instancias de sonido pausadas y la música.
        /// <para>Resumes all paused sound instances and music.</para>
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
        /// Detiene todas las instancias de sonido activas y la música.
        /// <para>Stops all active sound instances and music.</para>
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
        /// Limpia las instancias de sonido detenidas.
        /// <para>Cleans up stopped sound instances.</para>
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
        /// Libera todos los recursos de audio cacheados.
        /// <para>Disposes all cached audio resources.</para>
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
