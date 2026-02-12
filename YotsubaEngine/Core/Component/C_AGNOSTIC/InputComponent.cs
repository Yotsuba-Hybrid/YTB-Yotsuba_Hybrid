using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que permite la interacción mediante input con la entidad.
    /// <para>Component that enables input-driven interaction for an entity.</para>
    /// </summary>
    public struct InputComponent()
    {

        /// <summary>
        /// Bandera que almacena si un tipo de input se está usando por la entidad.
        /// <para>Bitmask storing which input types are used.</para>
        /// </summary>
        private int InputsInUse { get; set; } = 0;

        /// <summary>
        /// Asignaciones de teclado para acciones de la entidad.
        /// <para>Keyboard mappings for entity actions.</para>
        /// </summary>
        public Dictionary<ActionEntityInput, Keys> KeyBoard { get; set; } = new();

        /// <summary>
        /// Asignaciones de botones del gamepad para acciones de la entidad.
        /// <para>Gamepad button mappings for entity actions.</para>
        /// </summary>
        public Dictionary<ActionEntityInput, Buttons> GamePad { get; set; } = new();

        /// <summary>
        /// Índice del jugador para el gamepad asignado.
        /// <para>Player index for the assigned gamepad.</para>
        /// </summary>
        public PlayerIndex GamePadIndex { get; set; } = new();

        /// <summary>
        /// Asignaciones de botones del mouse para acciones de la entidad.
        /// <para>Mouse button mappings for entity actions.</para>
        /// </summary>
        public Dictionary<ActionEntityInput, MouseButton> Mouse { get; set; } = new();

        /// <summary>
        /// Comprueba si un tipo de input está habilitado.
        /// <para>Checks whether a specific input type is enabled.</para>
        /// </summary>
        /// <param name="input">Bandera de entrada a comprobar.<para>Input flag to check.</para></param>
        /// <returns>True si el tipo está habilitado.<para>True if the input type is enabled.</para></returns>
        public bool HasInput(InputInUse input)
        {
            return (InputsInUse & (int)input) != 0;
        }

        /// <summary>
        /// Añade un tipo de input al componente.
        /// <para>Adds an input type flag to the component.</para>
        /// </summary>
        /// <param name="input">Bandera de entrada a añadir.<para>Input flag to add.</para></param>
        public void AddInput(InputInUse input)
        {
            InputsInUse |= (int)input;
        }

        /// <summary>
        /// Elimina un tipo de input del componente.
        /// <para>Removes an input type flag from the component.</para>
        /// </summary>
        /// <param name="input">Bandera de entrada a remover.<para>Input flag to remove.</para></param>
        public void RemoveInput(InputInUse input)
        {
            InputsInUse &= ~(int)input;
        }
    }

    /// <summary>
    /// Indicadores de tipos de input soportados.
    /// <para>Flags indicating which input types are supported.</para>
    /// </summary>
    [Flags]
    public enum InputInUse : byte
    {
        /// <summary>
        /// Sin entrada habilitada.
        /// <para>No input enabled.</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// Entrada de mouse habilitada.
        /// <para>Mouse input enabled.</para>
        /// </summary>
        HasMouse = 1 << 0,
        /// <summary>
        /// Entrada de gamepad habilitada.
        /// <para>Gamepad input enabled.</para>
        /// </summary>
        HasGamepad = 1 << 1,
        /// <summary>
        /// Entrada de teclado habilitada.
        /// <para>Keyboard input enabled.</para>
        /// </summary>
        HasKeyboard = 1 << 2,
    }

    

}
