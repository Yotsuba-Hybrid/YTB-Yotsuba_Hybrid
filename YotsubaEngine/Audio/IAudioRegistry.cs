using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace YotsubaEngine.Audio
{
    /// <summary>
    /// Registro base para activos de audio usados por el motor.
    /// <para>Base registry for audio assets used by the engine.</para>
    /// </summary>
    public abstract class IAudioRegistry
    {
        /// <summary>
        /// Registro global de fábricas de efectos de sonido indexado por nombre.
        /// <para>Global sound effect factory registry keyed by asset name.</para>
        /// </summary>
        public static readonly Dictionary<string, Func<SoundEffect>> SoundEffects = new Dictionary<string, Func<SoundEffect>>();

        /// <summary>
        /// Registro global de fábricas de canciones indexado por nombre.
        /// <para>Global song factory registry keyed by asset name.</para>
        /// </summary>
        public static readonly Dictionary<string, Func<Song>> Songs = new Dictionary<string, Func<Song>>();

        /// <summary>
        /// Obtiene un SoundEffect por nombre.
        /// <para>Gets a SoundEffect by name.</para>
        /// </summary>
        /// <param name="name">Nombre del efecto de sonido. <para>Sound effect name.</para></param>
        /// <returns>Efecto de sonido encontrado o null. <para>Sound effect found or null.</para></returns>
        public virtual SoundEffect GetSoundEffect(string name)
        {
            if (SoundEffects.TryGetValue(name, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Obtiene una canción por nombre.
        /// <para>Gets a song by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la canción. <para>Song name.</para></param>
        /// <returns>Canción encontrada o null. <para>Song found or null.</para></returns>
        public virtual Song GetSong(string name)
        {
            if (Songs.TryGetValue(name, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Devuelve todos los nombres de efectos de sonido registrados.
        /// <para>Returns all registered sound effect names.</para>
        /// </summary>
        /// <returns>Nombres registrados. <para>Registered names.</para></returns>
        public IEnumerable<string> GetSoundEffectNames() => SoundEffects.Keys;

        /// <summary>
        /// Devuelve todos los nombres de canciones registrados.
        /// <para>Returns all registered song names.</para>
        /// </summary>
        /// <returns>Nombres registrados. <para>Registered names.</para></returns>
        public IEnumerable<string> GetSongNames() => Songs.Keys;
    }
}
