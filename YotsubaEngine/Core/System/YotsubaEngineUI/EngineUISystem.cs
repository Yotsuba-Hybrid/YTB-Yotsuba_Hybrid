using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.XML_SpriteSheet_Files;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.Graphics.ImGuiNet;
using YotsubaEngine.HighestPerformanceTypes;
using D = System.Drawing;
using Num = System.Numerics;
namespace YotsubaEngine.Core.System.YotsubaEngineUI
{
    /// <summary>
    /// Clase propia del engine (NO SE EJECUTA EN PRODUCCION) que coordina los subsistemas UI. Mantiene referencias compartidas
    /// (GameInfo, GuiRenderer, SelectedEntity, etc.) y delega el render a clases pequeñas.
    /// </summary>

    internal class EngineUISystem
    {

#if YTB
        bool showEngineEditor = true;
        bool pauseSystem = false;
#endif
        /// <summary>
        /// Almacenamiento de todos los mensajes que aparecen en la consola del Engine al usuario en tiempo real y de manera global en la app.
        /// </summary>
        public static YTB<(Color, string)> Messages { get; set; } = new YTB<(Color, string)>();

        /// <summary>
        /// Almacenamiento de todos los errores en la consola del engine.
        /// </summary>
        public static YTB<string> Errors { get; set; } = new YTB<string>();



        /// <summary>
        /// Aquí se guardan los datos del json con toda la info del juego creada desde la UI del engine.
        /// </summary>
        public static YTBGameInfo GameInfo { get; set; }

        /// <summary>
        /// Aquí se almacena la entidad seleccionada para poder compartirla en todo el engine mantener el estado del mismo
        /// </summary>
        public YTBEntity SelectedEntity { get; private set; }

        /// <summary>
        /// Aquí se almacena la escena seleccionada para poder compartirla en todo el engine mantener el estado del mismo
        /// </summary>
        public YTBScene SelectedScene { get; private set; }

        /// <summary>
        /// Referencia al SceneManagerUI del GameEngine
        /// </summary>
        private SceneManagerUI _sceneManager;

        /// <summary>
        /// Referencia al EntityManagerUI del GameEngine
        /// </summary>
        private EntityManagerUI _entityManager;

        /// <summary>
        /// Referencia al MenuBarUI del GameEngine
        /// </summary>
        private MenuBarUI _menuBar;

        /// <summary>
        /// Referencia al ConsoleUI del GameEngine
        /// </summary>
        private ConsoleUI _consoleUI;

#if YTB
        private ImGuiRenderer GuiRenderer = YTBGame.GuiRenderer;

        /// <summary>
        /// Referencia al DebugOverlayUI del GameEngine
        /// </summary>
        private DebugOverlayUI _debugOverlayUI;
#endif
        public static Action SaveChanges = () => { SaveGameInfo(); };

        public void InitializeSystem(EntityManager entities, ContentManager content)
        {

            // Al inicializar (seguro)
            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                ImGui.CreateContext();



            //GuiRenderer = new ImGuiRenderer(YTBGame.Instance);

            // CAMBIO 2: Esperar el resultado de la tarea de forma síncrona
            GameInfo = ReadYTBFile.ReadYTBGameFile().GetAwaiter().GetResult();

            // Esta línea ahora se ejecutará de forma segura antes del primer Update

            // Inicializar subsistemas
            _sceneManager = new SceneManagerUI(GameInfo, SelectEntity, SelectScene);
            _entityManager = new EntityManagerUI(() => SelectedEntity, () => SelectedScene, () => { SaveGameInfo(); });
            _menuBar = new MenuBarUI(() => YTBGlobalState.EngineBackground, ApplyBackgroundColor, SaveBackgroundSetting);
            _consoleUI = new ConsoleUI();
#if YTB
            _debugOverlayUI = new DebugOverlayUI();
#endif
            //EngineUISystem.SendLog("El EngineUiSystem está listo!");

            EventManager.Instance.Subscribe<OnGameFileWasReplaceByHistory>(GameFileWasReplace);
#if YTB
            EventManager.Instance.Subscribe<OnHiddeORShowUIEngineEditor>(OnHiddeORShowUIEngineEditorFunc);
            EventManager.Instance.Subscribe<OnShowGameUIHiddeEngineEditor>(OnHiddeORShowUIEngineEditorFunc);

            InitializeShowTextureImages();

            ImGui.StyleColorsClassic();
#endif
        }

#if YTB
        private void OnHiddeORShowUIEngineEditorFunc(OnShowGameUIHiddeEngineEditor editor)
        {
            if (!editor.ShowEngineEditor && !editor.ShowGameUI)
            {
                pauseSystem = true;
            }
            else
            {

                showEngineEditor = editor.ShowEngineEditor;
                pauseSystem = false;
            }
        }

