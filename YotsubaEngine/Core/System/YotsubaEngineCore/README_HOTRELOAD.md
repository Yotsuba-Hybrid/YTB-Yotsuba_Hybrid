# Sistema de HotReload de Assets - YotsubaEngine

## Descripción

El sistema de HotReload permite recompilar los assets del juego (texturas, fuentes, audio, etc.) en tiempo de ejecución sin necesidad de reiniciar el motor o el editor. Este sistema está diseñado para ser robusto, multiplataforma y compatible con AOT.

## Arquitectura

### Componentes Principales

1. **YTBContentBuilder** (`YTBContentBuilder.cs`)
   - Clase estática que maneja el proceso completo de compilación de assets
   - Ejecuta la compilación en un hilo separado para no bloquear el juego
   - Implementa dos pasos de compilación:
     1. Compilar el proyecto SandBoxGame.Content.csproj
     2. Ejecutar el compilador generado con los assets

2. **YTBGlobalState** (`YTBGlobalState.cs`)
   - Contiene la configuración global del motor
   - Proporciona `ContentProjectPath` con auto-detección del proyecto Content
   - Mantiene las rutas de assets compilados y en desarrollo

### Flujo de Compilación

```
┌─────────────────────┐
│ Usuario solicita    │
│ HotReload           │
└──────────┬──────────┘
           │
           v
┌─────────────────────────────────────────┐
│ Paso 1: Compilar SandBoxGame.Content    │
│ dotnet build SandBoxGame.Content.csproj │
└──────────┬──────────────────────────────┘
           │
           v (success)
┌─────────────────────────────────────────┐
│ Paso 2: Ejecutar compilador generado    │
│ SandBoxGame.Content.exe build -p ...    │
└──────────┬──────────────────────────────┘
           │
           v (success)
┌─────────────────────┐
│ Ejecutar callback   │
│ Assets listos       │
└─────────────────────┘
```

## Uso

### Ejemplo Básico

```csharp
using YotsubaEngine.Core.System.YotsubaEngineCore;

// Reconstruir assets sin callback
YTBContentBuilder.Rebuild();

// Reconstruir assets con callback
YTBContentBuilder.Rebuild(() => 
{
    Console.WriteLine("Assets recompilados exitosamente!");
    // Aquí puedes recargar texturas, etc.
});
```

### Configuración del Proyecto Content

Por defecto, el sistema intenta encontrar automáticamente el proyecto `SandBoxGame.Content.csproj`. Si tu estructura es diferente, puedes configurarlo manualmente:

```csharp
using YotsubaEngine.Core.System.YotsubaEngineCore;

// Configurar manualmente la ruta al proyecto Content
YTBGlobalState.ContentProjectPath = @"C:\MiJuego\Content\MiJuego.Content.csproj";
```

## Argumentos del Compilador

El compilador de assets (basado en MGCB) acepta los siguientes argumentos:

```
build -p {Platform} -s {SourceDir} -o {OutputDir} -i {IntermediateDir}
```

Donde:
- **Platform**: Plataforma objetivo (`DesktopGL`, `Windows`, `Android`, `iOS`, etc.)
- **SourceDir**: Directorio con los assets fuente (relativo al working directory)
- **OutputDir**: Directorio de salida para los assets compilados (.xnb)
- **IntermediateDir**: Directorio temporal para archivos intermedios

### Ejemplo de Argumentos Generados

```
build -p DesktopGL -s "SandBoxGame.Core/Assets" -o "C:\MiJuego\bin\Debug\net9.0" -i "C:\MiJuego\SandBoxGame.Content\obj\Debug\net9.0\content"
```

## Detección Automática de Plataforma

El sistema detecta automáticamente la plataforma objetivo según el sistema operativo:

| Sistema Operativo | Plataforma MonoGame |
|-------------------|---------------------|
| Windows           | DesktopGL           |
| Linux             | DesktopGL           |
| macOS             | DesktopGL           |

Esto es configurable modificando el método `GetMonoGamePlatform()` en `YTBContentBuilder.cs`.

## Logs del Sistema

El sistema genera logs detallados del proceso de compilación:

```
[HotReload] Iniciando proceso de reconstrucción de assets...
[HotReload] Proyecto Content: C:\...\SandBoxGame.Content.csproj
[HotReload] Plataforma detectada: DesktopGL
[HotReload] Paso 1/2: Compilando proyecto SandBoxGame.Content...
  [Build] Build succeeded
[HotReload] ✓ Proyecto Content compilado exitosamente.
[HotReload] Paso 2/2: Ejecutando compilador de assets...
  [MGCB] Building Assets/player.png
  [MGCB] Building Assets/font.spritefont
[HotReload] ✓ Assets compilados exitosamente.
[HotReload] ✓ Reconstrucción completada.
```

