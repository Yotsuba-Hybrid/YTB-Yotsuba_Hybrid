using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.TMX_Files.TiledCS;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Exceptions;
using YotsubaEngine.Graphics;
using YotsubaEngine.Templates;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
	/// <summary>
	/// Editor UI panel for inspecting and editing entity components.
	/// Panel de UI del editor para inspeccionar y editar componentes de entidades.
	/// </summary>
	internal class EntityManagerUI
	{
		private readonly Func<YTBEntity> _getSelectedEntity;
		private readonly Func<YTBScene> _getCurrentScene;
		private readonly Action _saveAction;
		private List<string> _textureAtlasFiles = new();
		private readonly string _contentPath;

		// Cache para evitar parsear XML constantemente
		private Dictionary<string, List<SubtextureInfo>> _subtextureCache = new();
		private Dictionary<string, List<AnimationInfo>> _animationCache = new();

        internal EntityManagerUI(Func<YTBEntity> getSelectedEntity, Func<YTBScene> getCurrentScene, Action saveAction, string contentPath = "Content")
		{
			_getSelectedEntity = getSelectedEntity;
			_getCurrentScene = getCurrentScene;
			_saveAction = saveAction;
			_contentPath = contentPath;
			LoadTextureAtlasFiles();

		}

		private void LoadTextureAtlasFiles()
		{
			_textureAtlasFiles.Clear();
			_subtextureCache.Clear();
			_animationCache.Clear();

			if (Directory.Exists(_contentPath))
			{
				var xmlFiles = Directory.GetFiles(_contentPath, "*.xml", SearchOption.AllDirectories);
				foreach (var file in xmlFiles)
				{
					string relativePath = Path.GetRelativePath(_contentPath, file).Replace("\\", "/");
					_textureAtlasFiles.Add(relativePath);
				}
			}
		}

		private List<SubtextureInfo> ParseSubtextures(string xmlPath)
		{
			if (_subtextureCache.ContainsKey(xmlPath))
				return _subtextureCache[xmlPath];

			var subtextures = new List<SubtextureInfo>();
			try
			{
				string fullPath = Path.Combine(_contentPath, xmlPath);
				if (!File.Exists(fullPath)) return subtextures;

				var doc = XDocument.Load(fullPath);
				var subtextureElements = doc.Descendants("subtexture");
				var i = 0;
				foreach (var element in subtextureElements)
				{
					var info = new SubtextureInfo
					{
						Name = element.Attribute("name")?.Value ?? "unnamed",
						X = int.Parse(element.Attribute("x")?.Value ?? "0"),
						Y = int.Parse(element.Attribute("y")?.Value ?? "0"),
						Width = int.Parse(element.Attribute("width")?.Value ?? "0"),
						Height = int.Parse(element.Attribute("height")?.Value ?? "0")
					};
					subtextures.Add(info);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error parsing XML: {ex.Message}");
			}

			_subtextureCache[xmlPath] = subtextures;
			return subtextures;
		}

		private List<AnimationInfo> ParseAnimations(string xmlPath)
		{
			if (_animationCache.ContainsKey(xmlPath))
				return _animationCache[xmlPath];

			var animations = new List<AnimationInfo>();
			try
			{
				string fullPath = Path.Combine(_contentPath, xmlPath);
				if (!File.Exists(fullPath)) return animations;

				var doc = XDocument.Load(fullPath);
				var animationElements = doc.Descendants("animation");

				foreach (var element in animationElements)
				{
					var info = new AnimationInfo
					{
						Name = element.Attribute("name")?.Value ?? "unnamed",
						Delay = int.Parse(element.Attribute("delay")?.Value ?? "100")
					};
					animations.Add(info);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error parsing animations: {ex.Message}");
			}

			_animationCache[xmlPath] = animations;
			return animations;
		}

		private string[] GetAnimationTypeNames()
		{
			return Enum.GetNames(typeof(AnimationType));
		}

		YTBEntity BaseEntity = EntityYTBXmlTemplate.GenerateNew();
		int itemSelected = 0;

		internal string TraduceComponentName(string component)
		{
			switch (component)
			{
				case "TransformComponent": return "Componente de Transformación" + $" ({component})";
				case "SpriteComponent2D": return "Componente de Sprite 2D" + $" ({component})";
				case "AnimationComponent2D": return "Componente de Animación 2D" + $" ({component})";
				case "RigidBodyComponent2D": return "Componente de Cuerpo Rígido 2D" + $" ({component})";
				case "ButtonComponent2D": return "Componente de Botón 2D" + $" ({component})";
				case "InputComponent": return "Componente de Entrada" + $" ({component})";
				case "CameraComponent3D": return "Componente de Cámara 3D" + $" ({component})";
				case "ScriptComponent": return "Componente de Script" + $" ({component})";
				case "TileMapComponent2D": return "Componente de TileMap 2D" + $" ({component})";
				case "FontComponent2D": return "Componente de Texto 2D" + $" ({component})";
				case "ShaderComponent": return "Componente de Shader" + $" ({component})";
				case "ModelComponent3D": return "Componente de Modelo 3D" + $" ({component})";
				default: return component;
            }
		}
		public void Render()
		{
			ImGui.Begin("Entity Manager");

			List<string> componentes = new List<string> {
				"TransformComponent", "SpriteComponent2D", "AnimationComponent2D",
				"RigidBodyComponent2D", "ButtonComponent2D", "InputComponent", "CameraComponent3D",
				"ScriptComponent", "TileMapComponent2D", "FontComponent2D", "ShaderComponent", "ModelComponent3D"
			};

			var YTBEntity = _getSelectedEntity();
			if (YTBEntity != null)
			{
				ImGui.TextColored(new Num.Vector4(0.5f, 0.8f, 1f, 1f), $"Entidad: {YTBEntity.Name}");
				ImGui.Separator();

				string nombreTemp = YTBEntity.Name;
				ImGui.PushItemWidth(250);
				if (ImGui.InputText("Nombre", ref nombreTemp, 100))
				{
					YTBEntity.Name = nombreTemp;

				}
				ImGui.PopItemWidth();

				ImGui.Spacing();
				if (YTBEntity.Components == null) YTBEntity.Components = [];

				int componentesValidos = YTBEntity.ComponentsCount - BaseEntity.Components.Where(x => YTBEntity.Components.Any(s => x.Equals(s))).Count();
				ImGui.SeparatorText($"{componentesValidos} COMPONENTES");


				ImGui.Spacing();

				foreach (var component in YTBEntity.Components.ToImmutableArray())
				{
					if (EntityYTBXmlTemplate.GenerateNew().Components.Any(x => x.Equals(component))) continue;

					// Botón para eliminar el componente
					// Se coloca ANTES del CollapsingHeader para mejor layout
					// NOTA: Eliminación inmediata sin confirmación para mantener flujo de trabajo rápido.
					// El componente puede recuperarse usando "Añadir Componente".
					if (ImGui.SmallButton($"X##{component.ComponentName}_delete"))
					{
						DeleteComponent(component);
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetTooltip("Eliminar componente");
					}
					
					// Colocar el CollapsingHeader en la misma línea
					ImGui.SameLine();
					
					if (ImGui.CollapsingHeader(TraduceComponentName(component.ComponentName), ImGuiTreeNodeFlags.DefaultOpen))
					{
						ImGui.Indent(15);

						switch (component.ComponentName)
						{
							case "TransformComponent":
								RenderTransformComponent(component);
								componentes.Remove(component.ComponentName);
								break;
							case "SpriteComponent2D":
								RenderSpriteComponent2D(component);
								componentes.Remove(component.ComponentName);

								break;
							case "AnimationComponent2D":
								RenderAnimationComponent2D(component);
								componentes.Remove(component.ComponentName);

								break;
							case "RigidBodyComponent2D":
								RenderRigidBodyComponent2D(component);
								componentes.Remove(component.ComponentName);

								break;
							case "ButtonComponent2D":
								RenderButtonComponent2D(component);
								componentes.Remove(component.ComponentName);

								break;
							case "InputComponent":
								RenderInputComponent(component);
								componentes.Remove(component.ComponentName);

								break;
						case "CameraComponent3D":
							RenderCameraComponent3D(component);
								componentes.Remove(component.ComponentName);

								break;
							case "ScriptComponent":
								RenderScriptComponent(component);
								componentes.Remove(component.ComponentName);
								break;
							case "TileMapComponent2D":
								RenderTileMapComponent(component);
								componentes.Remove(component.ComponentName);
								break;
							case "FontComponent2D":
								RenderFontComponent2D(component);
								componentes.Remove(component.ComponentName);
								break;
						case "ShaderComponent":
							RenderShaderComponent(component);
								componentes.Remove(component.ComponentName);
								break;
						case "ModelComponent3D":
							RenderModelComponent3D(component);
								componentes.Remove(component.ComponentName);
								break;
							default:
								//RenderGenericComponent(component);
								break;
						}

						ImGui.Unindent(15);
					}
				}
			}
			ImGui.Separator();


			if (YTBEntity != null)
				if (ImGui.Button("Añadir Componente"))
				{
					ImGui.OpenPopup("Agregar Componente");
				}

			if (ImGui.BeginPopupModal("Agregar Componente", ImGuiWindowFlags.AlwaysAutoResize))
			{
				string[] unselected = componentes.Where(w => !YTBEntity.Components.Any(a => a.Equals(w))).ToArray();
				int contador = unselected.Count();
				ImGui.Combo("Componentes", ref itemSelected, unselected.ToArray(), contador);
				if (contador == 0) ImGui.CloseCurrentPopup();
				if (ImGui.Button("Agregar", new Num.Vector2(100, 0)))
				{
					switch (unselected[itemSelected])
					{
						case nameof(TransformComponent):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(TransformComponent)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(TransformComponent)).Propiedades
									= EntityYTBXmlTemplate.TransformTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(TransformComponent),
									Propiedades = EntityYTBXmlTemplate.TransformTemplate().Propiedades
								});
							}

							break;

						case nameof(SpriteComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(SpriteComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(SpriteComponent2D)).Propiedades
									= EntityYTBXmlTemplate.Sprite2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(SpriteComponent2D),
									Propiedades = EntityYTBXmlTemplate.Sprite2DTemplate().Propiedades
								});
							}

							break;

						case nameof(AnimationComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(AnimationComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(AnimationComponent2D)).Propiedades
									= EntityYTBXmlTemplate.Animation2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(AnimationComponent2D),
									Propiedades = EntityYTBXmlTemplate.Animation2DTemplate().Propiedades
								});
							}

							break;

						case nameof(RigidBodyComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(RigidBodyComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(RigidBodyComponent2D)).Propiedades
									= EntityYTBXmlTemplate.Rigibody2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(RigidBodyComponent2D),
									Propiedades = EntityYTBXmlTemplate.Rigibody2DTemplate().Propiedades
								});
							}

							break;

						case nameof(ButtonComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(ButtonComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(ButtonComponent2D)).Propiedades
									= EntityYTBXmlTemplate.Button2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(ButtonComponent2D),
									Propiedades = EntityYTBXmlTemplate.Button2DTemplate().Propiedades
								});
							}

							break;

						case nameof(InputComponent):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(InputComponent)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(InputComponent)).Propiedades
									= EntityYTBXmlTemplate.InputTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(InputComponent),
									Propiedades = EntityYTBXmlTemplate.InputTemplate().Propiedades
								});
							}

							break;

					case "CameraComponent3D":
						if (YTBEntity.Components.Any(x => x.ComponentName == "CameraComponent3D"))
						{
							YTBEntity.Components.FirstOrDefault(x => x.ComponentName == "CameraComponent3D").Propiedades
								= EntityYTBXmlTemplate.CameraTemplate().Propiedades;
						}
						else
						{
							YTBEntity.Components.Add(new()
							{
								ComponentName = "CameraComponent3D",
								Propiedades = EntityYTBXmlTemplate.CameraTemplate().Propiedades
							});
						}

						break;
						case nameof(ScriptComponent):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(ScriptComponent)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(ScriptComponent)).Propiedades
									= EntityYTBXmlTemplate.ScriptTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(ScriptComponent),
									Propiedades = EntityYTBXmlTemplate.ScriptTemplate().Propiedades
								});
							}

							break;
						case nameof(TileMapComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(TileMapComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(TileMapComponent2D)).Propiedades
									= EntityYTBXmlTemplate.TileMap2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(TileMapComponent2D),
									Propiedades = EntityYTBXmlTemplate.TileMap2DTemplate().Propiedades
								});
							}

							break;
						case nameof(FontComponent2D):
							if (YTBEntity.Components.Any(x => x.ComponentName == nameof(FontComponent2D)))
							{
								YTBEntity.Components.FirstOrDefault(x => x.ComponentName == nameof(FontComponent2D)).Propiedades
									= EntityYTBXmlTemplate.Font2DTemplate().Propiedades;
							}
							else
							{
								YTBEntity.Components.Add(new()
								{
									ComponentName = nameof(FontComponent2D),
									Propiedades = EntityYTBXmlTemplate.Font2DTemplate().Propiedades
								});
							}

							break;
					case "ShaderComponent":
						if (YTBEntity.Components.Any(x => x.ComponentName == "ShaderComponent"))
						{
							YTBEntity.Components.FirstOrDefault(x => x.ComponentName == "ShaderComponent").Propiedades
								= EntityYTBXmlTemplate.ShaderTemplate().Propiedades;
						}
						else
						{
							YTBEntity.Components.Add(new()
							{
								ComponentName = "ShaderComponent",
								Propiedades = EntityYTBXmlTemplate.ShaderTemplate().Propiedades
							});
						}

						break;
					case "ModelComponent3D":
						if (YTBEntity.Components.Any(x => x.ComponentName == "ModelComponent3D"))
						{
							YTBEntity.Components.FirstOrDefault(x => x.ComponentName == "ModelComponent3D").Propiedades
								= EntityYTBXmlTemplate.Model3DTemplate().Propiedades;
						}
						else
						{
							YTBEntity.Components.Add(new()
							{
								ComponentName = "ModelComponent3D",
								Propiedades = EntityYTBXmlTemplate.Model3DTemplate().Propiedades
							});
						}

						break;
					}
				}

				ImGui.SameLine();
				if (ImGui.Button("Cancelar", new Num.Vector2(100, 0)))
				{
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}

			ImGui.End();
		}

		private void RenderTileMapComponent(YTBComponents component)
		{
			string tilemapPath = component.Propiedades[0].Item2;

			ImGui.TextDisabled(component.Propiedades[0].Item1);
			ImGui.SameLine();
			RenderEnumCombo(component, component.Propiedades[0], TiledManager.GetAllTMXFiles());

			// Botón para aplicar dimensiones del tilemap al transform
			if (!string.IsNullOrWhiteSpace(tilemapPath))
			{
				if (ImGui.Button("Aplicar dimensiones al transform component"))
				{
					ApplyTileMapDimensionsToTransform(component, tilemapPath);
				}
			}
		}

		#region Transform Component
		private void RenderTransformComponent(YTBComponents component)
		{
			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 is "Position" or "Size")
				{
					ImGui.TextDisabled(prop.Item1);
					ImGui.SameLine();
					ImGui.PushID(prop.Item1);

					string[] partes = value.Split(",");
					string[] etiquetas = prop.Item1 == "Position"
						? new[] { "X", "Y", "Z" }
						: new[] { "Ancho", "Alto", "Largo" };

					for (int i = 0; i < partes.Length && i < etiquetas.Length; i++)
					{
						string v = partes[i].Trim();
						ImGui.PushItemWidth(60);
						if (ImGui.InputText(etiquetas[i], ref v, 8))
							partes[i] = float.TryParse(v, out float res) ? v : partes[i];
						ImGui.PopItemWidth();
						if (i < partes.Length - 1) ImGui.SameLine();
					}

					ImGui.PopID();
					string nuevoValor = string.Join(",", partes);
					if (nuevoValor != value)
					{
						UpdateProperty(component, prop.Item1, nuevoValor);
					}

					// Botón "Sincronizar" para Size - copia dimensiones del SpriteComponent2D
					if (prop.Item1 == "Size")
					{
						ImGui.SameLine();
						if (ImGui.Button("Sincronizar"))
						{
							SyncSizeWithSpriteComponent(component);
						}
						if (ImGui.IsItemHovered())
						{
							ImGui.SetTooltip("Usar las mismas dimensiones del Sprite");
						}
					}

					continue;
				}

				if (prop.Item1 == "Color")
				{
					Color myColor = Color.White;
					var propInfo = ColorPicker.ParseColorPropertyInfo(value);
					if (propInfo != null)
						myColor = (Color)propInfo.GetValue(null)!;

					ColorPicker.RenderColorCombo(prop.Item1, ref myColor, (string newName) =>
					{
						UpdateProperty(component, prop.Item1, newName);
					});
					continue;
				}

				if (prop.Item1 == "SpriteEffects")
				{
					RenderEnumCombo(component, prop, new[] { "None", "FlipHorizontally", "FlipVertically" });
					continue;
				}

				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Sprite Component 2D
		private void RenderSpriteComponent2D(YTBComponents component)
		{
			string currentAtlasPath = GetPropertyValue(component, "TextureAtlasPath");
			List<SubtextureInfo> availableSubtextures = new();

			if (!string.IsNullOrEmpty(currentAtlasPath))
			{
				availableSubtextures = ParseSubtextures(currentAtlasPath);
			}

			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "TextureAtlasPath")
				{
					ImGui.TextDisabled("Texture Atlas");
					ImGui.SameLine();

					int selectedIndex = _textureAtlasFiles.IndexOf(value);
					if (selectedIndex < 0) selectedIndex = 0;

					ImGui.PushItemWidth(300);
					if (ImGui.BeginCombo("##TextureAtlasPath", selectedIndex >= 0 && selectedIndex < _textureAtlasFiles.Count
						? _textureAtlasFiles[selectedIndex] : "Seleccionar..."))
					{
						for (int i = 0; i < _textureAtlasFiles.Count; i++)
						{
							bool isSelected = (selectedIndex == i);
							if (ImGui.Selectable(_textureAtlasFiles[i], isSelected))
							{
								UpdateProperty(component, prop.Item1, _textureAtlasFiles[i]);
								// Limpiar sprite name al cambiar de atlas
								UpdateProperty(component, "SpriteName", "");
							}
							if (isSelected)
								ImGui.SetItemDefaultFocus();
						}
						ImGui.EndCombo();
					}
					ImGui.PopItemWidth();
					continue;
				}

				if (prop.Item1 == "SpriteName")
				{
					ImGui.TextDisabled("Sprite");
					ImGui.SameLine();

					int selectedIndex = availableSubtextures.FindIndex(s => s.Name == value);
					string displayName = selectedIndex >= 0 ? availableSubtextures[selectedIndex].Name : "Seleccionar sprite...";

					ImGui.PushItemWidth(300);
					if (ImGui.BeginCombo("##SpriteName", displayName))
					{
						for (int i = 0; i < availableSubtextures.Count; i++)
						{
							bool isSelected = (selectedIndex == i);
							if (ImGui.Selectable(availableSubtextures[i].Name, isSelected))
							{
								var subtexture = availableSubtextures[i];
								UpdateProperty(component, prop.Item1, subtexture.Name);
								// Auto-completar SourceRectangle
								string rect = $"{subtexture.X},{subtexture.Y},{subtexture.Width},{subtexture.Height}";
								UpdateProperty(component, "SourceRectangle", rect);
							}
							if (isSelected)
								ImGui.SetItemDefaultFocus();
						}
						ImGui.EndCombo();
					}
					ImGui.PopItemWidth();
					continue;
				}

				if (prop.Item1 == "SourceRectangle")
				{
					RenderRectangle(component, prop, new[] { "X", "Y", "Ancho", "Alto" });
					continue;
				}

				if (prop.Item1 == "IsVisible")
				{
					try
					{
						bool isVisible = bool.Parse(value.ToLower());
						if (ImGui.Checkbox("Visible", ref isVisible))
						{
							UpdateProperty(component, prop.Item1, isVisible.ToString().ToLower());
						}
					}
					catch { }
					continue;
				}

				if(prop.Item1 == "2.5D")
				{

					bool is2_5D = false;
					bool.TryParse(value, out is2_5D);

                    if(ImGui.Checkbox("Renderizar 2.5D", ref is2_5D))
					{
                        UpdateProperty(component, "2.5D", is2_5D.ToString());
                    }

                    continue;

                }

                ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Animation Component 2D
		private void RenderAnimationComponent2D(YTBComponents component)
		{
			string currentAtlasPath = GetPropertyValue(component, "TextureAtlasPath");
			List<AnimationInfo> availableAnimations = new();

			if (!string.IsNullOrEmpty(currentAtlasPath))
			{
				availableAnimations = ParseAnimations(currentAtlasPath);
			}

			var animationTypeNames = GetAnimationTypeNames();

			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "TextureAtlasPath")
				{
					ImGui.TextDisabled("Texture Atlas");
					ImGui.SameLine();

					int selectedIndex = _textureAtlasFiles.IndexOf(value);
					if (selectedIndex < 0) selectedIndex = 0;

					ImGui.PushItemWidth(300);
					if (ImGui.BeginCombo("##AnimTextureAtlasPath", selectedIndex >= 0 && selectedIndex < _textureAtlasFiles.Count
						? _textureAtlasFiles[selectedIndex] : "Seleccionar..."))
					{
						for (int i = 0; i < _textureAtlasFiles.Count; i++)
						{
							bool isSelected = (selectedIndex == i);
							if (ImGui.Selectable(_textureAtlasFiles[i], isSelected))
							{
								UpdateProperty(component, prop.Item1, _textureAtlasFiles[i]);
								// Limpiar animaciones al cambiar de atlas
								UpdateProperty(component, "AnimationBindings", "");
							}
							if (isSelected)
								ImGui.SetItemDefaultFocus();
						}
						ImGui.EndCombo();
					}
					ImGui.PopItemWidth();
					continue;
				}

				if (prop.Item1 == "AnimationBindings")
				{
					ImGui.TextDisabled("Animaciones Vinculadas");
					ImGui.Spacing();

					// Formato: AnimationType:AnimationName,AnimationType:AnimationName
					var bindings = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
										.Select(s => s.Trim())
										.Where(s => s.Contains(':'))
										.Select(s =>
										{
											var parts = s.Split(':');
											return (Type: parts[0], Name: parts[1]);
										})
										.ToList();

					for (int i = 0; i < bindings.Count; i++)
					{
						ImGui.PushID($"binding_{i}");

						// Dropdown para AnimationType
						int typeIndex = Array.IndexOf(animationTypeNames, bindings[i].Type);
						if (typeIndex < 0) typeIndex = 0;

						ImGui.PushItemWidth(120);
						if (ImGui.BeginCombo($"##type_{i}", animationTypeNames[typeIndex]))
						{
							for (int t = 0; t < animationTypeNames.Length; t++)
							{
								bool isSelected = (typeIndex == t);
								if (ImGui.Selectable(animationTypeNames[t], isSelected))
								{
									bindings[i] = (animationTypeNames[t], bindings[i].Name);
									UpdateProperty(component, prop.Item1,
										string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
								}
								if (isSelected)
									ImGui.SetItemDefaultFocus();
							}
							ImGui.EndCombo();
						}
						ImGui.PopItemWidth();

						ImGui.SameLine();
						ImGui.Text("→");
						ImGui.SameLine();

						// Dropdown para Animation Name del XML
						int animIndex = availableAnimations.FindIndex(a => a.Name == bindings[i].Name);
						string displayAnim = animIndex >= 0 ? availableAnimations[animIndex].Name : bindings[i].Name;

						ImGui.PushItemWidth(200);
						if (ImGui.BeginCombo($"##anim_{i}", displayAnim))
						{
							for (int a = 0; a < availableAnimations.Count; a++)
							{
								bool isSelected = (animIndex == a);
								if (ImGui.Selectable(availableAnimations[a].Name, isSelected))
								{
									bindings[i] = (bindings[i].Type, availableAnimations[a].Name);
									UpdateProperty(component, prop.Item1,
										string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
								}
								if (isSelected)
									ImGui.SetItemDefaultFocus();
							}
							ImGui.EndCombo();
						}
						ImGui.PopItemWidth();

						ImGui.SameLine();
						if (ImGui.Button($"X##remove_{i}"))
						{
							bindings.RemoveAt(i);
							UpdateProperty(component, prop.Item1,
								string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
						}

						ImGui.PopID();
					}

					if (ImGui.Button("+ Agregar Vinculación"))
					{
						if (availableAnimations.Any())
						{
							bindings.Add((animationTypeNames[0], availableAnimations[0].Name));
							UpdateProperty(component, prop.Item1,
								string.Join(",", bindings.Select(b => $"{b.Type}:{b.Name}")));
						}
					}

					if (!availableAnimations.Any())
					{
						ImGui.TextDisabled("(Selecciona un Texture Atlas con animaciones)");
					}

					continue;
				}

				if (prop.Item1 == "CurrentAnimationType")
				{
					RenderEnumCombo(component, prop, animationTypeNames);
					continue;
				}

				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region RigidBody Component 2D
		private void RenderRigidBodyComponent2D(YTBComponents component)
		{
			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "OffSetCollision" || prop.Item1 == "Velocity")
				{
					RenderVector2(component, prop, new[] { "X", "Y" });
					continue;
				}

				if (prop.Item1 == "GameType")
				{
					RenderEnumCombo(component, prop, new[] { "TopDown", "Platform" });
					continue;
				}

				if (prop.Item1 == "Mass")
				{
					RenderEnumCombo(component, prop, new[] {
					   "Collision", "Slow", "NoCollision"
					});
					continue;
				}

				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Button Component 2D
		private void RenderButtonComponent2D(YTBComponents component)
		{
			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "IsActive")
				{
					try
					{
						bool isActive = bool.Parse(value.ToLower());
						if (ImGui.Checkbox("Activo", ref isActive))
						{
							UpdateProperty(component, prop.Item1, isActive.ToString().ToLower());
						}
					}
					catch { }
					continue;
				}

				if (prop.Item1 == "EffectiveArea")
				{
					RenderRectangle(component, prop, new[] { "X", "Y", "Ancho", "Alto" });
					continue;
				}

				if (prop.Item1 == "Description")
				{
					ImGui.TextDisabled("Descripción");
					ImGui.PushItemWidth(300);
					if (ImGui.InputTextMultiline("##Description", ref value, 500, new Num.Vector2(300, 60)))
					{
						UpdateProperty(component, prop.Item1, value);
					}
					ImGui.PopItemWidth();
					continue;
				}

				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Input Component
		private void RenderInputComponent(YTBComponents component)
		{
			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "InputsInUse")
				{
					ImGui.TextDisabled("Inputs en Uso");
					var inputs = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
									 .Select(s => s.Trim()).ToList();

					bool hasMouse = inputs.Contains("HasMouse");
					bool hasKeyboard = inputs.Contains("HasKeyboard");
					bool hasGamepad = inputs.Contains("HasGamepad");

					if (ImGui.Checkbox("Mouse", ref hasMouse))
					{
						if (hasMouse && !inputs.Contains("HasMouse")) inputs.Add("HasMouse");
						else inputs.Remove("HasMouse");
						UpdateProperty(component, prop.Item1, string.Join(",", inputs));
					}

					if (ImGui.Checkbox("Teclado", ref hasKeyboard))
					{
						if (hasKeyboard && !inputs.Contains("HasKeyboard")) inputs.Add("HasKeyboard");
						else inputs.Remove("HasKeyboard");
						UpdateProperty(component, prop.Item1, string.Join(",", inputs));
					}

					if (ImGui.Checkbox("Gamepad", ref hasGamepad))
					{
						if (hasGamepad && !inputs.Contains("HasGamepad")) inputs.Add("HasGamepad");
						else inputs.Remove("HasGamepad");
						UpdateProperty(component, prop.Item1, string.Join(",", inputs));
					}
					continue;
				}

				if (prop.Item1 == "GamePadIndex")
				{
					RenderEnumCombo(component, prop, new[] { " ", "One", "Two", "Three", "Four" });
					continue;
				}

                if (prop.Item1 == "KeyboardMappings")
                {
                    ImGui.TextDisabled(prop.Item1 == "KeyboardMappings" ? "Mapeo de Teclado" : "Mapeo de Mouse");

                    // 1. PREPARACIÓN DE DATOS (Simulación de tus opciones disponibles)
                    // En tu código real, estas deberían venir de una clase estática o configuración global.
                    string[] todasLasAcciones = { "MoveUp", "MoveDown", "MoveLeft", "MoveRight", "Jump", "Attack" };
                    string[] todasLasTeclas = { "W", "A", "S", "D", "Space", "Enter", "Esc", "Shift", "Ctrl" };

                    // 2. PARSEO DEL STRING ACTUAL (Deserialización)
                    // Separamos por comas y luego por dos puntos para obtener pares [Acción, Tecla]
                    var paresActuales = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(x => x.Trim().Split(':'))
                                             .Where(x => x.Length == 2) // Aseguramos formato correcto
                                             .Select(x => new { Accion = x[0].Trim(), Tecla = x[1].Trim() })
                                             .ToList();

                    bool huboCambios = false;

                    // 3. BUCLE DE RENDERIZADO (Dibujar la UI)
                    // Usamos un for loop normal para poder modificar los valores por índice si es necesario
                    for (int i = 0; i < paresActuales.Count; i++)
                    {
                        // Importante: PushID asegura que los combos de la fila 1 no interfieran con la fila 2
                        ImGui.PushID(i);

                        // --- COMBO 1: LA ACCIÓN (Izquierda) ---
                        string accionActual = paresActuales[i].Accion;
                        int indexAccion = Array.IndexOf(todasLasAcciones, accionActual);
                        if (indexAccion == -1) indexAccion = 0; // Fallback si no existe

                        ImGui.PushItemWidth(150); // Ancho del primer combo
                        if (ImGui.Combo("##Accion", ref indexAccion, todasLasAcciones, todasLasAcciones.Length))
                        {
                            // Actualizamos la lista temporal
                            paresActuales[i] = new { Accion = todasLasAcciones[indexAccion], Tecla = paresActuales[i].Tecla };
                            huboCambios = true;
                        }
                        ImGui.PopItemWidth();

                        ImGui.SameLine(); // Pone el siguiente elemento en la misma línea

                        // --- COMBO 2: LA TECLA (Derecha) ---
                        string teclaActual = paresActuales[i].Tecla;
                        int indexTecla = Array.IndexOf(todasLasTeclas, teclaActual);
                        if (indexTecla == -1) indexTecla = 0;

                        ImGui.PushItemWidth(100); // Ancho del segundo combo
						{
							if (ImGui.Combo("##Tecla", ref indexTecla, todasLasTeclas, todasLasTeclas.Length))
							{
								// Actualizamos la lista temporal
								paresActuales[i] = new { Accion = paresActuales[i].Accion, Tecla = todasLasTeclas[indexTecla] };
								huboCambios = true;
							}
							ImGui.PopItemWidth();
						}

                        // Opcional: Botón para borrar la fila
                        ImGui.SameLine();
                        int Buttonid = GetUniqueImGuiID.GetID();

                        if (ImGui.Button("X"))
                        {
                            paresActuales.RemoveAt(i);
                            huboCambios = true;
                            i--; // Ajustamos índice porque borramos un elemento
                        }
                        GetUniqueImGuiID.ReleaseID(Buttonid);

                        ImGui.PopID();
                    }

                    // Botón para agregar nueva fila vacía (opcional)
                    if (ImGui.Button("+ Agregar Mapeo"))
                    {
                        paresActuales.Add(new { Accion = todasLasAcciones[0], Tecla = todasLasTeclas[0] });
                        huboCambios = true;
                    }

                    // 4. GUARDADO (Serialización)
                    // Si algo cambió en la UI, reconstruimos el string y actualizamos
                    if (huboCambios)
                    {
                        string nuevoValor = string.Join(", ", paresActuales.Select(p => $"{p.Accion}:{p.Tecla}"));
                        UpdateProperty(component, prop.Item1, nuevoValor);
                    }

                    continue; // Saltamos el resto del código para que no dibuje el InputText viejo
                }
    //            ImGui.PushItemWidth(250);
				//if (ImGui.InputText(prop.Item1, ref value, 100))
				//{
				//	UpdateProperty(component, prop.Item1, value);
				//}
				//ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Camera Component 3D
		private void RenderCameraComponent3D(YTBComponents component)
		{
			var currentScene = _getCurrentScene();
			List<string> entityNames = new();

			if (currentScene?.Entities != null)
			{
				entityNames = currentScene.Entities
					.Where(e => !string.IsNullOrEmpty(e.Name))
					.Select(e => e.Name)
					.ToList();
			}

			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "EntityName")
				{
					ImGui.TextDisabled("Entidad a Seguir");
					ImGui.SameLine();

					int selectedIndex = entityNames.IndexOf(value);
					string displayName = selectedIndex >= 0 ? entityNames[selectedIndex] : "Seleccionar entidad...";

					ImGui.PushItemWidth(250);
					if (ImGui.BeginCombo("##EntityName", displayName))
					{
						for (int i = 0; i < entityNames.Count; i++)
						{
							bool isSelected = (selectedIndex == i);
							if (ImGui.Selectable(entityNames[i], isSelected))
							{
								UpdateProperty(component, prop.Item1, entityNames[i]);
							}
							if (isSelected)
								ImGui.SetItemDefaultFocus();
						}
						ImGui.EndCombo();
					}
					ImGui.PopItemWidth();

					if (!entityNames.Any())
					{
						ImGui.TextDisabled("(No hay entidades en esta escena)");
					}

					continue;
				}

				if (prop.Item1 == "InitialPosition")
				{
					RenderVector3(component, prop, new[] { "X", "Y", "Z" });
					continue;
				}

				if (prop.Item1 == "OffsetCamera")
				{
					ImGui.TextDisabled("Offset de Cámara");
					ImGui.SameLine();
					ImGui.PushID("OffsetCamera");

					string[] partes = value.Split(",");
					string[] etiquetas = new[] { "X", "Y", "Z" };

					for (int i = 0; i < 3; i++)
					{
						string v = (i < partes.Length) ? partes[i].Trim() : "0";
						ImGui.PushItemWidth(60);
						if (ImGui.InputText(etiquetas[i], ref v, 10))
						{
							if (i < partes.Length)
								partes[i] = float.TryParse(v, out _) ? v : partes[i];
						}
						ImGui.PopItemWidth();
						if (i < 2) ImGui.SameLine();
					}

					ImGui.PopID();
					string nuevoValor = string.Join(",", partes);
					if (nuevoValor != value)
					{
						UpdateProperty(component, prop.Item1, nuevoValor);
					}

					// Tooltip de ayuda
					if (ImGui.IsItemHovered())
					{
						ImGui.SetTooltip("Desplazamiento de la cámara respecto a la entidad.\nY positivo = arriba, Z negativo = detrás del objetivo.");
					}
					continue;
				}

				if (prop.Item1 is "AngleView" or "NearRender" or "FarRender")
				{
					float floatValue = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed)
						? parsed
						: 0f;
					ImGui.PushItemWidth(100);
					if (ImGui.InputFloat(prop.Item1, ref floatValue))
					{
						UpdateProperty(component, prop.Item1, floatValue.ToString(CultureInfo.InvariantCulture));
					}
					ImGui.PopItemWidth();
					continue;
				}

				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}
		#endregion

		#region Script Component
		private void RenderScriptComponent(YTBComponents component)
		{
			try
			{
				if (component.Propiedades[0].Item1 == "Scripts")
				{
					string mainScript = component.Propiedades[0].Item2.Split("&;&")[0];

					string scriptType = mainScript.Split("&:&")[0];

					if (Enum.TryParse(typeof(ScriptComponentType), scriptType, out var result))
					{
						string scriptPath = mainScript.Split("&:&")[1];

						string[] scripts = GetAllScripts();
						int indexOfPredet;

						if (String.IsNullOrEmpty(scriptPath))
						{
							indexOfPredet = 0;
						}
						else
						{
							indexOfPredet = Array.IndexOf(scripts, scriptPath);
						}

						ImGui.Combo("Scripts", ref indexOfPredet, scripts, scripts.Length);

						component.Propiedades[0] = new Tuple<string, string>("Scripts", scriptType + "&:&" + scripts[indexOfPredet] + "&;&");

					}
				}
			}
			catch (Exception ex)
			{
				new GameWontRun(ex, GameWontRun.YTBErrors.ScriptHasError);
			}

		}

		private string[] GetAllScripts()
		{
			return YTBGame.ScriptRegistry.GetScripts().Select(s => s.Key).ToArray();
        }

		#endregion

		#region Helper Methods
		private string GetPropertyValue(YTBComponents component, string propertyName)
		{
			var prop = component.Propiedades.FirstOrDefault(p => p.Item1 == propertyName);
			return prop?.Item2 ?? string.Empty;
		}

		private void RenderVector2(YTBComponents component, Tuple<string, string> prop, string[] labels)
		{
			ImGui.TextDisabled(prop.Item1);
			ImGui.SameLine();
			ImGui.PushID(prop.Item1);

			string[] partes = prop.Item2.Split(",");

			for (int i = 0; i < partes.Length && i < 2; i++)
			{
				string v = partes[i].Trim();
				ImGui.PushItemWidth(60);
				if (ImGui.InputText(labels[i], ref v, 10))
					partes[i] = float.TryParse(v, out float res) ? v : partes[i];
				ImGui.PopItemWidth();
				if (i < 1) ImGui.SameLine();
			}

			ImGui.PopID();
			string nuevoValor = string.Join(",", partes);
			if (nuevoValor != prop.Item2)
			{
				UpdateProperty(component, prop.Item1, nuevoValor);
			}
		}

		private void RenderVector3(YTBComponents component, Tuple<string, string> prop, string[] labels)
		{
			ImGui.TextDisabled(prop.Item1);
			ImGui.SameLine();
			ImGui.PushID(prop.Item1);

			string[] partes = prop.Item2.Split(",");

			for (int i = 0; i < partes.Length && i < 3; i++)
			{
				string v = partes[i].Trim();
				ImGui.PushItemWidth(60);
				if (ImGui.InputText(labels[i], ref v, 10))
					partes[i] = float.TryParse(v, out float res) ? v : partes[i];
				ImGui.PopItemWidth();
				if (i < 2) ImGui.SameLine();
			}

			ImGui.PopID();
			string nuevoValor = string.Join(",", partes);
			if (nuevoValor != prop.Item2)
			{
				UpdateProperty(component, prop.Item1, nuevoValor);
			}
		}

		private void RenderRectangle(YTBComponents component, Tuple<string, string> prop, string[] labels)
		{
			ImGui.TextDisabled(prop.Item1);
			ImGui.SameLine();
			ImGui.PushID(prop.Item1);

			string[] partes = prop.Item2.Split(",");

			for (int i = 0; i < partes.Length && i < 4; i++)
			{
				string v = partes[i].Trim();
				ImGui.PushItemWidth(55);
				if (ImGui.InputText(labels[i], ref v, 8))
					partes[i] = int.TryParse(v, out int res) ? v : partes[i];
				ImGui.PopItemWidth();
				if (i < 3) ImGui.SameLine();
			}

			ImGui.PopID();
			string nuevoValor = string.Join(",", partes);
			if (nuevoValor != prop.Item2)
			{
				UpdateProperty(component, prop.Item1, nuevoValor);
			}
		}

		private void RenderEnumCombo(YTBComponents component, Tuple<string, string> prop, string[] options)
		{
			ImGui.TextDisabled(prop.Item1);
			ImGui.SameLine();

			int selectedIndex = Array.IndexOf(options, prop.Item2);
			if (selectedIndex < 0) selectedIndex = 0;

			ImGui.PushItemWidth(150);
			if (options.Length == 0) return;
			if (ImGui.BeginCombo($"##{prop.Item1}", options[selectedIndex]))
			{
				for (int i = 0; i < options.Length; i++)
				{
					bool isSelected = (selectedIndex == i);
					ImGui.PushID("model_" + i);
					if (ImGui.Selectable(options[i], isSelected))
					{
						UpdateProperty(component, prop.Item1, options[i]);
					}
					ImGui.PopID();
					if (isSelected)
						ImGui.SetItemDefaultFocus();
				}
				ImGui.EndCombo();
			}
			ImGui.PopItemWidth();
		}

		private void UpdateProperty(YTBComponents component, string propertyName, string newValue)
		{
			int index = component.Propiedades.FindIndex(x => x.Item1 == propertyName);
			if (index >= 0)
			{
				component.Propiedades[index] = new(propertyName, newValue);

			}
		}
		#endregion

		/// <summary>
		/// Sincroniza el Size del TransformComponent con las dimensiones del SpriteComponent2D.
		/// </summary>
		private void SyncSizeWithSpriteComponent(YTBComponents transformComponent)
		{
			var entity = _getSelectedEntity();
			if (entity == null) return;

			// Buscar el SpriteComponent2D en la misma entidad
			var spriteComponent = entity.Components.FirstOrDefault(c => c.ComponentName == "SpriteComponent2D");
			if (spriteComponent == null) return;

			// Obtener el SourceRectangle del sprite (formato: "X,Y,Width,Height")
			var sourceRectProp = spriteComponent.Propiedades.FirstOrDefault(p => p.Item1 == "SourceRectangle");
			if (sourceRectProp == null) return;

			string sourceRect = sourceRectProp.Item2;
			if (string.IsNullOrWhiteSpace(sourceRect)) return;

			// Parsear el SourceRectangle
			string[] parts = sourceRect.Split(',');
			if (parts.Length < 4) return;

			// Extraer Width y Height (índices 2 y 3)
			if (int.TryParse(parts[2].Trim(), out int width) && int.TryParse(parts[3].Trim(), out int height))
			{
				// Actualizar el Size del TransformComponent (formato: "Width,Height,Depth")
				string newSize = $"{width},{height},0";
				UpdateProperty(transformComponent, "Size", newSize);
			}
		}

		/// <summary>
		/// Aplica las dimensiones totales del TileMap al Size del TransformComponent.
		/// </summary>
		private void ApplyTileMapDimensionsToTransform(YTBComponents tileMapComponent, string tmxPath)
		{
			var entity = _getSelectedEntity();
			if (entity == null) return;

			// Buscar el TransformComponent en la misma entidad
			var transformComponent = entity.Components.FirstOrDefault(c => c.ComponentName == "TransformComponent");
			if (transformComponent == null)
			{
				Console.WriteLine("Error: No se encontró TransformComponent en la entidad.");
				return;
			}

			try
			{
				// Parsear el archivo TMX para obtener las dimensiones
				// Validación de seguridad: evitar path traversal
				string sanitizedPath = tmxPath.TrimStart('\\', '/');
				string fullPath = Path.GetFullPath(Path.Combine(_contentPath, sanitizedPath));
				string contentFullPath = Path.GetFullPath(_contentPath);
				
				// Verificar que el path resultante está dentro del directorio de contenido
				if (!fullPath.StartsWith(contentFullPath, StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine($"Error: Ruta de archivo TMX inválida: {tmxPath}");
					return;
				}
				
				if (!File.Exists(fullPath))
				{
					Console.WriteLine($"Error: Archivo TMX no encontrado: {fullPath}");
					return;
				}

				var doc = XDocument.Load(fullPath);
				var mapElement = doc.Root;
				if (mapElement == null)
				{
					Console.WriteLine($"Error: Archivo TMX inválido (sin elemento raíz): {tmxPath}");
					return;
				}

				// Obtener Width, Height, TileWidth, TileHeight con validación
				if (!int.TryParse(mapElement.Attribute("width")?.Value, out int width) || width <= 0)
				{
					Console.WriteLine($"Error: Atributo 'width' inválido en TMX: {tmxPath}");
					return;
				}
				if (!int.TryParse(mapElement.Attribute("height")?.Value, out int height) || height <= 0)
				{
					Console.WriteLine($"Error: Atributo 'height' inválido en TMX: {tmxPath}");
					return;
				}
				if (!int.TryParse(mapElement.Attribute("tilewidth")?.Value, out int tileWidth) || tileWidth <= 0)
				{
					Console.WriteLine($"Error: Atributo 'tilewidth' inválido en TMX: {tmxPath}");
					return;
				}
				if (!int.TryParse(mapElement.Attribute("tileheight")?.Value, out int tileHeight) || tileHeight <= 0)
				{
					Console.WriteLine($"Error: Atributo 'tileheight' inválido en TMX: {tmxPath}");
					return;
				}

				// Calcular las dimensiones totales del mapa
				int totalWidth = width * tileWidth;
				int totalHeight = height * tileHeight;

				// Actualizar el Size del TransformComponent
				string newSize = $"{totalWidth},{totalHeight},0";
				UpdateProperty(transformComponent, "Size", newSize);
				
				Console.WriteLine($"Dimensiones del tilemap aplicadas: {totalWidth}x{totalHeight}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error aplicando dimensiones del tilemap: {ex.Message}");
			}
		}

		#region Font Component 2D
		private List<string> _fontFiles = new();

		private void LoadFontFiles()
		{
			_fontFiles.Clear();

			if (!Directory.Exists(_contentPath))
				return;

			// HashSet para evitar duplicados cuando existan tanto .spritefont como .xnb
			var fontPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			// Buscar archivos .spritefont (fuentes sin compilar)
			var spriteFontFiles = Directory.GetFiles(_contentPath, "*.spritefont", SearchOption.AllDirectories);
			foreach (var file in spriteFontFiles)
			{
				string relativePath = Path.GetRelativePath(_contentPath, file)
					.Replace("\\", "/")
					.Replace(".spritefont", ""); // MonoGame Content Pipeline usa nombres sin extensión
				fontPaths.Add(relativePath);
			}

			// Buscar archivos .xnb (fuentes compiladas) - MonoGame compila las fuentes a este formato
			var xnbFiles = Directory.GetFiles(_contentPath, "*.xnb", SearchOption.AllDirectories);
			foreach (var file in xnbFiles)
			{
				// Filtrar solo archivos que probablemente sean fuentes
				// Las fuentes suelen estar en carpetas "Fonts" o "Font"
				string relativePath = Path.GetRelativePath(_contentPath, file)
					.Replace("\\", "/");

				// Solo considerar archivos XNB que estén en carpetas relacionadas con fuentes
				// o que tengan nombres típicos de fuentes
				if (relativePath.Contains("font", StringComparison.OrdinalIgnoreCase) || 
				    relativePath.Contains("Fonts/", StringComparison.OrdinalIgnoreCase) ||
				    relativePath.Contains("Font/", StringComparison.OrdinalIgnoreCase))
				{
					string pathWithoutExtension = relativePath.Replace(".xnb", "");
					fontPaths.Add(pathWithoutExtension);
				}
			}

			// Agregar a la lista ordenada
			_fontFiles.AddRange(fontPaths.OrderBy(x => x));
		}

		private void RenderFontComponent2D(YTBComponents component)
		{
			// Cargar fuentes si no se han cargado aún
			if (_fontFiles.Count == 0)
			{
				LoadFontFiles();
			}

			foreach (var prop in component.Propiedades.ToImmutableArray())
			{
				string value = prop.Item2;

				if (prop.Item1 == "Texto")
				{
					ImGui.TextDisabled("Texto");
					ImGui.PushItemWidth(300);
					if (ImGui.InputTextMultiline("##Texto", ref value, 500, new Num.Vector2(300, 100)))
					{
						UpdateProperty(component, prop.Item1, value);
					}
					ImGui.PopItemWidth();
					continue;
				}

				if (prop.Item1 == "Font")
				{
					ImGui.TextDisabled("Fuente");
					ImGui.SameLine();

					int selectedIndex = _fontFiles.IndexOf(value);
					if (selectedIndex < 0) selectedIndex = 0;

					ImGui.PushItemWidth(300);
					if (ImGui.BeginCombo("##Font", selectedIndex >= 0 && selectedIndex < _fontFiles.Count
						? _fontFiles[selectedIndex] : "Seleccionar fuente..."))
					{
						for (int i = 0; i < _fontFiles.Count; i++)
						{
							bool isSelected = (selectedIndex == i);
							if (ImGui.Selectable(_fontFiles[i], isSelected))
							{
								UpdateProperty(component, prop.Item1, _fontFiles[i]);
							}
							if (isSelected)
								ImGui.SetItemDefaultFocus();
						}
						ImGui.EndCombo();
					}
					ImGui.PopItemWidth();

					// Botón para recargar fuentes
					if (ImGui.Button("🔄 Recargar fuentes"))
					{
						LoadFontFiles();
					}
					continue;
				}

				// Fallback para propiedades no manejadas
				ImGui.PushItemWidth(250);
				if (ImGui.InputText(prop.Item1, ref value, 100))
				{
					UpdateProperty(component, prop.Item1, value);
				}
				ImGui.PopItemWidth();
			}
		}

		private void RenderShaderComponent(YTBComponents component)
		{
			var shaderPathProp = component.Propiedades.FirstOrDefault(p => p.Item1 == "ShaderPath");
			var isActiveProp = component.Propiedades.FirstOrDefault(p => p.Item1 == "IsActive");
			var paramsProp = component.Propiedades.FirstOrDefault(p => p.Item1 == "params");

			if (shaderPathProp != null)
			{
				string shaderPath = shaderPathProp.Item2;
				ImGui.TextDisabled("Ruta del Shader");
				ImGui.SameLine();
				ImGui.PushItemWidth(300);
				if (ImGui.InputText("##ShaderPath", ref shaderPath, 200))
				{
					UpdateProperty(component, shaderPathProp.Item1, shaderPath);
				}
				ImGui.PopItemWidth();
			}

			if (isActiveProp != null)
			{
				bool isActive = isActiveProp.Item2.Equals("true", StringComparison.OrdinalIgnoreCase);
				if (ImGui.Checkbox("Shader Activo", ref isActive))
				{
					UpdateProperty(component, isActiveProp.Item1, isActive ? "true" : "false");
				}
			}

			if (paramsProp != null)
			{
				ImGui.SeparatorText("Parámetros");
				var paramEntries = ParseShaderParams(paramsProp.Item2);
				bool changed = false;

				for (int i = 0; i < paramEntries.Count; i++)
				{
					var entry = paramEntries[i];
					ImGui.PushID($"param_{i}");

					ImGui.PushItemWidth(180);
					string nameValue = entry.Name;
					if (ImGui.InputText("##ParamName", ref nameValue, 50))
					{
						entry.Name = nameValue;
						changed = true;
					}
					ImGui.PopItemWidth();

					ImGui.SameLine();
					ImGui.Text("=");
					ImGui.SameLine();

					ImGui.PushItemWidth(120);
					string numericValue = entry.Value;
					if (ImGui.InputText("##ParamValue", ref numericValue, 30))
					{
						if (float.TryParse(numericValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
						{
							entry.Value = numericValue;
							changed = true;
						}
						else
						{
							ImGui.SetTooltip("El valor debe ser numérico.");
						}
					}
					ImGui.PopItemWidth();

					ImGui.SameLine();
					if (ImGui.SmallButton("X"))
					{
						paramEntries.RemoveAt(i);
						changed = true;
						i--;
					}

					ImGui.PopID();
				}

				if (ImGui.Button("+ Parámetro"))
				{
					paramEntries.Add(new ShaderParamEntry
					{
						Name = $"param{paramEntries.Count + 1}",
						Value = "0"
					});
					changed = true;
				}

				if (changed)
				{
					UpdateProperty(component, paramsProp.Item1, SerializeShaderParams(paramEntries));
				}
			}
		}
		#endregion

		private static List<ShaderParamEntry> ParseShaderParams(string rawParams)
		{
			var entries = new List<ShaderParamEntry>();
			if (string.IsNullOrWhiteSpace(rawParams))
				return entries;

			var pairs = rawParams.Split(',', StringSplitOptions.RemoveEmptyEntries);
			foreach (var pair in pairs)
			{
				var parts = pair.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 2)
					continue;

				entries.Add(new ShaderParamEntry
				{
					Name = parts[0].Trim(),
					Value = parts[1].Trim()
				});
			}

			return entries;
		}

		private static string SerializeShaderParams(IEnumerable<ShaderParamEntry> entries)
		{
			return string.Join(",", entries
				.Where(e => !string.IsNullOrWhiteSpace(e.Name))
				.Select(e => $"{e.Name}={e.Value}"));
		}

		private sealed class ShaderParamEntry
		{
			public string Name { get; set; } = string.Empty;
			public string Value { get; set; } = "0";
		}

		/// <summary>
		/// "Elimina" el componente copiando las propiedades del template vacío.
		/// Esto hace que el componente se oculte de la UI y no se procese en runtime.
		/// </summary>
		private void DeleteComponent(YTBComponents component)
		{
			// Obtener el template vacío (GenerateNew() contiene componentes con valores vacíos)
			var emptyEntity = EntityYTBXmlTemplate.GenerateNew();
			var emptyComponent = emptyEntity.Components.FirstOrDefault(c => c.ComponentName == component.ComponentName);

			if (emptyComponent == null)
			{
				Console.WriteLine($"Advertencia: No se encontró template vacío para el componente '{component.ComponentName}'");
				return;
			}

			// Copiar las propiedades del template vacío al componente actual
			// Esto hará que el componente coincida con el template vacío y se "oculte"
			component.Propiedades.Clear();
			component.Propiedades.AddRange(emptyComponent.Propiedades);
			
			Console.WriteLine($"Componente '{component.ComponentName}' eliminado (igualado al template vacío)");
		}

		#region Helper Classes
		/// <summary>
		/// Stores subtexture data parsed from a texture atlas XML.
		/// Almacena datos de subtexturas parseados desde un XML de atlas.
		/// </summary>
		private class SubtextureInfo
		{
			/// <summary>
			/// Subtexture name.
			/// Nombre de la subtextura.
			/// </summary>
			public string Name { get; set; }
			/// <summary>
			/// X coordinate in pixels.
			/// Coordenada X en píxeles.
			/// </summary>
			public int X { get; set; }
			/// <summary>
			/// Y coordinate in pixels.
			/// Coordenada Y en píxeles.
			/// </summary>
			public int Y { get; set; }
			/// <summary>
			/// Width in pixels.
			/// Ancho en píxeles.
			/// </summary>
			public int Width { get; set; }
			/// <summary>
			/// Height in pixels.
			/// Alto en píxeles.
			/// </summary>
			public int Height { get; set; }
		}

		/// <summary>
		/// Stores animation data parsed from a texture atlas XML.
		/// Almacena datos de animación parseados desde un XML de atlas.
		/// </summary>
		private class AnimationInfo
		{
			/// <summary>
			/// Animation name.
			/// Nombre de la animación.
			/// </summary>
			public string Name { get; set; }
			/// <summary>
			/// Frame delay in milliseconds.
			/// Retardo de frames en milisegundos.
			/// </summary>
			public int Delay { get; set; }
		}
		#endregion

		#region ModelComponent3D
		/// <summary>
		/// Renders the ModelComponent3D editor.
		/// Renderiza el editor del componente ModelComponent3D.
		/// </summary>
		/// <param name="component">Component to render. Componente a renderizar.</param>
		private void RenderModelComponent3D(YTBComponents component)
		{
			var modelPathProp = component.Propiedades.FirstOrDefault(p => p.Item1 == "ModelPath");
			var isVisibleProp = component.Propiedades.FirstOrDefault(p => p.Item1 == "IsVisible");

			if (modelPathProp != null)
			{
				// Get available models from IModelRegistry (populated by SetModelRegistry)
				string[] availableModels = IModelRegistry.AllModels;
				
				if (availableModels.Length > 1)
				{
					// Use dropdown to select from available models
					RenderEnumCombo(component, modelPathProp, availableModels);
					
					if (ImGui.IsItemHovered())
					{
						ImGui.SetTooltip("Selecciona un modelo 3D. Los modelos (.fbx, .obj, .dae) se detectan automáticamente de la carpeta Assets/");
					}
				}
				else
				{
					// Fallback to text input if no models found
					string modelPath = modelPathProp.Item2;
					ImGui.TextDisabled("Ruta del Modelo 3D");
					ImGui.SameLine();
					ImGui.PushItemWidth(300);
					if (ImGui.InputText("##ModelPath", ref modelPath, 200))
					{
						UpdateProperty(component, modelPathProp.Item1, modelPath);
					}
					ImGui.PopItemWidth();

					ImGui.TextColored(new Num.Vector4(1f, 0.8f, 0.2f, 1f), "⚠ No se encontraron modelos. Coloca archivos .fbx/.obj/.dae en Assets/ y llama a SetModelRegistry().");
				}
			}

			if (isVisibleProp != null)
			{
				bool isVisible = isVisibleProp.Item2.Equals("true", StringComparison.OrdinalIgnoreCase);
				if (ImGui.Checkbox("Visible", ref isVisible))
				{
					UpdateProperty(component, isVisibleProp.Item1, isVisible ? "true" : "false");
				}
			}
		}
		#endregion
	}
}