        private void OnHiddeORShowUIEngineEditorFunc(OnHiddeORShowUIEngineEditor editor)
        {
            showEngineEditor = !showEngineEditor;
        }

#endif
        private void GameFileWasReplace(OnGameFileWasReplaceByHistory history)
        {
            SendLog("Game file updated");
            GameInfo = ReadYTBFile.ReadYTBGameFile().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Delegado que cambia el estado de la entidad seleccionada de manera global
        /// </summary>
        /// <param name="entity"></param>
        private void SelectEntity(YTBEntity entity)
        {
            SelectedEntity = entity;
        }

        /// <summary>
        /// Delegado que cambia el estado de la escena seleccionada de manera global
        /// </summary>
        /// <param name="scene"></param>
        private void SelectScene(YTBScene scene)
        {
            SelectedScene = scene;
        }

        /// <summary>
        /// Método que guarda los cambios realizados al juego mediante la UI del Framework / Engine
        /// </summary>
        private static void SaveGameInfo()
        {
            WriteYTBFile.EditYTBGameFile(GameInfo);
        }

        private void SaveBackgroundSetting(string colorName)
        {
            // Aquí puedes serializar a un archivo de configuración si lo deseas
            // File.WriteAllText("engine_settings.json", $"{\"Background\":\"{colorName}\"}");
        }

        private void ApplyBackgroundColor(Color c)
        {
            YTBGlobalState.EngineBackground = c;
            // En el render del engine principal debes usar EngineBackground para limpiar
        }

        bool xmlExistente = false;
        bool todos = false;
        string spriteSheetName = "SpriteSheet";
        string spriteSheetFolder = "Spritesheets";
        public void UpdateSystem(SpriteBatch spriteBatch, GameTime gameTime)
        {
#if YTB
            if (pauseSystem)
            {
                return;

            }
#endif
#if YTB

            GuiRenderer.BeginLayout(gameTime);
#endif
            // Render de la capa base (dockspace + menubar)
            RenderizarCapaBaseEngine();
#if YTB


            if (showEngineEditor)
            {
                // Panels delegados
                _sceneManager.Render();
                _entityManager.Render();
                _consoleUI.Render();

                OnDropFiles();
                UpdateShowTextureImages();

              
            }

            // Renderizar Debug Overlay (siempre disponible cuando el juego está activo)
            _debugOverlayUI?.Render();
#endif
#if YTB

            GuiRenderer.EndLayout();
#endif

        }

        public void OnDropFiles()
        {
            if (DragAndDropSystem.Files.Count > 0)
            {

                // 2. Dibujamos el primer modal
                YTBGui.AbrirSeccion("Archivos Soltados", () =>
                {
                    YTBGui.Text("Archivos Soltados (Si cierras el Engine, desaparecera)", true);
                    YTBGui.Line();
                    bool state = true;
                    GetDropSelectionState(out bool spriteSheetXmlSelected, out bool imageSelected, out bool importSelected);

                    YTBGui.CheckBox("Seleccionar Todos", ref todos);

                    foreach (FileDropped file in DragAndDropSystem.Files)
                    {
                        bool shouldShow = !spriteSheetXmlSelected || file.Kind != DroppedFileKind.SpriteSheetXml || file.selected;
                        if (!shouldShow)
                            continue;

                        string name = Path.GetFileName(file.Name);
                        YTBGui.CheckBox(name, ref file.selected);
                        YTBGui.Line();
                    }

                    if (todos)
                    {
                        foreach (FileDropped file in DragAndDropSystem.Files)
                            file.selected = true;
                    }
                    ImGui.Spacing();

                    ImGui.SeparatorText("SpriteSheet");
                    ImGui.TextDisabled("Nombre");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(260);
                    ImGui.InputText("##SpriteSheetName", ref spriteSheetName, 64);
                    ImGui.PopItemWidth();

                    ImGui.TextDisabled("Carpeta en Assets");
                    ImGui.SameLine();
                    ImGui.PushItemWidth(260);
                    ImGui.InputText("##SpriteSheetFolder", ref spriteSheetFolder, 128);
                    ImGui.PopItemWidth();

                    if (spriteSheetXmlSelected)
                    {
                        YTBGui.Button("Crear SpriteSheet con el xml Seleccionado", () =>
                        {
                            var selectedXml = DragAndDropSystem.Files.FirstOrDefault(x => x.selected && x.Kind == DroppedFileKind.SpriteSheetXml);
                            var selectedImages = DragAndDropSystem.Files.Where(x => x.selected && x.Kind == DroppedFileKind.Image).ToList();
                            if (selectedXml != null)
                            {
                                TryImportSpriteSheetFromXml(selectedXml, selectedImages);
                                DragAndDropSystem.Files.Clear();
                                todos = false;
                            }
                        });
                    }

                    ImGui.Spacing();

                    YTBGui.Button("Crear SpriteSheet", () =>
                    {
                        var selectedImages = DragAndDropSystem.Files.Where(x => x.selected && x.Kind == DroppedFileKind.Image).ToList();
                        if (selectedImages.Count == 0)
                        {
                            SendLog("[SpriteSheet] Selecciona al menos una imagen para crear el atlas.", Color.Yellow);
                            return;
                        }

                        TryCreateSpriteSheetFromImages(selectedImages);
                        DragAndDropSystem.Files.Clear();
                        todos = false;
                    });

                    YTBGui.Continue();

                    // --- AQUÍ ESTÁ EL CAMBIO IMPORTANTE ---
                    YTBGui.Button("Agregar a SpriteSheet existente", () =>
                    {
                        if (!imageSelected)
                        {
                            SendLog("[SpriteSheet] Selecciona imágenes para agregar al atlas existente.", Color.Yellow);
                            return;
                        }

                        xmlExistente = true;
                        ImGui.CloseCurrentPopup(); // Cierra el modal "Archivos Soltados"
                        YTBGui.DispararModal("Seleccionar SpriteSheet"); // Abre el siguiente
                    });

                    if (importSelected)
                    {
                        YTBGui.Continue();
                        YTBGui.Button("Importar archivos a Assets", () =>
                        {
                            var selectedImports = DragAndDropSystem.Files.Where(x => x.selected && x.Kind != DroppedFileKind.Image && x.Kind != DroppedFileKind.SpriteSheetXml).ToList();
                            TryImportAssets(selectedImports);
                            DragAndDropSystem.Files.Clear();
                            todos = false;
                        });
                    }

                    ImGui.SeparatorText("");
                    YTBGui.Button("Cancelar", () =>
                    {
                        DragAndDropSystem.Files.Clear();
                        todos = false;
                        xmlExistente = false;
                    });

                });

                // 3. Manejamos el segundo modal
                if (xmlExistente)
                {
                    YTBGui.AbrirSeccion("Seleccionar SpriteSheet", () =>
                    {
                        foreach (string xml in LoadXmls())
                        {
                            YTBGui.Button(Path.GetFileName(xml), () =>
                            {
                                // Aquí agregaríamos el xml al spritesheet seleccionado
                                string[] images = DragAndDropSystem.Files.Where(x => x.Kind == DroppedFileKind.Image && x.selected).Select(s => s.Name).ToArray();

                                if (images.Length == 0)
                                {
                                    SendLog("[SpriteSheet] Selecciona imágenes para agregar al atlas.", Color.Yellow);
                                    return;
                                }

                                try
                                {
                                    TexturePacker.UpdateAtlas(xml, images);
                                }
                                catch (Exception ex)
                                {
                                    SendLog($"[SpriteSheet][ERROR] {ex.Message}", Color.Red);
                                }
                                xmlExistente = false;
                                DragAndDropSystem.Files.Clear();
                                ImGui.CloseCurrentPopup(); // Cerramos este modal al terminar
                            });
                        }

                        // Botón opcional para volver atrás o cancelar
                        ImGui.Separator();
                        YTBGui.Button("Cancelar", () =>
                        {
                            xmlExistente = false;
                        });

                    });
                }
            }

        }

        private static void GetDropSelectionState(out bool spriteSheetXmlSelected, out bool imageSelected, out bool importSelected)
        {
            spriteSheetXmlSelected = false;
            imageSelected = false;
            importSelected = false;

            foreach (var file in DragAndDropSystem.Files)
            {
                if (!file.selected)
                    continue;

                if (file.Kind == DroppedFileKind.SpriteSheetXml)
                {
                    spriteSheetXmlSelected = true;
                }
                else if (file.Kind == DroppedFileKind.Image)
                {
                    imageSelected = true;
                }
                else
                {
                    importSelected = true;
                }

                if (spriteSheetXmlSelected && imageSelected && importSelected)
                    return;
            }
        }

        /// <summary>
        /// Stores metadata for batched texture previews in the editor UI.
        /// Almacena metadatos para previews de texturas agrupadas en la UI del editor.
        /// </summary>
        public class TextureBatchInfo
        {
            public YTBEntity entidad;
            public string EntityName;
            public string sceneName;
            public float TotalWidth;  // Necesario para calcular UVs
            public float TotalHeight; // Necesario para calcular UVs
            public List<Rectangle> Regions = new();
        }
        // Cambiamos el valor del diccionario a nuestra nueva clase
        Dictionary<IntPtr, TextureBatchInfo> images = new();

        // Variables para rastrear texture region siendo arrastrada
        private string _draggedRegionName = null;
        private IntPtr _draggedTexturePtr = IntPtr.Zero;
        private TextureRegion _draggedRegion;

        public void InitializeShowTextureImages()
        {
#if YTB
            // BUG FIX: Texture preview disabled for DirectX and Vulkan backends
            // This feature causes visual artifacts (screen tearing, white horizontal lines) when
            // binding MonoGame Texture2D objects to ImGui via BindTexture() for entity thumbnails.
            // 
            // Affected backends: DirectX 11/12, Vulkan (all tested versions)
            // Works correctly on: OpenGL (DesktopGL), Android
            // 
            // Hypothesis: The texture binding creates a race condition or buffer corruption between
            // the game's render pipeline and ImGui's render pass on these graphics APIs.
            // 
            // TODO: Investigate proper texture synchronization, consider creating separate texture
            // copies for ImGui, or implement backend-specific workarounds.
            /*
            // Limpiamos la lista anterior para no duplicar si llamas a esto varias veces
            images.Clear();

            YTBGame game = (YTBGame)YTBGame.Instance;
            
            

            foreach (var scene in game.SceneManager.Scenes)
            {
                var manager = scene.EntityManager;

                foreach (var entity in manager.YotsubaEntities)
                {
                    if (!entity.HasComponent(Entity.YTBComponent.Sprite)) continue;

                    ref var textureComponent = ref manager.Sprite2DComponents[entity.Id];

                    // Obtenemos la textura real de MonoGame
                    var texture2D = textureComponent.Texture;
                    
                    IntPtr texturaId = GuiRenderer.BindTexture(texture2D);
                    Rectangle region = textureComponent.SourceRectangle;


                    var escena = GameInfo.Scene.FirstOrDefault(x => x.Name == scene.SceneName);
                    YTBEntity entidad = escena.Entities.FirstOrDefault(f => f.Name == entity.Name);

                    if (!images.TryGetValue(texturaId, out var batchInfo))
                    {
                        batchInfo = new TextureBatchInfo
                        {
                            TotalWidth = texture2D.Width,
                            TotalHeight = texture2D.Height,
                            EntityName = entity.Name,
                            sceneName = scene.SceneName,
                            entidad = entidad
                        };
                        images[texturaId] = batchInfo;
                    }

                    // Agregamos la región a la lista
                    batchInfo.Regions.Add(region);
                }
            }
            */
#endif
        }

        string busqueda = "";
        public void UpdateShowTextureImages()
        {

            YTBGui.AbrirSeccion("Texture Regions", () =>
            {
#if YTB
                foreach (var (regionKey, textureRegion) in YotsubaGraphicsManager.PreloadedTextureRegions)
                {
                    // Bind de la textura para obtener el IntPtr
                    IntPtr texturaId = GuiRenderer.BindTexture(textureRegion.Texture);
                    
                    // Tamaño del thumbnail
                    float thumbnailSize = 100f;
                    float aspectRatio = (float)textureRegion.Width / textureRegion.Height;
                    float displayWidth = thumbnailSize;
                    float displayHeight = thumbnailSize / aspectRatio;
                    
                    // Calcular UVs para mostrar solo la región específica del atlas
                    var uv0 = new Num.Vector2(
                        (float)textureRegion.SourceRectangle.X / textureRegion.Texture.Width,
                        (float)textureRegion.SourceRectangle.Y / textureRegion.Texture.Height
                    );
                    
                    var uv1 = new Num.Vector2(
                        (float)(textureRegion.SourceRectangle.X + textureRegion.SourceRectangle.Width) / textureRegion.Texture.Width,
                        (float)(textureRegion.SourceRectangle.Y + textureRegion.SourceRectangle.Height) / textureRegion.Texture.Height
                    );
                    
                    // Mostrar la región como botón
                    if (ImGui.ImageButton($"##region_{regionKey}", texturaId, new Num.Vector2(displayWidth, displayHeight), uv0, uv1))
                    {
                        // Al hacer clic, activar modo "siguiendo mouse"
                        _draggedRegionName = regionKey;
                        _draggedTexturePtr = texturaId;
                        _draggedRegion = textureRegion;
                    }
                    
                    // Tooltip con el nombre de la región al pasar el mouse
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip($"{regionKey}\n{textureRegion.Width}x{textureRegion.Height}");
                    }
                    
                    // Continuar en la misma línea para mostrar varias regiones horizontalmente
                    ImGui.SameLine();
                }
                
                // Salto de línea final
                ImGui.NewLine();
#endif
            });
            
            // Renderizar texture region siguiendo el mouse (si hay una activa)
#if YTB
            if (_draggedRegion.Texture != null && _draggedTexturePtr != IntPtr.Zero)
            {
                // Obtener posición actual del mouse
                var mousePos = ImGui.GetMousePos();
                
                // Tamaño para la región arrastrada
                float draggedSize = 100f;
                float aspectRatio = (float)_draggedRegion.Width / _draggedRegion.Height;
                float displayWidth = draggedSize;
                float displayHeight = draggedSize / aspectRatio;
                
                // Centrar la región en el cursor
                var drawPos = new Num.Vector2(
                    mousePos.X - displayWidth / 2f, 
                    mousePos.Y - displayHeight / 2f
                );
                
                // Calcular UVs para la región arrastrada
                var uv0 = new Num.Vector2(
                    (float)_draggedRegion.SourceRectangle.X / _draggedRegion.Texture.Width,
                    (float)_draggedRegion.SourceRectangle.Y / _draggedRegion.Texture.Height
                );
                
                var uv1 = new Num.Vector2(
                    (float)(_draggedRegion.SourceRectangle.X + _draggedRegion.SourceRectangle.Width) / _draggedRegion.Texture.Width,
                    (float)(_draggedRegion.SourceRectangle.Y + _draggedRegion.SourceRectangle.Height) / _draggedRegion.Texture.Height
                );
                
                // Usar una ventana invisible para renderizar la región encima de todo
                ImGui.SetNextWindowPos(drawPos);
                ImGui.SetNextWindowSize(new Num.Vector2(displayWidth, displayHeight));
                ImGui.Begin("##DraggedTextureRegion", 
                    ImGuiWindowFlags.NoTitleBar | 
                    ImGuiWindowFlags.NoResize | 
                    ImGuiWindowFlags.NoMove | 
                    ImGuiWindowFlags.NoScrollbar | 
                    ImGuiWindowFlags.NoScrollWithMouse |
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoBackground |
                    ImGuiWindowFlags.NoSavedSettings |
                    ImGuiWindowFlags.NoInputs);
                
                // Renderizar la imagen con UVs de la región
                ImGui.Image(_draggedTexturePtr, new Num.Vector2(displayWidth, displayHeight), uv0, uv1);
                
                ImGui.End();
                
                // Detectar clic para soltar la región
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    _draggedRegionName = null;
                    _draggedTexturePtr = IntPtr.Zero;
                    _draggedRegion = default;
                }
            }
#endif
           
            
            //COMENT PART
            // BUG FIX: Texture preview disabled for DirectX/Vulkan
            // This feature causes visual artifacts on DirectX and Vulkan backends.
            // The entity preview images section is temporarily disabled.
            // TODO: Re-enable when texture binding is fixed for all backends.
            /*
            YTBGui.AbrirSeccion("Entidades Visibles", () =>
            {
                ImGui.InputText("Buscar entidad", ref busqueda, 50);
            foreach (var kvp in images)
            {
                if (!String.IsNullOrEmpty(busqueda) && !kvp.Value.EntityName.Contains(busqueda, StringComparison.OrdinalIgnoreCase)) continue;
                // 1. Extraemos la info completa para tener acceso al TotalWidth/TotalHeight
                var batchInfo = kvp.Value;
                IntPtr texturaId = kvp.Key;

                foreach (var rect in batchInfo.Regions) // 'rect' es tu 'textureZone'
                {

                        

                        // Ajustar el tamaño de visualización
                        float aspectRatio = (float)rect.Width / rect.Height;
                        float displayWidth = 300;
                        float displayHeight = displayWidth / aspectRatio;
                        ImGui.BeginChild(batchInfo.EntityName, new Num.Vector2(displayWidth + 10, displayHeight + 10));


                    var uv0 = new Num.Vector2(
                        (float)rect.X / batchInfo.TotalWidth,    // <--- CORREGIDO
                        (float)rect.Y / batchInfo.TotalHeight    // <--- CORREGIDO
                    );

                    var uv1 = new Num.Vector2(
                        (float)(rect.X + rect.Width) / batchInfo.TotalWidth,   // <--- CORREGIDO
                        (float)(rect.Y + rect.Height) / batchInfo.TotalHeight  // <--- CORREGIDO
                    );

                    if (ImGui.ImageButton($"{batchInfo.EntityName} ({batchInfo.sceneName})", texturaId, new Num.Vector2(displayWidth, displayHeight), uv0, uv1))
                    {
                        SelectEntity(batchInfo.entidad);

                    }

                        // Mostrar datos para depurar
                        if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip($"Region: {rect}\nAtlas: {batchInfo.TotalWidth}x{batchInfo.TotalHeight}");

                    }

                       

                        ImGui.EndChild();
                        YTBGui.Continue();

                        ImGui.BeginChild($"2{batchInfo.EntityName}", new Num.Vector2(displayWidth + 10, displayHeight + 10));
                        ImGui.SeparatorText($"{batchInfo.EntityName}");

                        YTBGui.Text($"SCENE: ", true);
                        YTBGui.Continue();
                        YTBGui.Text($"{batchInfo.sceneName}");


                        //ImGui.SeparatorText("COMPONENTES ");

                        ImGui.EndChild();

                        //ImGui.Separator();
                    }
                }
            });
            */
        }

