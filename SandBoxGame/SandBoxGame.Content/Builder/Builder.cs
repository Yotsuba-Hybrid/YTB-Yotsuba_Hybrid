/// <summary>
/// Entry point for the Content Builder project, 
/// which when executed will build content according to the "Content Collection Strategy" defined in the Builder class.
/// </summary>
/// <remarks>
/// Make sure to validate the directory paths in the "ContentBuilderParams" for your specific project.
/// For more details regarding the Content Builder, see the MonoGame documentation: <tbc.>
/// 
/// Note: El nombre del juego configurado en YotsubaGameConfig.GameName se utiliza automáticamente
/// para el título de la ventana en tiempo de ejecución (ver YTBGame.cs).
/// Para acceder al nombre del juego en el builder, usa: YotsubaEngine.Core.System.YotsubaEngineCore.YTBContentBuilder.GetGameName()
/// </remarks>

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;
using System.Diagnostics;
using YotsubaEngine.YTBContentBuilder;
using YotsubaEngine.YTBContentBuilder.Assets;
using YotsubaEngine.YTBContentBuilder.Audio;
using YotsubaEngine.YTBContentBuilder.Models;
using YotsubaEngine.YTBContentBuilder.Scripting;

// Obtener la ruta al proyecto Core de forma relativa desde la ubicación del builder
// El builder se encuentra en: SandBoxGame/SandBoxGame.Content/Builder/
// Y SandBoxGame.Core está en: SandBoxGame/SandBoxGame.Core/
string currentDirectory = AppContext.BaseDirectory;
string platformsCoreDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "..", "..", "SandBoxGame.Core"));

// Fallback: Si el directorio no existe, intenta buscar relativo al directorio actual
if (!Directory.Exists(platformsCoreDirectory))
{
    platformsCoreDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SandBoxGame.Core"));
}

// Fallback adicional: buscar en el proyecto padre
if (!Directory.Exists(platformsCoreDirectory))
{
    string projectRoot = Directory.GetCurrentDirectory();
    while (!string.IsNullOrEmpty(projectRoot) && !Directory.Exists(Path.Combine(projectRoot, "SandBoxGame", "SandBoxGame.Core")))
    {
        projectRoot = Path.GetDirectoryName(projectRoot);
    }
    if (!string.IsNullOrEmpty(projectRoot))
    {
        platformsCoreDirectory = Path.Combine(projectRoot, "SandBoxGame", "SandBoxGame.Core");
    }
}


var contentCollectionArgs = new ContentBuilderParams()
{
    Mode = ContentBuilderMode.Builder,
    WorkingDirectory = platformsCoreDirectory, // Apunta a Platforms.Core donde están los Assets
    SourceDirectory = "Assets", // Carpeta de assets dentro de Platforms.Core
    Platform = TargetPlatform.DesktopGL
};

var builder = new Builder();

if (args is not null && args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(contentCollectionArgs);
}


Console.WriteLine($"[Builder] Using SandBoxGame.Core directory: {platformsCoreDirectory}");

ScriptRegistryGenerator.GenerateRegistry(platformsCoreDirectory, Path.Combine(platformsCoreDirectory, "ScriptManager"));

// Generar el registro de audio escaneando la carpeta Assets
string assetsDirectory = Path.Combine(platformsCoreDirectory, "Assets");
AudioRegistryGenerator.GenerateRegistry(assetsDirectory, Path.Combine(platformsCoreDirectory, "AudioRegistry"));
AssetRegistryGenerator.GenerateRegistry(assetsDirectory, Path.Combine(platformsCoreDirectory, "AssetRegister"));
ModelRegistryGenerator.GenerateRegistry(assetsDirectory, Path.Combine(platformsCoreDirectory, "ModelAssets"));
SpriteSheetPathValidator.ValidateImagePaths(assetsDirectory);

return builder.FailedToBuild > 0 ? -1 : 0;

/// <summary>
/// Builder de contenido para compilar assets del juego.
/// <para>Content builder used to compile game assets.</para>
/// </summary>
public class Builder : ContentBuilder
{
    /// <summary>
    /// Obtiene la colección de contenido a compilar.
    /// <para>Gets the content collection to build.</para>
    /// </summary>
    /// <returns>
    /// Colección de contenido configurada para el build.
    /// <para>Configured content collection for the build.</para>
    /// </returns>
    public override IContentCollection GetContentCollection()
    {
        var contentCollection = new ContentCollection();
        // include everything in the folder

        AssetsConfig.Apply(contentCollection);

        return contentCollection;
    }
}
