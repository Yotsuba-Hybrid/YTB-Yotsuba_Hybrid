using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace YotsubaEngine.Audio
{
    /// <summary>
    /// Base registry for audio assets used by the engine.
    /// Registro base para activos de audio usados por el motor.
    /// </summary>
    public abstract class IAudioRegistry
    {
        /// <summary>
        /// Global sound effect factory registry keyed by asset name.
        /// Registro global de fábricas de efectos de sonido indexado por nombre.
        /// </summary>
        public static readonly Dictionary<string, Func<SoundEffect>> SoundEffects = new Dictionary<string, Func<SoundEffect>>();

        /// <summary>
        /// Global song factory registry keyed by asset name.
        /// Registro global de fábricas de canciones indexado por nombre.
        /// </summary>
        public static readonly Dictionary<string, Func<Song>> Songs = new Dictionary<string, Func<Song>>();

        /// <summary>
        /// Gets a SoundEffect by name.
        /// Obtiene un SoundEffect por nombre.
        /// </summary>
        public virtual SoundEffect GetSoundEffect(string name)
        {
            if (SoundEffects.TryGetValue(name, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Gets a Song by name.
        /// Obtiene una canción por nombre.
        /// </summary>
        public virtual Song GetSong(string name)
        {
            if (Songs.TryGetValue(name, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Returns all registered sound effect names.
        /// Devuelve todos los nombres de efectos de sonido registrados.
        /// </summary>
        public IEnumerable<string> GetSoundEffectNames() => SoundEffects.Keys;

        /// <summary>
        /// Returns all registered song names.
        /// Devuelve todos los nombres de canciones registrados.
        /// </summary>
        public IEnumerable<string> GetSongNames() => Songs.Keys;
    }
}
