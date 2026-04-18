using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace YotsubaEngine.YTBContentBuilder.GameData
{
    public static class GameDataGenerator
    {
        private const int BATCH_SIZE = 50;
        private const string GameConfigFolder = "GameConfig";
        private const string GameFileName = "YotsubaGame.ytb";
        private const string ConfigFileName = "YotsubaGameConfig.ytb";

        public static void GenerateRegistry(string assetsPath, string outputPath)
        {
            string gameFilePath = Path.Combine(assetsPath, GameConfigFolder, GameFileName);
            string configFilePath = Path.Combine(assetsPath, GameConfigFolder, ConfigFileName);

            if (!File.Exists(gameFilePath))
            {
                Console.WriteLine($"[GameDataGenerator] Game file not found: {gameFilePath}. Skipping.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using YotsubaEngine.ActionFiles.YTB_Files;");
            sb.AppendLine();
            sb.AppendLine("namespace YotsubaEngine.ActionFiles.YTB_Files");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Auto-generated class containing pre-built game data.");
            sb.AppendLine("    /// Eliminates runtime JSON parsing for faster load times.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    internal static class GameDataRegistry");
            sb.AppendLine("    {");

            // Generate GetGameData method
            sb.AppendLine("        internal static (YTBGameInfo, YTBConfig) GetGameData()");
            sb.AppendLine("        {");
            sb.AppendLine("            var gameInfo = new YTBGameInfo");
            sb.AppendLine("            {");

            // Parse game file and generate scene list
            string content = File.ReadAllText(gameFilePath);
            using var gameDoc = JsonDocument.Parse(String.IsNullOrEmpty(content) ? "{}" : content);
            var root = gameDoc.RootElement;

            if (root.TryGetProperty("scene", out JsonElement scenesElement))
            {
                int sceneCount = 0;
                foreach (var _ in scenesElement.EnumerateArray())
                    sceneCount++;

                sb.Append("                Scene = new List<YTBScene>(");
                sb.Append(sceneCount);
                sb.AppendLine(")");
                sb.AppendLine("                {");

                int sceneIndex = 0;
                foreach (var scene in scenesElement.EnumerateArray())
                {
                    sb.Append("                    BuildScene_");
                    sb.Append(sceneIndex);
                    sb.AppendLine("(),");
                    sceneIndex++;
                }

                sb.AppendLine("                }");
            }
            else
            {
                sb.AppendLine("                Scene = new List<YTBScene>()");
            }

            sb.AppendLine("            };");
            sb.AppendLine();

            // Generate config
            sb.AppendLine("            var config = new YTBConfig");
            sb.AppendLine("            {");

            if (File.Exists(configFilePath))
            {
                using var configDoc = JsonDocument.Parse(File.ReadAllText(configFilePath));
                var configRoot = configDoc.RootElement;

                string gameName = GetJsonString(configRoot, "GameName", "YotsubaGame");
                string author = GetJsonString(configRoot, "Author", "YourName");
                string engineVersion = GetJsonString(configRoot, "EngineVersion", "1.0");

                sb.Append("                GameName = \"");
                sb.Append(EscapeString(gameName));
                sb.AppendLine("\",");
                sb.Append("                Author = \"");
                sb.Append(EscapeString(author));
                sb.AppendLine("\",");
                sb.Append("                EngineVersion = \"");
                sb.Append(EscapeString(engineVersion));
                sb.AppendLine("\"");
            }
            else
            {
                sb.AppendLine("                GameName = \"YotsubaGame\",");
                sb.AppendLine("                Author = \"YourName\",");
                sb.AppendLine("                EngineVersion = \"1.0\"");
            }

            sb.AppendLine("            };");
            sb.AppendLine();
            sb.AppendLine("            return (gameInfo, config);");
            sb.AppendLine("        }");

            // Generate BuildScene methods
            if (root.TryGetProperty("scene", out JsonElement scenes))
            {
                int sceneIndex = 0;
                foreach (var scene in scenes.EnumerateArray())
                {
                    GenerateSceneMethod(sb, scene, sceneIndex);
                    sceneIndex++;
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Write output
            var dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(outputPath + ".cs", sb.ToString());
            Console.WriteLine($"[GameDataGenerator] Generated {outputPath}.cs");
        }

        private static void GenerateSceneMethod(StringBuilder sb, JsonElement scene, int sceneIndex)
        {
            string sceneName = scene.TryGetProperty("name", out JsonElement nameEl) ? nameEl.GetString() ?? "" : "";

            // Collect entities
            var entities = new List<JsonElement>();
            if (scene.TryGetProperty("entities", out JsonElement entitiesEl))
            {
                foreach (var entity in entitiesEl.EnumerateArray())
                    entities.Add(entity.Clone());
            }

            int totalEntities = entities.Count;
            int batchCount = (totalEntities + BATCH_SIZE - 1) / BATCH_SIZE;

            sb.AppendLine();
            sb.Append("        private static YTBScene BuildScene_");
            sb.Append(sceneIndex);
            sb.AppendLine("()");
            sb.AppendLine("        {");
            sb.Append("            var entities = new List<YTBEntity>(");
            sb.Append(totalEntities);
            sb.AppendLine(");");

            for (int batch = 0; batch < batchCount; batch++)
            {
                sb.Append("            BuildScene_");
                sb.Append(sceneIndex);
                sb.Append("_Batch_");
                sb.Append(batch);
                sb.AppendLine("(entities);");
            }

            sb.AppendLine("            return new YTBScene");
            sb.AppendLine("            {");
            sb.Append("                Name = \"");
            sb.Append(EscapeString(sceneName));
            sb.AppendLine("\",");
            sb.AppendLine("                Entities = entities");
            sb.AppendLine("            };");
            sb.AppendLine("        }");

            // Generate batch methods
            for (int batch = 0; batch < batchCount; batch++)
            {
                int startIdx = batch * BATCH_SIZE;
                int endIdx = Math.Min(startIdx + BATCH_SIZE, totalEntities);

                sb.AppendLine();
                sb.Append("        private static void BuildScene_");
                sb.Append(sceneIndex);
                sb.Append("_Batch_");
                sb.Append(batch);
                sb.AppendLine("(List<YTBEntity> e)");
                sb.AppendLine("        {");

                for (int i = startIdx; i < endIdx; i++)
                {
                    GenerateEntity(sb, entities[i]);
                }

                sb.AppendLine("        }");
            }
        }

        private static void GenerateEntity(StringBuilder sb, JsonElement entity)
        {
            string entityName = entity.TryGetProperty("name", out JsonElement nameEl) ? nameEl.GetString() ?? "" : "";

            sb.AppendLine("            e.Add(new YTBEntity");
            sb.AppendLine("            {");
            sb.Append("                Name = \"");
            sb.Append(EscapeString(entityName));
            sb.AppendLine("\",");
            sb.AppendLine("                Components = new List<YTBComponents>");
            sb.AppendLine("                {");

            if (entity.TryGetProperty("components", out JsonElement componentsEl))
            {
                foreach (var component in componentsEl.EnumerateArray())
                {
                    GenerateComponent(sb, component);
                }
            }

            sb.AppendLine("                }");
            sb.AppendLine("            });");
        }

        private static void GenerateComponent(StringBuilder sb, JsonElement component)
        {
            string componentName = component.TryGetProperty("ComponentName", out JsonElement nameEl) ? nameEl.GetString() ?? "" : "";

            sb.AppendLine("                    new YTBComponents");
            sb.AppendLine("                    {");
            sb.Append("                        ComponentName = \"");
            sb.Append(EscapeString(componentName));
            sb.AppendLine("\",");
            sb.AppendLine("                        Propiedades = new List<Tuple<string, string>>");
            sb.AppendLine("                        {");

            if (component.TryGetProperty("properties", out JsonElement propsEl))
            {
                foreach (var prop in propsEl.EnumerateArray())
                {
                    string item1 = prop.TryGetProperty("Item1", out JsonElement i1) ? i1.GetString() ?? "" : "";
                    string item2 = prop.TryGetProperty("Item2", out JsonElement i2) ? i2.GetString() ?? "" : "";

                    sb.Append("                            new Tuple<string, string>(\"");
                    sb.Append(EscapeString(item1));
                    sb.Append("\", \"");
                    sb.Append(EscapeString(item2));
                    sb.AppendLine("\"),");
                }
            }

            sb.AppendLine("                        }");
            sb.AppendLine("                    },");
        }

        private static string GetJsonString(JsonElement element, string propertyName, string defaultValue)
        {
            if (element.TryGetProperty(propertyName, out JsonElement prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString() ?? defaultValue;
            return defaultValue;
        }

        private static string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\0", "\\0");
        }
    }
}
