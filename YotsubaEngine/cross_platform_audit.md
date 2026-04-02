# 🔍 Auditoría de Compatibilidad Multiplataforma — Yotsuba Hybrid Engine

**Fecha:** 2026-04-02  
**Alcance:** Windows ↔ Linux (DesktopGL)  
**Target Framework:** net10.0 | MonoGame 3.8.5-preview (DesktopGL)

---

## Resumen Ejecutivo

El motor Yotsuba Hybrid está **~85% listo para multiplataforma**. La mayoría de los problemas detectados se concentran en la capa de editor (`#if YTB`) y no en el runtime de producción. Se identificaron **9 hallazgos** clasificados por severidad.

| Severidad | Cantidad | Descripción |
|-----------|----------|-------------|
| 🔴 Crítico | 2 | Rompe la ejecución en Linux |
| 🟡 Medio | 4 | Funcionalidad degradada o riesgos en Linux |
| 🟢 Bajo | 3 | Mejores prácticas sin riesgo inminente |

### ✅ Lo que ya está bien hecho
- Uso extensivo de `Path.Combine()` para construir rutas
- Detección de plataforma centralizada en `YTBGlobalState` con `OperatingSystem.IsWindows()`, `IsLinux()`, etc.
- Sin P/Invoke ni DllImport: **cero dependencias de APIs nativas Win32**
- Uso de `RuntimeInformation.IsOSPlatform()` en `YTBContentBuilder`
- `TitleContainer.OpenStream()` para lectura de assets (compatible MonoGame)
- Normalización de rutas con `Replace('\\', '/')` en puntos críticos del pipeline de gráficos

---

## Hallazgo 1 — `System.Drawing.Common` en producción

