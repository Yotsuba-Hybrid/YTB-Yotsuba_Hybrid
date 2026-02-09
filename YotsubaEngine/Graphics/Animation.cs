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
    /// Manages sprite animation frames and timing.
    /// Administra frames de animación de sprites y tiempos.
    /// </summary>
    public class Animation(TextureRegion[] textureRegions, TimeSpan delay)
    {


        /// <summary>
        /// Texture regions that compose the animation.
        /// Regiones de textura que componen la animacion.
        /// </summary>
        public TextureRegion[] Regions = textureRegions;

        /// <summary>
        /// Delay between animation frames.
        /// Tiempo de retraso entre cada frame de la animacion.
        /// </summary>
        public TimeSpan Delay { get; set; } = delay;

        /// <summary>
        /// Time elapsed since the last frame change.
        /// Tiempo transcurrido desde el ultimo cambio de frame.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;

		/// <summary>
		/// Indicates whether the animation loops.
		/// Flag que indica si la animacion debe reiniciarse al llegar al final.
		/// </summary>
		public bool IsLooping { get; set; } = true;

		/// <summary>
		/// Indicates whether the animation has finished.
		/// Flag que indica si la animacion ha terminado.
		/// </summary>
		public bool IsFinished => !IsLooping && _currentFrame >= FrameCount - 1;

		/// <summary>
		/// Indicates whether the animation was marked as finished.
		/// Flag que indica si la animacion fue marcada como terminada.
		/// </summary>
		public bool FinishedWasMarked { get; set; } = false;

		/// <summary>
		/// Index of the current animation frame.
		/// Indice del frame actual de la animacion.
		/// </summary>
		private int _currentFrame { get; set; }

        /// <summary>
        /// Advances animation timing and returns the current frame.
        /// Avanza el tiempo de animación y devuelve el frame actual.
        /// </summary>
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
        /// Total number of frames in the animation.
        /// Cantidad total de frames en la animacion.
        /// </summary>
        public int FrameCount => Regions.Length;

        /// <summary>
        /// Initializes an animation with a single frame and default delay.
        /// Contructor secundario que inicializa una animacion con un solo frame y un retraso especifico.
        /// </summary>
        public Animation() : this([], TimeSpan.FromMilliseconds(100)) { }

        /// <summary>
        /// Resets the animation to the first frame.
        /// Reinicia la animación al primer frame.
        /// </summary>
        public void Reset()
        {
            _currentFrame = 0;
            ElapsedTime = TimeSpan.Zero;
            FinishedWasMarked = false;
		}
	}
}