### Niveles de Log

- `[HotReload]`: Información general del proceso
- `[HotReload][ERROR]`: Errores críticos
- `[Build]`: Salida del proceso de compilación del proyecto
- `[MGCB]`: Salida del compilador de assets (MonoGame Content Builder)
- `[MGCB][ERROR]`: Errores del compilador de assets (marcados con `[E]`)
- `[MGCB][Warning]`: Advertencias del compilador (marcadas con `[W]`)

## Manejo de Errores

El sistema maneja los siguientes escenarios de error:

1. **Proyecto Content no encontrado**
   - Mensaje: `[HotReload][ERROR] No se encontró el proyecto compilador en: ...`
   - Solución: Configurar `YTBGlobalState.ContentProjectPath` correctamente

2. **Fallo en compilación del proyecto Content**
   - Mensaje: `[HotReload][ERROR] Falló la compilación del proyecto Content. Abortando.`
   - Solución: Revisar logs de `[Build][Error]` para detalles

3. **Ejecutable del compilador no encontrado**
   - Mensaje: `[HotReload][ERROR] No se encontró el ejecutable compilado en: ...`
   - Solución: Verificar que el paso 1 haya completado exitosamente

4. **Fallo en compilación de assets**
   - Mensaje: `[HotReload][ERROR] El compilador finalizó con código: ...`
   - Solución: Revisar logs de `[MGCB][ERROR]` para ver qué asset falló

5. **Excepción en callback**
   - Mensaje: `[HotReload][ERROR] Excepción en callback: ...`
   - Solución: Revisar el código del callback proporcionado

## Compatibilidad

### Plataformas Soportadas

- ✅ Windows (x64, x86, ARM64)
- ✅ Linux (x64, ARM64)
- ✅ macOS (x64, ARM64)
- ❌ iOS (requiere adaptaciones adicionales)
- ❌ Android (requiere adaptaciones adicionales)
- ❌ Consolas (requiere SDKs específicos)

### Compatibilidad AOT

El sistema es **100% compatible con AOT** porque:
- No usa reflexión dinámica
- Todos los procesos son ejecutados mediante `Process.Start`
- No hay dependencias de JIT en tiempo de ejecución

### Requisitos

- .NET 9.0 o superior
- `dotnet` CLI disponible en PATH
- MonoGame 3.8.5-preview o superior

## Troubleshooting

### Problema: El compilador no encuentra los assets

**Síntoma:** El compilador ejecuta pero no encuentra los archivos de assets.

**Solución:** Verificar que `YTBGlobalState.DevelopmentAssetsPath` apunte correctamente a la carpeta de assets:

```csharp
YTBGlobalState.DevelopmentAssetsPath = @"C:\MiJuego\SandBoxGame.Core\Assets";
```

### Problema: El proceso se congela

**Síntoma:** El juego se congela durante la compilación.

**Causa:** El sistema ejecuta en un hilo separado (`Task.Run`), pero el callback puede estar bloqueando el hilo principal.

**Solución:** Asegurarse de que el callback no realice operaciones bloqueantes o pesadas.

### Problema: Los assets compilados no se actualizan

**Síntoma:** Los assets se recompilan pero el juego sigue usando los antiguos.

**Causa:** El `ContentManager` mantiene los assets cacheados en memoria.

**Solución:** Implementar lógica de recarga en el callback:

```csharp
YTBContentBuilder.Rebuild(() => 
{
    // Limpiar cache del ContentManager
    YTBGlobalState.ContentManager.Unload();
    
    // Recargar assets necesarios
    var texture = YTBGlobalState.ContentManager.Load<Texture2D>("player");
});
```

## Mejoras Futuras

- [ ] Soporte para compilación incremental (solo assets modificados)
- [ ] Sistema de watch automático para detectar cambios en archivos
- [ ] UI del editor integrada para mostrar progreso de compilación
- [ ] Soporte para plataformas móviles (iOS/Android)
- [ ] Cache de dependencias para acelerar compilaciones
- [ ] Paralelización de compilación de assets independientes

## Referencias

- [MonoGame Content Pipeline](https://docs.monogame.net/articles/content_pipeline/index.html)
- [MGCB Editor](https://docs.monogame.net/articles/tools/mgcb_editor.html)
- [Process.Start Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start)

---

**Última actualización:** 2026-01-16  
**Autor:** YotsubaEngine Team
