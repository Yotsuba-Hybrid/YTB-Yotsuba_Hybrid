# YotsubaHybrid Copilot Instructions

## Build, test, and lint commands

All active projects target `net10.0`, so use a .NET 10 SDK.

### Engine/tooling builds used in CI

```powershell
dotnet restore YTBContentBuilder\YTBContentBuilder.csproj
dotnet build YTBContentBuilder\YTBContentBuilder.csproj --configuration Release --no-restore

dotnet restore YotsubaEngine\YotsubaEngine.csproj
dotnet build YotsubaEngine\YotsubaEngine.csproj --configuration Release --no-restore
```

### Build/run the sample game (DesktopGL)

```powershell
dotnet build SandBoxGame\SandBoxGame.DesktopGL\SandBoxGame.DesktopGL.csproj --configuration Debug
dotnet run --project SandBoxGame\SandBoxGame.DesktopGL\SandBoxGame.DesktopGL.csproj --configuration Debug
```

### Build content pipeline only

```powershell
dotnet build SandBoxGame\SandBoxGame.Content\SandBoxGame.Content.csproj --configuration Debug
```

### Tests

There is currently no test project in this repository (no `*Test*.csproj` and no `dotnet test` usage in workflows), so there is no single-test command yet.

### Lint

There is currently no repository lint command or lint workflow step.

## High-level architecture

This repo is split into three cooperating parts:

1. `YotsubaEngine` (runtime engine library)
   - ECS-style runtime (`EntityManager`, components, systems, `Scene`, `SceneManager`).
   - `YTBGame` is the engine host (`Game` subclass) that loads scenes and drives update/draw.

2. `SandBoxGame` (example game + platform launchers)
   - Platform `Program.cs` files create `SandBoxGame.Core.YTBProgram`.
   - `YTBProgram : YTBGame` sets registries, engine global paths, and game config.

3. `YTBContentBuilder` + `SandBoxGame.Content` (content+code generation pipeline)
   - `SandBoxGame.Content\Builder\Builder.cs` runs MonoGame content build over `SandBoxGame.Core\Assets`.
   - Same build step generates/refreshes:
     - `SandBoxGame.Core\ScriptManager.cs`
     - `SandBoxGame.Core\AssetRegister.cs`
     - `SandBoxGame.Core\AudioRegistry.cs`
     - `SandBoxGame.Core\ModelAssets.cs`
   - Platform projects import `SandBoxGame.Content\BuildContent.targets`, so content is built before app build.

Runtime loading flow:

1. `YTBGame.LoadContent()` calls `YTBFileToGameData.GenerateSceneManager(...)`.
2. `ReadYTBFile` reads `.ytb` files (`YotsubaGame.ytb`, `YotsubaGameConfig.ytb`) under `Assets\GameConfig`/compiled content.
3. `YTBFileToGameData` converts string-based component data into runtime components and builds scenes/entities.
4. `Scene.Initialize()` wires all systems; `Scene.Update()` and `Scene.Draw()` execute system order.
5. `ScriptSystem` resolves scripts through the generated `ScriptRegistry` (AOT-safe, no runtime reflection scan).

## Key repository conventions

- `YTB<T>` is the engine’s custom high-performance collection; entity id and component storage index are expected to stay aligned.
- Component loading from `.ytb` is string-contract based (`ComponentName` and property names), so schema changes must update both templates and parser switch logic.
- Script classes are discovered through generation, not reflection:
  - script class must inherit `BaseScript`
  - script class must be marked with `[Script]`
  - content build regenerates `ScriptManager.cs`
- Do not manually edit generated registry files in `SandBoxGame.Core` (`ScriptManager.cs`, `AssetRegister.cs`, `AudioRegistry.cs`, `ModelAssets.cs`).
- Conditional compile configuration is meaningful:
  - projects define `Debug;Release;YTB`
  - engine/editor-specific behavior is guarded by `#if YTB` / `#if YOTSUBA`
- Tilemap collisions in physics are layer-name driven: collision checks only run for tilemap layers whose name contains `"Collision"`.
- Keep JSON model changes compatible with source-generated serialization context (`YotsubaJsonContext`) used by `.ytb` read/write code.
- Existing engine documentation convention is bilingual XML docs in a single `/// <summary>` block (English + Spanish), per `YotsubaEngine\.github\instructions\DocumentationMode.instructions.md`.
