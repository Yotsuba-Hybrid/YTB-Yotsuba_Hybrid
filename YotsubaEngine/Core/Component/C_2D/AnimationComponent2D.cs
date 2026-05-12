using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YotsubaEngine.Attributes;
using YotsubaEngine.Graphics;

namespace YotsubaEngine.Core.Component.C_2D
{
    /// <summary>
    /// Componente que almacena animaciones de sprites para representar movimiento.
    /// <para>Component that stores sprite animations for movement.</para>
    /// </summary>
    [UIComponent("Animación 2D", nameof(AnimationComponent2D))]
    public partial struct AnimationComponent2D
    {
        /// <summary>
        /// Diccionario donde se almacenan todas las animaciones del componente.
        /// </summary>
        private readonly Dictionary<AnimationType, Animation> Animations = new Dictionary<AnimationType, Animation>();

        /// <summary>
        /// Ruta al XML del atlas de animaciones. Bridge de serialización.
        /// </summary>
        [UIComponentValue("Atlas de texturas", "TextureAtlasPath",
            "Ruta al XML del atlas que contiene las animaciones.",
            "El atlas no existe o no es válido.",
            ValueConverterForRead: "RenderTextureAtlasUI",
            defaultValue: "", inactiveValue: "")]
        public string TextureAtlasPath { get; set; }

        /// <summary>
        /// Vínculos entre AnimationType y nombre de animación en el atlas.
        /// Formato: "idle:idle_anim,walk:walk_anim,...".
        /// </summary>
        [UIComponentValue("Animaciones vinculadas", "AnimationBindings",
            "Asignaciones AnimationType→nombre de animación del atlas.",
            "Formato esperado: 'tipo:nombre,tipo:nombre' (separadores ',' y ':').",
            ValueConverterForRead: "RenderAnimationBindingsUI",
            defaultValue: "", inactiveValue: "")]
        public string AnimationBindings { get; set; }

        /// <summary>
        /// Tipo de animación que arrancará activa al cargar la entidad.
        /// El runtime usa esto junto con <see cref="CurrentAnimation"/> para indexar la Animation real.
        /// </summary>
        [UIComponentValue("Tipo de animación actual", nameof(CurrentAnimationType),
            "Animación inicial al cargar la entidad.",
            "Debe coincidir con un valor del enum AnimationType.",
            defaultValue: "none", inactiveValue: "")]
        public AnimationType CurrentAnimationType { get; set; }

        /// <summary>
        /// Tupla runtime con el tipo activo y la Animation real (no serializada). Mantenida por AnimationSystem2D.
        /// </summary>
        public ValueTuple<AnimationType, Animation> CurrentAnimation { get; set; }

        /// <summary>
        /// Crea el componente con las animaciones proporcionadas.
        /// </summary>
        public AnimationComponent2D(params Tuple<AnimationType, Animation>[] tuples)
        {
            foreach (var tup in tuples)
                Animations.Add(tup.Item1, tup.Item2);
        }

        public AnimationComponent2D()
        {
        }

        /// <summary>
        /// Agrega o reemplaza una animación.
        /// </summary>
        public void AddAnimation(AnimationType animationType, Animation animation)
        {
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
        /// </summary>
        public void RemoveAnimation(AnimationType animationType)
        {
            if (Animations is null)
                return;
            Animations.Remove(animationType);
        }

        /// <summary>
        /// Obtiene una animación por su tipo.
        /// </summary>
        public readonly Animation GetAnimation(AnimationType animationType)
        {
            if (Animations is null)
                throw new KeyNotFoundException("AnimationComponent2D has no animations dictionary (default/uninitialized state).");
            return Animations[animationType];
        }

        /// <summary>
        /// Comprueba si existe una animación para el tipo indicado.
        /// </summary>
        public bool ContainsAnimation(AnimationType type) => Animations != null && Animations.ContainsKey(type);

        /// <summary>
        /// Activa la animación solicitada (actualiza <see cref="CurrentAnimation"/> y <see cref="CurrentAnimationType"/>).
        /// </summary>
        public void ActivateAnimation(AnimationType type)
        {
            CurrentAnimationType = type;
            CurrentAnimation = (type, GetAnimation(type));
        }
    }

    /// <summary>
    /// Define los tipos de animación disponibles.
    /// </summary>
    public enum AnimationType
    {
        none,
        idle,
        walk,
        run,
        jump,
        crouch,
        attack,
        hurt,
        die
    }
}
