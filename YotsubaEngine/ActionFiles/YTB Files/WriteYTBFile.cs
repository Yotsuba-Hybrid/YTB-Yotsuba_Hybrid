#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Templates;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Proporciona ayudas para escribir archivos de datos YTB.
    /// <para>Provides helpers to write YTB game data files.</para>
    /// </summary>
    public static class WriteYTBFile
    {
        /// <summary>
        /// Nombre del archivo donde se guardan las escenas y entidades del juego.
        /// <para>Name of the game data file.</para>
        /// </summary>
        public static string JSONGameName => ReadYTBFile.JSONGameName;

        /// <summary>
        /// Nombre del archivo en donde se guardan los datos del autor, nombre del juego, etc.
        /// <para>Name of the configuration file.</para>
        /// </summary>
        public static string JSONGameConfigName => ReadYTBFile.JSONGameConfigName;

        /// <summary>
        /// Nombre del archivo en donde se guarda el historial de los cambios del engine.
        /// <para>Name of the engine history file.</para>
        /// </summary>
        public static string JSONHistoryGameXMLName => ReadYTBFile.JSONGameSaveName;

        /// <summary>
        /// Relative folder for configuration files.
        /// Carpeta relativa donde se guardan los archivos de configuración del juego
        /// </summary>
        private const string GameConfigFolder = "GameConfig";

        /// <summary>
        /// Development path where .ytb files are written.
        /// Ruta completa de desarrollo donde se escriben los archivos .ytb
        /// </summary>
        private static string DevelopmentConfigPath => Path.Combine(YTBGlobalState.DevelopmentAssetsPath, GameConfigFolder);

        /// <summary>
        /// Método para crear (si no existen) los archivos necesarios del juego.
        /// <para>Creates default game files when missing.</para>
        /// </summary>
        public static void CreateYTBGameFile()
        {
            try
            {
                // Asegurar que la carpeta GameConfig existe
                if (!Directory.Exists(DevelopmentConfigPath))
                {
                    Directory.CreateDirectory(DevelopmentConfigPath);
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Carpeta GameConfig creada en: {DevelopmentConfigPath}");
                }

                var gameFilePath = Path.Combine(DevelopmentConfigPath, JSONGameName);
                var configFilePath = Path.Combine(DevelopmentConfigPath, JSONGameConfigName);
                var historyFilePath = Path.Combine(DevelopmentConfigPath, JSONHistoryGameXMLName);

                // Crear archivo del juego con escena y cámara por defecto
                if (!File.Exists(gameFilePath))
                {
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Creando archivo de juego por defecto: {JSONGameName}");
                    
                    // Crear entidad de cámara
                    var cameraEntity = EntityYTBXmlTemplate.GenerateNew();
                    cameraEntity.Name = "Camera";
                    
                    // Configurar el componente de cámara
                    int cameraComponentIndex = cameraEntity.Components.FindIndex(x => x.ComponentName == "CameraComponent3D");
                    if (cameraComponentIndex >= 0)
                    {
                        cameraEntity.Components[cameraComponentIndex] = EntityYTBXmlTemplate.CameraTemplate();
                        
                        // Asegurar que EntityName apunte a la propia cámara
                        var entityNameIndex = cameraEntity.Components[cameraComponentIndex].Propiedades.FindIndex(x => x.Item1 == "EntityName");

                        if (entityNameIndex >= 0)
                        {
                            cameraEntity.Components[cameraComponentIndex].Propiedades[entityNameIndex] = new Tuple<string, string>("EntityName", "Camera");
                        }
                        else
                        {
                            cameraEntity.Components[cameraComponentIndex].Propiedades.Add(new Tuple<string, string>("EntityName", "Camera"));
                        }
                    }

                    // Crear una segunda entidad por defecto
                    var defaultEntity = EntityYTBXmlTemplate.GenerateNew();
                    defaultEntity.Name = "First Entity";

                    // Crear el archivo con la escena por defecto
                    var defaultGameInfo = new YTBGameInfo
                    {
                        Scene = new List<YTBScene>
                        {
                            new YTBScene
                            {
                                Name = "First Scene",
                                Entities = new List<YTBEntity> { cameraEntity, defaultEntity }
                            }
                        }
                    };

                    File.WriteAllText(gameFilePath, JsonSerializer.Serialize(defaultGameInfo, YotsubaJsonContext.Default.YTBGameInfo));
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Archivo {JSONGameName} creado exitosamente con escena y cámara por defecto.");
                }

                // Crear archivo de configuración por defecto
                if (!File.Exists(configFilePath))
                {
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Creando archivo de configuración por defecto: {JSONGameConfigName}");
                    
                    var defaultConfig = new YTBConfig
                    {
                        Author = "YourName",
                        EngineVersion = "1.0",
                        GameName = "YotsubaGame"
                    };

                    File.WriteAllText(configFilePath, JsonSerializer.Serialize(defaultConfig, YotsubaJsonContext.Default.YTBConfig));
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Archivo {JSONGameConfigName} creado exitosamente.");
                }

                // Crear archivo de historial vacío si no existe
                if (!File.Exists(historyFilePath))
                {
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Creando archivo de historial por defecto: {JSONHistoryGameXMLName}");
                    File.WriteAllText(historyFilePath, JsonSerializer.Serialize(new List<YTBEngineHistory>(), YotsubaJsonContext.Default.ListYTBEngineHistory));
                    EngineUISystem.SendLog($"[CreateYTBGameFile] Archivo {JSONHistoryGameXMLName} creado exitosamente.");
                }

                EngineUISystem.SendLog("[CreateYTBGameFile] Todos los archivos necesarios están presentes.");
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"[CreateYTBGameFile] ERROR: No se pudieron crear los archivos del juego. Excepción: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the game data file.
        /// Método para editar el archivo del juego
        /// </summary>
        /// <param name="GameEdit">Updated game info. Información del juego actualizada.</param>
        internal static void EditYTBGameFile(YTBGameInfo GameEdit)
        {
            var gameFilePath = Path.Combine(DevelopmentConfigPath, JSONGameName);
            var historyFilePath = Path.Combine(DevelopmentConfigPath, JSONHistoryGameXMLName);

            if (!File.Exists(gameFilePath)) 
                CreateYTBGameFile();

            string game = JsonSerializer.Serialize<YTBGameInfo>(GameEdit, YotsubaJsonContext.Default.YTBGameInfo);
            File.WriteAllText(gameFilePath, game);

            string fecha = DateTime.Now.ToString("G");

            YTBEngineHistory history = new YTBEngineHistory
            {
                GameDevelopVersion = GameEdit,
                SaveTime = fecha
            };

            // Comprobar si tiene data primero
            if (File.Exists(historyFilePath))
            {
                var existingHistoryData = File.ReadAllText(historyFilePath);
                if (!string.IsNullOrWhiteSpace(existingHistoryData))
                {
                    var existingHistory = JsonSerializer.Deserialize<List<YTBEngineHistory>>(existingHistoryData, YotsubaJsonContext.Default.ListYTBEngineHistory);

                    existingHistory.Add(history);
                    File.WriteAllText(historyFilePath,
                        JsonSerializer.Serialize<List<YTBEngineHistory>>(existingHistory, YotsubaJsonContext.Default.ListYTBEngineHistory));
                    EngineUISystem.SendLog($"CAMBIOS GUARDADOS" + " --- " + fecha);
                    return;
                }
                else
                {
                    File.WriteAllText(historyFilePath,
                    JsonSerializer.Serialize<List<YTBEngineHistory>>([history], YotsubaJsonContext.Default.ListYTBEngineHistory));

                    EngineUISystem.SendLog($"CAMBIOS GUARDADOS" + " --- " + fecha);
                    return;
                }
            }
        }

        /// <summary>
        /// Método para editar la configuración del juego.
        /// <para>Updates the game configuration file.</para>
        /// </summary>
        /// <param name="GameConfigEdit">Configuración actualizada. <para>Updated configuration.</para></param>
        public static void EditYTBGameConfigFile(YTBConfig GameConfigEdit)
        {
            var configFilePath = Path.Combine(DevelopmentConfigPath, JSONGameConfigName);

            if (!File.Exists(configFilePath)) 
                CreateYTBGameFile();

            string gameConfig = JsonSerializer.Serialize<YTBConfig>(GameConfigEdit, YotsubaJsonContext.Default.YTBConfig);
            File.WriteAllText(configFilePath, gameConfig);
        }

        /// <summary>
        /// Método para agregar una entidad al juego desde el archivo del juego.
        /// <para>Adds a new entity to the game file.</para>
        /// </summary>
        /// <param name="name">Nombre de la entidad. <para>Entity name.</para></param>
        /// <param name="sceneName">Nombre de la escena. <para>Scene name.</para></param>
        /// <returns>Tarea de la operación. <para>Task for the operation.</para></returns>
        public static async Task AddEntityAsync(string name, string sceneName)
        {
            var GameFile = await ReadYTBFile.ReadYTBGameFile();

            var game = GameFile.Scene.Where(i => i.Name == sceneName).FirstOrDefault();
            if(game == null)
            {
                EngineUISystem.SendLog($"ERRORR: LA ESCENA {sceneName} NO EXISTE!!!");
                return;
            }

            if(game.EntitiesCount > 0)
            {
                if(game.Entities.Any(x => x.Name == name))
                {
                    EngineUISystem.SendLog($"ERRORR: EL NOMBRE {name} YA FUE ASOCIADO A UNA ENTIDAD!!!");
                    return;
                }
            }
            game.Entities.Add(new YTBEntity()
            {
                Name = name,
                Components = []
            });

            EditYTBGameFile(GameFile);

            EngineUISystem.SendLog($"Entidad creada correctamente: {name} con {game.Entities.FirstOrDefault(x => x.Name == name).ComponentsCount} componentes");
        }

        /// <summary>
        /// Método para agregar una nueva escena al juego desde el archivo del juego.
        /// <para>Adds a new scene to the game file.</para>
        /// </summary>
        /// <param name="name">Nombre de la escena. <para>Scene name.</para></param>
        /// <returns>Tarea de la operación. <para>Task for the operation.</para></returns>
        public static async Task AddSceneAsync(string name)
        {
            var GameFile = await ReadYTBFile.ReadYTBGameFile();
            if (GameFile.Scene.Any(Scene => Scene.Name == name))
            {
                EngineUISystem.SendLog($"¡¡¡ERRORR: NOMBRE {name} -------- Este nombre ya esta siendo usado por otra escena --------!!!");
                return;
            }

            GameFile.Scene.Add(new YTBScene() { Name = name, Entities = [] });

            EditYTBGameFile(GameFile);
            EngineUISystem.SendLog($"Escena creada correctamente: {name} con {GameFile.Scene.FirstOrDefault(x => x.Name == name).EntitiesCount} entidades");
        }

        /// <summary>
        /// Método para agregar un componente a una entidad del juego desde el archivo del juego.
        /// <para>Adds a component to an entity in the game file.</para>
        /// </summary>
        /// <param name="ComponentTypeString">Nombre del tipo de componente. <para>Component type name.</para></param>
        /// <param name="sceneName">Nombre de la escena. <para>Scene name.</para></param>
        /// <param name="entityName">Nombre de la entidad. <para>Entity name.</para></param>
        /// <returns>Tarea de la operación. <para>Task for the operation.</para></returns>
        public static async Task AddComponentAsync(string ComponentTypeString, string sceneName , string entityName)
        {
            var GameFile = await ReadYTBFile.ReadYTBGameFile();

            var scene = GameFile.Scene.FirstOrDefault(scenes => scenes.Name == sceneName);

            var entity = scene.Entities.FirstOrDefault(x => x.Name == entityName);

            if (entity == null)
            {
                EngineUISystem.SendLog($"¡¡¡ERRORR: NOMBRE {entityName} -------- No hay ninguna entidad con este nombre --------!!!");
                return;
            }

            if (entity.Components.Any(x => x.ComponentName == ComponentTypeString))
            {
                EngineUISystem.SendLog($"¡¡¡ERRORR: NOMBRE {entityName} -------- Esta entidad ya tenia el componente {ComponentTypeString} --------!!!");
                return;
            }

            entity.Components.Add(new YTBComponents()
            {
                ComponentName = ComponentTypeString,
                Propiedades = []
            });

            EditYTBGameFile(GameFile);

            EngineUISystem.SendLog($"Componente agregado correctamente: {entityName} con {ComponentTypeString} listo!");
        }

        /// <summary>
        /// Método para agregar una propiedad a un componente de una entidad específica dentro de una escena del juego.
        /// <para>Adds or updates properties on a component within the game file.</para>
        /// </summary>
        /// <param name="componentType">Nombre del tipo de componente. <para>Component type name.</para></param>
        /// <param name="sceneName">Nombre de la escena. <para>Scene name.</para></param>
        /// <param name="entityName">Nombre de la entidad. <para>Entity name.</para></param>
        /// <param name="properties">Propiedades a agregar o actualizar. <para>Properties to add/update.</para></param>
        /// <returns>Tarea de la operación. <para>Task for the operation.</para></returns>
        public static async Task AddComponentPropertyAsync(
                             string componentType,
                             string sceneName,
                             string entityName,
                             params Tuple<string, string>[] properties)
        {
            var game = await ReadYTBFile.ReadYTBGameFile();

            // Buscar escena
            var scene = game.Scene.FirstOrDefault(x => x.Name == sceneName);
            if (scene == null)
            {
                EngineUISystem.SendLog($"ERROR: No se encontró la escena '{sceneName}'.");
                return;
            }

            // Buscar entidad
            var entity = scene.Entities.FirstOrDefault(x => x.Name == entityName);
            if (entity == null)
            {
                EngineUISystem.SendLog($"ERROR: No hay ninguna entidad llamada '{entityName}' en la escena '{sceneName}'.");
                return;
            }

            // Buscar componente
            var component = entity.Components.FirstOrDefault(x => x.ComponentName == componentType);
            if (component == null)
            {
                EngineUISystem.SendLog($"ERROR: No se encontró el componente '{componentType}' en la entidad '{entityName}'.");
                return;
            }

            // Agregar o actualizar propiedades
            foreach (var property in properties)
            {
                var existingIndex = component.Propiedades.FindIndex(p => p.Item1 == property.Item1);
                if (existingIndex >= 0)
                {
                    // Actualiza propiedad existente
                    component.Propiedades[existingIndex] = property;
                }
                else
                {
                    // Agrega nueva propiedad
                    component.Propiedades.Add(property);
                }
            }

            EditYTBGameFile(game);
            EngineUISystem.SendLog($"Componente editado correctamente: {entityName} con {componentType} listo y {entity.Components.FirstOrDefault(x => x.ComponentName == componentType).PropiedadesCount} propiedades!");
        }


        internal static async Task RefactorYTBFile()
        {
            YTBGameInfo gameInfo = await ReadYTBFile.ReadYTBGameFile();
            YTBEntity templateEntity = EntityYTBXmlTemplate.GenerateNew();

            foreach (YTBComponents componentTemplate in templateEntity.Components)
            {
                foreach (YTBScene scene in gameInfo.Scene)
                {
                    foreach (YTBEntity entity in scene.Entities)
                    {
                        foreach (YTBComponents component in entity.Components)
                        {
                            if (component.ComponentName != componentTemplate.ComponentName) continue;

                            foreach ((string propertyName, string propertyValue) in componentTemplate.Propiedades)
                            {
                                if(component.Propiedades.Any(p => p.Item1 == propertyName)) continue;

                                component.Propiedades.Add(new Tuple<string, string>(propertyName, propertyValue));
                            }
                        }
                    }
                }
            }

            EditYTBGameFile(gameInfo);
        }
    }
}
