
using ImGuiNET;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Xml.Linq;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Exceptions;
using static YotsubaEngine.Exceptions.GameWontRun;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
	/// <summary>
	/// Renders the engine console panel with runtime messages and errors.
	/// Renderiza el panel de consola del motor con mensajes y errores en runtime.
	/// </summary>
	public class ConsoleUI
	{
		public void Render()
		{
			ImGui.Begin("Consola");
			foreach ((Color, string) message in EngineUISystem.Messages.Reverse())
			{
				ImGui.TextColored(message.Item1.ToVector4().ToNumerics(), message.Item2);
			}


			CameraNotFoundModal();


			GameWithoutScenes();

			ImGui.End();
		}

		/// <summary>
		/// Modal que muestra el error de cuando falta una camara en una escena.
		/// </summary>
		private void CameraNotFoundModal()
		{
			if (GameWontRun.HasError(YTBErrors.CameraNotFound))
			{
				ImGui.OpenPopup(nameof(YTBErrors.CameraNotFound));
			}

			if (ImGui.BeginPopupModal(nameof(YTBErrors.CameraNotFound), ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.Tooltip))
			{
				ImGui.TextWrapped("Error Critico");
				ImGui.TextColored(Color.Red.ToVector4().ToNumerics(),
					"No se ha encontrado una cámara en la escena. Debe agregar una cámara para correr el juego.");

				if (ImGui.Button("De Acuerdo, lo arreglare"))
				{
					GameWontRun.RemoveError(YTBErrors.CameraNotFound);
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}

		}

		/// <summary>
		/// Modal que muestra el error de cuando no hay escenas en el juego
		/// </summary>
		private void GameWithoutScenes()
		{
			if (GameWontRun.HasError(YTBErrors.GameWithoutScenes))
			{
				ImGui.OpenPopup(nameof(YTBErrors.GameWithoutScenes));
			}

			if (ImGui.BeginPopupModal(nameof(YTBErrors.GameWithoutScenes), ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.Tooltip))
			{
				ImGui.TextWrapped("Error Critico");
				ImGui.TextColored(Color.Red.ToVector4().ToNumerics(),
					"Tu Videojuego no tiene escenas, considera crear una para iniciar");

				if (ImGui.Button("De Acuerdo, lo hare"))
				{
					GameWontRun.RemoveError(YTBErrors.GameWithoutScenes);
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
		}

		public void GameSceneWithoutEntities()
		{
			if (GameWontRun.HasError(YTBErrors.GameSceneWithoutEntities))
			{
				ImGui.OpenPopup(nameof(YTBErrors.GameSceneWithoutEntities));
			}

			if (ImGui.BeginPopupModal(nameof(YTBErrors.GameSceneWithoutEntities), ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.Tooltip))
			{
				ImGui.TextWrapped("Error Critico");
				ImGui.TextColored(Color.Red.ToVector4().ToNumerics(),
					"Una de tus escenas no tiene entidades. Si lo dejas asi, tu juego en produccion no funcionara.");

				if (ImGui.Button("De Acuerdo, lo hare"))
				{
					GameWontRun.RemoveError(YTBErrors.GameSceneWithoutEntities);
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
		}
	}
}
