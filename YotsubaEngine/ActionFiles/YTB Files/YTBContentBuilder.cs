using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YotsubaEngine.ActionFiles.YTB_Files;
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI;
#endif
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Core.System.YotsubaEngineCore
{
    /// <summary>
    /// Sistema de compilación de assets en caliente (HotReload) para YotsubaEngine.
    /// <para>Hot-reload asset build system for YotsubaEngine.</para>
    /// </summary>
    public class YTBContentBuilder
    {
        /// <summary>
        /// Obtiene el nombre del juego desde YotsubaGameConfig.
        /// <para>Gets the game name from YotsubaGameConfig.</para>
        /// </summary>
        /// <returns>El nombre configurado o "YotsubaGame" por defecto. <para>The configured game name, or "YotsubaGame" by default.</para></returns>
        public static string GetGameName()
        {
#if YTB
            try
            {
#endif

                var config = YTBGlobalState.GameData.Item2;
                
                if (!string.IsNullOrWhiteSpace(config?.GameName))
                {
                    return config.GameName;
                }

#if YTB

            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"[YTBContentBuilder] No se pudo leer GameName: {ex.Message}");
            }
#endif


                return "YotsubaGame";
        }

        /// <summary>
        /// Determina la plataforma objetivo de MonoGame según el sistema operativo actual.
        /// </summary>
        private static string GetMonoGamePlatform()
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
              //  return "WindowsDX12"; // O "WindowsDX" si prefieres DirectX
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
             //   return "DesktopVK";
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
              //  return "DesktopVK";
            
            // Fallback
            return "DesktopGL";
        }

        /// <summary>
        /// Reconstruye los assets del juego en un hilo separado (fire-and-forget).
        /// <para>Rebuilds game assets on a separate thread (fire-and-forget).</para>
        /// </summary>
        public static void Rebuild()
        {
            Task.Run(async () => await RebuildAsync(() => { }));
        }

        /// <summary>
        /// Reconstruye los assets del juego en un hilo separado y ejecuta un callback al finalizar.
        /// <para>Rebuilds game assets on a separate thread and runs a callback when finished.</para>
        /// </summary>
        /// <param name="fn">Callback al finalizar. <para>Callback to run when compilation finishes.</para></param>
        public static void Rebuild(Action fn)
        {
            Task.Run(async () => await RebuildAsync(fn));
        }

        /// <summary>
        /// Lógica interna asíncrona que maneja el proceso completo de compilación.
        /// Flujo:
        /// 1. Compilar el proyecto SandBoxGame.Content.csproj (genera el ejecutable del compilador)
        /// 2. Ejecutar ese ejecutable con los argumentos correctos para compilar los assets
        /// </summary>
        private static async Task RebuildAsync(Action fn)
        {
#if YTB
            EngineUISystem.SendLog("[HotReload] Iniciando proceso de reconstrucción de assets...");
#endif
            // ═══════════════════════════════════════════════════════════════
            // PASO 1: VALIDAR RUTAS
            // ═══════════════════════════════════════════════════════════════
            
            string contentProjectPath = YTBGlobalState.ContentProjectPath;
            string contentProjectDir = Path.GetDirectoryName(contentProjectPath);

#if YTB
            EngineUISystem.SendLog($"[HotReload] Proyecto Content: {contentProjectPath}");
#endif
            if (!File.Exists(contentProjectPath))
            {
#if YTB

                EngineUISystem.SendLog($"[HotReload][ERROR] No se encontró el proyecto compilador en: {contentProjectPath}");
                EngineUISystem.SendLog($"[HotReload][ERROR] Por favor, configura YTBGlobalState.ContentProjectPath correctamente.");
#endif

                return;
            }

            // Determinar plataforma
            string platform = GetMonoGamePlatform();
#if YTB
            EngineUISystem.SendLog($"[HotReload] Plataforma detectada: {platform}");
#endif

// ═══════════════════════════════════════════════════════════════
// PASO 2: COMPILAR EL PROYECTO SandBoxGame.Content
// ═══════════════════════════════════════════════════════════════
#if YTB
            EngineUISystem.SendLog("[HotReload] Paso 1/2: Compilando proyecto SandBoxGame.Content...");
#endif
            bool buildSuccess = await BuildContentProject(contentProjectPath, contentProjectDir);
            
            if (!buildSuccess)
            {
#if YTB

                EngineUISystem.SendLog("[HotReload][ERROR] Falló la compilación del proyecto Content. Abortando.");
#endif
                return;
            }

#if YTB
            EngineUISystem.SendLog("[HotReload] ✓ Proyecto Content compilado exitosamente.");
#endif

// ═══════════════════════════════════════════════════════════════
// PASO 3: EJECUTAR EL COMPILADOR DE ASSETS
// ═══════════════════════════════════════════════════════════════

#if YTB
            EngineUISystem.SendLog("[HotReload] Paso 2/2: Ejecutando compilador de assets...");
#endif
            bool compileSuccess = await RunContentCompiler(contentProjectPath, platform);

            if (!compileSuccess)
            {
#if YTB
                EngineUISystem.SendLog("[HotReload][ERROR] Falló la compilación de assets.");
#endif
                return;
            }

            // ═══════════════════════════════════════════════════════════════
            // PASO 4: FINALIZAR Y EJECUTAR CALLBACK
            // ═══════════════════════════════════════════════════════════════

#if YTB
            EngineUISystem.SendLog("[HotReload] ✓ Assets compilados exitosamente.");
#endif
            try
            {
                fn?.Invoke();
#if YTB
                EngineUISystem.SendLog("[HotReload] ✓ Reconstrucción completada.");
#endif
            }
            catch (Exception ex)
            {
#if YTB
                EngineUISystem.SendLog($"[HotReload][ERROR] Excepción en callback: {ex.Message}");
#endif
            }
        }

        /// <summary>
        /// Compila el proyecto SandBoxGame.Content.csproj usando dotnet build.
        /// </summary>
        private static async Task<bool> BuildContentProject(string projectPath, string workingDirectory)
        {
            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{projectPath}\" --configuration Debug",
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (var process = new Process { StartInfo = info })
                {
#if YTB
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                            EngineUISystem.SendLog($"  [Build] {e.Data}");
                    };
                    
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                            EngineUISystem.SendLog($"  [Build][Error] {e.Data}");
                    };
