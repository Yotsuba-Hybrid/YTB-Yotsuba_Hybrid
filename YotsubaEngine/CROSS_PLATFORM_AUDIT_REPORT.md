# 🔍 Informe de Auditoría Multiplataforma - YotsubaEngine

**Fecha:** 2026-04-07  
**Versión del motor:** YotsubaHybrid  
**Framework base:** MonoGame 3.8.5-preview  
**Target Framework:** net10.0  

---

## 📖 Aclaración Importante

**`#if YTB` y `#if YOTSUBA`** = Código del **editor del engine**. Este código:
- Solo se compila en modo desarrollo/debug (configuración YTB)
- Sirve para dibujar la UI del editor y herramientas de desarrollo
- **No forma parte del juego final producido**
- Solo se usa en Desktop durante el desarrollo

**Este informe se enfoca en el código FUERA de esas directivas**, que es el que realmente se ejecutará en el runtime del juego en todas las plataformas.

---

## 📊 Clasificación de Plataformas

| Categoría | Plataformas |
|-----------|-------------|
| **Desktop** | Windows, Linux, macOS |
| **Móviles** | Android, iOS |
| **Consolas** | Nintendo Switch, PlayStation 5, Xbox Series X |

---

## 1. 📦 Dependencias C++/Nativas que Requieren Compilación por Plataforma

### 1.1 Hexa.NET.ImGui y Backends Nativos

**Ubicación:** `YotsubaEngine.csproj:24-32`

```xml
<ProjectReference Include="..\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui\Hexa.NET.ImGui.csproj" />
<ProjectReference Include="..\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImNodes\Hexa.NET.ImNodes.csproj" />
<ProjectReference Include="..\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImPlot\Hexa.NET.ImPlot.csproj" />
<ProjectReference Include="..\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGuizmo\Hexa.NET.ImGuizmo.csproj" />
<ProjectReference Include="..\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui-2.2.4\Hexa.NET.ImGui.Backends\Hexa.NET.ImGui.Backends.csproj" />
```

**Archivo de copia:** `CopyAllImGuiNatives.targets` importa las librerías nativas en todos los proyectos de plataforma.

**⚠️ IMPORTANTE:** ImGui se usa en el **código del editor** (`#if YTB`), pero TAMBIÉN en el runtime (`YTBGame.cs:214-315`) para UI del juego.

**Estado de backends nativos por plataforma:**

| Plataforma | Backend ImGui | Estado |
|------------|---------------|--------|
| Windows (DesktopGL) | OpenGL | ✅ Funciona |
| Linux | OpenGL | ✅ Funciona |
| macOS | OpenGL | ✅ Funciona |
| Android | OpenGL ES 3.0 | ✅ Funciona (`ImGuiImplOpenGL3.Init("#version 300 es")`) |
| iOS | ❌ FALTANTE | 🔴 Requiere backend Metal |
| Switch | ❌ NO EXISTE | ❌ Requiere SDK Nintendo |
| PlayStation | ❌ NO EXISTE | ❌ Requiere SDK Sony |
| Xbox | ❌ NO EXISTE | ❌ Requiere DirectX 11/12 |

**Código relevante en `ImGuiRenderer.cs:145-156`:**
```csharp
[MethodImpl(MethodImplOptions.NoInlining)]
public void InitNativeBackend()
{
    _useNativeBackend = true;
    var context = ImGui.GetCurrentContext();
    ImGuiImplOpenGL3.SetCurrentContext(context);
    ImGuiImplOpenGL3.Init("#version 300 es");// Solo OpenGL ES
    ImGuiImplOpenGL3.CreateFontsTexture();
}
```

