using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using YotsubaEngine.Attributes;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que permite la interacción mediante input con la entidad.
    /// <para>Component that enables input-driven interaction for an entity.</para>
    /// </summary>
    [UIComponent("Input", nameof(InputComponent))]
    public partial struct InputComponent()
    {

        /// <summary>
        /// Bandera runtime con qué tipos de input están habilitados (bitmask de InputInUse).
        /// </summary>
        private int InputFlags { get; set; } = 0;

        /// <summary>
        /// Bridge serializable: lista CSV de flags activos ("HasMouse,HasKeyboard,HasGamepad").
        /// </summary>
        [UIComponentValue("Entradas en uso", "InputsInUse",
            "Lista CSV de flags activos: HasMouse, HasKeyboard, HasGamepad.",
            "Valor no reconocido — use HasMouse/HasKeyboard/HasGamepad.",
            ValueConverterForRead: "RenderInputsInUseUI",
            defaultValue: "", inactiveValue: "")]
        public string InputsInUse { get; set; }

        /// <summary>
        /// Asignaciones de teclado para acciones de la entidad (runtime).
        /// </summary>
        public Dictionary<ActionEntityInput, Keys> KeyBoard { get; set; } = new();

        /// <summary>
        /// Bridge serializable: "MoveUp:W,MoveDown:S,...". El parse runtime puebla KeyBoard.
        /// </summary>
        [UIComponentValue("Mapeo de teclado", "KeyboardMappings",
            "Mapeo Acción→Tecla. Formato: 'MoveUp:W,MoveDown:S,...'.",
            "Formato esperado: 'accion:tecla,accion:tecla' (separadores ',' y ':').",
            ValueConverterForRead: "RenderKeyboardMappingsUI",
            defaultValue: "MoveUp:W,\nMoveDown:S,\nMoveLeft:A,\nMoveRight:D,", inactiveValue: "")]
        public string KeyboardMappings { get; set; }

        /// <summary>
        /// Asignaciones de botones del gamepad para acciones de la entidad (runtime).
        /// </summary>
        public Dictionary<ActionEntityInput, Buttons> GamePad { get; set; } = new();

        /// <summary>
        /// Índice del jugador para el gamepad asignado.
        /// </summary>
        [UIComponentValue("Índice de mando", nameof(GamePadIndex),
            "Índice de PlayerIndex del gamepad asignado.",
            "Valor de PlayerIndex no válido.",
            defaultValue: "", inactiveValue: "")]
        public PlayerIndex GamePadIndex { get; set; } = new();

        /// <summary>
        /// Asignaciones de botones del mouse para acciones de la entidad (runtime).
        /// </summary>
        public Dictionary<ActionEntityInput, MouseButton> Mouse { get; set; } = new();

        /// <summary>
        /// Bridge serializable: "MoveUp:Left,MoveDown:Right,...". El parse runtime puebla Mouse.
        /// </summary>
        [UIComponentValue("Mapeo de ratón", "MouseMappings",
            "Mapeo Acción→Botón. Formato: 'Action:Button,...'.",
            "Formato esperado: 'accion:boton,accion:boton'.",
            defaultValue: "", inactiveValue: "")]
        public string MouseMappings { get; set; }

        /// <summary>
        /// Comprueba si un tipo de input está habilitado.
        /// </summary>
        public bool HasInput(InputInUse input)
        {
            return (InputFlags & (int)input) != 0;
        }

        /// <summary>
        /// Añade un tipo de input al componente.
        /// </summary>
        public void AddInput(InputInUse input)
        {
            InputFlags |= (int)input;
        }

        /// <summary>
        /// Elimina un tipo de input del componente.
        /// </summary>
        public void RemoveInput(InputInUse input)
        {
            InputFlags &= ~(int)input;
        }
    }

    /// <summary>
    /// Indicadores de tipos de input soportados.
    /// </summary>
    [Flags]
    public enum InputInUse : byte
    {
        None = 0,
        HasMouse = 1 << 0,
        HasGamepad = 1 << 1,
        HasKeyboard = 1 << 2,
    }

}
