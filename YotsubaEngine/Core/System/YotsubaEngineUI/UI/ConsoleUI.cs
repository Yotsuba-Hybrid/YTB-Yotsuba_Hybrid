
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
	/// Renderiza el panel de consola del motor con mensajes y errores en runtime.
	/// <para>Renders the engine console panel with runtime messages and errors.</para>
	/// </summary>
	public class ConsoleUI
	{
		/// <summary>
		/// Renderiza la consola del motor.
		/// <para>Renders the engine console.</para>
		/// </summary>
		public void Render()
		{
			ImGui.Begin("Consola");
			foreach ((Color, string) message in EngineUISystem.Messages._ytb.Reverse())
			{
				ImGui.TextColored(message.Item1.ToVector4().ToNumerics(), message.Item2);
			}

			GameWithoutScenes();

			RenderErrorDetailsPanel();

			ImGui.End();
		}

		/// <summary>
		/// Renders a panel listing all accumulated parsing/validation errors with full context.
		/// Renderiza un panel listando todos los errores de parseo/validación acumulados con contexto completo.
		/// </summary>
		private void RenderErrorDetailsPanel()
		{
			if (GameWontRun.ErrorDetails.Count == 0)
				return;

            // Prioritize critical error modals over the details panel
            if (
                GameWontRun.HasError(YTBErrors.GameWithoutScenes) ||
                GameWontRun.HasError(YTBErrors.GameSceneWithoutEntities))
            {
                return;
            }

			if (!ImGui.IsPopupOpen("Errores de Parseo"))
				ImGui.OpenPopup("Errores de Parseo");



			if (ImGui.BeginPopupModal("Errores de Parseo", ImGuiWindowFlags.AlwaysVerticalScrollbar | ImGuiWindowFlags.AlwaysAutoResize))
			{


				ImGui.TextWrapped("Se encontraron errores al procesar los archivos del juego.");
				ImGui.TextWrapped("Corrija los errores y presione 'Recompilar Assets' para intentar nuevamente.");
				ImGui.Separator();

                if (ImGui.Button("Entendido, los corregire"))
                {
                    GameWontRun.ErrorDetails.Clear();
                    ImGui.CloseCurrentPopup();
                }
				ImGui.Separator();
                for (int i = 0; i < GameWontRun.ErrorDetails.Count; i++)
				{
					var detail = GameWontRun.ErrorDetails[i];
					ImGui.PushID(i);

					ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), $"Error #{i + 1}: {detail.ErrorType}");

					if (!string.IsNullOrEmpty(detail.SceneName))
						ImGui.Text($"  Escena: {detail.SceneName}");
					if (!string.IsNullOrEmpty(detail.EntityName))
						ImGui.Text($"  Entidad: {detail.EntityName}");
					if (!string.IsNullOrEmpty(detail.ComponentName))
						ImGui.Text($"  Componente: {detail.ComponentName}");
					if (!string.IsNullOrEmpty(detail.PropertyName))
						ImGui.Text($"  Propiedad: {detail.PropertyName}");

					ImGui.TextColored(Color.Yellow.ToVector4().ToNumerics(), $"  Detalle: {detail.Message}");

					if (!string.IsNullOrEmpty(detail.HowToFix))
						ImGui.TextColored(Color.LimeGreen.ToVector4().ToNumerics(), $"  Como arreglar: {detail.HowToFix}");

					ImGui.Separator();
					ImGui.PopID();
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
				if (!ImGui.IsPopupOpen(nameof(YTBErrors.GameWithoutScenes)))
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

		/// <summary>
		/// Muestra el modal cuando una escena no tiene entidades.
		/// <para>Shows the modal when a scene has no entities.</para>
		/// </summary>
		public void GameSceneWithoutEntities()
		{
			if (GameWontRun.HasError(YTBErrors.GameSceneWithoutEntities))
			{
				if (!ImGui.IsPopupOpen(nameof(YTBErrors.GameSceneWithoutEntities)))
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
