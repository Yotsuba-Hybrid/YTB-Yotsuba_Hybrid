using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Exceptions;
using YotsubaEngine.HighestPerformanceTypes;
using YotsubaEngine.Templates;
using YotsubaEngine.YTB_Toolkit;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Scene Manager sencillo y estable: lista de escenas con sus entidades,
    /// botones para crear/eliminar y confirmaciones modales.
    /// </summary>
    public class SceneManagerUI
    {
        private YTBGameInfo _gameInfo;
        private readonly Action<YTBEntity> _onSelectEntity;
        private readonly Action<YTBScene> _onSelectScene;
        private string _nuevaEscenaNombre = string.Empty;
        private string _nuevoNombreEscena = string.Empty;
        private YTBScene? _sceneToRename = null;
        private YTB<(string, string)> SceneAndEntityForDelete = [];
        
        // --- Variables para mover/duplicar entidades ---
        private YTBEntity? _entityToMoveOrDuplicate = null;
        private YTBScene? _sourceSceneForEntity = null;
        private bool _isMovingEntity = false; // true = mover, false = duplicar
        private string _moveOrDuplicatePopupTitle = string.Empty;
        internal SceneManagerUI(YTBGameInfo gameInfo, Action<YTBEntity> onSelectEntity, Action<YTBScene> onSelectScene)
        {
            _gameInfo = gameInfo;
            _onSelectEntity = onSelectEntity;
            _onSelectScene = onSelectScene;

            EventManager.Instance.Subscribe<OnGameFileWasReplaceByHistory>(GameFileWasReplace);

          
        }

   
        // -------------------------------------------------------------------------------
        // VARIABLES DE CLASE (Definelas fuera de Render)
        // -------------------------------------------------------------------------------
        private string _sceneToDelete = "";
        private bool _triggerDeleteScene = false;

        // Variables para Renombrar (NUEVAS)
        private bool _triggerRenameScene = false;

        bool deleteEntities = false;
        string deleteName = "";

        public void Render()
        {
            ImGui.Begin("Scene Manager");

            // --- encabezado ---
            ImGui.TextColored(new Num.Vector4(0.45f, 0.8f, 1f, 1f), "Escenas del proyecto");
            ImGui.Separator();
            ImGui.Checkbox("Eliminar Entidad", ref deleteEntities);

            // Lógica de eliminación masiva de entidades
            if (SceneAndEntityForDelete.Count > 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, ImGuiThemeColors.ERROR);

                if (ImGui.Button($"Eliminar {SceneAndEntityForDelete.Count}"))
                {
                    foreach (var (sceneName, entityName) in SceneAndEntityForDelete)
                    {
                        var ytbScene = _gameInfo.Scene.FirstOrDefault(s => s.Name == sceneName);
                        if (ytbScene != null)
                        {
                            var entityToRemove = ytbScene.Entities.FirstOrDefault(e => e.Name == entityName);
                            if (entityToRemove != null)
                            {
                                ytbScene.Entities.Remove(entityToRemove);
                            }
                        }
                    }
                    WriteYTBFile.EditYTBGameFile(_gameInfo);
                    SceneAndEntityForDelete.Clear();
                }

                ImGui.PopStyleColor(1);
            }

            ImGui.Separator();
            ImGui.Spacing();

            int id = 0;
            string[] options = ["Play scene", "Preview", "Add entity", "Rename scene", "Delete scene"];

            // --- recorrer escenas ---
            foreach (var scene in _gameInfo.Scene.ToImmutableArray())
            {
                if (scene.Entities == null) scene.Entities = [];
                bool open = ImGui.CollapsingHeader($"{scene.Name} ({scene.EntitiesCount} entidades)", ImGuiTreeNodeFlags.DefaultOpen);

                if (open)
                {
                    ImGui.Indent(10);

                    // lista de entidades
                    if (scene.Entities.Any())
                    {
                        foreach (var entity in scene.Entities.ToImmutableArray())
                        {
                            if (string.IsNullOrEmpty(entity.Name)) entity.Name = "Sin Nombre - ID" + Guid.NewGuid();

                            ImGui.PushID(id);

                            bool marcada = SceneAndEntityForDelete.Contains((scene.Name, entity.Name));
                            if (marcada) ImGui.PushStyleColor((int)ImGuiCol.Text, ImGuiThemeColors.ERROR);

                            if (ImGui.Selectable(entity.Name))
                            {
                                if (deleteEntities)
                                {
                                    if (!SceneAndEntityForDelete.Contains((scene.Name, entity.Name)))
                                        SceneAndEntityForDelete.Add((scene.Name, entity.Name));
                                    else
                                        SceneAndEntityForDelete.Remove((scene.Name, entity.Name));
                                }
                                else
                                {
                                    _onSelectEntity?.Invoke(entity);
                                    _onSelectScene?.Invoke(scene);
                                }
                            }

                            if (marcada) ImGui.PopStyleColor(1);

                            // Menú contextual entidad
                            if (ImGui.BeginPopupContextItem($"EntityContextMenu_{scene.Name}_{entity.Name}"))
                            {
                                if (ImGui.MenuItem("Mover a otra escena..."))
                                {
                                    _entityToMoveOrDuplicate = entity;
                                    _sourceSceneForEntity = scene;
                                    _isMovingEntity = true;
                                    _moveOrDuplicatePopupTitle = $"Mover entidad '{entity.Name}'";
                                    ImGui.OpenPopup("MoveOrDuplicateEntityPopup");
                                }

                                if (ImGui.MenuItem("Duplicar en otra escena..."))
                                {
                                    _entityToMoveOrDuplicate = entity;
                                    _sourceSceneForEntity = scene;
                                    _isMovingEntity = false;
                                    _moveOrDuplicatePopupTitle = $"Duplicar entidad '{entity.Name}'";
                                    ImGui.OpenPopup("MoveOrDuplicateEntityPopup");
                                }
                                ImGui.EndPopup();
                            }

                            // Modal eliminar entidad individual
                            if (ImGui.BeginPopupModal($"Confirma la eliminación de la entidad (irreversible)", ImGuiWindowFlags.AlwaysAutoResize))
                            {
                                ImGui.InputText($"Escribe el nombre de la entidad ({entity.Name})", ref deleteName, 100);
                                bool eliminarPressed = false;
                                if (deleteName == entity.Name) eliminarPressed = ImGui.Button("Eliminar");

                                if (eliminarPressed)
                                {
                                    var ytbScene = _gameInfo.Scene.FirstOrDefault(s => s.Entities.Contains(entity));
                                    ytbScene.Entities.Remove(ytbScene.Entities.FirstOrDefault(f => f.Name == entity.Name));
                                    WriteYTBFile.EditYTBGameFile(_gameInfo);
                                    deleteName = "";
                                    ImGui.CloseCurrentPopup();
                                }

                                if (ImGui.Button("Cerrar")) ImGui.CloseCurrentPopup();
                                ImGui.EndPopup();
                            }

                            ImGui.PopID();
                            id++;
                        }
                    }
                    else
                    {
                        ImGui.TextDisabled("Sin entidades en esta escena.");
                    }

                    ImGui.Spacing();

                  

                    // --- MENU DE OPCIONES ---
                    if (ImGui.BeginMenu("Options ##" + scene.Name))
                    {
                        foreach (string option in options)
                        {
                            if (ImGui.MenuItem(option))
                            {
                                switch (option)
                                {
                                    case "Play scene":
                                        SystemCall.ChangeScene(scene.Name);
                                        break;
                                    case "Preview":
                                        SystemCall.ChangeScenePreview(scene.Name);
                                        break;
                                    case "Add entity":
                                        CreateNewEntity(scene);
                                        break;
                                    case "Rename scene":
                                        // CORRECCIÓN AQUÍ TAMBIÉN:
                                        _sceneToRename = scene;
                                        _nuevoNombreEscena = scene.Name;
                                        _triggerRenameScene = true; // Activar bandera
                                        break;
                                    case "Delete scene":
                                        _sceneToDelete = scene.Name;
                                        _triggerDeleteScene = true;
                                        break;
                                }
                            }
                        }
                        ImGui.EndMenu();
                    }

                    ImGui.Unindent(10);
                }
            } // Fin del foreach de escenas

            // =========================================================================================
            // GESTIÓN DE APERTURA DE POPUPS (FUERA DEL BUCLE)
            // =========================================================================================

            // 1. Trigger para Eliminar
            if (_triggerDeleteScene)
            {
                ImGui.OpenPopup("Confirmar Eliminar Escena");
                _triggerDeleteScene = false;
            }

            // 2. Trigger para Renombrar (NUEVO)
            if (_triggerRenameScene)
            {
                ImGui.OpenPopup("Renombrar Escena");
                _triggerRenameScene = false;
            }


            // =========================================================================================
            // DEFINICIÓN DE MODALES
            // =========================================================================================

            // Modal Eliminar Escena
            if (ImGui.BeginPopupModal("Confirmar Eliminar Escena", ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.TextColored(new Num.Vector4(1f, 0.6f, 0.6f, 1f), "¿Seguro que deseas eliminar esta escena?");
                ImGui.Text(_sceneToDelete);
                ImGui.Separator();

                if (ImGui.Button("Sí, eliminar", new Num.Vector2(110, 0)))
                {
                    var sceneObj = _gameInfo.Scene.FirstOrDefault(x => x.Name == _sceneToDelete);
                    if (sceneObj != null) DeleteScene(sceneObj);
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancelar", new Num.Vector2(110, 0))) ImGui.CloseCurrentPopup();

                ImGui.EndPopup();
            }

            // Modal Renombrar Escena
            if (ImGui.BeginPopupModal("Renombrar Escena", ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("Nuevo nombre de la escena:");
                ImGui.InputText("##nuevoNombreEscena", ref _nuevoNombreEscena, 128);
                ImGui.Separator();

                bool nombreValido = true;
                string mensajeError = string.Empty;

                if (string.IsNullOrWhiteSpace(_nuevoNombreEscena))
                {
                    nombreValido = false;
                    mensajeError = "El nombre no puede estar vacío.";
                }
                else if (_sceneToRename != null && _nuevoNombreEscena != _sceneToRename.Name)
                {
                    if (_gameInfo.Scene.Any(s => s.Name == _nuevoNombreEscena))
                    {
                        nombreValido = false;
                        mensajeError = "Ya existe una escena con ese nombre.";
                    }
                }

                if (!nombreValido) ImGui.TextColored(new Num.Vector4(1f, 0.4f, 0.4f, 1f), mensajeError);

                ImGui.Spacing();

                if (!nombreValido) ImGui.BeginDisabled();

                if (ImGui.Button("Renombrar", new Num.Vector2(100, 0)))
                {
                    if (_sceneToRename != null && nombreValido)
                    {
                        RenameScene(_sceneToRename, _nuevoNombreEscena);
                        _sceneToRename = null;
                        _nuevoNombreEscena = string.Empty;
                    }
                    ImGui.CloseCurrentPopup();
                }

                if (!nombreValido) ImGui.EndDisabled();

                ImGui.SameLine();
                if (ImGui.Button("Cancelar", new Num.Vector2(100, 0)))
                {
                    _sceneToRename = null;
                    _nuevoNombreEscena = string.Empty;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            // Modal Nueva Escena
            if (ImGui.BeginPopupModal("Nueva Escena", ImGuiWindowFlags.AlwaysAutoResize))
            {
                // ... (Tu código de Nueva escena tal cual) ...
                ImGui.Text("Nombre de la nueva escena:");
                ImGui.InputText("##nombreEscena", ref _nuevaEscenaNombre, 128);
                ImGui.Separator();
                if (ImGui.Button("Crear", new Num.Vector2(100, 0)))
                {
                    if (!string.IsNullOrWhiteSpace(_nuevaEscenaNombre))
                    {
                        CreateScene(_nuevaEscenaNombre);
                        _nuevaEscenaNombre = string.Empty;
                    }
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancelar", new Num.Vector2(100, 0)))
                {
                    _nuevaEscenaNombre = string.Empty;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            // --- crear nueva escena (botón disparador) ---
            ImGui.Separator();
            ImGui.Spacing();
            if (ImGui.Button("Nueva escena", new Num.Vector2(ImGui.GetContentRegionAvail().X, 0)))
            {
                ImGui.OpenPopup("Nueva Escena");
            }

            // --- Popup para mover/duplicar entidad ---
            if (ImGui.BeginPopupModal("MoveOrDuplicateEntityPopup", ImGuiWindowFlags.AlwaysAutoResize))
            {
                // ... (Tu código de mover entidad tal cual) ...
                ImGui.TextColored(new Num.Vector4(0.45f, 0.8f, 1f, 1f), _moveOrDuplicatePopupTitle);
                ImGui.Separator();
                // ...
                if (ImGui.Button("Cancelar", new Num.Vector2(250, 0))) ImGui.CloseCurrentPopup();
                // ...
                ImGui.EndPopup();
            }

            ImGui.End();
        }


        // --- operaciones básicas ---
        private void CreateScene(string name)
        {
            var scene = new YTBScene { Name = name, Entities = new List<YTBEntity>() };
            
            // Crear entidad MainCamera por defecto
            var mainCamera = EntityYTBXmlTemplate.GenerateNew();
            mainCamera.Name = "MainCamera";
            
            // Configurar TransformComponent con posición inicial en origen
            var transformComponent = mainCamera.Components.FirstOrDefault(c => c.ComponentName == "TransformComponent");
            if (transformComponent != null)
            {
                var positionProp = transformComponent.Propiedades.FirstOrDefault(p => p.Item1 == "Position");
                if (positionProp != null)
                {
                    int index = transformComponent.Propiedades.IndexOf(positionProp);
                    transformComponent.Propiedades[index] = new Tuple<string, string>("Position", "0,0,0");
                }
            }
            
            // Configurar CameraComponent con valores válidos
            int cameraComponentIndex = mainCamera.Components.FindIndex(x => x.ComponentName == "CameraComponent");
            if (cameraComponentIndex >= 0)
            {
                // Reemplazar el componente vacío con un template válido
                mainCamera.Components[cameraComponentIndex] = EntityYTBXmlTemplate.CameraTemplate();
                
                // Configurar EntityName para que la cámara siga a la entidad MainCamera
                var entityNameIndex = mainCamera.Components[cameraComponentIndex].Propiedades.FindIndex(x => x.Item1 == "EntityName");
                if (entityNameIndex >= 0)
                {
                    mainCamera.Components[cameraComponentIndex].Propiedades[entityNameIndex] = new Tuple<string, string>("EntityName", "MainCamera");
                }
                else
                {
                    mainCamera.Components[cameraComponentIndex].Propiedades.Add(new Tuple<string, string>("EntityName", "MainCamera"));
                }
            }
            
            // Agregar la cámara a la escena
            scene.Entities.Add(mainCamera);
            
            _gameInfo.Scene.Add(scene);
            WriteYTBFile.EditYTBGameFile(_gameInfo);

            GameWontRun.RemoveError(GameWontRun.YTBErrors.GameWithoutScenes);

		}

        private void DeleteScene(YTBScene scene)
        {
            _gameInfo.Scene.Remove(scene);
            WriteYTBFile.EditYTBGameFile(_gameInfo);
        }

        private void RenameScene(YTBScene scene, string newName)
        {
            scene.Name = newName;
            WriteYTBFile.EditYTBGameFile(_gameInfo);
        }

        private void CreateNewEntity(YTBScene scene)
        {
            var entity = EntityYTBXmlTemplate.GenerateNew();
            entity.Name = $"Entidad_{scene.EntitiesCount + 1}";
            scene.Entities.Add(entity);
            WriteYTBFile.EditYTBGameFile(_gameInfo);
        }

        private void GameFileWasReplace(OnGameFileWasReplaceByHistory history)
        {
            Task<YTBGameInfo> newGameInfo = ReadYTBFile.ReadYTBGameFile();
            Task.WaitAll(newGameInfo);
            _gameInfo = newGameInfo.Result;
        }

        // ================================================================================
        // Métodos para mover y duplicar entidades entre escenas
        // ================================================================================

        /// <summary>
        /// Mueve una entidad de una escena origen a una escena destino.
        /// Valida que no sea la única cámara principal antes de mover.
        /// </summary>
        private void MoveEntityToScene(YTBEntity entity, YTBScene sourceScene, YTBScene targetScene)
        {
            // Validación: no mover la única entidad con CameraComponent si es la cámara principal
            if (IsMainCamera(entity, sourceScene))
            {
                // Contar cuántas entidades con CameraComponent hay en la escena origen
                int cameraCount = CountCamerasInScene(sourceScene);
                
                if (cameraCount <= 1)
                {
                    // No permitir mover la única cámara
                    Console.WriteLine($"[SceneManagerUI] No se puede mover la única cámara principal de la escena '{sourceScene.Name}'.");
                    return;
                }
            }
            
            // Generar nombre único en la escena destino si ya existe
            string newName = GenerateUniqueName(entity.Name, targetScene);
            
            // Remover de la escena origen
            sourceScene.Entities.Remove(entity);
            
            // Cambiar el nombre si fue necesario
            if (newName != entity.Name)
            {
                entity.Name = newName;
            }
            
            // Agregar a la escena destino
            targetScene.Entities.Add(entity);
            
            // Guardar cambios
            WriteYTBFile.EditYTBGameFile(_gameInfo);
            
            Console.WriteLine($"[SceneManagerUI] Entidad '{entity.Name}' movida de '{sourceScene.Name}' a '{targetScene.Name}'.");
        }

        /// <summary>
        /// Duplica una entidad en la escena destino con un nombre único.
        /// Copia profunda de todos los componentes de forma compatible con AOT.
        /// </summary>
        private void DuplicateEntityToScene(YTBEntity entity, YTBScene targetScene)
        {
            // Clonar la entidad de forma segura (AOT-compatible)
            YTBEntity clonedEntity = CloneEntity(entity);
            
            // Generar nombre único
            string baseName = $"{entity.Name}_Copy";
            clonedEntity.Name = GenerateUniqueName(baseName, targetScene);
            
            // Agregar la entidad clonada a la escena destino
            targetScene.Entities.Add(clonedEntity);
            
            // Guardar cambios
            WriteYTBFile.EditYTBGameFile(_gameInfo);
            
            Console.WriteLine($"[SceneManagerUI] Entidad '{entity.Name}' duplicada en '{targetScene.Name}' como '{clonedEntity.Name}'.");
        }

        /// <summary>
        /// Clona una entidad de forma manual (sin reflexión) para compatibilidad con AOT.
        /// Copia profunda de todos los componentes y sus propiedades.
        /// </summary>
        private YTBEntity CloneEntity(YTBEntity entity)
        {
            var clonedEntity = new YTBEntity
            {
                Name = entity.Name,
                Components = new List<YTBComponents>()
            };
            
            // Clonar cada componente
            foreach (var component in entity.Components)
            {
                var clonedComponent = new YTBComponents
                {
                    ComponentName = component.ComponentName,
                    Propiedades = new List<Tuple<string, string>>()
                };
                
                // Clonar cada propiedad (creamos nuevas tuplas para garantizar independencia de datos)
                foreach (var prop in component.Propiedades)
                {
                    clonedComponent.Propiedades.Add(new Tuple<string, string>(prop.Item1, prop.Item2));
                }
                
                clonedEntity.Components.Add(clonedComponent);
            }
            
            return clonedEntity;
        }

        /// <summary>
        /// Genera un nombre único para una entidad en una escena específica.
        /// Si el nombre ya existe, agrega sufijo numérico (_1, _2, etc.).
        /// </summary>
        private string GenerateUniqueName(string baseName, YTBScene scene)
        {
            string candidateName = baseName;
            int counter = 1;
            
            // Verificar si el nombre ya existe
            while (scene.Entities.Any(e => e.Name == candidateName))
            {
                candidateName = $"{baseName}_{counter}";
                counter++;
            }
            
            return candidateName;
        }

        /// <summary>
        /// Verifica si una entidad tiene un CameraComponent.
        /// </summary>
        private bool IsMainCamera(YTBEntity entity, YTBScene scene)
        {
            return entity.Components.Any(c => c.ComponentName == "CameraComponent");
        }

        /// <summary>
        /// Cuenta cuántas entidades con CameraComponent hay en una escena.
        /// </summary>
        private int CountCamerasInScene(YTBScene scene)
        {
            return scene.Entities.Count(e => e.Components.Any(c => c.ComponentName == "CameraComponent"));
        }
    }
}
