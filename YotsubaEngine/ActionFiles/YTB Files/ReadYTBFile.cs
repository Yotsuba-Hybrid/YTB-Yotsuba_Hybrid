
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
				// Verificar y crear archivos si no existen (solo en Windows/desarrollo)
				if (OperatingSystem.IsWindows())
				{
					// Asegurar que existen los archivos necesarios antes de leerlos
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
					// La recompilación solo funciona en Windows donde hay acceso al sistema de archivos
					if (!OperatingSystem.IsWindows())
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

						// Fix: Read the entire file content and deserialize/serialize to minify
						// This properly handles pretty-printed JSON with whitespace and newlines
						string jsonContent = await File.ReadAllTextAsync(ytbFile);
						
						// Deserialize and re-serialize without indentation to create minified JSON
						try
						{
							var gameData = JsonSerializer.Deserialize<YTBGameInfo>(jsonContent, YotsubaJsonContext.Default.YTBGameInfo);
							string minifiedJson = JsonSerializer.Serialize(gameData, YotsubaJsonContext.Default.YTBGameInfo);

							Directory.CreateDirectory(Path.GetDirectoryName(destinyFile));
							await File.WriteAllTextAsync(destinyFile, minifiedJson);
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
				throw new GameWontRun(ex, YTBErrors.GameEngineCannotUpdateFiles);
#endif
//+:cnd:noEmit
			}

			Task<string> JSONGame;
			Task<string> JSONGameConfig;

			if (OperatingSystem.IsWindows())
			{
				var gameFilePath = Path.Combine(CompiledConfigPath, JSONGameName);
				var configFilePath = Path.Combine(CompiledConfigPath, JSONGameConfigName);
				
				JSONGame = File.ReadAllTextAsync(gameFilePath);
				JSONGameConfig = File.ReadAllTextAsync(configFilePath);
			}
			else // IMPLEMENTACIÓN ANDROID
			{
				// En Android, TitleContainer necesita rutas RELATIVAS desde la carpeta assets/
				// No rutas absolutas del sistema de archivos
				string gameFilePathRelative = Path.Combine(YTBGlobalState.CompiledAssetsFolderName, GameConfigFolder, JSONGameName);
				string configFilePathRelative = Path.Combine(YTBGlobalState.CompiledAssetsFolderName, GameConfigFolder, JSONGameConfigName);

				// Función local para leer el stream del asset y convertirlo a string
				/// <summary>
				/// Reads a packaged asset into a string.
				/// Lee un asset empaquetado y lo convierte en string.
				/// </summary>
				/// <param name="path">Asset path. Ruta del asset.</param>
				/// <returns>Asset content. Contenido del asset.</returns>
				string ReadAssetContent(string path)
				{
					//path = "Content" + path.Split("Content").LastOrDefault();
					try
					{
						// TitleContainer busca dentro de 'assets/' en el APK.
						// Normalizar los slashes a '/' para Android.

						string androidPath;

						if (path.Contains("\\"))
							androidPath = path.Replace('\\', '/');
						else
							androidPath = path;
//-:cnd:noEmit
#if YTB
							EngineUISystem.SendLog($"Android: Intentando leer asset en: {androidPath}");
#endif
//+:cnd:noEmit

						using (var stream = TitleContainer.OpenStream(androidPath))
						using (var reader = new StreamReader(stream))
						{
							return reader.ReadToEnd();
						}
					}
					catch (FileNotFoundException fnfEx)
					{
//-:cnd:noEmit
#if YTB
						EngineUISystem.SendLog($"Android: Archivo no encontrado: {path} - {fnfEx.Message}");
#endif
//+:cnd:noEmit
						return string.Empty;
					}
					catch (Exception ex)
					{
//-:cnd:noEmit
#if YTB
						EngineUISystem.SendLog($"Android: Error leyendo asset {path}: {ex.Message}");
#endif
//+:cnd:noEmit
						return string.Empty;
					}
				}

				// Envolvemos la lectura síncrona en un Task para mantener compatibilidad
				JSONGame = Task.Run(() => ReadAssetContent(gameFilePathRelative));
				JSONGameConfig = Task.Run(() => ReadAssetContent(configFilePathRelative));
			}

			await Task.WhenAll(JSONGame, JSONGameConfig);

			// Verificación de nulidad o vacío
			if (String.IsNullOrEmpty(JSONGame.Result))
			{
				if (OperatingSystem.IsWindows())
				{
					EngineUISystem.SendLog("[ReadYTBFiles] Archivo de juego vacío o no encontrado. Creando archivo por defecto...");
					
					// Crear escena por defecto con cámara
					var cameraEntity = new YTBEntity
					{
						Name = "Camera",
						Components = new List<YTBComponents>
						{
							EntityYTBXmlTemplate.CameraTemplate()
						}
					};
					
					// Asegurar que la cámara tenga el EntityName correcto
					var cameraComponent = cameraEntity.Components[0];
					var entityNameIndex = cameraComponent.Propiedades.FindIndex(x => x.Item1 == "EntityName");
					if (entityNameIndex >= 0)
					{
						cameraComponent.Propiedades[entityNameIndex] = new Tuple<string, string>("EntityName", "Camera");
					}
					else
					{
						cameraComponent.Propiedades.Add(new Tuple<string, string>("EntityName", "Camera"));
					}

					var defaultEntity = EntityYTBXmlTemplate.GenerateNew();
					defaultEntity.Name = "First Entity";

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

					string defaultGameInfoJson = JsonSerializer.Serialize(defaultGameInfo, YotsubaJsonContext.Default.YTBGameInfo);
					var gameFilePath = Path.Combine(CompiledConfigPath, JSONGameName);
					var fullPath = gameFilePath;

					Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
					File.WriteAllText(fullPath, defaultGameInfoJson);

					JSONGame = Task.FromResult(defaultGameInfoJson);
				}
				else
				{
//-:cnd:noEmit
#if YTB
					EngineUISystem.SendLog($"Error Fatal: No se encontró el archivo del juego en el APK.");
#endif
//+:cnd:noEmit
					throw new FileNotFoundException($"Es Android y no se encontró el archivo en los assets");
				}
			}

			YTBGameInfo GameInfo = JsonSerializer.Deserialize<YTBGameInfo>(JSONGame.Result, YotsubaJsonContext.Default.YTBGameInfo);

			// Manejar configuración vacía o inexistente
			YTBConfig GameConfig;
			if (!String.IsNullOrEmpty(JSONGameConfig.Result))
			{
				GameConfig = JsonSerializer.Deserialize<YTBConfig>(JSONGameConfig.Result, YotsubaJsonContext.Default.YTBConfig);
			}
			else
			{
				EngineUISystem.SendLog("[ReadYTBFiles] Archivo de configuración vacío o no encontrado. Creando configuración por defecto...");
				
				GameConfig = new YTBConfig
				{
					Author = "YourName",
					EngineVersion = "1.0",
					GameName = "YotsubaGame"
				};

				// Guardar la configuración por defecto si estamos en Windows
				if (OperatingSystem.IsWindows())
				{
					var configFilePath = Path.Combine(CompiledConfigPath, JSONGameConfigName);
					Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
					File.WriteAllText(configFilePath, JsonSerializer.Serialize(GameConfig, YotsubaJsonContext.Default.YTBConfig));
				}
			}

			EngineUISystem.SendLog("Archivos leídos correctamente...");

			return await Task.FromResult<(YTBGameInfo, YTBConfig)>((GameInfo, GameConfig));
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
