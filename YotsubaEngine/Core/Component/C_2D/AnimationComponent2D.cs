using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YotsubaEngine.Graphics;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente que almacena animaciones de sprites para representar movimiento.
    /// <para>Component that stores sprite animations for movement.</para>
    /// </summary>
    public struct AnimationComponent2D
    {
        /// <summary>
        /// Diccionario donde se almacenan todas las animaciones del componente.
        /// <para>Dictionary storing all animations for the component.</para>
        /// </summary>
        private readonly Dictionary<AnimationType, Animation> Animations = new Dictionary<AnimationType, Animation>();

        /// <summary>
        /// Almacena la animación actualmente activa.
        /// <para>Stores the currently active animation.</para>
        /// </summary>
        public ValueTuple<AnimationType, Animation> CurrentAnimationType { get; set; }

        /// <summary>
        /// Crea el componente con las animaciones proporcionadas.
        /// <para>Creates the component with the provided animations.</para>
        /// </summary>
        /// <param name="tuples">Pares de animación a registrar.<para>Animation pairs to register.</para></param>
        public AnimationComponent2D(params Tuple<AnimationType, Animation>[] tuples)
        {
            foreach(var tup in tuples)
                Animations.Add(tup.Item1, tup.Item2);
        }

        /// <summary>
        /// Agrega o reemplaza una animación.
        /// <para>Adds or replaces an animation entry.</para>
        /// </summary>
        /// <param name="animationType">Tipo de animación.<para>Animation type key.</para></param>
        /// <param name="animation">Instancia de animación.<para>Animation instance.</para></param>
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
        /// Elimina una animación.
        /// <para>Removes an animation entry.</para>
        /// </summary>
        /// <param name="animationType">Tipo de animación.<para>Animation type key.</para></param>
        public void RemoveAnimation(AnimationType animationType)
        {
            Animations.Remove(animationType);
        }

        /// <summary>
        /// Obtiene una animación por su tipo.
        /// <para>Retrieves an animation by type.</para>
        /// </summary>
        /// <param name="animationType">Tipo de animación.<para>Animation type key.</para></param>
        /// <returns>La animación solicitada.<para>The requested animation.</para></returns>
        public readonly Animation GetAnimation(AnimationType animationType)
        {
            return Animations[animationType];
        }

        /// <summary>
        /// Comprueba si existe una animación para el tipo indicado.
        /// <para>Checks whether an animation exists for the given type.</para>
        /// </summary>
        /// <param name="type">Tipo de animación a comprobar.<para>Animation type to check.</para></param>
        /// <returns>True si existe la animación.<para>True if the animation exists.</para></returns>
        public bool ContainsAnimation(AnimationType type) => Animations.ContainsKey(type);

        /// <summary>
        /// Activa la animación solicitada.
        /// <para>Activates the requested animation.</para>
        /// </summary>
        /// <param name="type">Tipo de animación a activar.<para>Animation type to activate.</para></param>
        public void ActivateAnimation(AnimationType type)
        {
            CurrentAnimationType = (AnimationType.walk, GetAnimation(type));
        }
    }

    /// <summary>
    /// Define los tipos de animación disponibles.
    /// <para>Defines the available animation types.</para>
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Sin animación.
        /// <para>No animation.</para>
        /// </summary>
        none,
        /// <summary>
        /// Animación de reposo.
        /// <para>Idle animation.</para>
        /// </summary>
        idle,
        /// <summary>
        /// Animación de caminar.
        /// <para>Walking animation.</para>
        /// </summary>
        walk,
        /// <summary>
        /// Animación de correr.
        /// <para>Running animation.</para>
        /// </summary>
        run,
        /// <summary>
        /// Animación de salto.
        /// <para>Jumping animation.</para>
        /// </summary>
        jump,
        /// <summary>
        /// Animación de agacharse.
        /// <para>Crouching animation.</para>
        /// </summary>
        crouch,
        /// <summary>
        /// Animación de ataque.
        /// <para>Attack animation.</para>
        /// </summary>
        attack,
        /// <summary>
        /// Animación de daño.
        /// <para>Hurt animation.</para>
        /// </summary>
        hurt,
        /// <summary>
        /// Animación de muerte.
        /// <para>Death animation.</para>
        /// </summary>
        die
    }
}