        public List<string> LoadXmls()
        {
            List<string> xmls = new List<string>();
            string path = YTBFileToGameData.ContentManager.RootDirectory;
            if (!Directory.Exists(path)) return new List<string>();

            var xmlFiles = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
            foreach (var file in xmlFiles)
            {
                xmls.Add(file);
            }

            return xmls;
        }

        private static readonly string[] SpriteSheetImageExtensions = { ".png", ".jpg", ".jpeg" };

        private void TryCreateSpriteSheetFromImages(List<FileDropped> selectedImages)
        {
            string sanitizedName = SanitizeFileName(spriteSheetName);
            if (string.IsNullOrWhiteSpace(sanitizedName))
            {
                SendLog("[SpriteSheet] Define un nombre válido para el SpriteSheet.", Color.Yellow);
                return;
            }

            string assetsRoot = YTBGlobalState.DevelopmentAssetsPath;
            if (string.IsNullOrWhiteSpace(assetsRoot))
            {
                SendLog("[SpriteSheet] Assets path no configurado.", Color.Red);
                return;
            }

            string normalizedFolder = NormalizeRelativeAssetPath(spriteSheetFolder);
            if (string.IsNullOrWhiteSpace(normalizedFolder) && !string.IsNullOrWhiteSpace(spriteSheetFolder))
            {
                SendLog("[SpriteSheet] Carpeta inválida para el SpriteSheet.", Color.Yellow);
                return;
            }
            string outputDirectory = BuildAssetsDirectory(assetsRoot, normalizedFolder);
            if (string.IsNullOrEmpty(outputDirectory))
            {
                SendLog("[SpriteSheet] Carpeta inválida para el SpriteSheet.", Color.Yellow);
                return;
            }

            Directory.CreateDirectory(outputDirectory);

            string pngPath = Path.Combine(outputDirectory, sanitizedName + ".png");
            string xmlPath = Path.Combine(outputDirectory, sanitizedName + ".xml");

            if (File.Exists(pngPath) || File.Exists(xmlPath))
            {
                SendLog("[SpriteSheet] Ya existe un atlas con ese nombre en la carpeta indicada.", Color.Yellow);
                return;
            }

            try
            {
                string[] imagePaths = selectedImages.Select(x => x.Name).ToArray();
                List<SpriteInfo> sprites = new List<SpriteInfo>(TexturePacker.GetImages(imagePaths));
                TexturePacker.CalculatePositions(sprites, 2048, out int finalW, out int finalH);
                TexturePacker.GenerateAtlas(sprites, finalW, finalH, pngPath);

                string relativeImagePath = BuildRelativeAssetPath(normalizedFolder, sanitizedName);
                TexturePacker.ExportXML(sprites, "", xmlPath, relativeImagePath);

                SendLog($"[SpriteSheet] Atlas creado: {relativeImagePath}", Color.Green);
                YTBContentBuilder.Rebuild();
            }
            catch (PlatformNotSupportedException ex)
            {
                SendLog($"[SpriteSheet][ERROR] {ex.Message}", Color.Red);
            }
            catch (Exception ex)
            {
                SendLog($"[SpriteSheet][ERROR] {ex.Message}", Color.Red);
            }
        }

