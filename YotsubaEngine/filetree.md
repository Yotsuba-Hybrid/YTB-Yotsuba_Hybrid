# YotsubaEngine - Estructura de Archivos

**Última actualización:** 2026-02-07

Esta es la estructura completa de directorios y archivos del proyecto YotsubaEngine.

> **Nota:** Este archivo documenta la arquitectura del motor. Se actualiza regularmente para reflejar cambios en la estructura del proyecto.
> **Actualizado por Copilot:** Se verificó la estructura principal del eje antes de entregar este cambio.

```
.
├── ActionFiles                          # Manejo de assets y archivos del juego
│   ├── ImGuiEditor
│   │   └── imgui.ini                    # Configuración del editor ImGui
│   ├── TMX Files                        # Soporte para mapas de Tiled Editor
│   │   └── TiledCS
│   │       └── TiledManager.cs          # Gestor de archivos .tmx
│   ├── XML SpriteSheet Files            # Manejo de TextureAtlas XML
│   │   ├── SpriteInfo.cs                # Información de sprites individuales
│   │   ├── SpriteSheetFiles.cs          # Gestión de hojas de sprites
│   │   └── TexturePacker.cs             # Parser de formato TexturePacker
│   └── YTB Files                        # Sistema de archivos .ytb (JSON)
│       ├── ReadYTBFile.cs               # Lectura de archivos .ytb
│       ├── WriteYTBFile.cs              # Escritura de archivos .ytb
│       ├── YTBConfig.cs                 # Configuración del juego
│       ├── YTBEngineHistory.cs          # Historial de cambios
│       ├── YTBFileToGameData.cs         # Conversión de .ytb a datos de juego
│       ├── YTBGameInfo.cs               # Información del juego
│       └── YotsubaJsonContext.cs        # Contexto de serialización JSON
├── Attributes
│   └── ScriptAttributte.cs              # Atributo [Script] para marcar scripts
├── Audio                                # Sistema de audio completo
│   ├── AudioAssets.cs                   # Assets de audio
│   ├── AudioSystem.cs                   # Sistema de audio (música, SFX, volumen)
│   └── IAudioRegistry.cs                # Interfaz para registro de audio
├── Core                                 # Núcleo del engine (ECS)
│   ├── Component                        # Todos los componentes del engine
│   │   ├── C_2D                         # Componentes 2D
│   │   │   ├── AnimationComponent2D.cs  # Animaciones 2D con múltiples estados
│   │   │   ├── ButtonComponent2D.cs     # Botones interactivos
│   │   │   ├── FontComponent2D.cs       # Renderizado de texto
│   │   │   ├── RigidBodyComponent2D.cs  # Física 2D (Platform/TopDown)
│   │   │   ├── ShaderComponent2D.cs     # Efectos de shader en 2D
│   │   │   ├── SpriteComponent2D.cs     # Sprites 2D
│   │   │   └── TileMapComponent2D.cs    # Mapas de tiles (Tiled)
│   │   ├── C_3D                         # Componentes 3D
│   │   │   ├── ModelComponent3D.cs      # Modelos 3D
│   │   │   └── RigidBodyComponent3D.cs  # Física 3D
│   │   └── C_AGNOSTIC                   # Componentes independientes de dimensión
│   │       ├── CameraComponent.cs       # Sistema de cámara (2D/3D)
│   │       ├── InputComponent.cs        # Manejo de input (teclado, gamepad, mouse)
│   │       ├── RigidBody.cs             # Base para física
│   │       ├── ScriptComponent.cs       # Vinculación de scripts
│   │       └── TransformComponent.cs    # Posición, rotación, escala
│   ├── Entity
│   │   └── Yotsuba.cs                   # Clase base de entidades
│   ├── System                           # Todos los sistemas del engine
│   │   ├── Contract
│   │   │   └── ISystem.cs               # Interfaz base de sistemas
│   │   ├── GumUI                        # Sistema de UI avanzado
│   │   │   ├── YTBGum.cs                # Core de GumUI
│   │   │   ├── YTBGumControls.cs        # Controles (Button, TextBox, Slider, etc.)
│   │   │   ├── YTBGumLayouts.cs         # Layouts (Vertical, Horizontal, Grid)
│   │   │   ├── YTBGumService.cs         # Servicios de GumUI
│   │   │   └── YTBGumStyles.cs          # Sistema de estilos reutilizables
│   │   ├── S_2D                         # Sistemas 2D
│   │   │   ├── AnimationSystem2D.cs     # Procesamiento de animaciones 2D
│   │   │   ├── ButtonSystem2D.cs        # Sistema de botones interactivos
│   │   │   ├── DebugDrawSystem.cs       # Dibujado de debug
│   │   │   ├── FontSystem2D.cs          # Renderizado de fuentes
│   │   │   ├── GumUISystem2D.cs         # Sistema de UI
│   │   │   ├── PhysicsSystem2D.cs       # Física 2D (colisiones, gravedad)
│   │   │   ├── RenderSystem2D.cs        # Renderizado 2D
│   │   │   └── TileMapSystem2D.cs       # Renderizado de tilemaps
│   │   ├── S_3D
│   │   │   └── RenderSystem3D.cs        # Renderizado 3D
│   │   ├── S_AGNOSTIC                   # Sistemas generales
│   │   │   ├── CameraSystem.cs          # Sistema de cámara
│   │   │   ├── InputSystem.cs           # Procesamiento de input
│   │   │   └── ScriptSystem.cs          # Ejecución de scripts
│   │   ├── YotsubaEngineCore            # Core del engine
│   │   │   ├── CheckScriptListeners.cs  # Verificación de listeners de scripts
│   │   │   ├── DragAndDropSystem.cs     # Sistema de drag & drop de archivos
│   │   │   ├── FontDragSystem.cs        # Drag & drop específico de fuentes
│   │   │   ├── README_FONTDRAG.md       # Documentación de font drag
│   │   │   ├── README_HOTRELOAD.md      # Documentación de hot-reload
│   │   │   ├── YTBContentBuilder.cs     # Constructor de contenido
│   │   │   └── YTBGlobalState.cs        # Estado global del engine
│   │   └── YotsubaEngineUI              # Editor integrado ImGui
│   │       ├── UI
│   │       │   ├── ColorPicker.cs       # Selector de color avanzado
│   │       │   ├── ConsoleUI.cs         # Consola de debug interactiva
│   │       │   ├── DebugOverlayUI.cs    # Overlay de información de rendimiento
│   │       │   ├── EntityManagerUI.cs   # Editor de entidades y componentes
│   │       │   ├── HistoryUI.cs         # Historial de cambios del proyecto
│   │       │   ├── MenuBarUI.cs         # Menú principal del editor
│   │       │   └── SceneManagerUI.cs    # Gestor visual de escenas
│   │       ├── EngineUISystem.cs        # Sistema de UI del engine
│   │       └── YTBGui.cs                # GUI principal
│   ├── YTBControls                      # Controles custom del engine
│   │   ├── Buttons
│   │   │   └── YTBButton.cs             # Botón customizado
│   │   └── YTBControl.cs                # Base de controles
│   ├── YTBMath
│   │   └── YTBCartessian.cs             # Matemáticas cartesianas
│   └── YotsubaGame                      # Gestores del juego
│       ├── Scripting                    # Sistema de scripting
│       │   ├── BaseScript.cs            # Clase base de scripts
│       │   ├── IScriptManager.cs        # Interfaz de gestor de scripts
│       │   ├── ScriptInterfaces.cs      # Interfaces de eventos (ICollisionListener, etc.)
│       │   └── ScriptLoader.cs          # Cargador de scripts
│       ├── EntityManager.cs             # Gestor de entidades
│       ├── EventManager.cs              # Sistema de eventos global
│       ├── InputManager.cs              # Gestor de input
│       ├── Scene.cs                     # Clase de escena
│       └── SceneManager.cs              # Gestor de escenas
├── Events
│   └── YTBEvents.cs                     # Eventos del engine
├── Exceptions
│   ├── AddComponentInDiferentEntityIndexException.cs
│   └── GameWontRun.cs                   # Excepciones custom
├── Fonts
│   ├── Hud.xnb                          # Fuente compilada HUD
│   ├── JetBrainsMono-VariableFont_wght.ttf
│   └── LibertinusMath-Regular.ttf       # Fuentes incluidas
├── Graphics                             # Sistema de gráficos
│   ├── ImGui                            # Integración ImGui
│   │   ├── DrawVertDeclaration.cs
│   │   ├── GetUniqueImGuiID.cs
│   │   ├── ImGuiAdvancedControls.cs
│   │   └── ImGuiRenderer.cs             # Renderer de ImGui
│   ├── Shaders                          # Shaders incluidos
│   │   ├── BrightnessContrast.fx        # Shader de brillo/contraste
│   │   ├── ColorTint.fx                 # Shader de tinte de color
│   │   ├── Grayscale.fx                 # Shader de escala de grises
│   │   ├── README_SHADERS.md            # Documentación de shaders
│   │   ├── SHADER_COMPILATION_GUIDE.md  # Guía de compilación
│   │   ├── Saturation.fx                # Shader de saturación
│   │   ├── SceneTransition.cs           # Transiciones de escena
│   │   ├── ShaderManager.cs             # Gestor de shaders
│   │   └── Transition.fx                # Shader de transición
│   ├── TextureAtlasXMLExamples
│   │   └── TextureAtlasExample.xml      # Ejemplo de TextureAtlas
│   ├── Animation.cs                     # Sistema de animaciones
│   ├── Graphics3D.cs                    # Utilidades 3D
│   ├── IModelRegistry.cs                # Clase base abstracta para registro de modelos 3D
│   ├── TextureAtlas.cs                  # Manejo de TextureAtlas
│   ├── TextureRegion.cs                 # Regiones de textura
│   └── YotsubaGraphicsManager.cs        # Gestor de gráficos
├── HighestPerformanceTypes
│   └── YTB.cs                           # Tipos optimizados de alto rendimiento
├── Input                                # Sistema de input
│   ├── GamePadInfo.cs                   # Información de gamepad
│   ├── InputHelpers.cs                  # Helpers de input
│   ├── KeyboardInfo.cs                  # Información de teclado
│   ├── MouseInfo.cs                     # Información de mouse
│   └── TouchInfo.cs                     # Soporte táctil (móviles)
├── Tasks                                # Tareas y documentación del proyecto
│   ├── Complete                         # Tareas completadas
│   │   ├── Feature - Input.md
│   │   ├── ISSUE_HOTRELOAD_SYSTEM.md
│   │   ├── InputComponent-Usage.md
│   │   ├── NEXT_STEPS.md
│   │   ├── README.md
│   │   └── SUMMARY_HOTRELOAD.md
│   ├── IASuggestions                    # Sugerencias de IA
│   │   └── RuntimeComponentLoader.cs
│   ├── InProcess                        # En proceso
│   │   └── Feature - ImGui.md
│   └── Pending                          # Pendientes
│       ├── Feature - DragAndDrop.md
│       ├── Feature - TileMap - TileMapCollitions.md
│       └── ISSUES-REPORT.md
├── Templates
│   └── EntityYTBXmlTemplate.cs          # Templates de componentes para .ytb
├── UserCases                            # Casos de uso y ejemplos
│   ├── README_WASD_PHYSICS.md           # Ejemplo de física WASD
│   ├── SpriteBatch.md                   # Ejemplo de SpriteBatch
│   └── YTBGumUI.md                      # Ejemplo de GumUI
├── YTB_Toolkit                          # Herramientas del engine
│   ├── SystemCall.cs                    # Llamadas útiles del sistema
│   └── WASD.cs                          # Sistema WASD automático
├── Game1.cs                             # Clase Game1 de MonoGame
├── README.md                            # ⭐ Documentación principal completa
├── YTBGame.cs                           # Clase principal del juego
├── YotsubaEngine.csproj                 # Archivo del proyecto
├── YotsubaEngine.slnx                   # Solución
├── filetree.md                          # 📄 Este archivo
└── imgui.ini                            # Configuración de ImGui

Total: 46 directorios, 134 archivos
```

