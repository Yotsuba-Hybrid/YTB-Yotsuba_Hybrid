using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YotsubaEngine.Graphics;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Component that stores sprite animations for movement.
    /// Componente que añade la funcionalidad de mostrar varios sprites para representar movimiento
    /// </summary>
    public struct AnimationComponent2D
    {
        /// <summary>
        /// Dictionary storing all animations for the component.
        /// Diccionario donde se almacenan todas las animaciones del componente
        /// </summary>
        private readonly Dictionary<AnimationType, Animation> Animations = new Dictionary<AnimationType, Animation>();

        /// <summary>
        /// Stores the currently active animation.
        /// Se actualiza con el cambio de animacion, aqui se guarda la animacion que esta corriendo actualmente;
        /// </summary>
        public ValueTuple<AnimationType, Animation> CurrentAnimationType { get; set; }

        /// <summary>
        /// Creates the component with the provided animations.
        /// Constructor principal, recibe un arreglo de animaciones.
        /// </summary>
        /// <param name="tuples">Animation pairs to register. Pares de animación a registrar.</param>
        public AnimationComponent2D(params Tuple<AnimationType, Animation>[] tuples)
        {
            foreach(var tup in tuples)
                Animations.Add(tup.Item1, tup.Item2);
        }

        /// <summary>
        /// Adds or replaces an animation entry.
        /// Metodo para agregar una nueva animacion.
        /// </summary>
        /// <param name="animationType">Animation type key. Tipo de animación.</param>
        /// <param name="animation">Animation instance. Instancia de animación.</param>
        public void AddAnimation(AnimationType animationType, Animation animation)
        {
            // Inicializa solo si está nulo (lazy initialization)
            if (Animations is null)
            {
                Unsafe.AsRef(in Animations) = new Dictionary<AnimationType, Animation>();
            }

            if (!Animations.ContainsKey(animationType))
                Animations.Add(animationType, animation);
            else
                Animations[animationType] = animation;

		}

        /// <summary>
        /// Removes an animation entry.
        /// Metodo para eliminar una animacion.
        /// </summary>
        /// <param name="animationType">Animation type key. Tipo de animación.</param>
        public void RemoveAnimation(AnimationType animationType)
        {
            Animations.Remove(animationType);
        }

        /// <summary>
        /// Retrieves an animation by type.
        /// Metodo para obtener una animacion facilmente.
        /// </summary>
        /// <param name="animationType">Animation type key. Tipo de animación.</param>
        /// <returns>The requested animation. La animación solicitada.</returns>
        public readonly Animation GetAnimation(AnimationType animationType)
        {
            return Animations[animationType];
        }

        /// <summary>
        /// Checks whether an animation exists for the given type.
        /// Comprueba si existe una animación para el tipo indicado.
        /// </summary>
        public bool ContainsAnimation(AnimationType type) => Animations.ContainsKey(type);

        /// <summary>
        /// Activates the requested animation.
        /// Metodo para activar una animacion
        /// </summary>
        /// <param name="type">Animation type to activate. Tipo de animación a activar.</param>
        public void ActivateAnimation(AnimationType type)
        {
            CurrentAnimationType = (AnimationType.walk, GetAnimation(type));
        }
    }

    /// <summary>
    /// Defines the available animation types.
    /// Define los tipos de animación disponibles.
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// No animation.
        /// Sin animación.
        /// </summary>
        none,
        /// <summary>
        /// Idle animation.
        /// Animación de reposo.
        /// </summary>
        idle,
        /// <summary>
        /// Walking animation.
        /// Animación de caminar.
        /// </summary>
        walk,
        /// <summary>
        /// Running animation.
        /// Animación de correr.
        /// </summary>
        run,
        /// <summary>
        /// Jumping animation.
        /// Animación de salto.
        /// </summary>
        jump,
        /// <summary>
        /// Crouching animation.
        /// Animación de agacharse.
        /// </summary>
        crouch,
        /// <summary>
        /// Attack animation.
        /// Animación de ataque.
        /// </summary>
        attack,
        /// <summary>
        /// Hurt animation.
        /// Animación de daño.
        /// </summary>
        hurt,
        /// <summary>
        /// Death animation.
        /// Animación de muerte.
        /// </summary>
        die
    }
}
