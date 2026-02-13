# YotsubaEngine

Motor de juegos con arquitectura **ECS (Entity-Component-System)** sobre MonoGame, orientado a rendimiento, flujo de trabajo iterativo y soporte multiplataforma.

---

## Tabla de contenido

1. [Qué es YotsubaEngine](#qué-es-yotsubaengine)
2. [Objetivos de diseño](#objetivos-de-diseño)
3. [Arquitectura (visión de alto nivel)](#arquitectura-visión-de-alto-nivel)
4. [Módulos principales](#módulos-principales)
5. [Flujo de frame](#flujo-de-frame)
6. [Primeros pasos](#primeros-pasos)
7. [Configuraciones de compilación](#configuraciones-de-compilación)
8. [Estructura del proyecto](#estructura-del-proyecto)
9. [Scripting y registro de scripts](#scripting-y-registro-de-scripts)
10. [Sistema de assets y archivos .ytb](#sistema-de-assets-y-archivos-ytb)
11. [UI (ImGui + Gum)](#ui-imgui--gum)
12. [Rendimiento y buenas prácticas](#rendimiento-y-buenas-prácticas)
13. [Troubleshooting](#troubleshooting)
14. [Roadmap sugerido](#roadmap-sugerido)

---

## Qué es YotsubaEngine

YotsubaEngine es una librería de runtime para construir juegos 2D/3D basada en:

- **Entidades** ligeras con ID y máscara de componentes.
- **Componentes** como datos.
- **Sistemas** para lógica por dominio (render, física, input, scripts, UI).

La clase anfitriona principal es `YTBGame`, que centraliza la configuración de runtime, acceso global a servicios (script/model registry, eventos, scene manager) y la inicialización de subsistemas del motor.

## Objetivos de diseño

- **Multiplataforma**: desktop y mobile (según proyectos de host).
- **Rendimiento**: contenedores especializados y estructuras simples.
- **Escalabilidad**: ECS y separación clara entre datos y comportamiento.
- **Tooling-friendly**: integración con ImGui/Gum y carga de assets configurable.

## Arquitectura (visión de alto nivel)

```text
YTBGame (host)
  ├── SceneManager
  │    └── Scene
  │         └── EntityManager
  │              ├── YotsubaEntities
  │              ├── Component arrays (Transform, Sprite, Physics, ...)
  │              └── Singletons de escena (ej. Camera)
  ├── EventManager
  ├── ScriptRegistry / ModelRegistry
  └── Subsystems (Input, Render2D, Render3D, Physics2D, Script, UI, ...)
```

El `EntityManager` mantiene almacenamiento paralelo por tipo de componente para indexación rápida por `entity.Id`, lo cual reduce búsqueda dinámica y facilita procesado por lotes.

## Módulos principales

### 1) Core

- `Core/Entity/Yotsuba.cs`: entidad base.
- `Core/Component/*`: datos de juego (2D, 3D, agnóstico).
- `Core/System/*`: lógica por dominio (render, física, input, scripts, UI).
- `Core/YotsubaGame/*`: escena, administrador de entidades, eventos, scripts.

### 2) Runtime host

- `YTBGame.cs`: ciclo de vida del juego y coordinación de servicios.

### 3) Graphics

- Render 2D/3D, shaders, atlas y utilidades de GPU.

### 4) Input / Audio / Assets

- Entrada multiplataforma, audio, y sistema de archivos `.ytb` para configuración y datos del juego.

### 5) High-performance types

- `HighestPerformanceTypes/YTB.cs`: arreglo dinámico optimizado para cargas del motor.

## Flujo de frame

Aunque la orquestación exacta depende del host/proyecto, el flujo típico es:

1. **Input**: capturar estado de teclado/mouse/gamepad/touch.
2. **Scripting / lógica**: actualizar comportamiento por entidad/sistema.
3. **Física**: resolver movimiento, colisiones y constraints.
4. **Render**: dibujar 2D/3D y overlays de UI/debug.
5. **UI tooling**: ImGui/Gum para herramientas editoriales en runtime.

## Primeros pasos

### Requisitos

- .NET SDK 10.0
- Restore de paquetes NuGet (MonoGame, ImGui.NET, Gum.MonoGame)

### Compilar el motor

```bash
dotnet restore YotsubaEngine/YotsubaEngine.csproj
dotnet build YotsubaEngine/YotsubaEngine.csproj -c Debug
```

### Compilar proyecto ejemplo (DesktopGL)

```bash
dotnet restore SandBoxGame/SandBoxGame.DesktopGL/SandBoxGame.DesktopGL.csproj
dotnet build SandBoxGame/SandBoxGame.DesktopGL/SandBoxGame.DesktopGL.csproj -c Debug
```

> Nota: el target framework actual del repo está en `net10.0`.

## Configuraciones de compilación

El motor y proyectos asociados usan configuraciones:

- `Debug`
- `Release`
- `YTB` (habilita constantes/flags específicos del ecosistema Yotsuba)

La configuración `YTB` activa símbolos y comportamiento orientado a trabajo de engine/debug (según los `DefineConstants` y propiedades del `.csproj`).

## Estructura del proyecto

Para una vista detallada y árbol completo, revisa:

- [`filetree.md`](filetree.md)

Ese archivo documenta módulos, convenciones y distribución general de carpetas.

## Scripting y registro de scripts

El motor desacopla creación de scripts mediante registro:

- `YTBGame.ScriptRegistry` expone una abstracción para instanciar scripts.
- `ScriptLoader` recibe una ruta/nombre y solicita al registro una instancia concreta.

Ventajas:

- Menor acoplamiento entre runtime y tipos concretos de script.
- Mejor control para escenarios AOT/entornos con restricciones de reflexión.

## Sistema de assets y archivos .ytb

El motor mantiene lógica para leer/escribir metadata y configuración del juego en formato `.ytb` (JSON con convención propia), incluyendo:

- Configuración general del juego.
- Historial de cambios.
- Datos serializados de entidades/escenas.

Revisa el namespace `ActionFiles/YTB Files` para detalles de lectura/escritura y mapeos.

## UI (ImGui + Gum)

YotsubaEngine integra dos capas de UI:

- **ImGui**: tooling, debug, paneles de engine/editor.
- **Gum**: sistema de UI declarativa/controles/layouts para interfaces del juego.

El host (`YTBGame`) inicializa servicios de UI según plataforma/configuración de engine.

## Rendimiento y buenas prácticas

1. Prefiere componentes “plain data” y evita lógica pesada dentro de ellos.
2. Mantén procesamiento por sistema y evita dependencias cruzadas innecesarias.
3. Evita asignaciones por frame en `Update/Draw` (listas temporales, LINQ hot-path, etc.).
4. Aprovecha almacenamiento por índice (`entity.Id`) para acceso directo.
5. Centraliza inicialización de servicios globales (script/model registry, content, etc.) al inicio del juego.

## Troubleshooting

### `dotnet: command not found`

Instala el SDK de .NET 10 y verifica:

```bash
dotnet --info
```

### Fallos al restaurar paquetes MonoGame/Gum

- Revisa conectividad a NuGet.
- Limpia caché si hay conflictos:

```bash
dotnet nuget locals all --clear
dotnet restore
```

### Fuentes/UI no cargan en runtime

- Verifica `Content.RootDirectory` y copia de archivos de `Fonts/` al output.
- Comprueba que la ruta de fuente usada por ImGui exista en el build output.

## Roadmap sugerido

- Guía completa de API pública por subsistema.
- Tutorial oficial de “primer juego 2D” y “primer escenario 3D”.
- Matriz de compatibilidad por plataforma.
- Suite de tests automatizados para validación de ECS/sistemas core.
- Benchmarks reproducibles para `YTB<T>` y rutas críticas de render/update.

---

Si estás adoptando el motor en un proyecto nuevo, el siguiente paso recomendado es documentar **tu propia convención de escenas/componentes** sobre esta base para mantener consistencia del equipo.