        private void TryImportSpriteSheetFromXml(FileDropped xmlFile, List<FileDropped> selectedImages)
        {
            if (xmlFile == null || string.IsNullOrWhiteSpace(xmlFile.Name))
            {
                SendLog("[SpriteSheet] No se encontró el XML seleccionado.", Color.Yellow);
                return;
            }

            string assetsRoot = YTBGlobalState.DevelopmentAssetsPath;
            if (string.IsNullOrWhiteSpace(assetsRoot))
            {
                SendLog("[SpriteSheet] Assets path no configurado.", Color.Red);
                return;
            }

            string normalizedFolder = NormalizeRelativeAssetPath(spriteSheetFolder);
            if (string.IsNullOrWhiteSpace(normalizedFolder) && !string.IsNullOrWhiteSpace(spriteSheetFolder))
            {
                SendLog("[SpriteSheet] Carpeta inválida para importar el XML.", Color.Yellow);
                return;
            }
            string outputDirectory = BuildAssetsDirectory(assetsRoot, normalizedFolder);
            if (string.IsNullOrEmpty(outputDirectory))
            {
                SendLog("[SpriteSheet] Carpeta inválida para importar el XML.", Color.Yellow);
                return;
            }

            Directory.CreateDirectory(outputDirectory);

            try
            {
                XDocument document = XDocument.Load(xmlFile.Name);
                XElement root = document.Root;
                if (root == null)
                {
                    SendLog("[SpriteSheet] XML inválido.", Color.Red);
                    return;
                }

                XAttribute imageAttr = root.Attribute("imagepath");
                if (imageAttr == null || string.IsNullOrWhiteSpace(imageAttr.Value))
                {
                    SendLog("[SpriteSheet] El XML no tiene imagepath válido.", Color.Red);
                    return;
                }

                string sourceImagePath = ResolveSpriteSheetImage(xmlFile.Name, imageAttr.Value, selectedImages);
                if (string.IsNullOrWhiteSpace(sourceImagePath))
                {
                    SendLog("[SpriteSheet] No se encontró la imagen referenciada por el XML.", Color.Red);
                    return;
                }

                string outputImageName = Path.GetFileName(sourceImagePath);
                string outputImagePath = Path.Combine(outputDirectory, outputImageName);
                if (File.Exists(outputImagePath))
                {
                    SendLog("[SpriteSheet] La imagen ya existe en la carpeta destino.", Color.Yellow);
                    return;
                }
                string outputXmlName = Path.GetFileName(xmlFile.Name);
                string outputXmlPath = Path.Combine(outputDirectory, outputXmlName);
                if (File.Exists(outputXmlPath))
                {
                    SendLog("[SpriteSheet] El XML ya existe en la carpeta destino.", Color.Yellow);
                    return;
                }

                File.Copy(sourceImagePath, outputImagePath, false);

                string relativeImagePath = BuildRelativeAssetPath(normalizedFolder, Path.GetFileNameWithoutExtension(outputImageName));
                imageAttr.Value = relativeImagePath;
                document.Save(outputXmlPath);

                SendLog($"[SpriteSheet] XML importado: {relativeImagePath}", Color.Green);
                YTBContentBuilder.Rebuild();
            }
            catch (Exception ex)
            {
                SendLog($"[SpriteSheet][ERROR] {ex.Message}", Color.Red);
            }
        }