> **Severidad:** 🔴 Crítico  
> **Archivo:** [EngineUISystem.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/System/YotsubaEngineUI/EngineUISystem.cs#L23)  
> **Archivo:** [YotsubaEngine.csproj](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/YotsubaEngine.csproj#L22)

### Problema

```csharp
// Línea 23 de EngineUISystem.cs
using D = System.Drawing;

// Línea 1228-1236
public static D.Color Vector4ToColor(Vector4 v)
{
    return D.Color.FromArgb(
        (int)(v.W * 255f),
        (int)(v.X * 255f),
        (int)(v.Y * 255f),
        (int)(v.Z * 255f)
    );
}
```

`System.Drawing.Common` **no es soportado en Linux desde .NET 6+**. A partir de .NET 6, Microsoft lo marcó como sólo-Windows y lanzará `PlatformNotSupportedException` en Linux a menos que se configure el switch `System.Drawing.EnableUnixSupport`.

Adicionalmente, el paquete NuGet está incluido como dependencia directa del proyecto:

```xml
<PackageReference Include="System.Drawing.Common" Version="10.0.3" />
```

### Corrección

El método `Vector4ToColor` solo convierte componentes RGBA. Se puede reemplazar trivialmente con la conversión directa de MonoGame (`Microsoft.Xna.Framework.Color`) que ya está disponible en el ámbito del método:

```csharp
// ✅ REFACTORIZADO: Sin dependencia de System.Drawing
public static Color Vector4ToColor(Vector4 v)
{
    return new Color(v.X, v.Y, v.Z, v.W);
}
```

> [!IMPORTANT]
> También se debe auditar si `System.Drawing.Common` se usa en **algún otro lugar** del proyecto. Si no se usa en ningún otro sitio, **eliminar el paquete NuGet** del `.csproj`. Si se necesita procesamiento de imágenes, ya tienes `SixLabors.ImageSharp` como dependencia — que es 100% multiplataforma.

---

## Hallazgo 2 — `.Replace("\\", "/")` solo normaliza una dirección

> **Severidad:** 🟡 Medio  
> **Archivos afectados:**
> - [EntityManagerUI.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/System/YotsubaEngineUI/UI/EntityManagerUI.cs#L59) (líneas 59, 1625, 1637)
> - [TiledManager.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/ActionFiles/TMX%20Files/TiledCS/TiledManager.cs#L40) (línea 40)
> - [EngineUISystem.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/System/YotsubaEngineUI/EngineUISystem.cs#L1108) (líneas 1108, 1134)
> - [SpriteSheetFiles.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/ActionFiles/XML%20SpriteSheet%20Files/SpriteSheetFiles.cs#L32) (línea 32)

### Problema

Muchos archivos normalizan rutas con `Replace("\\", "/")`, lo cual convierte separadores Windows a formato POSIX. Esto **funciona** en la práctica porque tanto .NET como MonoGame aceptan `/` como separador en todas las plataformas, pero la lógica es inconsistente en `TiledManager.cs`:

```csharp
// TiledManager.cs línea 60-61
relative.StartsWith(_contentPath + "/", StringComparison.OrdinalIgnoreCase) ||
relative.StartsWith(_contentPath + "\\", StringComparison.OrdinalIgnoreCase)
```

Esto compara tanto `/` como `\\`, pero `relative` nunca fue normalizado antes de la comparación. En Linux, `Path.GetRelativePath` nunca devuelve `\\`, así que la rama `\\` es código muerto, pero no peligroso.

### Corrección recomendada

Centralizar la normalización en un helper del engine:

```csharp
/// <summary>
/// Normaliza una ruta para que use '/' como separador, compatible con todas las plataformas.
/// <para>Normalizes a path to use '/' separators, compatible across all platforms.</para>
/// </summary>
public static string NormalizePath(string path)
{
    if (string.IsNullOrWhiteSpace(path)) return string.Empty;
    return path.Replace('\\', '/');
}
```

Y usar este helper en todos los puntos donde se hace `Replace("\\", "/")` para mantener consistencia.

---

## Hallazgo 3 — `TiledManager.TrimStart` con separadores hardcoded

> **Severidad:** 🟡 Medio  
> **Archivo:** [TiledManager.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/ActionFiles/TMX%20Files/TiledCS/TiledManager.cs#L59)

### Problema

```csharp
// Línea 59
string relative = tmxPath.TrimStart('\\', '/');
```

Esto es correcto y funciona en ambas plataformas. Sin embargo la lógica que sigue tiene un riesgo (líneas 60-61):

```csharp
if (relative.StartsWith(_contentPath + "/", StringComparison.OrdinalIgnoreCase) ||
    relative.StartsWith(_contentPath + "\\", StringComparison.OrdinalIgnoreCase))
```

En Linux, el filesystem es **case-sensitive**. Comparar con `OrdinalIgnoreCase` es correcto para el nombre de carpeta (ya que `Content` vs `content` podría variar), pero hay un riesgo: si alguien nombra sus assets con mayúsculas/minúsculas inconsistentes, Linux fallará al abrir el archivo físico aunque esta comparación pase.

### Corrección

El `OrdinalIgnoreCase` en la comparación de prefijos está bien. Pero se debería agregar una nota en la documentación del engine advirtiendo que **los nombres de archivos de assets deben ser consistentes en mayúsculas/minúsculas** para compatibilidad con Linux.

---

## Hallazgo 4 — `YTBContentBuilder.GetMonoGamePlatform()` devuelve `WindowsDX12` para Windows

> **Severidad:** 🔴 Crítico  
> **Archivo:** [YTBContentBuilder.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/ActionFiles/YTB%20Files/YTBContentBuilder.cs#L45-L56)

### Problema

```csharp
private static string GetMonoGamePlatform()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return "WindowsDX12";
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return "DesktopVK";
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        return "DesktopVK";
    return "DesktopGL";
}
```

El proyecto usa el paquete `MonoGame.Framework.DesktopGL`, pero el content builder devuelve `WindowsDX12` para Windows y `DesktopVK` para Linux. Esto genera assets compilados para backends incompatibles con el que el engine realmente usa.

**En Linux**, compilar assets con plataforma `DesktopVK` producirá shaders y texturas en formato Vulkan — pero si el runtime está corriendo con `DesktopGL` (OpenGL), esos assets no se cargarán o se renderizarán incorrectamente.

### Corrección

La plataforma de compilación debe coincidir con el paquete NuGet del proyecto:

```csharp
/// <summary>
/// Determina la plataforma objetivo de MonoGame según el backend usado.
/// El proyecto usa MonoGame.Framework.DesktopGL, así que siempre compila para DesktopGL.
/// </summary>
private static string GetMonoGamePlatform()
{
    // El proyecto referencia MonoGame.Framework.DesktopGL.
    // Todos los assets deben compilarse para DesktopGL independientemente del SO.
    return "DesktopGL";
}
```

> [!CAUTION]
> Si en el futuro se soportan múltiples backends (DirectX para Windows, Vulkan para Linux), esta lógica tendría sentido. Pero actualmente con `DesktopGL`, **todos** los assets deben compilarse como `DesktopGL`.

---

## Hallazgo 5 — `FontSystem2D` condiciona renderizado a `IsWindows()`

> **Severidad:** 🟡 Medio  
> **Archivo:** [FontSystem2D.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/System/S_2D/FontSystem2D.cs#L116)

### Problema

```csharp
// Línea 116 (dentro de #if YTB)
bool canRenderUIElements = IsGameActive || !OperatingSystem.IsWindows();
```

Esto significa: "renderiza las fuentes si el juego está activo, O si NO estamos en Windows". La intención es que en Linux/macOS siempre se rendericen, pero en Windows solo cuando el juego esté en modo play de editor. La lógica funciona, pero es confusa y frágil.

### Corrección

Hacer la intención explícita:

```csharp
#if YTB
    // En modo editor: renderizar siempre en plataformas sin editor (no-Windows),
    // o solo cuando el juego está activo si el editor está presente.
    bool canRenderUIElements = IsGameActive || !YTBGlobalState.IsDesktop;
#endif
```

Esto hace que la condición sea sobre "tiene editor" en vez de "es Windows", lo cual escala mejor si se activa el editor en Linux.

---

## Hallazgo 6 — `Process.Start` con URLs en `MenuBarUI`

> **Severidad:** 🟢 Bajo  
> **Archivo:** [MenuBarUI.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/System/YotsubaEngineUI/UI/MenuBarUI.cs#L87-L101)

### Problema

```csharp
Process.Start(new ProcessStartInfo
{
    FileName = "https://www.leshylabs.com/apps/sstool/",
    UseShellExecute = true
});
```

### Análisis

Con `UseShellExecute = true`, `Process.Start` con una URL funciona en:
- ✅ Windows (abre el navegador predeterminado)
- ✅ Linux con .NET 6+ (usa `xdg-open` internamente)
- ✅ macOS (usa `open` internamente)

**Esto funciona correctamente en Linux.** No se requiere cambio. Solo se documenta como informativo.

---

## Hallazgo 7 — `Environment.SpecialFolder.LocalApplicationData` para fuentes en móviles

> **Severidad:** 🟢 Bajo  
> **Archivo:** [YTBGame.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/YTBGame.cs#L231)

### Problema

```csharp
outputFontsDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
    "Fonts");
```

### Análisis

Este código está protegido por `if (IsMobile)`, así que solo se ejecuta en Android/iOS. En desktop (Windows/Linux) se usa:

```csharp
outputFontsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
```

**Esto es correcto.** `AppDomain.CurrentDomain.BaseDirectory` funciona igual en Windows y Linux. No se requiere cambio.

---

## Hallazgo 8 — `SpriteSheetFiles.CheckSpriteSheetFiles` con `separatorFolder`

> **Severidad:** 🟡 Medio  
> **Archivo:** [SpriteSheetFiles.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/ActionFiles/XML%20SpriteSheet%20Files/SpriteSheetFiles.cs#L78-L79)

### Problema

```csharp
string absolutePath = Path.Combine(Path.GetDirectoryName(xmlPath)!, oldImagePath)
    .Replace('\\', '/').Split(separatorFolder).ToList().Last();
```

El parámetro `separatorFolder` es un string que se usa para `Split`. Si el caller pasa un separador con `\\` desde Windows, la operación de split no encontrará ese separador en Linux donde las rutas usan `/`.

### Corrección

Normalizar `separatorFolder` antes del split:

```csharp
string normalizedPath = Path.Combine(Path.GetDirectoryName(xmlPath)!, oldImagePath)
    .Replace('\\', '/');
string normalizedSeparator = separatorFolder.Replace('\\', '/');
string absolutePath = normalizedPath.Split(normalizedSeparator).Last();
```

---

## Hallazgo 9 — `Scene.cs` campo `isWindows` no utilizado

> **Severidad:** 🟢 Bajo  
> **Archivo:** [Scene.cs](file:///c:/YTBEngine/YotsubaHybrid/YotsubaEngine/Core/YotsubaGame/Scene.cs#L23)

### Problema

```csharp
bool isWindows = OperatingSystem.IsWindows();
```

Este campo se asigna pero no se referencia en ninguna parte del archivo. Es código muerto.

### Corrección

Eliminarlo para limpieza:

```diff
 public class Scene : IDisposable
 {
-    bool isWindows = OperatingSystem.IsWindows();
```

---

## ✅ Áreas sin problemas

### P/Invoke y llamadas nativas
- **Ningún `DllImport` encontrado** en todo el codebase.
- Sin dependencias de `user32.dll`, `kernel32.dll`, ni APIs X11/Wayland.
- ImGui se maneja a través de `Hexa.NET.ImGui` que ya es multiplataforma.

### Manejo de Gráficos e Input
- El `InputSystem` usa las APIs estándar de MonoGame (`Keyboard`, `Mouse`, `GamePad`) que son multiplataforma.
- La resolución de ventana es configurable via parámetros, sin valores hardcodeados exclusivos de Windows.
- El sistema de cámara es agnóstico de plataforma.
- CapsLock como toggle de modo engine puede comportarse diferente en algunos window managers de Linux (X11 vs Wayland), pero esto no es un bug del engine.

### Pipeline de Contenidos
- Todas las rutas de assets usan `Path.Combine()`.
- `TextureAtlasGenerator` normaliza rutas con `Replace('\\', '/')` correctamente.
- `TitleContainer.OpenStream()` es el mecanismo correcto de MonoGame para leer assets.
- Los archivos `.ytb` (JSON) usan `System.Text.Json` con source generators (`YotsubaJsonContext`), que es AOT-compatible y multiplataforma.

### Detección de plataforma
- Centralizada en `YTBGlobalState` con flags `readonly static` — evaluados una sola vez.
- La duplicación de `IsMobile`/`IsDesktop` entre `YTBGame.cs` y `YTBGlobalState.cs` es un code smell (no un bug), pero no afecta portabilidad.

---

## 📋 Plan de acción sugerido (priorizado)

| # | Acción | Severidad | Esfuerzo |
|---|--------|-----------|----------|
| 1 | Corregir `GetMonoGamePlatform()` para devolver `DesktopGL` | 🔴 | 5 min |
| 2 | Eliminar `System.Drawing.Common` y refactorizar `Vector4ToColor` | 🔴 | 15 min |
| 3 | Normalizar `separatorFolder` en `SpriteSheetFiles` | 🟡 | 5 min |
| 4 | Refactorizar `FontSystem2D` condición de renderizado | 🟡 | 5 min |
| 5 | Centralizar helper `NormalizePath` | 🟡 | 20 min |
| 6 | Eliminar campo `isWindows` muerto de `Scene.cs` | 🟢 | 1 min |
| 7 | Documentar case-sensitivity de assets para Linux | 🟢 | 10 min |

> [!TIP]
> Los hallazgos 1 y 4 son los únicos que **romperían** la ejecución del engine en Linux. El resto son mejoras de robustez.

---

## Verificación recomendada

Para validar la portabilidad después de aplicar los cambios:

```bash
# 1. Compilar en Linux
dotnet build YotsubaEngine.csproj -c Debug

# 2. Verificar que no queden referencias a System.Drawing
dotnet list YotsubaEngine.csproj package | grep Drawing

# 3. Ejecutar el content builder y verificar que compile assets como DesktopGL
dotnet run --project SandBoxGame.Content -- build -p DesktopGL -s Assets -o Content
```
