using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;
using static YotsubaEngine.Core.System.S_AGNOSTIC.InputSystem;

namespace YotsubaEngine.Core.System.YTBDragAndDrop
{
    /// <summary>
    /// Gestiona el arrastrar y soltar y el arrastre de entidades en el editor.
    /// <para>Handles drag-and-drop and entity dragging behaviors in the editor.</para>
    /// </summary>
    internal class DragAndDropSystem : ISystem
    {
        /// <summary>
        /// Almacena los archivos soltados en la ventana.
        /// <para>Stores files dropped into the window.</para>
        /// </summary>
        public static readonly YTB<FileDropped> Files = new YTB<FileDropped>();

        /// <summary>
        /// Obtiene la última posición de soltado en coordenadas de ventana.
        /// <para>Gets the last drop position in window coordinates.</para>
        /// </summary>
        public static Point DropPosition { get; private set; }

        /// <summary>
        /// Holds the current entity manager reference.
        /// Contiene la referencia actual del administrador de entidades.
        /// </summary>
        EntityManager EntityManager;

        /// <summary>
        /// Caches the event manager instance.
        /// Almacena en caché la instancia del administrador de eventos.
        /// </summary>
        EventManager EventManager;

        /// <summary>
        /// Inicializa el sistema de arrastrar y soltar con el contexto de entidades.
        /// <para>Initializes the drag-and-drop system with entity context.</para>
        /// </summary>
        /// <param name="entities">Administrador de entidades. <para>Entity manager.</para></param>
        public void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            EventManager = EventManager.Instance;
        }