## Estructura Resumen

### 📦 Componentes
- **2D:** 7 componentes (Animation, Button, Font, RigidBody, Shader, Sprite, TileMap)
- **3D:** 2 componentes (Model, RigidBody)
- **Agnostic:** 5 componentes (Camera, Input, RigidBody, Script, Transform)

### 🎮 Sistemas
- **2D:** 8 sistemas (Animation, Button, Debug, Font, GumUI, Physics, Render, TileMap)
- **3D:** 1 sistema (Render)
- **Agnostic:** 3 sistemas (Camera, Input, Script)
- **Core:** 3 sistemas (DragAndDrop, FontDrag, CheckScriptListeners)
- **UI:** 1 sistema (EngineUI)
- **GumUI:** 5 archivos (Core, Controls, Layouts, Service, Styles)

### 🎨 Shaders Incluidos
1. **BrightnessContrast.fx** - Ajusta brillo y contraste
2. **ColorTint.fx** - Aplica tinte de color
3. **Grayscale.fx** - Convierte a escala de grises
4. **Saturation.fx** - Ajusta saturación
5. **Transition.fx** - Efectos de transición entre escenas

### 📝 Archivos de Configuración
- **YotsubaGame.ytb** - Escenas y entidades (en proyecto de juego)
- **YotsubaGameConfig.ytb** - Configuración del juego
- **YotsubaEngineHistory.ytb** - Historial de cambios
- **imgui.ini** - Configuración del editor