**Código en `YTBGame.cs:284-291` (FUERA de #if YTB):**
```csharp
if (YTBGlobalState.IsAndroid)
{
    GuiRenderer.InitNativeBackend();  // OpenGL ES para Android
}
else if (!YTBGlobalState.IsIOS)
{
    GuiRenderer.RebuildFontAtlas();  // Desktop OpenGL
}
// ⚠️ iOS se salta sin inicialización - FALTABA BACKEND METAL
```

---

### 1.2 Librerías SixLabors.ImageSharp

**Ubicación:** `YotsubaEngine.csproj:21`

```xml
<PackageReference Include="SixLabors.ImageSharp" Version="3.1.12" />
```

**Estado:** ✅ Multiplataforma. No requiere libs nativas.

---

## 2. 🔍 Código de Runtime que Requiere Validación por Plataforma

### 2.1 Detección de Plataforma en YTBGlobalState.cs

**Archivo:** `YTBGlobalState.cs:29-76`

```csharp
public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
public readonly static bool IsWindows = OperatingSystem.IsWindows();
public readonly static bool IsMacOS = OperatingSystem.IsMacOS();
public readonly static bool IsLinux = OperatingSystem.IsLinux();
public readonly static bool IsAndroid = OperatingSystem.IsAndroid();
public readonly static bool IsIOS = OperatingSystem.IsIOS();
public readonly static bool IsMacCatalist = OperatingSystem.IsMacCatalyst();
public readonly static bool IsTvOS = OperatingSystem.IsTvOS();
public readonly static bool IsWatchOS = OperatingSystem.IsWatchOS();
```

**❌ FALTA:** No existe detección para consolas:
- Sin `IsConsole`
- Sin `IsSwitch`, `IsPlayStation`, `IsXbox`

---

### 2.2 Enum Platforms.cs

**Archivo:** `Platforms.cs`

```csharp
public enum Platforms
{
    Windows_DX12,
    Avalonia_GL,
    Desktop_GL,
    Desktop_VK,
    Android,
    IOS
    // ❌ FALTAN: NintendoSwitch, PlayStation5, XboxSeriesX
}
```

---

## 3. 🔴 Código del Runtime que Causará Problemas en Plataformas Específicas

### 3.1 System.Drawing.Common - BLOQUEANTE para Linux/iOS/Android/Consolas

**Archivo:** `EngineUISystem.cs:23` y método en línea 1228-1236

**⚠️ NOTA:** Este archivo está mayormente dentro de `#if YTB`, pero la referencia `using D = System.Drawing;` está FUERA de la directiva.

```csharp
using D = System.Drawing;  // ❌ Línea 23, FUERA de #if YTB

// Líneas 1228-1236, dentro de clase pero método usado en YTB
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

**🔴 PROBLEMA:**
- `System.Drawing.Common` está obsoleto desde .NET 6 para plataformas no-Windows
- Linux: `PlatformNotSupportedException`
- iOS/Android: No soportado
- Consolas: No soportado

**Solución:** Reemplazar con `Microsoft.Xna.Framework.Color`:
```csharp
public static Color Vector4ToColor(Vector4 v)
{
    return new Color(v.X, v.Y, v.Z, v.W);
}
```

---

### 3.2 Inicialización de ImGui para iOS - FALTANTE

**Archivo:** `YTBGame.cs:284-291`

```csharp
// FUERA de #if YTB - Este código SÍ se ejecuta en el juego final
if (YTBGlobalState.IsAndroid)
{
    GuiRenderer.InitNativeBackend();  // ✅ Android tiene backend OpenGL ES
}
else if (!YTBGlobalState.IsIOS)
{
    GuiRenderer.RebuildFontAtlas();  // ✅ Desktop OpenGL
}
// 🔴 iOS: No hay backend! Se salta sin inicializar
```

**Problema:** iOS usa **Metal**, no OpenGL. El código actual:
1. Android: Inicializa backend OpenGL ES ✅
2. Desktop (no iOS): Usa `RebuildFontAtlas()` ✅
3. iOS: **No hace nada** ❌

**Solución requerida:** Implementar backend Metal para iOS o usar fallback.

---

### 3.3 Inicialización de ImPlot/ImNodes/ImGuizmo Solo en Desktop

**Archivo:** `YTBGame.cs:294-309`

```csharp
// FUERA de #if YTB - Se ejecuta en runtime
if (YTBGlobalState.IsDesktop)
{
    ImPlot.SetImGuiContext(guiContext);
    var plotContext = ImPlot.CreateContext();
    ImPlot.SetCurrentContext(plotContext);
    
    ImNodes.SetImGuiContext(guiContext);
    var nodesContext = ImNodes.CreateContext();
    ImNodes.SetCurrentContext(nodesContext);
    var editorCtx = ImNodes.EditorContextCreate();
    ImNodes.EditorContextSet(editorCtx);
    ImNodes.StyleColorsDark(ImNodes.GetStyle());
    
    ImGuizmo.SetImGuiContext(guiContext);
}
```

**Estado:**
- ✅ Desktop: Inicializa ImPlot, ImNodes, ImGuizmo
- ✅ Móviles: No inicializa (correcto, no se usan en móviles)
- ❓ Consolas: **EL CÓDIGO ASUME QUE SI NO ES DESKTOP, ES MÓVIL**

**Problema:** La lógica usa `if (IsDesktop)` sin considerar consolas. Si se compila para consola:
- `IsDesktop` = `false`
- `IsMobile` = `false`
- ¿Qué pasa? El código entra en el `else` implícito y no inicializa estas extensiones.

---

### 3.4 Enum Platforms Incompleto para Consolas

**Archivo:** `Platforms.cs`

```csharp
public enum Platforms
{
    Windows_DX12,  // ✅
    Avalonia_GL,   // ✅
    Desktop_GL,    // ✅
    Desktop_VK,    // ✅
    Android,       // ✅
    IOS            // ✅
    // ❌ FALTAN CONSOLAS
}
```

**Faltan:** `NintendoSwitch`, `PlayStation5`, `XboxSeriesX`

---

## 4. ⚠️ Código que Debe Validarse para Consolas

### 4.1 Sistema de Input (GamePad)

**Archivo:** `InputSystem.cs` - Todo el archivo está FUERA de `#if YTB`

```csharp
private void ProcessGamePad(int entityId, ref InputComponent inputComponent, GameTime gameTime)
{
    GamePadInfo gamePadInfo = new GamePadInfo(inputComponent.GamePadIndex);
    // MonoGame abstrae el GamePad, pero...
}
```

**Estado:**
- ✅ Desktop: Funciona con controllers
- ⚠️ Android: Funciona con controllers Bluetooth
- ⚠️ iOS: Requiere MFi controller certificado
- ❓ Consolas: **Sin validar** - Los botones específicos varían

---

### 4.2 Sistema de Salida del Juego

**Archivo:** `YTBGame.cs:398-403`

```csharp
#if YOTSUBA
// ✅ Este código SÍ está dentro de #if YOTSUBA, por lo que es del editor
if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
    || Keyboard.GetState().IsKeyDown(Keys.Escape))
    Exit();
#endif
```

**Estado:** ✅ Correctamente dentro de `#if YOTSUBA`, no afecta el runtime.

---

### 4.3 Configuración de Orientación

**Archivo:** `YTBGame.cs:186`

```csharp
// FUERA de #if YTB - Se ejecuta en runtime
graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
```

**Estado:** ✅ Correcto para móviles y consolas.

---

### 4.4 Carga de Fuentes

**Archivo:** `YTBGame.cs:224-276`

```csharp
// FUERA de #if YTB - Se ejecuta en runtime
if (YTBGlobalState.IsDesktop)
{
    unsafe
    {
        io.Fonts.AddFontFromFileTTF(fuentePrincipal, ...);
        // ... carga de fuentes con iconos Nerd Fonts
        io.Fonts.Build();
    }
}
// Móviles y Consolas: No cargan fuentes personalizadas
```

**Estado:**
- ✅ Desktop: Carga fuentes TTF personalizadas
- ⚠️ Móviles: Usa fuente default de ImGui
- ❓ Consolas: Usa fuente default (no validado)

---

## 5. ✅ Código del Editor (`#if YTB`) que Debe Funcionar en Desktop

### 5.1 El Editor Solo se Usa en Desktop

**El siguiente código está dentro de `#if YTB` y debe funcionar correctamente en Desktop:**

| Archivo | Funcionalidad | Problema Detectado |
|---------|---------------|-------------------|
| `EngineUISystem.cs` | UI del editor | ⚠️ Usa `System.Drawing.Common` |
| `Console.WriteLine()` (múltiples) | Debug del editor | ⚠️ `Console.ForegroundColor` falla en algunos casos |
| `YTBContentBuilder.cs` | Compilación de assets hotreload | ✅ Usa `dotnet` CLI (solo Desktop) |
| `DragAndDropSystem.cs` | Drag & drop de archivos | ✅ Solo Desktop tiene file system |

**Problema en el Editor:**
- `System.Drawing.Common` causará problemas si se intenta ejecutar el editor en Linux
- Solución: Usar `Color` de MonoGame o `SixLabors.ImageSharp`

---

## 6. 📋 Resumen: Código del Runtime por Plataforma

| Funcionalidad | Desktop | Android | iOS | Consolas |
|----------------|---------|---------|-----|----------|
| **Core Engine** | ✅ | ✅ | ⚠️ | ❓ Sin validar |
| **ImGui Render** | ✅ | ✅ | 🔴 Falta Metal | ❌ Sin backend |
| **ImGui Fonts** | ✅ Custom | ⚠️ Default | ⚠️ Default | ❓ Default |
| **ImPlot/ImNodes** | ✅ Inicializado | ✅ No iniciado | ✅ No iniciado | ❓ Sin validar |
| **Touch Input** | ❌ N/A | ✅ | ✅ | ✅ Controles |
| **GamePad** | ✅ | ⚠️ BT Controller | ⚠️ MFi Controller | ❓ Sin validar |
| **Keyboard/Mouse** | ✅ | ❌ | ❌ | ❌ |
| **System.Drawing** | ✅ | ✅ (no usado) | ✅ (no usado) | ✅ (no usado) |
| **Detección de plataforma** | ✅ | ✅ | ✅ | ❌ FALTA |

**Leyenda:**
- ✅ Funciona correctamente
- ⚠️ Funciona con limitaciones
- 🔴 Bloqueante / causa error
- ❌ No aplicable o no soportado
- ❓ No validado / sin implementar

---

## 7. 🔎 Líneas Específicas con Referencias por Archivo

### 7.1 ImGuiRenderer.cs - Backend Nativo

| Línea | Código | Plataformas | Estado |
|-------|--------|-------------|--------|
| 145-156 | `InitNativeBackend()` | Android/iOS | 🔴 iOS falta Metal |
| 153 | `ImGuiImplOpenGL3.Init("#version 300 es")` | Android | ✅ |
| 235-238 | `if (!OperatingSystem.IsAndroid()) SetupDesktopTextInput()` | Desktop/No-Android | ⚠️ Falta validación iOS |
| 288-290 | `if (OperatingSystem.IsAndroid()) UpdateAndroidInput(io)` | Android | ✅ |
| 373-390 | `UpdateAndroidInput()` | Android | ✅ |

### 7.2 YTBGame.cs - Inicialización

| Línea | Código | Plataformas | Estado |
|-------|--------|-------------|--------|
| 35 | `IsMobile = OperatingSystem.IsAndroid() \|\| OperatingSystem.IsIOS()` | Runtime | ✅ |
| 41 | `IsDesktop = ...` | Runtime | ⚠️ Sin consolas |
| 218-221 | `if (!IsMobile) io.ConfigFlags \|= ImGuiConfigFlags.DockingEnable` | Runtime | ✅ |
| 231-276 | Carga de fuentes solo en `IsDesktop` | Runtime | ⚠️ Consolas = default |
| 284-291 | Inicialización de ImGui por plataforma | Runtime | 🔴 iOS sin backend |
| 294-309 | `if (IsDesktop) { ImPlot/ImNodes/ImGuizmo }` | Runtime | ⚠️ Sin validar consolas |

### 7.3 YTBGlobalState.cs - Detección

| Línea | Código | Estado |
|-------|--------|--------|
| 29 | `IsMobile = OperatingSystem.IsAndroid() \|\| OperatingSystem.IsIOS()` | ✅ |
| 34 | `IsDesktop = OperatingSystem.IsMacOS() \|\| OperatingSystem.IsLinux() \|\| OperatingSystem.IsWindows()` | ⚠️ Falta consolas |
| 58 | `IsAndroid = OperatingSystem.IsAndroid()` | ✅ |
| 64 | `IsIOS = OperatingSystem.IsIOS()` | ✅ |
| **FALTA** | `IsConsole`, `IsSwitch`, etc. | ❌ |

### 7.4 Platforms.cs - Enum

| Línea | Valor | Estado |
|-------|-------|--------|
| 9 | `Windows_DX12` | ✅ |
| 10 | `Avalonia_GL` | ✅ |
| 11 | `Desktop_GL` | ✅ |
| 12 | `Desktop_VK` | ✅ |
| 13 | `Android` | ✅ |
| 14 | `IOS` | ✅ |
| **FALTA** | `NintendoSwitch`, `PlayStation5`, `XboxSeriesX` | ❌ |

---

## 8. 🛠️ Problemas y Soluciones

### 8.1 System.Drawing.Common en EngineUISystem.cs

**Problema:** La referencia y uso de `System.Drawing` está FUERA de `#if YTB`.

**Archivo:** `EngineUISystem.cs:23`

**Solución:**
```csharp
// ELIMINAR:
// using D = System.Drawing;

// Reemplazar Vector4ToColor con implementación de MonoGame:
public static Color Vector4ToColor(Vector4 v)
{
    return new Color(v.X, v.Y, v.Z, v.W);
}
```

---

### 8.2 Backend Metal para iOS

**Problema:** iOS usa Metal, no OpenGL ES. El código actual no inicializa ImGui en iOS.

**Archivo:** `YTBGame.cs:284-291`

**Solución sugerida:**
```csharp
if (YTBGlobalState.IsAndroid)
{
    GuiRenderer.InitNativeBackend();  // OpenGL ES
}
else if (YTBGlobalState.IsIOS)
{
    // TODO: Implementar backend Metal
    // GuiRenderer.InitMetalBackend();
    // Fallback temporal:
    GuiRenderer.RebuildFontAtlas();
}
else
{
    GuiRenderer.RebuildFontAtlas();  // Desktop OpenGL
}
```

---

### 8.3 Detección de Consolas

**Problema:** No existe detección de consolas.

**Archivo:** `Platforms.cs` y `YTBGlobalState.cs`

**Solución:**
```csharp
// Platforms.cs
public enum Platforms
{
    Windows_DX12,
    Avalonia_GL,
    Desktop_GL,
    Desktop_VK,
    Android,
    IOS,
    NintendoSwitch,
    PlayStation5,
    XboxSeriesX
}

// YTBGlobalState.cs
public readonly static bool IsConsole = false;  // Configurar via directivas
#if NINTENDO_SWITCH
    public readonly static bool IsSwitch = true;
#elif PLAYSTATION
    public readonly static bool IsPlayStation = true;
#elif XBOX
    public readonly static bool IsXbox = true;
#else
    public readonly static bool IsSwitch = false;
    public readonly static bool IsPlayStation = false;
    public readonly static bool IsXbox = false;
#endif
```

---

### 8.4 Validación para Consolas en ImGui

**Problema:** Las consolas quedarían sin backend de ImGui.

**Archivo:** `YTBGame.cs`

**Solución:**
```csharp
if (YTBGlobalState.IsAndroid)
{
    GuiRenderer.InitNativeBackend();
}
else if (YTBGlobalState.IsIOS)
{
    // Backend Metal requerido
}
else if (YTBGlobalState.IsConsole)
{
    // Backend específico de consola requerido
    // Requiere SDK propietario
}
else
{
    GuiRenderer.RebuildFontAtlas();  // Desktop
}
```

---

## 9. 📝 Conclusiones

### Estado Actual por Plataforma

| Plataforma | Runtime Funcional | Bloqueantes |
|------------|-------------------|-------------|
| **Windows** | ✅ 95% | Ninguno |
| **Linux** | ⚠️ 90% | System.Drawing (editor) |
| **macOS** | ✅ 95% | Ninguno |
| **Android** | ✅ 90% | Verificar libs ARM64 |
| **iOS** | 🔴 60% | Falta backend Metal para ImGui |
| **Consolas** | ❓ Sin validar | Sin detección, sin backends |

### Editores del Motor en Desktop

| Archivo | Estado en Desktop |
|---------|-------------------|
| `EngineUISystem.cs` | ⚠️ System.Drawing.Common en Linux |
| `YTBContentBuilder.cs` | ✅ Requiere dotnet CLI |
| `DragAndDropSystem.cs` | ✅ Solo Desktop |
| Resto del editor | ✅ Funciona |

### Próximos Pasos Prioritarios

1. 🔴 **Implementar backend Metal para iOS** - ImGui no inicializa
2. 🔴 **Eliminar System.Drawing.Common** - Declaración `using` fuera de directiva
3. 🟡 **Añadir detección de consolas** - `IsConsole`, `IsSwitch`, etc.
4. 🟡 **Extender enum Platforms** - Añadir consolas
5. 🟢 **Validar Input para consolas** - GamePad buttons específicos

---

*Informe corregido tras aclaración sobre directivas `#if YTB/YOTSUBA`.*