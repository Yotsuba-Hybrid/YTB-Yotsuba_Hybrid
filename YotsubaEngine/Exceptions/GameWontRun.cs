using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YotsubaEngineUI;

namespace YotsubaEngine.Exceptions
{
	/// <summary>
	/// Almacena el contexto detallado de un error de parseo o validación.
	/// <para>Stores detailed context about a parsing or validation error.</para>
	/// </summary>
	public class ErrorDetail
	{
		/// <summary>
		/// Tipo de error asociado.
		/// <para>Associated error type.</para>
		/// </summary>
		public GameWontRun.YTBErrors ErrorType { get; set; }
		/// <summary>
		/// Nombre de la escena relacionada.
		/// <para>Related scene name.</para>
		/// </summary>
		public string SceneName { get; set; }
		/// <summary>
		/// Nombre de la entidad relacionada.
		/// <para>Related entity name.</para>
		/// </summary>
		public string EntityName { get; set; }
		/// <summary>
		/// Nombre del componente relacionado.
		/// <para>Related component name.</para>
		/// </summary>
		public string ComponentName { get; set; }
		/// <summary>
		/// Nombre de la propiedad relacionada.
		/// <para>Related property name.</para>
		/// </summary>
		public string PropertyName { get; set; }
		/// <summary>
		/// Mensaje principal del error.
		/// <para>Main error message.</para>
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// Sugerencia para corregir el problema.
		/// <para>Suggestion to fix the issue.</para>
		/// </summary>
		public string HowToFix { get; set; }

