using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Component that enables input-driven interaction for an entity.
    /// Componente que añade la funcionalidad de que el jugador interaccione mediante el input con la entidad
    /// </summary>
    public struct InputComponent()
    {

        /// <summary>
        /// Bitmask storing which input types are used.
        /// Bandera que almacena si un tipo de input se esta usando por la entidad
        /// </summary>
        private int InputsInUse { get; set; } = 0;

        /// <summary>
        /// Keyboard mappings for entity actions.
        /// Listado de las teclas 
        /// </summary>
        public Dictionary<ActionEntityInput, Keys> KeyBoard { get; set; } = new();

        /// <summary>
        /// Gamepad button mappings for entity actions.
        /// Listado de los botones del gamepad
        /// </summary>
        public Dictionary<ActionEntityInput, Buttons> GamePad { get; set; } = new();

        /// <summary>
        /// Player index for the assigned gamepad.
        /// Almacena el index del mando que usara el jugador
        /// </summary>
        public PlayerIndex GamePadIndex { get; set; } = new();

        /// <summary>
        /// Mouse button mappings for entity actions.
        /// Listado de los botones del mouse
        /// </summary>
        public Dictionary<ActionEntityInput, MouseButton> Mouse { get; set; } = new();

        /// <summary>
        /// Checks whether a specific input type is enabled.
        /// Método para comprobar el si un tipo de input es usado por la entidad
        /// </summary>
        /// <param name="input">Input flag to check. Bandera de entrada a comprobar.</param>
        /// <returns>True if the input type is enabled. True si el tipo está habilitado.</returns>
        public bool HasInput(InputInUse input)
        {
            return (InputsInUse & (int)input) != 0;
        }

        /// <summary>
        /// Adds an input type flag to the component.
        /// Método para añadir un tipo de input al componente
        /// </summary>
        /// <param name="input">Input flag to add. Bandera de entrada a añadir.</param>
        public void AddInput(InputInUse input)
        {
            InputsInUse |= (int)input;
        }

        /// <summary>
        /// Removes an input type flag from the component.
        /// Método para remover un tipo de input al componente
        /// </summary>
        /// <param name="input">Input flag to remove. Bandera de entrada a remover.</param>
        public void RemoveInput(InputInUse input)
        {
            InputsInUse &= ~(int)input;
        }
    }

    /// <summary>
    /// Flags indicating which input types are supported.
    /// Flags para comprobar que tipo de input soporta la entidad
    /// </summary>
    [Flags]
    public enum InputInUse : byte
    {
        /// <summary>
        /// No input enabled.
        /// Sin entrada habilitada.
        /// </summary>
        None = 0,
        /// <summary>
        /// Mouse input enabled.
        /// Entrada de mouse habilitada.
        /// </summary>
        HasMouse = 1 << 0,
        /// <summary>
        /// Gamepad input enabled.
        /// Entrada de gamepad habilitada.
        /// </summary>
        HasGamepad = 1 << 1,
        /// <summary>
        /// Keyboard input enabled.
        /// Entrada de teclado habilitada.
        /// </summary>
        HasKeyboard = 1 << 2,
    }

    

}
