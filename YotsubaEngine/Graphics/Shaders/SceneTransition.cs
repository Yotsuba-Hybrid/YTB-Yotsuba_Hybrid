using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace YotsubaEngine.Graphics.Shaders
{
    /// <summary>
    /// Sistema de transiciones entre escenas usando shaders.
    /// Compatible con AOT y multiplataforma.
    /// </summary>
    public class SceneTransition
    {
        private Effect _transitionEffect;
        private float _progress;
        private bool _isTransitioning;
        private float _duration;
        private float _elapsed;
        private Action _onTransitionComplete;
        private TransitionType _type;

        /// <summary>
        /// El progreso actual de la transición (0.0 a 1.0).
        /// </summary>
        public float Progress => _progress;

        /// <summary>
        /// Indica si hay una transición activa.
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        public SceneTransition()
        {
            _progress = 0f;
            _isTransitioning = false;
        }

        /// <summary>
        /// Inicia una transición de escena.
        /// Si ya hay una transición en curso, se completará inmediatamente antes de iniciar la nueva.
        /// </summary>
        /// <param name="type">Tipo de transición (Fade o Dissolve)</param>
        /// <param name="duration">Duración en segundos</param>
        /// <param name="onComplete">Callback al completar la transición</param>
        public void StartTransition(TransitionType type, float duration, Action onComplete = null)
        {
            if (_isTransitioning)
            {
                // Si ya hay una transición, completarla inmediatamente
                // Esto asegura que el callback anterior se ejecute antes de la nueva transición
                CompleteTransition();
            }

            _type = type;
            _duration = duration;
            _elapsed = 0f;
            _progress = 0f;
            _isTransitioning = true;
            _onTransitionComplete = onComplete;

            // Cargar el shader de transición
            _transitionEffect = ShaderManager.LoadShader("Shaders/Transition");
        }

        /// <summary>
        /// Actualiza la transición.
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Update(GameTime gameTime)
        {
            if (!_isTransitioning) return;

            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _progress = MathHelper.Clamp(_elapsed / _duration, 0f, 1f);

            if (_progress >= 1f)
            {
                CompleteTransition();
            }
        }

        /// <summary>
        /// Aplica la transición al SpriteBatch.
        /// Llamar esto al dibujar la escena.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch para renderizar</param>
        /// <returns>El efecto a usar, o null si no hay transición</returns>
        public Effect GetTransitionEffect()
        {
            if (!_isTransitioning || _transitionEffect == null)
                return null;

            // Configurar parámetros del shader
            _transitionEffect.Parameters["Progress"]?.SetValue(_progress);
            _transitionEffect.Parameters["TransitionType"]?.SetValue((float)_type);

            return _transitionEffect;
        }

        /// <summary>
        /// Completa la transición inmediatamente.
        /// </summary>
        private void CompleteTransition()
        {
            _isTransitioning = false;
            _progress = 1f;
            _onTransitionComplete?.Invoke();
            _onTransitionComplete = null;
        }

        /// <summary>
        /// Cancela la transición actual.
        /// </summary>
        public void CancelTransition()
        {
            _isTransitioning = false;
            _progress = 0f;
            _onTransitionComplete = null;
        }
    }

    /// <summary>
    /// Tipos de transición disponibles.
    /// </summary>
    public enum TransitionType
    {
        /// <summary>
        /// Transición de fundido simple (fade in/out).
        /// </summary>
        Fade = 0,

        /// <summary>
        /// Transición de disolución usando textura de ruido.
        /// </summary>
        Dissolve = 1
    }
}
