using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.YTBControls.Buttons
{
    ///<summary>
    /// Represents a button control within the YTB user interface framework. Use this class to create interactive button elements that can respond to user input. Inherits common functionality from <see cref="YTBControl"/>.
    ///
    /// Representa un control de botón dentro del framework de interfaz de usuario YTB. Utiliza esta clase para crear elementos de botón interactivos que pueden responder a la entrada del usuario. Hereda funcionalidad común de <see cref="YTBControl"/>.
    ///</summary>
    public class YTBButton : YTBControl
    {
        ///<summary>
        /// Stores the transform values associated with each button part.
        ///
        /// Almacena los valores de transformación asociados a cada parte del botón.
        ///</summary>
        private readonly Dictionary<ButtonPart, int> PartTransforms;

        ///<summary>
        /// Gets or sets the set of rectangle properties that are currently in use.
        ///
        /// Obtiene o establece el conjunto de propiedades de rectángulo que están actualmente en uso.
        ///</summary>
        private ButtonPart Properties = 0;

        ///<summary>
        /// Represents the position and size of the object as a rectangle.
        ///
        /// Representa la posición y el tamaño del objeto como un rectángulo.
        ///</summary>
        private Rectangle Pos;
        ///<summary>
        /// Initializes a new instance of the YTBButton class. Parameters: entityManager - entity manager to handle entities; position - rectangle representing position and size; action - action to execute on button press.
        ///
        /// Inicializa una nueva instancia de la clase YTBButton. Parámetros: entityManager - administrador de entidades para manejar entidades; position - rectángulo que representa la posición y el tamaño; action - acción a ejecutar al presionar el botón.
        ///</summary>
        public YTBButton(EntityManager entityManager, Rectangle position, Action action) : base(entityManager)
        {
            PartTransforms = new();
            PartTransforms[ButtonPart.Background] = Add(Color.White);
            Pos = position;

            var buttonComponent = new ButtonComponent2D()
            {
                Action = action,
                EffectiveArea = position,
                IsActive = true
            };
            ref Yotsuba entity = ref EntityManager.YotsubaEntities[PartTransforms[ButtonPart.Background]];
            EntityManager.AddButtonComponent2D(entity, buttonComponent);
        }

        ///<summary>
        /// Sets the background color, position, and size of the button's background part. Parameters: BackgroundColor - the color to apply; position - the 2D position; Size - the 2D size.
        ///
        /// Establece el color de fondo, posición y tamaño de la parte de fondo del botón. Parámetros: BackgroundColor - el color a aplicar; position - la posición 2D; Size - el tamaño 2D.
        ///</summary>
        public void SetButton(Color BackgroundColor, Vector2 position, Vector2 Size)
        {
            ref var transform = ref EntityManager.TransformComponents[PartTransforms[ButtonPart.Background]];
            transform.Position = new Vector3(position, 0f);
            transform.Size = new Vector3(Size, 0);
            transform.Color = BackgroundColor;
            transform.LayerDepth = 0.2f;  // Background: behind borders (higher = further back with BackToFront)
            transform.Scale = 1f;
        }

        ///<summary>
        /// Sets the visual state of a specified button part by applying the given color and border thickness. Parameters: part - the button part to update; color - color to apply; thickness - border thickness in pixels (default 1).
        ///
        /// Establece el estado visual de una parte específica del botón aplicando el color y grosor de borde dados. Parámetros: part - la parte del botón a actualizar; color - color a aplicar; thickness - grosor del borde en píxeles (por defecto 1).
        ///
        /// Throws ArgumentOutOfRangeException if part is ButtonPart.Text or undefined. / Lanza ArgumentOutOfRangeException si part es ButtonPart.Text o un valor indefinido.
        ///</summary>
        public void SetButtonState(ButtonPart part, Color color, float thickness = 1)
        {
            TransformComponent transformComponent = new();
            transformComponent.Color = color;
            transformComponent.Scale = 1f;  // CRITICAL: Must be 1 to be visible

            Rectangle rectangle = new Rectangle();

            switch (part)
            {
                    case ButtonPart.Background:
                    rectangle = Pos;
                    break;
                    case ButtonPart.Border_Top:
                    rectangle = new Rectangle(Pos.X, Pos.Y, Pos.Width, (int)thickness);
                    break;
                    case ButtonPart.Border_Left:
                        rectangle = new Rectangle(Pos.X, Pos.Y, (int)thickness, Pos.Height);
                    break;
                    case ButtonPart.Border_Bottom:
                    rectangle = new Rectangle(Pos.X, Pos.Y + Pos.Height - (int)thickness, Pos.Width, (int)thickness);
                    break;
                    case ButtonPart.Border_Right:
                    rectangle = new Rectangle(Pos.X + Pos.Width - (int)thickness, Pos.Y, (int)thickness, Pos.Height);
                    break;
                    case ButtonPart.Text:
                    throw new ArgumentOutOfRangeException(nameof(part), part, null);
                    default:
                    throw new ArgumentOutOfRangeException(nameof(part), part, null);
            }

            transformComponent.Position = new Vector3(rectangle.X, rectangle.Y, 0);
            transformComponent.Size = new Vector3(rectangle.Width, rectangle.Height, 0);
            transformComponent.LayerDepth = 0.1f;  // Borders: in front of background (lower = closer with BackToFront)


            AddButtonState(part, transformComponent, rectangle);
        }

        ///<summary>
        /// Adds a new button state for the specified part with the given transform and rectangle if it does not already exist. Parameters: part - the button part; transform - transform component; rect - rectangle area.
        ///
        /// Agrega un nuevo estado de botón para la parte especificada con la transformación y rectángulo dados si aún no existe. Parámetros: part - la parte del botón; transform - componente de transformación; rect - área del rectángulo.
        ///</summary>
        private void AddButtonState(ButtonPart part, TransformComponent transform, Rectangle rect)
        {
            if (!HasComponent(part))
            {
                PartTransforms[part] = Add(transform.Color, transform);
                AddComponent(part);

                ref Yotsuba entity = ref EntityManager.YotsubaEntities[PartTransforms[part]];
                entity.Name = Name + "_" + part.ToString();
                var buttonComponent = new ButtonComponent2D()
                {
                    Action = null,
                    EffectiveArea = rect,
                    IsActive = true
                };
                EntityManager.AddButtonComponent2D(entity, buttonComponent);
            }
        }

        ///<summary>
        /// Method to check if the entity has a specific component. Parameters: component - component to check. Returns true if the component exists, false otherwise.
        ///
        /// Método para verificar si la entidad tiene un componente específico. Parámetros: component - componente a verificar. Devuelve verdadero si el componente existe, falso en caso contrario.
        ///</summary>
        private bool HasComponent(ButtonPart component) => (Properties & component) != 0;

        ///<summary>
        /// Method to add a component to the entity. Parameters: component - component to add.
        ///
        /// Método para agregar un componente a la entidad. Parámetros: component - componente a agregar.
        ///</summary>
        private void AddComponent(ButtonPart component) => Properties |= component;

        ///<summary>
        /// Method to remove a component from the entity. Parameters: component - component to remove.
        ///
        /// Método para remover un componente de la entidad. Parámetros: component - componente a remover.
        ///</summary>
        private void RemoveComponent(ButtonPart component)
        {
            Properties &= ~component;
        }

        

        ///<summary>
        /// Specifies the set of rectangle regions used in rendering operations, such as background, borders, and text areas. This enumeration supports bitwise combination of its values to indicate multiple regions in use simultaneously. Individual flags correspond to specific visual elements.
        ///
        /// Especifica el conjunto de regiones rectangulares utilizadas en operaciones de renderizado, como fondo, bordes y áreas de texto. Esta enumeración soporta la combinación bit a bit de sus valores para indicar múltiples regiones en uso simultáneamente. Las banderas individuales corresponden a elementos visuales específicos.
        ///</summary>
        [Flags]
        public enum ButtonPart : int
        {
            None = 0,
            Background = 1 << 1,
            Border_Top = 1 << 2,
            Border_Bottom = 1 << 3,
            Border_Left = 1 << 4,
            Border_Right = 1 << 5,
            Text = 1 << 6,
        }

        ///<summary>
        /// Specifies the visual or interactive state of a user interface button. Use this enumeration to represent the current state of a button in UI logic, such as rendering or event handling.
        ///
        /// Especifica el estado visual o interactivo de un botón de interfaz de usuario. Utiliza esta enumeración para representar el estado actual de un botón en la lógica de la interfaz de usuario, como renderizado o manejo de eventos.
        ///</summary>
        public enum ButtonState
        {
            Normal,
            Hovered,
            Pressed,
            Disabled
        }
    }
}