#endif

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    
                    await process.WaitForExitAsync();

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
#if YTB
                EngineUISystem.SendLog($"[HotReload][ERROR] Excepción al compilar proyecto Content: {ex.Message}");
#endif
                return false;
            }
        }

        /// <summary>
        /// Ejecuta el compilador de assets generado por SandBoxGame.Content.
        /// Según BuildContent.targets:
        /// - Argumentos: build -p {Platform} -s {SourceDir} -o {OutputDir} -i {IntermediateDir}
        /// </summary>
        private static async Task<bool> RunContentCompiler(string contentProjectFilePath, string platform)
        {
            // 1. Limpiar y obtener el directorio base del proyecto
            // Entrada: "C:\...\SandBoxGame.Content.csproj"
            string projectBaseDir = Path.GetDirectoryName(contentProjectFilePath);

            // Configuración (puedes pasarlo como parámetro si cambia entre Debug/Release)
            string configuration = "Debug";
            string targetFramework = "net10.0"; // Asegúrate de que coincida con tu .csproj

            // 2. Construir la ruta al ejecutable (La herramienta MGCB compilada)
            // MSBuild: $(MSBuildThisFileDirectory)bin\$(Configuration)\net10.0\SandBoxGame.Content.dll
            string executablePath = Path.Combine(
                projectBaseDir,
                "bin",
                configuration,
                targetFramework,
                "SandBoxGame.Content.dll"
            );

            // Validación de seguridad
            if (!File.Exists(executablePath))
            {
#if YTB
                EngineUISystem.SendLog($"[HotReload][ERROR] No se encontró la herramienta de contenido en: {executablePath}");
                EngineUISystem.SendLog($"[HotReload][INFO] Asegúrate de haber compilado el proyecto SandBoxGame.Content al menos una vez.");
#endif
                return false;
            }

            // 3. Definir Working Directory
            // MSBuild: $(MSBuildThisFileDirectory)..\
            // Esto sube un nivel desde donde está el .csproj (hacia la raíz de la solución generalmente)
            string workingDirectory = Path.GetFullPath(Path.Combine(projectBaseDir, ".."));

            // 4. Definir Rutas de Argumentos
            // Source: Relativo al Working Directory
            string sourceDirectory = Path.Combine("SandBoxGame.Core", "Assets");

            // Output: Donde está corriendo ESTE juego ahora mismo
            string outputDirectory = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Intermediate: Carpeta obj dentro del proyecto de contenido
            string intermediateDirectory = Path.Combine(projectBaseDir, "obj", configuration, targetFramework, "content");

            // 5. Construir Argumentos
            // MSBuild: build -p $(MonoGamePlatform) -s ...
            string arguments = $"build -p {platform} -s \"{sourceDirectory}\" -o \"{outputDirectory}\" -i \"{intermediateDirectory}\"";

#if YTB
            // Logs para depuración
            EngineUISystem.SendLog($"[HotReload] Tool: {Path.GetFileName(executablePath)}");
            EngineUISystem.SendLog($"[HotReload] WorkDir: {workingDirectory}");
            EngineUISystem.SendLog($"[HotReload] Args: {arguments}");
#endif

            var info = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"exec \"{executablePath}\" {arguments}",
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            try
            {
                using (var process = new Process { StartInfo = info })
                {
#if YTB

                    process.OutputDataReceived += (s, e) =>
                    {

                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            if (e.Data.Contains("error", StringComparison.OrdinalIgnoreCase) || e.Data.Contains("[E]"))
                                EngineUISystem.SendLog($"  [MGCB][ERROR] {e.Data}");
                            else if (e.Data.Contains("warning", StringComparison.OrdinalIgnoreCase) || e.Data.Contains("[W]"))
                                EngineUISystem.SendLog($"  [MGCB][WARN] {e.Data}");
                            else
                                EngineUISystem.SendLog($"  [MGCB] {e.Data}");
                    }

                }
                ;



                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                            EngineUISystem.SendLog($"  [MGCB][STDERR] {e.Data}");
                    };
#endif

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
#if YTB
                        EngineUISystem.SendLog($"[HotReload][FAIL] Código de salida: {process.ExitCode}");
#endif
                        return false;
                    }

#if YTB
                    EngineUISystem.SendLog("[HotReload] Compilación de assets finalizada con éxito.");
#endif
                    return true;
                }
            }
            catch (Exception ex)
            {
#if YTB
                EngineUISystem.SendLog($"[HotReload][EXCEPTION] {ex.Message}");
#endif
                return false;
            }
        }
    }
}