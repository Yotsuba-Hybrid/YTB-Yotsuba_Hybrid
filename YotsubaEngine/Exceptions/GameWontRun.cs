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
	/// Exception that signals the game cannot run due to a critical error.
	/// Esta es una clase que indica que el juego no puede ejecutarse debido a una excepción crítica.
	/// </summary>
	public class GameWontRun : Exception
    {

		/// <summary>
		/// Stores error flags for the engine.
		/// Activa o desactiva flags de errores
		/// </summary>
		public static long ErrorStorage { get; set; } = 0;

		/// <summary>
		/// Indicates whether the game cannot run due to a critical exception.
		/// Está propiedad indica si el juego no se ejecutará debido a una excepción crítica.
		/// Si es true, el juego no se ejecutara, solo el editor visual para poder corregir el error.
		/// </summary>
		public static bool GameWontRunByException { get; set; } = false;

		/// <summary>
		/// Describes why the game cannot run due to a critical exception.
		/// Propiedad que describe por que el juego no se ejecutará debido a una excepción crítica.
		/// </summary>
		public static string CauseWontRunByException { get; set; } = "";

		/// <summary>
		/// Additional details about the critical exception.
		/// Detalle adicional sobre por que el juego no se ejecutará debido a una excepción crítica.
		/// </summary>
		public static string DetailWontRunByException { get; set; } = "";

		/// <summary>
		/// Initializes a new GameWontRun exception with a critical error.
		/// Inicializa una nueva instancia de la clase GameWontRun con una excepción crítica que impide la ejecución del juego.
		/// Este constructor registra la excepción y sus detalles en el sistema de UI del motor.
		/// Por favor, ten en cuenta que cuando se lanza esta excepción, el juego no podrá ejecutarse.
		///	Yotsuba Engine mantendra el editor visual abierto para que puedas corregir el error.
		/// </summary>
		/// <param name="ex">Exception that triggered the failure. Excepción que causó el fallo.</param>
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
		/// Initializes a new GameWontRun exception with a message.
		/// Inicializa una nueva excepción GameWontRun con un mensaje.
		/// </summary>
		/// <param name="ex">Error message. Mensaje de error.</param>
		/// <param name="error">Error flag. Bandera de error.</param>
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
		/// Activates the error flag in the engine.
		/// Metodo que activa la alerta de error y su tipo en el engine
		/// </summary>
		/// <param name="error">Error flag to set. Bandera de error a activar.</param>
		public static void SetError(YTBErrors error)
		{
			ErrorStorage |= (long)error;
		}

		/// <summary>
		/// Clears an error flag in the engine.
		/// Metodo que desactiva un error en el engine
		/// </summary>
		/// <param name="error">Error flag to remove. Bandera de error a desactivar.</param>
		public static void RemoveError(YTBErrors error)
		{
			ErrorStorage &= ~(long)error;
		}

		/// <summary>
		/// Checks whether an error flag is set.
		/// Metodo para comprobar si hay un error en la aplicacion
		/// </summary>
		/// <param name="error">Error flag to check. Bandera de error a comprobar.</param>
		/// <returns>True if set. True si está activo.</returns>
		public static bool HasError(YTBErrors error) => (ErrorStorage & (long)error) != 0;

		/// <summary>
		/// Resets all error flags and allows the game to run again.
		/// Resetea todas las banderas de error y permite que el juego vuelva a ejecutarse.
		/// </summary>
		public static void Reset()
		{
			ErrorStorage = 0;
			GameWontRunByException = false;
			CauseWontRunByException = string.Empty;
			DetailWontRunByException = string.Empty;
			EngineUISystem.SendLog("GameWontRun state has been reset. Game can run again.");
		}

		[Flags]
		public enum YTBErrors : long
		{
			/// <summary>
			/// Unknown error.
			/// Error desconocido.
			/// </summary>
			Unknown = 1 << 0,
			/// <summary>
			/// Camera not found.
			/// Cámara no encontrada.
			/// </summary>
			CameraNotFound = 1 << 1,
			/// <summary>
			/// Camera follows nothing.
			/// La cámara no sigue a ninguna entidad.
			/// </summary>
			CameraFollowNothing = 1 << 2,
			/// <summary>
			/// Game has no scenes.
			/// El juego no tiene escenas.
			/// </summary>
			GameWithoutScenes = 1 << 3,
			/// <summary>
			/// Scene has no entities.
			/// La escena no tiene entidades.
			/// </summary>
			GameSceneWithoutEntities = 1 << 4,
			/// <summary>
			/// Engine cannot update configuration files.
			/// El motor no puede actualizar los archivos de configuración.
			/// </summary>
			GameEngineCannotUpdateFiles = 1 << 5,
			/// <summary>
			/// Script error detected.
			/// Se detectó un error de script.
			/// </summary>
			ScriptHasError = 1 << 6,
			/// <summary>
			/// Entity cannot be added to WASD toolkit.
			/// La entidad no puede agregarse al toolkit WASD.
			/// </summary>
			EntityCannotAddToWasdYTB_toolkit = 1 << 7,

			/// <summary>
			/// Indicates that the entity's follow camera is not appropriate for the current context.
			/// </summary>
			/// <remarks>This flag can be used to signal that the default camera behavior should be overridden or
			/// adjusted for the entity. The specific meaning and handling of this flag may depend on the application's camera
			/// management logic.</remarks>
			EntityFollowCameraIsNotAppropiate = 1 << 8,

			ShaderWontBeParse = 1 << 9,

			/// <summary>
			/// Model 3D could not be loaded.
			/// El modelo 3D no pudo cargarse.
			/// </summary>
			Model3DLoadFailed = 1 << 10,
        }
	}
}