        /// <summary>
        /// Actualiza el comportamiento de arrastre para cada entidad durante el pase compartido.
        /// <para>Updates drag behavior for each entity during the shared update pass.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        /// <param name="time">Tiempo de juego. <para>Game time.</para></param>
        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
#if YTB
            EntityDrag(ref Entidad);
#endif
        }

        /// <summary>
        /// Inicializa el estado por entidad cuando sea necesario.
        /// <para>Initializes per-entity state when needed.</para>
        /// </summary>
        /// <param name="Entidad">Instancia de entidad. <para>Entity instance.</para></param>
        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            // Drag-and-drop initialization is handled in InitializeSystem
        }

        /// <summary>
        /// Actualiza el sistema en cada frame.
        /// <para>Updates the system each frame.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void UpdateSystem(GameTime gameTime)
        {
            // Per-entity update logic is handled in SharedEntityForEachUpdate
        }



        // Variables a nivel de CLASE (Manager/System)
        /// <summary>
        /// Tracks the currently dragged entity identifier.
        /// Controla el identificador de la entidad arrastrada actualmente.
        /// </summary>
        private int? _draggedEntityId = null;

        /// <summary>
        /// Stores the drag offset between entity position and cursor.
        /// Almacena el desplazamiento entre la posición de la entidad y el cursor.
        /// </summary>
        private Vector2 _dragOffset = Vector2.Zero;

        /// <summary>
        /// Aplica la lógica de arrastre a la entidad especificada.
        /// <para>Applies drag logic to the specified entity.</para>
        /// </summary>
        /// <param name="entity">Entidad objetivo. <para>Target entity.</para></param>
        public void EntityDrag(ref Yotsuba entity)
        {

            var input = InputManager.Instance;

            var mouseState = input.Mouse.CurrentState;



            if (!entity.HasComponent(YTBComponent.Transform) || entity.HasComponent(YTBComponent.TileMap)) return;

            Span<TransformComponent> transformComponentsSpan = EntityManager.TransformComponents.AsSpan();
            ref TransformComponent transform = ref transformComponentsSpan[entity.Id];

            // RenderSystem2D.IsGameActive is only defined in DEBUG builds.
            // In Release builds, consider only platform when deciding UI rendering.
//-:cnd:noEmit
#if YTB
            bool canRenderUIElements = RenderSystem2D.IsGameActive || !OperatingSystem.IsWindows();
#else
            bool canRenderUIElements = !OperatingSystem.IsWindows();
#endif
//+:cnd:noEmit

            float currentZoom =
//-:cnd:noEmit
#if YTB

             canRenderUIElements ?
#endif
//+:cnd:noEmit
             YTBGlobalState.CameraZoom
//-:cnd:noEmit
#if YTB
             :

            RenderSystem2D.EDITOR_SCALE_CAMERA
#endif
//+:cnd:noEmit
             ;

            Vector2 offset =

//-:cnd:noEmit
#if YTB
                 canRenderUIElements ?
#endif
//+:cnd:noEmit
                 YTBGlobalState.OffsetCamera
//-:cnd:noEmit
#if YTB
                 : new Vector2(RenderSystem2D.EDITOR_OFFSET_CAMERA_X, RenderSystem2D.EDITOR_OFFSET_CAMERA_Y)
#endif
//+:cnd:noEmit
                 ;

            // Datos básicos de la entidad
            Vector2 entityPos = new Vector2(transform.Position.X, transform.Position.Y);
            Vector2 entitySize = new Vector2(transform.Size.X, transform.Size.Y);

            // --- VARIABLES DE CÁLCULO ---
            Vector2 currentMousePos; // La posición del mouse que usaremos para comparar
            Rectangle entityRect;    // El área de colisión de la entidad

            // -----------------------------------------------------------------------
            // CASO 1: ES INTERFAZ DE USUARIO (BUTTON 2D)
            // -----------------------------------------------------------------------
            if (entity.HasComponent(YTBComponent.Button2D))
            {
                // UI: Usamos coordenadas de pantalla directas
                currentMousePos = new Vector2(mouseState.X, mouseState.Y);

                // UI: Generalmente el origen es la esquina superior izquierda
                entityRect = new Rectangle(entityPos.ToPoint(), entitySize.ToPoint());
            }
            // -----------------------------------------------------------------------
            // CASO 2: ES ENTIDAD DEL MUNDO (SPRITE NORMAL)
            // -----------------------------------------------------------------------
            else
            {
                // MUNDO: Necesitamos convertir el mouse de Pantalla -> Mundo
                Vector2 screenMousePos = new Vector2(mouseState.X, mouseState.Y);

                if (EntityManager.Camera != null)
                {
                    // 1. Obtenemos la matriz de vista actual (Idéntica a la del UpdateSystem)
                    ref var camTransform = ref transformComponentsSpan[EntityManager.Camera.EntityToFollow];
                    var viewport = ((YTBGame)YTBGame.Instance).GraphicsDevice.Viewport;
                    Vector2 screenCenter = new Vector2(viewport.Width / 2f, viewport.Height / 2f);

                    Matrix viewMatrix = EntityManager.Camera.Get2DViewMatrix(
                        new Vector2(camTransform.Position.X, camTransform.Position.Y) + offset,
                        screenCenter,
                        currentZoom,
                        0f
                        );

                    // 2. INVERTIMOS la matriz para ir "hacia atrás" (de Pantalla a Mundo)
                    Matrix inverseView = Matrix.Invert(viewMatrix);

                    // 3. Transformamos el mouse
                    currentMousePos = Vector2.Transform(screenMousePos, inverseView);
                }
                else
                {
                    // Si no hay cámara, Screen == World
                    currentMousePos = screenMousePos;
                }

                // MUNDO: En tu UpdateSystem, los sprites se dibujan centrados (origin = size * 0.5).
                // Por lo tanto, el rectángulo de colisión debe centrarse en la posición.
                entityRect = new Rectangle(
                    (int)(entityPos.X - entitySize.X / 2f), // Restamos mitad del ancho
                    (int)(entityPos.Y - entitySize.Y / 2f), // Restamos mitad del alto
                    (int)(entitySize.X * transform.Scale),
                    (int)(entitySize.Y * transform.Scale)
                );
            }

            // -----------------------------------------------------------------------
            // LÓGICA DE ARRASTRE (COMÚN PARA AMBOS)
            // -----------------------------------------------------------------------

            // 1. INICIO DEL ARRASTRE
            if (_draggedEntityId == null)
            {
                if (input.Mouse.IsButtonDown(MouseButton.Left) && entityRect.Contains(currentMousePos))
                {
                    _draggedEntityId = entity.Id;
                    // Calculamos el offset usando la posición correcta del mouse (Mundo o Pantalla según el caso)
                    _dragOffset = entityPos - currentMousePos;
                }
            }
            // 2. DURANTE EL ARRASTRE
            else if (_draggedEntityId == entity.Id)
            {
                if (input.Mouse.IsButtonDown(MouseButton.Left) && input.Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    // Nueva posición = Mouse (convertido) + Offset
                    Vector2 newPos = currentMousePos + _dragOffset;
                    transform.SetPosition(newPos.X, newPos.Y, transform.Position.Z);
                }
                else
                {
                    // 3. SOLTAR EL ARRASTRE (Guardar cambios)
                    GuardarCambiosEnEscena(entity, transform); // Extraje esto a un método helper para limpieza
                    _draggedEntityId = null;
                }
            }
        }

        // Método auxiliar para no ensuciar la lógica principal
        /// <summary>
        /// Persists transform changes back into the scene data.
        /// Persiste los cambios de transformación en los datos de la escena.
        /// </summary>
        private void GuardarCambiosEnEscena(Yotsuba entity, TransformComponent transform)
        {
            var gameInfo = EngineUISystem.GameInfo;
            var gameClass = (YTBGame)YTBGame.Instance;
            string sceneName = gameClass.SceneManager.CurrentScene.SceneName;

            var currentScene = gameInfo.Scene.FirstOrDefault(x => x.Name == sceneName);
            if (currentScene == null) return;

            var entityData = currentScene.Entities.FirstOrDefault(x => x.Name == entity.Name);
            if (entityData == null) return;

            var transformComponentData = entityData.Components.FirstOrDefault(x => x.ComponentName == nameof(TransformComponent));
            if (transformComponentData == null) return;

            foreach (var prop in transformComponentData.Propiedades.ToImmutableArray())
            {
                if (prop.Item1 == "Position")
                {
                    UpdateProperty(transformComponentData, prop.Item1, $"{transform.Position.X},{transform.Position.Y},{transform.Position.Z}");
                }
            }
        }

        /// <summary>
        /// Updates a property value in the serialized component data.
        /// Actualiza un valor de propiedad en los datos serializados del componente.
        /// </summary>
        private void UpdateProperty(YTBComponents component, string propertyName, string newValue)
        {
            int index = component.Propiedades.FindIndex(x => x.Item1 == propertyName);
            if (index >= 0)
            {
                component.Propiedades[index] = new(propertyName, newValue);

            }
        }

        /// <summary>
        /// Maneja los eventos de soltado de archivos y encola archivos soportados.
        /// <para>Handles window file drop events and queues supported files.</para>
        /// </summary>
        /// <param name="sender">Origen del evento. <para>Event source.</para></param>
        /// <param name="e">Argumentos del evento de soltado. <para>File drop event arguments.</para></param>
        public static void Window_FileDrop(object sender, FileDropEventArgs e)
        {
            MouseState mouseState = Mouse.GetState();
            DropPosition = mouseState.Position;

            foreach (string file in e.Files)
            {
                if (Files.ToArray().Any(x => x.Name == file)) continue;

                if (TryGetDroppedFileKind(file, out var kind))
                {
                    EngineUISystem.SendLog($"Archivo soltado: {file}");
                    Files.Add(new(file, kind));
                }
            }
        }

        private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg" };
        private static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".wav", ".ogg", ".mp3", ".wma" };
        private static readonly HashSet<string> FontExtensions = new(StringComparer.OrdinalIgnoreCase) { ".ttf", ".spritefont" };
        private static readonly HashSet<string> ScriptExtensions = new(StringComparer.OrdinalIgnoreCase) { ".cs" };
        private static readonly HashSet<string> ShaderExtensions = new(StringComparer.OrdinalIgnoreCase) { ".fx", ".mgfxo" };
        private static readonly HashSet<string> ConfigExtensions = new(StringComparer.OrdinalIgnoreCase) { ".ytb" };
        private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase) { ".txt" };

        private static bool TryGetDroppedFileKind(string filePath, out DroppedFileKind kind)
        {
            kind = DroppedFileKind.Other;

            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (ImageExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Image;
                return true;
            }

            if (extension == ".xml")
            {
                kind = IsSpriteSheetXml(filePath) ? DroppedFileKind.SpriteSheetXml : DroppedFileKind.Xml;
                return true;
            }

            if (AudioExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Audio;
                return true;
            }

            if (FontExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Font;
                return true;
            }

            if (ScriptExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Script;
                return true;
            }

            if (ShaderExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Shader;
                return true;
            }

            if (ConfigExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Config;
                return true;
            }

            if (TextExtensions.Contains(extension))
            {
                kind = DroppedFileKind.Text;
                return true;
            }

            return false;
        }

        private static bool IsSpriteSheetXml(string xmlPath)
        {
            try
            {
                XDocument document = XDocument.Load(xmlPath);
                XElement root = document.Root;
                return root != null &&
                       root.Attribute("imagepath") != null &&
                       string.Equals(root.Name.LocalName, "textureatlas", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

    }

    /// <summary>
    /// Tipos de archivos aceptados al soltar en la ventana.
    /// <para>Types of files accepted when dropped onto the window.</para>
    /// </summary>
    public enum DroppedFileKind
    {
        Image,
        SpriteSheetXml,
        Xml,
        Audio,
        Font,
        Script,
        Shader,
        Config,
        Text,
        Other
    }

    /// <summary>
    /// Representa los metadatos de un archivo soltado.
    /// <para>Represents metadata for a dropped file.</para>
    /// </summary>
    /// <param name="name">Nombre del archivo. <para>File name.</para></param>
    /// <param name="kind">Tipo de archivo. <para>File kind.</para></param>
    public class FileDropped(string name, DroppedFileKind kind)
    {
        /// <summary>
        /// Indica si el archivo está seleccionado en la UI.
        /// <para>Indicates whether the file is selected in the UI.</para>
        /// </summary>
        public bool selected = false;

        /// <summary>
        /// Obtiene o establece la ruta o nombre del archivo.
        /// <para>Gets or sets the file path or name.</para>
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// Obtiene el tipo del archivo soltado.
        /// <para>Gets the type of the dropped file.</para>
        /// </summary>
        public DroppedFileKind Kind { get; } = kind;
    }
}
