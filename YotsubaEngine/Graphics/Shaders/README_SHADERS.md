# Sistema de Shaders en YotsubaEngine

## Descripción

El sistema de shaders de YotsubaEngine permite aplicar efectos visuales a las entidades 2D de forma eficiente y multiplataforma. Todos los shaders son compatibles con AOT (Ahead-of-Time compilation) y funcionan en todas las plataformas soportadas por MonoGame.

## Shaders Disponibles

### 1. Grayscale (Escala de Grises)
Convierte la textura a escala de grises usando el método de luminosidad.

**Archivo:** `Shaders/Grayscale.fx`

**Uso:**
```csharp
// Cargar el shader
var grayscaleShader = YotsubaGraphicsManager.GrayscaleShaderGenerator(contentManager);

// Agregar el componente a una entidad
entityManager.AddShaderComponent2D(entity, grayscaleShader);
```

### 2. Saturation (Saturación de Color)
Controla la saturación del color de la textura.

**Archivo:** `Shaders/Saturation.fx`

**Parámetros:**
- `Saturation` (float): 0.0 = escala de grises, 1.0 = color normal, >1.0 = sobresaturado

**Uso:**
```csharp
// Crear shader con saturación específica
var saturationShader = YotsubaGraphicsManager.SaturationShaderGenerator(contentManager, 0.5f);

// Agregar a entidad
entityManager.AddShaderComponent2D(entity, saturationShader);

// Modificar saturación dinámicamente
ref var shaderComp = ref entityManager.Shader2DComponents[entity.Id];
shaderComp.Parameters.Saturation = 1.5f; // Sobresaturación
```

### 3. ColorTint (Tinte de Color)
Aplica un tinte de color a la textura.

**Archivo:** `Shaders/ColorTint.fx`

**Parámetros:**
- `TintColor` (Vector4): Color RGB del tinte, alpha controla la intensidad

**Uso:**
```csharp
// Crear tinte rojo con 50% de intensidad
var tintColor = new Vector4(1.0f, 0.0f, 0.0f, 0.5f); // RGB + intensidad en alpha
var tintShader = YotsubaGraphicsManager.ColorTintShaderGenerator(contentManager, tintColor);

entityManager.AddShaderComponent2D(entity, tintShader);
```

### 4. BrightnessContrast (Brillo y Contraste)
Ajusta el brillo y contraste de la textura.

**Archivo:** `Shaders/BrightnessContrast.fx`

**Parámetros:**
- `Brightness` (float): -1.0 a 1.0 (0.0 = sin cambio)
- `Contrast` (float): 0.0 a 2.0 (1.0 = sin cambio)

**Uso:**
```csharp
var bcShader = YotsubaGraphicsManager.BrightnessContrastShaderGenerator(
    contentManager, 
    brightness: 0.2f,  // Más brillante
    contrast: 1.3f     // Más contraste
);

entityManager.AddShaderComponent2D(entity, bcShader);
```

### 5. Transition (Transición)
Shader para transiciones entre escenas o efectos de fade/dissolve.

**Archivo:** `Shaders/Transition.fx`

**Parámetros:**
- `Progress` (float): 0.0 = sin transición, 1.0 = completamente transitado
- `TransitionType` (float): 0 = fade, 1 = dissolve

**Uso:**
```csharp
var transitionEffect = contentManager.Load<Effect>("Shaders/Transition");
var transitionShader = new ShaderComponent2D(transitionEffect, "Transition", true);
transitionShader.Parameters.Progress = 0.5f;      // 50% de transición
transitionShader.Parameters.TransitionType = 0.0f; // Fade

entityManager.AddShaderComponent2D(entity, transitionShader);
```

## Sistema de Transiciones de Escena

Para facilitar las transiciones entre escenas, usa la clase `SceneTransition`:

```csharp
using YotsubaEngine.Graphics.Shaders;

// Crear sistema de transición
var sceneTransition = new SceneTransition();

// Iniciar transición
sceneTransition.StartTransition(
    TransitionType.Fade, 
    duration: 2.0f,      // 2 segundos
    onComplete: () => {
        // Cambiar a nueva escena aquí
        sceneManager.LoadScene("NuevaEscena");
    }
);

// En el método Update
sceneTransition.Update(gameTime);

// En el método Draw (si quieres aplicar el efecto a toda la pantalla)
var transitionEffect = sceneTransition.GetTransitionEffect();
if (transitionEffect != null)
{
    // Aplicar el efecto al dibujar
}
```

## ShaderManager

El `ShaderManager` gestiona la carga y caché de shaders:

