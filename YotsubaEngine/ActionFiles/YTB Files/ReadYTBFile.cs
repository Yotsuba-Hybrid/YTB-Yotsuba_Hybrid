
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Templates;
using static YotsubaEngine.Exceptions.GameWontRun;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
	/// <summary>
	/// Proporciona ayudas para leer archivos de datos YTB.
	/// <para>Provides helpers to read YTB game data files.</para>
	/// </summary>
	public static class ReadYTBFile
	{
		/// <summary>
		/// Nombre del archivo donde se guardan las escenas y entidades del juego.
		/// <para>Name of the game data file containing scenes and entities.</para>
		/// </summary>
		public const string JSONGameName = "YotsubaGame.ytb";

		/// <summary>
		/// Nombre del archivo en donde se guardan los datos del autor, nombre del juego, etc.
		/// <para>Name of the configuration file with author and game metadata.</para>
		/// </summary>
		public const string JSONGameConfigName = "YotsubaGameConfig.ytb";

		/// <summary>
		/// Nombre del archivo en donde se guarda el historial de los cambios del engine.
		/// <para>Name of the engine history file.</para>
		/// </summary>
		public const string JSONGameSaveName = "YotsubaEngineHistory.ytb";

		/// <summary>
		/// Relative folder for game configuration files.
		/// Carpeta relativa donde se guardan los archivos de configuración del juego
		/// </summary>
		private const string GameConfigFolder = "GameConfig";

		/// <summary>
		/// Development path where .ytb files are stored.
		/// Ruta completa de desarrollo donde se encuentran los archivos .ytb
		/// </summary>
		private static string DevelopmentConfigPath => Path.Combine(YTBGlobalState.DevelopmentAssetsPath, GameConfigFolder);

		/// <summary>
		/// Compiled assets path where .ytb files are stored.
		/// Ruta completa de assets compilados donde se encuentran los archivos .ytb compilados
		/// </summary>
		private static string CompiledConfigPath => Path.Combine(YTBGlobalState.CompiledAssetsPath, GameConfigFolder);

        /// <summary>
        /// Reads the game and configuration files, creating defaults when needed.
        /// Método para Leer los archivos del juego.
        /// Cubre casos donde no existe la carpeta GameConfig o los archivos .ytb.
        /// </summary>
        /// <param name="recompilar">Whether to force recompilation. Si se debe forzar la recompilación.</param>
        /// <returns>Tuple with game info and config. Tupla con datos del juego y configuración.</returns>
        internal static async Task<(YTBGameInfo, YTBConfig)> ReadYTBFiles(bool recompilar = false)
        {
            try
            {
                // Verificar y crear archivos si no existen (solo en Desktop/desarrollo)
                if (YTBGlobalState.IsDesktop)
                {
                    if (!Directory.Exists(DevelopmentConfigPath) ||
                        !File.Exists(Path.Combine(DevelopmentConfigPath, JSONGameName)) ||
                        !File.Exists(Path.Combine(DevelopmentConfigPath, JSONGameConfigName)))
                    {
                        EngineUISystem.SendLog("[ReadYTBFiles] Archivos .ytb no encontrados. Creando archivos por defecto...");
                        WriteYTBFile.CreateYTBGameFile();
                    }
                }

                if (recompilar)
                {
                    if (!YTBGlobalState.IsDesktop)
                    {
                        //-:cnd:noEmit
#if YTB
						EngineUISystem.SendLog("Recompilación no disponible en esta plataforma");
#endif
                        //+:cnd:noEmit
                    }
                    else
                    {
                        string ytbFile = Path.Combine(DevelopmentConfigPath, JSONGameName);
                        string destinyFile = Path.Combine(CompiledConfigPath, JSONGameName);

                        try
                        {
                            // Optimización: Leer y escribir usando Streams para minificar
                            using (var readStream = File.OpenRead(ytbFile))
                            {
                                var gameData = await JsonSerializer.DeserializeAsync<YTBGameInfo>(readStream, YotsubaJsonContext.Default.YTBGameInfo);

                                Directory.CreateDirectory(Path.GetDirectoryName(destinyFile));
                                using (var writeStream = File.Create(destinyFile))
                                {
                                    await JsonSerializer.SerializeAsync(writeStream, gameData, YotsubaJsonContext.Default.YTBGameInfo);
                                }
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            EngineUISystem.SendLog($"[ReadYTBFiles] Error parsing YTB file '{ytbFile}': {jsonEx.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //-:cnd:noEmit
#if YTB
				_= new GameWontRun(ex, YTBErrors.GameEngineCannotUpdateFiles);
#endif
                //+:cnd:noEmit
            }

            YTBGameInfo GameInfo = null;
            YTBConfig GameConfig = null;

            if (YTBGlobalState.IsDesktop)
            {
                var gameFilePath = Path.Combine(CompiledConfigPath, JSONGameName);
                var configFilePath = Path.Combine(CompiledConfigPath, JSONGameConfigName);

                if (File.Exists(gameFilePath))
                {
                    using (var stream = File.OpenRead(gameFilePath))
                        GameInfo = await JsonSerializer.DeserializeAsync<YTBGameInfo>(stream, YotsubaJsonContext.Default.YTBGameInfo);
                }

                if (File.Exists(configFilePath))
                {
                    using (var stream = File.OpenRead(configFilePath))
                        GameConfig = await JsonSerializer.DeserializeAsync<YTBConfig>(stream, YotsubaJsonContext.Default.YTBConfig);
                }
            }
            else // IMPLEMENTACIÓN MÓVIL (Android/iOS)
            {
                string gameFilePathRelative = Path.Combine(YTBGlobalState.CompiledAssetsFolderName, GameConfigFolder, JSONGameName).Replace('\\', '/');
                string configFilePathRelative = Path.Combine(YTBGlobalState.CompiledAssetsFolderName, GameConfigFolder, JSONGameConfigName).Replace('\\', '/');

                /// <summary>
                /// Lee un asset empaquetado y lo deserializa directamente sin crear strings en memoria.
                /// </summary>
                async Task<T> ReadAndDeserializeAssetAsync<T>(string path, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
                {
                    try
                    {
                        //-:cnd:noEmit
#if YTB
						EngineUISystem.SendLog($"Android: Intentando leer asset directo a objeto: {path}");
#endif
                        //+:cnd:noEmit
                        using (var stream = TitleContainer.OpenStream(path))
                        {
                            // Magia aquí: Pasa del Stream del ZIP directo al objeto C# usando Source Generators
                            return await JsonSerializer.DeserializeAsync<T>(stream, typeInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        //-:cnd:noEmit
#if YTB
						EngineUISystem.SendLog($"Android: Error leyendo/deserializando asset {path}: {ex.Message}");
#endif
                        //+:cnd:noEmit
                        return default;
                    }
                }

                // Ejecutamos ambas lecturas de stream en paralelo
                var gameTask = ReadAndDeserializeAssetAsync(gameFilePathRelative, YotsubaJsonContext.Default.YTBGameInfo);
                var configTask = ReadAndDeserializeAssetAsync(configFilePathRelative, YotsubaJsonContext.Default.YTBConfig);

                await Task.WhenAll(gameTask, configTask);

                GameInfo = gameTask.Result;
                GameConfig = configTask.Result;
            }

            // Verificación de nulidad (Reemplaza el antiguo String.IsNullOrEmpty)
            if (GameInfo == null)
            {
                if (YTBGlobalState.IsDesktop)
                {
                    EngineUISystem.SendLog("[ReadYTBFiles] Archivo de juego vacío o no encontrado. Creando archivo por defecto...");

                    var cameraEntity = new YTBEntity
                    {
                        Name = "Camera",
                        Components = new List<YTBComponents> { EntityYTBXmlTemplate.CameraTemplate() }
                    };

                    var entityNameIndex = cameraEntity.Components[0].Propiedades.FindIndex(x => x.Item1 == "EntityName");
                    if (entityNameIndex >= 0)
                        cameraEntity.Components[0].Propiedades[entityNameIndex] = new Tuple<string, string>("EntityName", "Camera");
                    else
                        cameraEntity.Components[0].Propiedades.Add(new Tuple<string, string>("EntityName", "Camera"));

                    var defaultEntity = EntityYTBXmlTemplate.GenerateNew();
                    defaultEntity.Name = "First Entity";

                    GameInfo = new YTBGameInfo
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

                    var gameFilePath = Path.Combine(CompiledConfigPath, JSONGameName);
                    Directory.CreateDirectory(Path.GetDirectoryName(gameFilePath));

                    using (var writeStream = File.Create(gameFilePath))
                    {
                        await JsonSerializer.SerializeAsync(writeStream, GameInfo, YotsubaJsonContext.Default.YTBGameInfo);
                    }
                }
                else
                {
                    //-:cnd:noEmit
#if YTB
					EngineUISystem.SendLog($"Error Fatal: No se encontró o no se pudo leer el archivo del juego en el APK.");
#endif
                    //+:cnd:noEmit
                    throw new FileNotFoundException($"Es Android y no se encontró el archivo en los assets");
                }
            }

            // Manejar configuración vacía o inexistente
            if (GameConfig == null)
            {
                EngineUISystem.SendLog("[ReadYTBFiles] Archivo de configuración vacío o no encontrado. Creando configuración por defecto...");

                GameConfig = new YTBConfig
                {
                    Author = "YourName",
                    EngineVersion = "1.0",
                    GameName = "YotsubaGame"
                };

                if (YTBGlobalState.IsDesktop)
                {
                    var configFilePath = Path.Combine(CompiledConfigPath, JSONGameConfigName);
                    Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));

                    using (var writeStream = File.Create(configFilePath))
                    {
                        await JsonSerializer.SerializeAsync(writeStream, GameConfig, YotsubaJsonContext.Default.YTBConfig);
                    }
                }
            }

            EngineUISystem.SendLog("Archivos leídos correctamente...");

            return (GameInfo, GameConfig);
        }
        /// <summary>
        /// Reads only the game data file.
        /// Método para leer solo el archivo del juego
        /// </summary>
        /// <returns>Game info. Información del juego.</returns>
        internal static async Task<YTBGameInfo> ReadYTBGameFile()
		{
			EngineUISystem.SendLog("Leyendo Archivos del juego...");
			var gameFilePath = Path.Combine(DevelopmentConfigPath, JSONGameName);

			string jsonGameContent = await File.ReadAllTextAsync(gameFilePath);

			if (string.IsNullOrEmpty(jsonGameContent))
			{
				YTBGameInfo gameInfo = new()
				{
					Scene = new List<YTBScene>()
				};

				gameInfo.Scene.Add(new YTBScene
				{
					Name = "New Scene",
					Entities = new List<YTBEntity>()
				});

				WriteYTBFile.EditYTBGameFile(gameInfo);
				jsonGameContent = await File.ReadAllTextAsync(gameFilePath);
			}

			YTBGameInfo GameInfo = JsonSerializer.Deserialize(jsonGameContent, YotsubaJsonContext.Default.YTBGameInfo);

			EngineUISystem.SendLog("Archivos leídos correctamente...");

			return GameInfo;
		}

		/// <summary>
		/// Método para leer solo el archivo de historial del game engine del juego.
		/// <para>Reads the engine history file for the game.</para>
		/// </summary>
		/// <returns>Entradas de historial. <para>History entries.</para></returns>
		public static List<YTBEngineHistory> ReadYTBGameFileHistory()
		{
			var historyFilePath = Path.Combine(DevelopmentConfigPath, JSONGameSaveName);

			if (File.Exists(historyFilePath))
			{
				var existingHistoryData = File.ReadAllText(historyFilePath);
				if (!string.IsNullOrEmpty(existingHistoryData))
					return JsonSerializer.Deserialize<List<YTBEngineHistory>>(existingHistoryData, YotsubaJsonContext.Default.ListYTBEngineHistory);

				return new List<YTBEngineHistory>();
			}

			return new List<YTBEngineHistory>();
		}

		/// <summary>
		/// Método para leer solo el archivo de configuración.
		/// <para>Reads only the configuration file.</para>
		/// </summary>
		/// <returns>Configuración del juego. <para>Game configuration.</para></returns>
		public static async Task<YTBConfig> ReadYTBGameConfigFile()
		{
			EngineUISystem.SendLog("Leyendo Configuración del juego...");
			var configFilePath = Path.Combine(DevelopmentConfigPath, JSONGameConfigName);

			// Crear archivo de configuración si no existe
			if (!File.Exists(configFilePath))
			{
				EngineUISystem.SendLog("[ReadYTBGameConfigFile] Archivo de configuración no encontrado. Creando archivo por defecto...");
				WriteYTBFile.CreateYTBGameFile();
			}

			string jsonConfigContent = await File.ReadAllTextAsync(configFilePath);

			YTBConfig GameConfig = JsonSerializer.Deserialize<YTBConfig>(jsonConfigContent, YotsubaJsonContext.Default.YTBConfig);

			EngineUISystem.SendLog("Archivos leídos correctamente...");

			return GameConfig;
		}
	}
}