        private void TryImportAssets(List<FileDropped> selectedImports)
        {
            if (selectedImports == null || selectedImports.Count == 0)
            {
                SendLog("[Assets] Selecciona archivos para importar.", Color.Yellow);
                return;
            }

            string assetsRoot = YTBGlobalState.DevelopmentAssetsPath;
            if (string.IsNullOrWhiteSpace(assetsRoot))
            {
                SendLog("[Assets] Assets path no configurado.", Color.Red);
                return;
            }

            string scriptsRoot = GetScriptsRoot(assetsRoot);

            foreach (var file in selectedImports)
            {
                string fileName = Path.GetFileName(file.Name);
                if (string.IsNullOrWhiteSpace(fileName))
                    continue;

                string destinationRoot = file.Kind == DroppedFileKind.Script ? scriptsRoot : assetsRoot;
                string destinationFolder = GetDestinationFolder(file.Kind);
                string destinationDirectory = string.IsNullOrWhiteSpace(destinationFolder)
                    ? destinationRoot
                    : Path.Combine(destinationRoot, destinationFolder);

                Directory.CreateDirectory(destinationDirectory);

                string destinationPath = Path.Combine(destinationDirectory, fileName);
                if (File.Exists(destinationPath))
                {
                    SendLog($"[Assets] Ya existe: {destinationPath}", Color.Yellow);
                    continue;
                }

                try
                {
                    File.Copy(file.Name, destinationPath, false);
                    SendLog($"[Assets] Importado: {destinationPath}", Color.Green);
                }
                catch (Exception ex)
                {
                    SendLog($"[Assets][ERROR] {ex.Message}", Color.Red);
                }
            }

            YTBContentBuilder.Rebuild();
        }

