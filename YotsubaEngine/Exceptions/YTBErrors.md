# Catálogo de Errores de Yotsuba Engine (`YTBErrors`)

Este documento lista todos los errores posibles que el motor puede detectar durante el parseo de archivos YTB y la generación de escenas. Cada error se registra con contexto completo (escena, entidad, componente, propiedad) y se muestra al developer en la consola del editor.

---

## Errores Críticos de Estructura del Juego

Estos errores detienen la ejecución del juego por completo. Se lanzan con `throw new GameWontRun(...)`.

### `Unknown` (1 << 0)
| Campo | Valor |
|---|---|
| **Tipo** | Error desconocido / genérico |
| **Mensaje al usuario** | `"No se pudo actualizar el juego correctamente, revise el log para mas detalles."` |
| **Cuándo ocurre** | Cuando ocurre una excepción no prevista durante `UpdateStateOfSceneManager` o `GenerateSceneManager`. |
| **Cómo se corrige** | Revise el log de la consola para identificar la causa raíz del error. |

---

### `CameraNotFound` (1 << 1)
| Campo | Valor |
|---|---|
| **Tipo** | Cámara no encontrada |
| **Mensaje al usuario** | `"El juego necesita que la escena {SceneName} tenga una camara para poder continuar."` |
| **Modal UI** | Título: **"Error Critico"** — `"No se ha encontrado una cámara en la escena. Debe agregar una cámara para correr el juego."` |
| **Cuándo ocurre** | Cuando una escena no tiene ninguna entidad con `CameraComponent3D`. |
| **Cómo se corrige** | Agregue una entidad con un componente `CameraComponent3D` a la escena afectada. |

---

### `CameraFollowNothing` (1 << 2)
| Campo | Valor |
|---|---|
| **Tipo** | La cámara no sigue a ninguna entidad |
| **Mensaje al usuario** | `"El juego necesita que en la escena {SceneName} la camara siga a una entidad. Por favor, selecciona una entidad para que la camara la siga."` |
| **Cuándo ocurre** | Cuando la propiedad `EntityName` de la cámara no corresponde a ninguna entidad existente en la escena. |
| **Cómo se corrige** | En el componente `CameraComponent3D`, asigne un nombre de entidad válido en la propiedad `EntityName`. |

---

### `GameWithoutScenes` (1 << 3)
| Campo | Valor |
|---|---|
| **Tipo** | El juego no tiene escenas |
| **Mensaje al usuario** | `"El juego necesita tener escenas para poder funcionar, por favor, considera crear una."` |
| **Modal UI** | Título: **"Error Critico"** — `"Tu Videojuego no tiene escenas, considera crear una para iniciar"` |
| **Cuándo ocurre** | Cuando el `SceneManager` se genera sin ninguna escena. |
| **Cómo se corrige** | Cree al menos una escena en el proyecto. |

---

### `GameSceneWithoutEntities` (1 << 4)
| Campo | Valor |
|---|---|
| **Tipo** | Una escena no tiene entidades |
| **Mensaje al usuario** | `"Tu escena {Name} no tiene entidades. Si lo dejas asi, tu juego en produccion no funcionara."` |
| **Modal UI** | Título: **"Error Critico"** — `"Una de tus escenas no tiene entidades. Si lo dejas asi, tu juego en produccion no funcionara."` |
| **Cuándo ocurre** | Cuando `element.Entities` es `null` en una escena. |
| **Cómo se corrige** | Agregue al menos una entidad a la escena afectada. |

---

### `GameEngineCannotUpdateFiles` (1 << 5)
| Campo | Valor |
|---|---|
| **Tipo** | El motor no puede actualizar archivos de configuración |
| **Cuándo ocurre** | Cuando hay un error de escritura en los archivos de configuración del engine. |
| **Cómo se corrige** | Verifique permisos de escritura en el directorio del proyecto y que los archivos no estén bloqueados. |

---

### `ScriptHasError` (1 << 6)
| Campo | Valor |
|---|---|
| **Tipo** | Error detectado en un script |
| **Cuándo ocurre** | Cuando un script del usuario tiene errores de compilación o ejecución. |
| **Cómo se corrige** | Revise y corrija los errores en el script indicado. |

---

### `EntityCannotAddToWasdYTB_toolkit` (1 << 7)
| Campo | Valor |
|---|---|
| **Tipo** | La entidad no puede agregarse al toolkit WASD |
| **Cuándo ocurre** | Cuando una entidad no cumple con los requisitos para ser agregada al sistema de movimiento WASD. |
| **Cómo se corrige** | Verifique que la entidad tenga los componentes requeridos para el toolkit WASD. |

---

### `EntityFollowCameraIsNotAppropiate` (1 << 8)
| Campo | Valor |
|---|---|
| **Tipo** | La cámara de seguimiento de la entidad no es apropiada |
| **Cuándo ocurre** | Cuando el comportamiento de cámara asignado a la entidad no es compatible con el contexto actual. |
| **Cómo se corrige** | Ajuste la configuración de cámara de la entidad para que sea compatible con la escena. |