### 🔧 Herramientas
- **SystemCall.cs** - API de utilidades
- **WASD.cs** - Sistema de movimiento automático
- **YTBGlobalState.cs** - Estado global del engine

---

## Convenciones del Proyecto

### 📏 Nomenclatura
- **Prefijo YTB**: Tipos principales del engine usan el prefijo "YTB" (ej: `YTBGame`, `YTBGum`, `YTBButton`)
- **Componentes**: Sufijo `Component` + dimensión (ej: `SpriteComponent2D`, `CameraComponent`)
- **Sistemas**: Sufijo `System` + dimensión (ej: `RenderSystem2D`, `PhysicsSystem2D`)
- **Archivos .ytb**: Formato JSON con extensión personalizada para configuración del juego

### 🏗️ Arquitectura
El motor sigue el patrón **ECS (Entity-Component-System)**:
- **Entidades** (`Yotsuba`): Contenedores con ID y bitmask de componentes
- **Componentes**: Datos puros sin lógica (ej: `TransformComponent`, `SpriteComponent2D`)
- **Sistemas**: Lógica que procesa entidades con componentes específicos

### 🎯 Prioridades de Diseño
1. **Multiplataforma**: Windows, Linux, macOS, iOS, Android
2. **AOT-Compatible**: Sin reflexión dinámica en runtime
3. **Alto Rendimiento**: Minimizar asignaciones en `Update`/`Draw`
4. **Hot-Reload**: Recarga de scripts y assets en tiempo de desarrollo

### 📁 Archivos Clave
- **YTBGame.cs**: Punto de entrada principal del motor
- **YotsubaEngine.csproj**: Configuración del proyecto .NET
- **filetree.md**: Este archivo (documentación de estructura)
- **README.md**: Documentación completa de APIs y uso

---

**Nota:** Esta estructura excluye los directorios `bin/`, `obj/`, `.vs/` y `.git/` que son generados automáticamente.