        private static string ResolveSpriteSheetImage(string xmlPath, string imagePathHint, List<FileDropped> selectedImages)
        {
            string imageBaseName = Path.GetFileNameWithoutExtension(imagePathHint);
            if (!string.IsNullOrWhiteSpace(imageBaseName))
            {
                foreach (var image in selectedImages)
                {
                    string candidateName = Path.GetFileNameWithoutExtension(image.Name);
                    if (string.Equals(candidateName, imageBaseName, StringComparison.OrdinalIgnoreCase))
                        return image.Name;
                }
            }

            string xmlDirectory = Path.GetDirectoryName(xmlPath) ?? string.Empty;
            string normalizedHint = imagePathHint.Replace('\\', '/');
            bool hasExtension = Path.HasExtension(normalizedHint);

            if (hasExtension)
            {
                string candidatePath = Path.IsPathRooted(normalizedHint)
                    ? normalizedHint
                    : Path.Combine(xmlDirectory, normalizedHint);

                if (File.Exists(candidatePath))
                    return candidatePath;
            }

            string hintWithoutExtension = Path.ChangeExtension(normalizedHint, null);
            foreach (string extension in SpriteSheetImageExtensions)
            {
                string candidate = $"{hintWithoutExtension}{extension}";
                string candidatePath = Path.IsPathRooted(candidate)
                    ? candidate
                    : Path.Combine(xmlDirectory, candidate);

                if (File.Exists(candidatePath))
                    return candidatePath;
            }

            return string.Empty;
        }

        private static string NormalizeRelativeAssetPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            string normalized = path.Replace('\\', '/').Trim();
            normalized = normalized.TrimStart('/');

            if (normalized.Contains("..", StringComparison.Ordinal))
                return string.Empty;

            return normalized;
        }

        private static string BuildAssetsDirectory(string assetsRoot, string normalizedFolder)
        {
            if (string.IsNullOrWhiteSpace(assetsRoot))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedFolder))
                return assetsRoot;

            return Path.Combine(assetsRoot, normalizedFolder);
        }

        private static string BuildRelativeAssetPath(string normalizedFolder, string fileName)
        {
            string combined = string.IsNullOrWhiteSpace(normalizedFolder)
                ? fileName
                : Path.Combine(normalizedFolder, fileName);

            return combined.Replace('\\', '/');
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = name;

            foreach (var ch in invalidChars)
            {
                sanitized = sanitized.Replace(ch, '_');
            }

            return sanitized.Trim();
        }

        private static string GetScriptsRoot(string assetsRoot)
        {
            string baseDir = Path.GetDirectoryName(assetsRoot);
            return string.IsNullOrWhiteSpace(baseDir) ? assetsRoot : baseDir;
        }

        private static string GetDestinationFolder(DroppedFileKind kind)
        {
            return kind switch
            {
                DroppedFileKind.Audio => "Audio",
                DroppedFileKind.Font => "Fonts",
                DroppedFileKind.Script => "Scripts",
                DroppedFileKind.Shader => "Shaders",
                DroppedFileKind.Config => "GameConfig",
                _ => string.Empty
            };
        }

        private void RenderizarCapaBaseEngine()
        {
#if YTB
            if (pauseSystem)
                return;
#endif
            // Usamos el mismo código de ventana raíz pero delegando la barra de menú
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Num.Vector4(0, 0, 0, 0));

            ImGuiWindowFlags windowFlags =
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.MenuBar;

            ImGui.Begin("DockSpaceRoot", windowFlags);
            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor();

            // Barra de menú delegada
            if (ImGui.BeginMenuBar())
            {
                _ = _menuBar.RenderMenuBarAsync();
                ImGui.EndMenuBar();
            }




            uint dockspaceId = ImGui.GetID("MainDockspace");
            ImGui.DockSpace(dockspaceId, Num.Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);

            ImGui.End();
        }



        public static D.Color Vector4ToColor(Vector4 v)
        {
            return D.Color.FromArgb(
                (int)(v.W * 255f),   // Alpha
                (int)(v.X * 255f),   // Red
                (int)(v.Y * 255f),   // Green
                (int)(v.Z * 255f)    // Blue
            );
        }

        public static void SendLog(string message, Color Color)
        {
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {

                if (Color == Color.Red)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (Color == Color.White)
                    Console.ForegroundColor = ConsoleColor.White;
                else if (Color == Color.Yellow)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (Color == Color.Green)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(message);

                if (Color == Color.Red)
                {
                    Messages.Add((Color, message));
                    return;
                }

                if (GameWontRun.GameWontRunByException)
                {
                    Messages.Add((Color.Yellow, message));
                    return;
                }

                Messages.Add((Color, message));
            }
        }

        public static void SendLog(string message)
        {
            SendLog(message, Color.White);
        }
    }
}