---

## Errores de Parseo de Componentes

Estos errores se registran con `_ = new GameWontRun(...)` (no detienen el parseo, acumulan errores). Se muestran en el panel **"Errores de Parseo"** de la consola con contexto completo.

### Formato del panel UI de errores:

```
Error #N: {ErrorType}           ← Rojo
  Escena: {SceneName}
  Entidad: {EntityName}
  Componente: {ComponentName}
  Propiedad: {PropertyName}
  Detalle: {Message}            ← Amarillo
  Como arreglar: {HowToFix}    ← Verde
```

---

### `ShaderWontBeParse` (1 << 9)
| Campo | Valor |
|---|---|
| **Componente** | `ShaderComponent` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `ShaderPath` | `"ShaderPath está vacío o es nulo"` | Asigne una ruta válida al shader en la propiedad ShaderPath del componente ShaderComponent |
| *(general)* | `"Error al parsear ShaderComponent: {detalle}"` | Revise que la ruta del shader y sus parámetros sean válidos |

---

### `Model3DLoadFailed` (1 << 10)
| Campo | Valor |
|---|---|
| **Componente** | `ModelComponent3D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `ModelPath` | `"Error al cargar ModelComponent3D: {detalle}"` | Revise que la ruta del modelo 3D sea válida y el archivo exista en el Content Pipeline |

---

### `TransformParseFailed` (1 << 11)
| Campo | Valor |
|---|---|
| **Componente** | `TransformComponent` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `Scale` | `"No se pudo parsear Scale: '{valor}'"` | Asegúrese de que Scale sea un número decimal válido (ej: 1.0) |
| `Rotation` | `"No se pudo parsear Rotation: '{valor}'"` | Asegúrese de que Rotation sea un número decimal válido (ej: 0.0) |
| `Position` | `"No se pudo parsear Position: '{valor}'"` | Asegúrese de que Position tenga el formato `X,Y,Z` con números decimales (ej: 0,0,0) |
| `Size` | `"No se pudo parsear Size: '{valor}'"` | Asegúrese de que Size tenga el formato `X,Y,Z` con números decimales (ej: 1,1,1) |
| `LayerDepth` | `"No se pudo parsear LayerDepth: '{valor}'"` | Asegúrese de que LayerDepth sea un número entero válido (ej: 0) |
| `SpriteEffects` | `"No se pudo parsear SpriteEffects: '{valor}'"` | Asegúrese de que SpriteEffects sea un valor válido: `None`, `FlipHorizontally`, `FlipVertically` |
| `Color` | `"Color no reconocido: '{valor}', usando White por defecto"` | Use un nombre de color válido de XNA (ej: Red, Blue, White, Black) |

---

### `SpriteParseFailed` (1 << 12)
| Campo | Valor |
|---|---|
| **Componente** | `SpriteComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `TextureAtlasPath` | `"TextureAtlasPath está vacío o es nulo"` | Asigne una ruta válida al atlas de texturas en el componente SpriteComponent2D |
| *(general)* | `"Error al parsear SpriteComponent2D: {detalle}"` | Revise que todas las propiedades del sprite (TextureAtlasPath, SpriteName, SourceRectangle) sean válidas |

---

### `RigidBody2DParseFailed` (1 << 13)
| Campo | Valor |
|---|---|
| **Componente** | `RigidBodyComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `OffSetCollision` | `"No se pudo parsear OffSetCollision: '{valor}'"` | Asegúrese de que OffSetCollision tenga el formato `X,Y` con números enteros (ej: 0,0) |
| `Velocity` | `"No se pudo parsear Velocity: '{valor}'"` | Asegúrese de que Velocity tenga el formato `X,Y` con números decimales (ej: 0,0) |
| `GameType` | `"No se pudo parsear GameType: '{valor}'"` | Asegúrese de que GameType sea un valor válido del enum `GameType` |
| `Mass` | `"No se pudo parsear Mass: '{valor}'"` | Asegúrese de que Mass sea un valor válido del enum `MassLevel` |

---

### `ButtonParseFailed` (1 << 14)
| Campo | Valor |
|---|---|
| **Componente** | `ButtonComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `EffectiveArea` | `"No se pudo parsear EffectiveArea: '{valor}'"` | Asegúrese de que EffectiveArea tenga el formato `X,Y,Width,Height` con números enteros (ej: 0,0,100,50) |

---

