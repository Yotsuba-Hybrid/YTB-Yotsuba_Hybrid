using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Graphics;

namespace YotsubaEngine.ActionFiles.XML_SpriteSheet_Files
{
	/// <summary>
	/// Utilidades para validar y generar datos XML de sprite sheets.
	/// <para>Utilities for validating and generating sprite sheet XML data.</para>
	/// </summary>
	public class SpriteSheetFiles
	{
		/// <summary>
		/// Escanea y actualiza archivos XML de sprite sheets bajo una carpeta raíz.
		/// <para>Scans and updates sprite sheet XML files under a root folder.</para>
		/// </summary>
		/// <param name="rootFolder">Carpeta raíz a escanear. <para>Root folder to scan.</para></param>
		/// <param name="separatorFolder">Separador de carpeta esperado. <para>Expected folder separator.</para></param>
		public static void CheckSpriteSheetFiles(string rootFolder, string separatorFolder)
		{
			rootFolder = rootFolder.Replace('\\', '/');
			var xmlFiles = Directory.GetFiles(rootFolder, "*.xml", SearchOption.AllDirectories);

			foreach (var xmlPath in xmlFiles)
			{
				try
				{

					GenerateAnimationsFromXml(xmlPath);

					string xmlText = File.ReadAllText(xmlPath);
					bool textModified = false;

					// 1. Eliminar el namespace molesto (Solo esto es necesario)
					// Usamos un Replace simple. Si el archivo tiene espacios raros en el xmlns, esto podría fallar,
					// pero para tus archivos generados automáticamente funcionará.
					if (xmlText.Contains("xmlns=\"http://www.w3.org/1999/xhtml\""))
					{
						xmlText = xmlText.Replace("xmlns=\"http://www.w3.org/1999/xhtml\"", "");
						textModified = true;
					}

					// OJO: NO borres el <?xml version... ?>, XDocument.Parse lo necesita a veces para saber el encoding.

					// 2. Parsear
					XDocument document = XDocument.Parse(xmlText);

					// 3. Lógica de ImagePath
					var imageAttr = document.Root.Attribute("imagepath");
					if (imageAttr == null) continue;

					string oldImagePath = imageAttr.Value;

					// Si ya tiene barras, asumimos que ya fue procesado y saltamos
					if (oldImagePath.Contains("/") || oldImagePath.Contains("\\"))
					{
						// Si solo modificamos el namespace (textModified), guardamos y seguimos.
						if (textModified)
						{
							document.Save(xmlPath);
							Console.WriteLine($"✅ Namespace corregido en: {xmlPath}");
						}
						continue;
					}

					// Construir ruta absoluta
					string absolutePath = Path.Combine(Path.GetDirectoryName(xmlPath)!, oldImagePath)
												.Replace('\\', '/').Split(separatorFolder).ToList().Last();

					imageAttr.Value = absolutePath;

					document.Save(xmlPath);

					Console.WriteLine($"✅ Actualizado completo: {xmlPath}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"❌ Error procesando {xmlPath}: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// Genera nodos de animación faltantes basados en el nombre de subtexturas.
		/// <para>Generates missing animation nodes based on subtexture naming.</para>
		/// </summary>
		/// <param name="xmlPath">Ruta del XML a procesar. <para>XML path to process.</para></param>
		public static void GenerateAnimationsFromXml(string xmlPath)
		{
			if (!File.Exists(xmlPath)) return;

			try
			{
				XDocument doc = XDocument.Load(xmlPath);
				XElement root = doc.Root;
				if (root == null) return;

				// 1. Analizar texturas para encontrar posibles animaciones
				var subtextures = root.Elements("subtexture").ToList();
				var detectedGroups = new Dictionary<string, List<FrameInfo>>();
				Regex numberPattern = new Regex(@"^(.*?)(\d+)$");

				foreach (var sub in subtextures)
				{
					string regionName = sub.Attribute("name")?.Value;
					if (string.IsNullOrEmpty(regionName)) continue;

					Match match = numberPattern.Match(regionName);
					string animName;
					int frameIndex = 0;

					if (match.Success)
					{
						// Limpia guiones bajos o espacios al final del nombre (ej: "Run_" -> "Run")
						animName = match.Groups[1].Value.TrimEnd('_', '-', ' ');
						if (string.IsNullOrEmpty(animName)) animName = "General";
						int.TryParse(match.Groups[2].Value, out frameIndex);
					}
					else
					{
						animName = regionName;
					}

					if (!detectedGroups.ContainsKey(animName))
						detectedGroups[animName] = new List<FrameInfo>();

					detectedGroups[animName].Add(new FrameInfo { RegionName = regionName, Index = frameIndex });
				}

				// --- CAMBIO IMPORTANTE: NO BORRAMOS LAS ANIMACIONES EXISTENTES ---
				// root.Elements("animation").Remove(); <--- ESTO SE ELIMINÓ

				bool huboCambios = false;

				// 2. Solo agregar animaciones que NO existan
				foreach (var group in detectedGroups)
				{
					string animationName = group.Key;

					// Verificamos si ya existe una etiqueta <animation> con este nombre
					bool exists = root.Elements("animation")
									  .Any(a => a.Attribute("name")?.Value == animationName);

					if (exists)
					{
						// Si existe, la ignoramos para proteger tu trabajo manual
						Console.WriteLine($"[Info] La animación '{animationName}' ya existe. Se omite.");
						continue;
					}

					// Si no existe, la creamos
					var sortedFrames = group.Value.OrderBy(f => f.Index).ToList();

					// Ignoramos grupos de 1 solo frame si prefieres no crear animaciones para sprites sueltos
					if (sortedFrames.Count <= 1) continue;

					XElement animNode = new XElement("animation",
						new XAttribute("name", animationName),
						new XAttribute("delay", "150") // Delay por defecto
					);

					foreach (var frame in sortedFrames)
					{
						animNode.Add(new XElement("Frame", new XAttribute("region", frame.RegionName)));
					}

					root.Add(animNode);
					Console.WriteLine($"[Nuevo] Se generó la animación: {animationName}");
					huboCambios = true;
				}

				if (huboCambios)
				{
					doc.Save(xmlPath);
					Console.WriteLine($"[Éxito] Archivo actualizado: {xmlPath}");
				}
				else
				{
					Console.WriteLine("[Info] No se encontraron animaciones nuevas. El archivo no se tocó.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Error] {ex.Message}");
			}
		}

		/// <summary>
		/// Almacena datos de frame para agrupar animaciones.
		/// <para>Stores frame data for animation grouping.</para>
		/// </summary>
		private class FrameInfo
		{
			/// <summary>
			/// Obtiene o establece el nombre de región para el frame.
			/// <para>Gets or sets the region name for the frame.</para>
			/// </summary>
			public string RegionName { get; set; }

			/// <summary>
			/// Obtiene o establece el índice del frame para ordenamiento.
			/// <para>Gets or sets the frame index for sorting.</para>
			/// </summary>
			public int Index { get; set; }
		}

	}
}
