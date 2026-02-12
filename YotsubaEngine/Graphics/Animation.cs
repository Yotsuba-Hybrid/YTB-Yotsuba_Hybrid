using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Administra frames de animación de sprites y tiempos.
    /// <para>Manages sprite animation frames and timing.</para>
    /// </summary>
    /// <param name="textureRegions">Regiones que componen la animación. <para>Regions that compose the animation.</para></param>
    /// <param name="delay">Retraso entre frames. <para>Delay between frames.</para></param>
    public class Animation(TextureRegion[] textureRegions, TimeSpan delay)
    {


        /// <summary>
        /// Regiones de textura que componen la animación.
        /// <para>Texture regions that compose the animation.</para>
        /// </summary>
        public TextureRegion[] Regions = textureRegions;

        /// <summary>
        /// Tiempo de retraso entre cada frame de la animación.
        /// <para>Delay between animation frames.</para>
        /// </summary>
        public TimeSpan Delay { get; set; } = delay;

        /// <summary>
        /// Tiempo transcurrido desde el último cambio de frame.
        /// <para>Time elapsed since the last frame change.</para>
        /// </summary>
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

		/// <summary>
		/// Indica si la animación se reinicia al llegar al final.
		/// <para>Indicates whether the animation loops.</para>
		/// </summary>
		public bool IsLooping { get; set; } = true;

		/// <summary>
		/// Indica si la animación ha terminado.
		/// <para>Indicates whether the animation has finished.</para>
		/// </summary>
		public bool IsFinished => !IsLooping && _currentFrame >= FrameCount - 1;

		/// <summary>
		/// Indica si la animación fue marcada como terminada.
		/// <para>Indicates whether the animation was marked as finished.</para>
		/// </summary>
		public bool FinishedWasMarked { get; set; } = false;

		/// <summary>
		/// Index of the current animation frame.
		/// Indice del frame actual de la animacion.
		/// </summary>
		private int _currentFrame { get; set; }

        /// <summary>
        /// Avanza el tiempo de animación y devuelve el frame actual.
        /// <para>Advances animation timing and returns the current frame.</para>
        /// </summary>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        /// <returns>Frame actual. <para>Current frame.</para></returns>
        public TextureRegion CurrentFrame(GameTime time)
        {
            ElapsedTime += time.ElapsedGameTime;

            if (ElapsedTime >= Delay)
            {
                if (_currentFrame >= FrameCount - 1)
                {
                    if (IsLooping)
                    {
                        _currentFrame = 0;
                    }
                }
                else
                {
                    _currentFrame++;
                    ElapsedTime = TimeSpan.Zero;
                }
            }
            return Regions[_currentFrame];
        }

        /// <summary>
        /// Cantidad total de frames en la animación.
        /// <para>Total number of frames in the animation.</para>
        /// </summary>
        public int FrameCount => Regions.Length;

        /// <summary>
        /// Constructor secundario que inicializa una animación con un solo frame y un retraso específico.
        /// <para>Initializes an animation with a single frame and default delay.</para>
        /// </summary>
        public Animation() : this([], TimeSpan.FromMilliseconds(100)) { }

        /// <summary>
        /// Reinicia la animación al primer frame.
        /// <para>Resets the animation to the first frame.</para>
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            ElapsedTime = TimeSpan.Zero;
            FinishedWasMarked = false;
		}
	}
}