### `AnimationParseFailed` (1 << 15)
| Campo | Valor |
|---|---|
| **Componente** | `AnimationComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `AnimationBindings` | `"AnimationType no válido: '{valor}'"` | Asegúrese de que AnimationType sea un valor válido del enum `AnimationType` |
| `CurrentAnimationType` | `"CurrentAnimationType no válido: '{valor}'"` | Asegúrese de que CurrentAnimationType sea un valor válido del enum `AnimationType` |

---

### `CameraParseFailed` (1 << 16)
| Campo | Valor |
|---|---|
| **Componente** | `CameraComponent3D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `InitialPosition` | `"No se pudo parsear InitialPosition: '{valor}'"` | Asegúrese de que InitialPosition tenga el formato `X,Y,Z` con números decimales (ej: 0,0,0) |
| `AngleView` | `"No se pudo parsear AngleView: '{valor}'"` | Asegúrese de que AngleView sea un número decimal válido (ej: 45.0) |
| `NearRender` | `"No se pudo parsear NearRender: '{valor}'"` | Asegúrese de que NearRender sea un número decimal válido (ej: 0.1) |
| `FarRender` | `"No se pudo parsear FarRender: '{valor}'"` | Asegúrese de que FarRender sea un número decimal válido (ej: 1000.0) |

---

### `InputParseFailed` (1 << 17)
| Campo | Valor |
|---|---|
| **Componente** | `InputComponent` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `InputsInUse` | `"Flag de input inválido: '{valor}'"` | Asegúrese de que cada flag de InputsInUse sea un valor válido del enum `InputInUse` |
| `GamePadIndex` | `"GamePadIndex inválido: '{valor}'"` | Asegúrese de que GamePadIndex sea un valor válido: `One`, `Two`, `Three`, `Four` |
| `KeyboardMappings` | `"Formato inválido en KeyboardMappings: '{valor}'. Formato esperado: 'Action:Key'"` | Cada mapping debe tener el formato `Accion:Tecla` separados por coma (ej: MoveUp:W,MoveDown:S) |
| `KeyboardMappings` | `"Acción inválida en KeyboardMappings: '{valor}'"` | Asegúrese de que la acción sea un valor válido del enum `ActionEntityInput` |
| `KeyboardMappings` | `"Tecla inválida en KeyboardMappings: '{valor}'"` | Asegúrese de que la tecla sea un valor válido del enum `Keys` (ej: W, A, S, D, Space) |
| `MouseMappings` | `"Formato inválido en MouseMappings: '{valor}'. Formato esperado: 'Action:Button'"` | Cada mapping debe tener el formato `Accion:Boton` separados por coma (ej: Attack:Left,Dash:Right) |
| `MouseMappings` | `"Acción inválida en MouseMappings: '{valor}'"` | Asegúrese de que la acción sea un valor válido del enum `ActionEntityInput` |
| `MouseMappings` | `"Botón de mouse inválido en MouseMappings: '{valor}'"` | Asegúrese de que el botón sea un valor válido del enum `MouseButton` (ej: Left, Right, Middle) |

---

### `ScriptParseFailed` (1 << 18)
| Campo | Valor |
|---|---|
| **Componente** | `ScriptComponent` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| *(reservado)* | *(Actualmente no se dispara desde YTBFileToGameData, reservado para uso futuro)* | — |

---

### `FontParseFailed` (1 << 19)
| Campo | Valor |
|---|---|
| **Componente** | `FontComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| `Font` | `"La propiedad Font está vacía o es nula"` | Asigne una fuente válida en la propiedad Font del componente FontComponent2D |
| *(general)* | `"Error al parsear FontComponent2D: {detalle}"` | Revise que todas las propiedades del FontComponent2D sean válidas |

---

### `TileMapParseFailed` (1 << 20)
| Campo | Valor |
|---|---|
| **Componente** | `TileMapComponent2D` |

| Propiedad | Mensaje al usuario | Cómo arreglar |
|---|---|---|
| *(reservado)* | *(Actualmente no se dispara desde YTBFileToGameData, reservado para uso futuro)* | — |

---

## Comportamiento de Limpieza de Errores

Cuando el developer presiona **"Recompilar Assets"** en la barra de menú, se ejecuta `UpdateStateOfSceneManager()`, que llama a `GameWontRun.Reset()` al inicio. Esto:

1. Limpia **todas** las banderas de error (`ErrorStorage = 0`)
2. Restablece `GameWontRunByException = false`
3. Limpia las listas de mensajes (`CauseWontRunByException`, `DetailWontRunByException`)
4. Vacía `ErrorDetails` (la lista de errores detallados)

Si la recompilación es exitosa, los errores permanecen limpios. Si se detectan nuevos errores, se registran nuevamente con su contexto actualizado.

---

## Referencia de la Clase `ErrorDetail`

```csharp
public class ErrorDetail
{
    public GameWontRun.YTBErrors ErrorType { get; set; }
    public string SceneName { get; set; }
    public string EntityName { get; set; }
    public string ComponentName { get; set; }
    public string PropertyName { get; set; }
    public string Message { get; set; }
    public string HowToFix { get; set; }
}
```

Cada `ErrorDetail` se crea automáticamente al usar el constructor contextual:
```csharp
_ = new GameWontRun(YTBErrors.ErrorType, "NombreEscena", "NombreEntidad", "NombreComponente", "NombrePropiedad", "Mensaje descriptivo", "Cómo arreglarlo");
```
