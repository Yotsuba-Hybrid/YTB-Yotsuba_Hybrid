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
    /// Representa un control de botón dentro del framework de interfaz de usuario YTB para crear elementos interactivos.
    /// <para>Represents a button control within the YTB user interface framework to create interactive elements.</para>
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
        /// Inicializa una nueva instancia de la clase YTBButton.
        /// <para>Initializes a new instance of the YTBButton class.</para>
        ///</summary>
        /// <param name="entityManager">Administrador de entidades para manejar entidades. <para>Entity manager to handle entities.</para></param>
        /// <param name="position">Rectángulo que representa la posición y el tamaño. <para>Rectangle representing position and size.</para></param>
        /// <param name="action">Acción a ejecutar al presionar el botón. <para>Action to execute on button press.</para></param>
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
        /// Establece el color de fondo, posición y tamaño de la parte de fondo del botón.
        /// <para>Sets the background color, position, and size of the button's background part.</para>
        ///</summary>
        /// <param name="BackgroundColor">Color a aplicar. <para>Color to apply.</para></param>
        /// <param name="position">Posición 2D. <para>2D position.</para></param>
        /// <param name="Size">Tamaño 2D. <para>2D size.</para></param>
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
        /// Establece el estado visual de una parte específica del botón aplicando el color y grosor de borde.
        /// <para>Sets the visual state of a specified button part by applying the given color and border thickness.</para>
        ///</summary>
        /// <param name="part">Parte del botón a actualizar. <para>Button part to update.</para></param>
        /// <param name="color">Color a aplicar. <para>Color to apply.</para></param>
        /// <param name="thickness">Grosor del borde en píxeles (por defecto 1). <para>Border thickness in pixels (default 1).</para></param>
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
        /// Especifica el conjunto de regiones rectangulares utilizadas en renderizado, como fondo, bordes y áreas de texto.
        /// <para>Specifies the set of rectangle regions used in rendering operations, such as background, borders, and text areas.</para>
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
        /// Especifica el estado visual o interactivo de un botón de interfaz de usuario.
        /// <para>Specifies the visual or interactive state of a user interface button.</para>
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