```csharp
using YotsubaEngine.Graphics.Shaders;

// Inicializar (solo una vez al inicio del juego)
ShaderManager.Initialize(contentManager);

// Cargar un shader
Effect myShader = ShaderManager.LoadShader("Shaders/Saturation");

// Verificar si está cargado
bool isLoaded = ShaderManager.IsShaderLoaded("Shaders/Saturation");

// Clonar un shader para uso independiente (consume más memoria)
Effect clonedShader = ShaderManager.CloneShader("Shaders/Saturation");

// Descargar todos los shaders al cambiar de escena
ShaderManager.UnloadAll();
```

## Uso Avanzado: Shader Personalizado

Para crear tu propio shader:

1. Crea un archivo `.fx` en la carpeta `Shaders/`
2. Compílalo con el Content Pipeline de MonoGame
3. Cárgalo en código:

```csharp
// Cargar shader personalizado
var customEffect = contentManager.Load<Effect>("Shaders/MiShader");

// Crear componente
var customShader = new ShaderComponent2D(customEffect, "MiTecnica", true);

// Configurar parámetros manualmente
customShader.Parameters.Saturation = 0.8f;
// O acceder directamente al Effect:
customEffect.Parameters["MiParametro"]?.SetValue(1.5f);

// Agregar a entidad
entityManager.AddShaderComponent2D(entity, customShader);
```

## Consideraciones de Rendimiento

1. **Batching:** Las entidades con shaders se dibujan en pases separados, lo que rompe el batching. Usa shaders solo cuando sea necesario.

2. **Clonación de Efectos:** Si necesitas parámetros diferentes por entidad, clona el efecto:
   ```csharp
   var clonedEffect = ShaderManager.CloneShader("Shaders/Saturation");
   ```

3. **Activar/Desactivar Shaders:** Puedes desactivar temporalmente un shader sin eliminarlo:
   ```csharp
   ref var shaderComp = ref entityManager.Shader2DComponents[entity.Id];
   shaderComp.IsActive = false; // El shader no se aplicará
   ```

4. **Minimizar Draw Calls:** Agrupa entidades que usan el mismo shader para reducir cambios de estado.

## Multiplataforma y AOT

Todos los shaders están diseñados para funcionar en:
- Windows (DirectX)
- Linux/macOS (OpenGL)
- iOS/Android (OpenGL ES)
- Consolas (con las adaptaciones necesarias)

Los shaders usan `#if OPENGL` para adaptar automáticamente el código según la plataforma.

## Compilación de Shaders

Los archivos `.fx` deben compilarse usando el Content Pipeline de MonoGame. Asegúrate de agregarlos a tu proyecto de contenido (`.mgcb`).

Para compilar manualmente:
```bash
mgcb-editor Content.mgcb
```

Agrega los archivos `.fx` y configura el procesador como "Effect - MonoGame".

## Ejemplos Completos

### Ejemplo 1: Entidad con Efecto de Daño (Flash Rojo)

```csharp
// Al recibir daño, aplicar tinte rojo
var damageFlash = YotsubaGraphicsManager.ColorTintShaderGenerator(
    contentManager, 
    new Vector4(1.0f, 0.0f, 0.0f, 0.8f)
);
entityManager.AddShaderComponent2D(playerEntity, damageFlash);

// Después de 0.2 segundos, remover el shader
// (implementar con timer o coroutine)
```

### Ejemplo 2: Entidad en Estado "Congelado"

```csharp
// Reducir saturación y aumentar brillo para efecto de hielo
var frozenShader = YotsubaGraphicsManager.SaturationShaderGenerator(contentManager, 0.3f);
entityManager.AddShaderComponent2D(frozenEntity, frozenShader);
```

### Ejemplo 3: Transición de Escena con Fade

```csharp
var transition = new SceneTransition();

// Fade out de 1 segundo antes de cambiar de escena
transition.StartTransition(TransitionType.Fade, 1.0f, () => {
    sceneManager.ChangeScene("MainMenu");
});
```

## Notas Importantes

- Los shaders solo afectan a entidades con `SpriteComponent2D` y `TransformComponent`.
- Los shaders NO se aplican a botones UI en el segundo pase de renderizado (por diseño).
- Modificar parámetros del shader en tiempo real no causa asignaciones de memoria (struct pass-by-reference).

## Referencia de Tutoriales

Este sistema se basa en los tutoriales oficiales de MonoGame:
- [Tutorial Básico de Shaders 2D](https://docs.monogame.net/articles/tutorials/building_2d_games/24_shaders/index.html)
- [Tutorial Avanzado de Shaders 2D](https://sponsors.monogame.net/articles/tutorials/advanced/2d_shaders/01_introduction/)