		/// <summary>
		/// Devuelve una descripción legible del error.
		/// <para>Returns a readable description of the error.</para>
		/// </summary>
		/// <returns>Descripción del error. <para>Error description.</para></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append($"[{ErrorType}] ");
			if (!string.IsNullOrEmpty(SceneName)) sb.Append($"Escena: {SceneName} | ");
			if (!string.IsNullOrEmpty(EntityName)) sb.Append($"Entidad: {EntityName} | ");
			if (!string.IsNullOrEmpty(ComponentName)) sb.Append($"Componente: {ComponentName} | ");
			if (!string.IsNullOrEmpty(PropertyName)) sb.Append($"Propiedad: {PropertyName} | ");
			sb.Append(Message);
			if (!string.IsNullOrEmpty(HowToFix)) sb.Append($" | Fix: {HowToFix}");
			return sb.ToString();
		}
	}

	/// <summary>
	/// Esta clase indica que el juego no puede ejecutarse debido a una excepción crítica.
	/// <para>Exception that signals the game cannot run due to a critical error.</para>
	/// </summary>
	public class GameWontRun : Exception
    {

		/// <summary>
		/// Almacena las banderas de error del motor.
		/// <para>Stores error flags for the engine.</para>
		/// </summary>
		public static long ErrorStorage { get; set; } = 0;

		/// <summary>
		/// Lista de errores detallados para mostrar en la UI.
		/// <para>List of detailed error descriptions for UI display.</para>
		/// </summary>
		public static List<ErrorDetail> ErrorDetails { get; set; } = new List<ErrorDetail>();

		/// <summary>
		/// Indica si el juego no se ejecutará debido a una excepción crítica.
		/// <para>Indicates whether the game cannot run due to a critical exception.</para>
		/// </summary>
		public static bool GameWontRunByException { get; set; } = false;

		/// <summary>
		/// Describe por qué el juego no se ejecutará debido a una excepción crítica.
		/// <para>Describes why the game cannot run due to a critical exception.</para>
		/// </summary>
		public static string CauseWontRunByException { get; set; } = "";

		/// <summary>
		/// Detalle adicional sobre por qué el juego no se ejecutará debido a una excepción crítica.
		/// <para>Additional details about the critical exception.</para>
		/// </summary>
		public static string DetailWontRunByException { get; set; } = "";

		/// <summary>
		/// Inicializa una nueva instancia de GameWontRun con una excepción crítica.
		/// <para>Initializes a new GameWontRun exception with a critical error.</para>
		/// </summary>
		/// <param name="ex">Excepción que causó el fallo. <para>Exception that triggered the failure.</para></param>
		/// <param name="error">Bandera de error. <para>Error flag.</para></param>
		public GameWontRun(Exception ex, YTBErrors error) : base(ex.Message, ex)
        {
            GameWontRunByException = true;
			CauseWontRunByException = ex.Message;
			DetailWontRunByException = string.Join(", ", ex.Data.Values.Cast<object>());
			SetError(error);

			EngineUISystem.SendLog("Game Will Not Work", Color.Red);
			EngineUISystem.SendLog("GameWontRun Exception: " + CauseWontRunByException, Color.Red);
			EngineUISystem.SendLog("GameWontRun Exception Detail: " + DetailWontRunByException, Color.Red);
			EngineUISystem.SendLog(error.ToString(), Color.Red);
		}

		/// <summary>
		/// Inicializa una nueva excepción GameWontRun con un mensaje.
		/// <para>Initializes a new GameWontRun exception with a message.</para>
		/// </summary>
		/// <param name="ex">Mensaje de error. <para>Error message.</para></param>
		/// <param name="error">Bandera de error. <para>Error flag.</para></param>
		public GameWontRun(string ex, YTBErrors error) :  base(ex)
		{
			GameWontRunByException = true;
			CauseWontRunByException = ex;
			SetError(error);
			EngineUISystem.SendLog("Game Will Not Work", Color.Red);
			EngineUISystem.SendLog("GameWontRun Exception: " + CauseWontRunByException, Color.Red);
			EngineUISystem.SendLog(error.ToString(), Color.Red);

		}

		/// <summary>
		/// Inicializa una nueva excepción GameWontRun con contexto completo sobre dónde ocurrió el error.
		/// <para>Initializes a new GameWontRun exception with full context about where the error occurred.</para>
		/// </summary>
		/// <param name="error">Bandera de error. <para>Error flag.</para></param>
		/// <param name="sceneName">Nombre de la escena. <para>Scene name.</para></param>
		/// <param name="entityName">Nombre de la entidad. <para>Entity name.</para></param>
		/// <param name="componentName">Nombre del componente. <para>Component name.</para></param>
		/// <param name="propertyName">Nombre de la propiedad. <para>Property name.</para></param>
		/// <param name="message">Mensaje descriptivo. <para>Descriptive message.</para></param>
		/// <param name="howToFix">Cómo arreglar el error. <para>How to fix the error.</para></param>
		public GameWontRun(YTBErrors error, string sceneName, string entityName, string componentName, string propertyName, string message, string howToFix) : base(message)
		{
			GameWontRunByException = true;
			CauseWontRunByException = message;
			SetError(error);

			var detail = new ErrorDetail
			{
				ErrorType = error,
				SceneName = sceneName,
				EntityName = entityName,
				ComponentName = componentName,
				PropertyName = propertyName,
				Message = message,
				HowToFix = howToFix
			};
			ErrorDetails.Add(detail);

			EngineUISystem.SendLog("Game Will Not Work", Color.Red);
			EngineUISystem.SendLog(detail.ToString(), Color.Red);
		}


		/// <summary>
		/// Método que activa la alerta de error y su tipo en el motor.
		/// <para>Activates the error flag in the engine.</para>
		/// </summary>
		/// <param name="error">Bandera de error a activar. <para>Error flag to set.</para></param>
		public static void SetError(YTBErrors error)
		{
			ErrorStorage |= (long)error;
		}

		/// <summary>
		/// Método que desactiva un error en el motor.
		/// <para>Clears an error flag in the engine.</para>
		/// </summary>
		/// <param name="error">Bandera de error a desactivar. <para>Error flag to remove.</para></param>
		public static void RemoveError(YTBErrors error)
		{
			ErrorStorage &= ~(long)error;
		}

		/// <summary>
		/// Método para comprobar si hay un error en la aplicación.
		/// <para>Checks whether an error flag is set.</para>
		/// </summary>
		/// <param name="error">Bandera de error a comprobar. <para>Error flag to check.</para></param>
		/// <returns>True si está activo. <para>True if set.</para></returns>
		public static bool HasError(YTBErrors error) => (ErrorStorage & (long)error) != 0;

		/// <summary>
		/// Resetea todas las banderas de error y permite que el juego vuelva a ejecutarse.
		/// <para>Resets all error flags and allows the game to run again.</para>
		/// </summary>
		public static void Reset()
		{
			ErrorStorage = 0;
			GameWontRunByException = false;
			CauseWontRunByException = string.Empty;
			DetailWontRunByException = string.Empty;
			ErrorDetails.Clear();
			EngineUISystem.SendLog("GameWontRun state has been reset. Game can run again.");
		}

		[Flags]
		/// <summary>
		/// Tipos de errores críticos reportados por el motor.
		/// <para>Critical error types reported by the engine.</para>
		/// </summary>
		public enum YTBErrors : long
		{
			/// <summary>
			/// Error desconocido.
			/// <para>Unknown error.</para>
			/// </summary>
			Unknown = 1 << 0,
			/// <summary>
			/// Cámara no encontrada.
			/// <para>Camera not found.</para>
			/// </summary>
			CameraNotFound = 1 << 1,
			/// <summary>
			/// La cámara no sigue a ninguna entidad.
			/// <para>Camera follows nothing.</para>
			/// </summary>
			CameraFollowNothing = 1 << 2,
			/// <summary>
			/// El juego no tiene escenas.
			/// <para>Game has no scenes.</para>
			/// </summary>
			GameWithoutScenes = 1 << 3,
			/// <summary>
			/// La escena no tiene entidades.
			/// <para>Scene has no entities.</para>
			/// </summary>
			GameSceneWithoutEntities = 1 << 4,
			/// <summary>
			/// El motor no puede actualizar los archivos de configuración.
			/// <para>Engine cannot update configuration files.</para>
			/// </summary>
			GameEngineCannotUpdateFiles = 1 << 5,
			/// <summary>
			/// Se detectó un error de script.
			/// <para>Script error detected.</para>
			/// </summary>
			ScriptHasError = 1 << 6,
			/// <summary>
			/// La entidad no puede agregarse al toolkit WASD.
			/// <para>Entity cannot be added to WASD toolkit.</para>
			/// </summary>
			EntityCannotAddToWasdYTB_toolkit = 1 << 7,

			/// <summary>
			/// Indica que la cámara de seguimiento de la entidad no es apropiada para el contexto actual.
			/// <para>Indicates that the entity's follow camera is not appropriate for the current context.</para>
			/// </summary>
			/// <remarks>Esta bandera se usa para indicar que el comportamiento de cámara por defecto debe ajustarse.
			/// <para>This flag can be used to signal that the default camera behavior should be overridden or adjusted.</para>
			/// </remarks>
			EntityFollowCameraIsNotAppropiate = 1 << 8,

			/// <summary>
			/// Error al parsear shaders.
			/// <para>Shader parse failed.</para>
			/// </summary>
			ShaderWontBeParse = 1 << 9,

			/// <summary>
			/// El modelo 3D no pudo cargarse.
			/// <para>Model 3D could not be loaded.</para>
			/// </summary>
			Model3DLoadFailed = 1 << 10,

			/// <summary>
			/// Error al parsear TransformComponent.
			/// <para>TransformComponent parse failed.</para>
			/// </summary>
			TransformParseFailed = 1 << 11,

			/// <summary>
			/// Error al parsear SpriteComponent2D.
			/// <para>SpriteComponent2D parse failed.</para>
			/// </summary>
			SpriteParseFailed = 1 << 12,

			/// <summary>
			/// Error al parsear RigidBodyComponent2D.
			/// <para>RigidBodyComponent2D parse failed.</para>
			/// </summary>
			RigidBody2DParseFailed = 1 << 13,

			/// <summary>
			/// Error al parsear ButtonComponent2D.
			/// <para>ButtonComponent2D parse failed.</para>
			/// </summary>
			ButtonParseFailed = 1 << 14,

			/// <summary>
			/// Error al parsear AnimationComponent2D.
			/// <para>AnimationComponent2D parse failed.</para>
			/// </summary>
			AnimationParseFailed = 1 << 15,

			/// <summary>
			/// Error al parsear CameraComponent3D.
			/// <para>CameraComponent3D parse failed.</para>
			/// </summary>
			CameraParseFailed = 1 << 16,

			/// <summary>
			/// Error al parsear InputComponent.
			/// <para>InputComponent parse failed.</para>
			/// </summary>
			InputParseFailed = 1 << 17,

			/// <summary>
			/// Error al parsear ScriptComponent.
			/// <para>ScriptComponent parse failed.</para>
			/// </summary>
			ScriptParseFailed = 1 << 18,

			/// <summary>
			/// Error al parsear FontComponent2D.
			/// <para>FontComponent2D parse failed.</para>
			/// </summary>
			FontParseFailed = 1 << 19,

			/// <summary>
			/// Error al parsear TileMapComponent2D.
			/// <para>TileMapComponent2D parse failed.</para>
			/// </summary>
			TileMapParseFailed = 1 << 20,

			/// <summary>
			/// Componente desconocido encontrado en el archivo .ytb.
			/// <para>Unknown component found in .ytb file.</para>
			/// </summary>
			ComponentUnknown = 1 << 21,
        }
	}
}
