using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace YotsubaEngine.Graphics.Shaders
{
    /// <summary>
    /// Sistema de transiciones entre escenas usando shaders, compatible con AOT y multiplataforma.
    /// <para>Scene transition system using shaders, AOT and cross-platform compatible.</para>
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
        /// <para>Current transition progress (0.0 to 1.0).</para>
        /// </summary>
        public float Progress => _progress;

        /// <summary>
        /// Indica si hay una transición activa.
        /// <para>Indicates whether a transition is active.</para>
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        /// <summary>
        /// Crea una nueva transición de escena.
        /// <para>Creates a new scene transition.</para>
        /// </summary>
        public SceneTransition()
        {
            _progress = 0f;
            _isTransitioning = false;
        }

        /// <summary>
        /// Inicia una transición de escena y completa la anterior si está activa.
        /// <para>Starts a scene transition and completes the previous one if active.</para>
        /// </summary>
        /// <param name="type">Tipo de transición (Fade o Dissolve). <para>Transition type (Fade or Dissolve).</para></param>
        /// <param name="duration">Duración en segundos. <para>Duration in seconds.</para></param>
        /// <param name="onComplete">Callback al completar la transición. <para>Callback invoked when the transition completes.</para></param>
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
        /// <para>Updates the transition.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
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
        /// Aplica la transición al SpriteBatch al dibujar la escena.
        /// <para>Applies the transition to the SpriteBatch when drawing the scene.</para>
        /// </summary>
        /// <returns>Efecto a usar, o null si no hay transición. <para>Effect to use, or null if no transition.</para></returns>
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
        /// <para>Cancels the current transition.</para>
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
    /// <para>Available transition types.</para>
    /// </summary>
    public enum TransitionType
    {
        /// <summary>
        /// Transición de fundido simple (fade in/out).
        /// <para>Simple fade in/out transition.</para>
        /// </summary>
        Fade = 0,

        /// <summary>
        /// Transición de disolución usando textura de ruido.
        /// <para>Dissolve transition using a noise texture.</para>
        /// </summary>
        Dissolve = 1
    }
}
