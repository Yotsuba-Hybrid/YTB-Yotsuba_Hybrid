# SpriteBatch - Guía Completa de Parámetros

## Introducción

El `SpriteBatch` es la clase principal de MonoGame para renderizar sprites 2D de manera eficiente. Agrupa múltiples llamadas de dibujo (Draw) en un solo "batch" para optimizar el rendimiento de la GPU.

## Método `SpriteBatch.Begin()`

Este método inicia una operación de batch de sprites. Todos los parámetros son opcionales y tienen valores predeterminados.

### Firma del Método

```csharp
public void Begin(
    SpriteSortMode sortMode = SpriteSortMode.Deferred,
    BlendState blendState = null,
    SamplerState samplerState = null,
    DepthStencilState depthStencilState = null,
    RasterizerState rasterizerState = null,
    Effect effect = null,
    Matrix? transformMatrix = null
)
```

---

## 1. SpriteSortMode (`sortMode`)

Determina el orden en que los sprites son dibujados y cómo se agrupan para el renderizado.

### Opciones Disponibles

| Opción | Descripción |
|--------|-------------|
| `Deferred` | **(Por defecto)** Los sprites se agrupan y ordenan al final del batch para la entrega más eficiente a la GPU. Es la opción más eficiente para la mayoría de casos. |
| `Immediate` | Cada sprite se dibuja inmediatamente al llamar `Draw()`. Menos eficiente, pero permite cambiar el estado de la GPU entre dibujos. Útil para shaders dinámicos. |
| `Texture` | Optimiza los cambios de textura agrupando todos los sprites por su textura. Reduce los "draw calls" cuando tienes múltiples sprites con diferentes texturas. |
| `BackToFront` | Ordena los sprites por su profundidad de capa (layerDepth) de atrás hacia adelante. Esencial para transparencias correctas. |
| `FrontToBack` | Ordena los sprites por su profundidad de capa de adelante hacia atrás. Útil para optimización con z-buffer en sprites opacos. |

### Ejemplo de Uso

```csharp
// Para transparencias correctas con ordenamiento
spriteBatch.Begin(SpriteSortMode.BackToFront);

// Para máximo rendimiento sin transparencias
spriteBatch.Begin(SpriteSortMode.Deferred);

// Para shaders dinámicos que cambian entre sprites
spriteBatch.Begin(SpriteSortMode.Immediate);
```

### Cuándo Usar Cada Uno

- **Deferred**: Juegos con muchos sprites del mismo tipo, sin necesidad de ordenamiento especial.
- **Immediate**: Cuando necesitas cambiar shaders o estados entre cada sprite.
- **Texture**: Cuando tienes muchas texturas diferentes y quieres optimizar cambios de textura.
- **BackToFront**: Para sprites transparentes que se superponen.
- **FrontToBack**: Para sprites opacos con optimización de z-buffer.

---

## 2. BlendState (`blendState`)

Controla cómo los colores de los sprites se mezclan con los colores ya existentes en el render target (pantalla).

### Opciones Predefinidas

| Opción | Descripción |
|--------|-------------|
| `AlphaBlend` | **(Por defecto si es `null`)** Soporta transparencia usando el canal alfa de la imagen. Los sprites premultiplican el alfa. |
| `NonPremultiplied` | Similar a AlphaBlend pero para texturas que **NO** tienen alfa premultiplicado. Usar si tus PNGs se ven con bordes oscuros. |
| `Additive` | Los colores se **suman**. Ideal para efectos de luz, brillos, partículas de fuego, explosiones. |
| `Opaque` | Desactiva la mezcla. Dibuja sprites como sólidos, ignorando la transparencia. Máximo rendimiento para fondos. |

### Ejemplo de Uso

```csharp
// Para sprites normales con transparencia
spriteBatch.Begin(blendState: BlendState.AlphaBlend);

// Para efectos de luz/brillo
spriteBatch.Begin(blendState: BlendState.Additive);

// Para fondos sin transparencia (más eficiente)
spriteBatch.Begin(blendState: BlendState.Opaque);

// Para texturas PNG sin alfa premultiplicado
spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
```

### Diferencia entre AlphaBlend y NonPremultiplied

- **AlphaBlend**: La textura tiene sus colores RGB ya multiplicados por el alfa. (MonoGame ContentPipeline hace esto por defecto)
- **NonPremultiplied**: La textura tiene los colores RGB originales. Usar cuando cargas texturas manualmente sin el ContentPipeline.

---

## 3. SamplerState (`samplerState`)

Controla cómo se muestrean las texturas cuando se escalan, rotan o transforman. **Este es el parámetro crítico para pixel art vs arte suavizado.**

### Opciones Predefinidas

| Opción | Filtrado | Wrapping | Descripción |
|--------|----------|----------|-------------|
| `LinearClamp` | Linear (Bilinear) | Clamp | **(Por defecto)** Suaviza los píxeles al escalar. Clampea coordenadas UV. **Causa blur en pixel art.** |
| `LinearWrap` | Linear (Bilinear) | Wrap | Suaviza los píxeles. Las texturas se repiten al salir del rango [0,1]. |
| `PointClamp` | Point (Nearest) | Clamp | Muestreo de vecino más cercano. **Ideal para pixel art.** Mantiene bordes nítidos. |
| `PointWrap` | Point (Nearest) | Wrap | Vecino más cercano con repetición de textura. |
| `AnisotropicClamp` | Anisotropic | Clamp | Filtrado avanzado para superficies inclinadas. Usado principalmente en 3D. |
| `AnisotropicWrap` | Anisotropic | Wrap | Filtrado anisotrópico con repetición de textura. |

### Tipos de Filtrado

#### Linear (Bilinear)
- Interpola entre los 4 píxeles más cercanos
- Produce resultados suaves/borrosos
- Ideal para arte de alta resolución o fotografías
- **NO recomendado para pixel art**

#### Point (Nearest Neighbor)
- Selecciona el píxel más cercano sin interpolación
- Mantiene los bordes de los píxeles nítidos
- **Recomendado para pixel art y tilemaps**
- Produce el aspecto "retro" clásico

#### Anisotropic
- Filtrado avanzado que considera el ángulo de la superficie
- Mejor calidad para texturas vistas en ángulo
- Más costoso en rendimiento
- Usado principalmente en juegos 3D

### Tipos de Wrapping (Direccionamiento)

#### Clamp
- Las coordenadas UV fuera del rango [0,1] se "clampean" al borde
- El borde de la textura se extiende infinitamente
- Ideal para sprites individuales

#### Wrap
- Las coordenadas UV fuera del rango [0,1] se repiten
- La textura se repite como un mosaico
- Ideal para fondos repetitivos, texturas de terreno

### Ejemplo de Uso

```csharp
// ✅ CORRECTO para pixel art y tilemaps
spriteBatch.Begin(samplerState: SamplerState.PointClamp);

// Para arte de alta resolución con escalado suave
spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

// Para fondos que se repiten (scrolling parallax)
spriteBatch.Begin(samplerState: SamplerState.PointWrap);

// Para texturas suaves que se repiten
spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
```

### ⚠️ Problema Común: Sprites Borrosos

Si tus sprites o tilemaps se ven borrosos al hacer zoom o escalar:

```csharp
// ❌ INCORRECTO - Causa blur en pixel art
spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

// ✅ CORRECTO - Mantiene píxeles nítidos
spriteBatch.Begin(samplerState: SamplerState.PointClamp);
```

---

## 4. DepthStencilState (`depthStencilState`)

Controla cómo se manejan los datos de profundidad (z-buffer) y stencil para el renderizado.

### Opciones Predefinidas

| Opción | Descripción |
|--------|-------------|
| `Default` | Habilita el buffer de profundidad para lectura y escritura. Usado en 3D. |
| `DepthRead` | Habilita el buffer de profundidad solo para lectura (no escribe). Útil para partículas transparentes en 3D. |
| `None` | Desactiva el buffer de profundidad. **(Común para juegos 2D)** |

### Propiedades Principales

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `DepthBufferEnable` | bool | Activa/desactiva el test de profundidad |
| `DepthBufferWriteEnable` | bool | Activa/desactiva la escritura al z-buffer |
| `DepthBufferFunction` | CompareFunction | Función de comparación de profundidad |
| `StencilEnable` | bool | Activa/desactiva el stencil buffer |
| `StencilFunction` | CompareFunction | Función para test de stencil |
| `ReferenceStencil` | int | Valor de referencia para comparación |

### Ejemplo de Uso

```csharp
// Para juegos 2D (sin z-buffer)
spriteBatch.Begin(depthStencilState: DepthStencilState.None);

// Para juegos 3D con profundidad
spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

// Para partículas transparentes en 3D
spriteBatch.Begin(depthStencilState: DepthStencilState.DepthRead);
```

---

## 5. RasterizerState (`rasterizerState`)

Determina cómo las primitivas (triángulos, líneas) se convierten en píxeles.

### Opciones Predefinidas

| Opción | Descripción |
|--------|-------------|
| `CullClockwise` | Descarta triángulos con sentido de las agujas del reloj. |
| `CullCounterClockwise` | **(Por defecto)** Descarta triángulos con sentido contrario a las agujas del reloj. |
| `CullNone` | No descarta ningún triángulo. Dibuja ambas caras. |

### Propiedades Principales

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `CullMode` | CullMode | Modo de descarte de caras (None, ClockwiseFace, CounterClockwiseFace) |
| `FillMode` | FillMode | Cómo rellenar triángulos (`Solid` o `Wireframe`) |
| `DepthBias` | float | Offset de profundidad para evitar z-fighting |
| `DepthClipEnable` | bool | Habilita recorte por profundidad |
| `ScissorTestEnable` | bool | Habilita recorte por rectángulo scissor |
| `MultiSampleAntiAlias` | bool | Habilita antialiasing multisample |

### Ejemplo de Uso

```csharp
// Configuración estándar para 2D
spriteBatch.Begin(rasterizerState: RasterizerState.CullCounterClockwise);

// Para debug (ver ambas caras de los sprites)
spriteBatch.Begin(rasterizerState: RasterizerState.CullNone);
```

---

## 6. Effect (`effect`)

Permite aplicar un shader personalizado a todos los sprites del batch.

### Ejemplo de Uso

```csharp
// Cargar efecto
Effect grayscaleEffect = Content.Load<Effect>("Shaders/Grayscale");

// Aplicar shader a todo el batch
spriteBatch.Begin(effect: grayscaleEffect);
spriteBatch.Draw(sprite, position, Color.White);
spriteBatch.End();
```

### Consideraciones

- El efecto se aplica a **todos** los sprites dentro del Begin/End
- Para diferentes efectos por sprite, usa `SpriteSortMode.Immediate`
- Los parámetros del shader se configuran antes de llamar `Begin()` o `Draw()`

---

## 7. transformMatrix (`transformMatrix`)

Aplica una matriz de transformación a todos los sprites. Ideal para implementar cámaras 2D.

### Ejemplo de Uso

```csharp
// Crear matriz de cámara
Matrix cameraMatrix = 
    Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0f) *
    Matrix.CreateRotationZ(rotation) *
    Matrix.CreateScale(zoom, zoom, 1f) *
    Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0f);

// Aplicar cámara al batch
spriteBatch.Begin(transformMatrix: cameraMatrix);
```

### Componentes de la Matriz de Cámara

1. **Traslación de posición**: Mueve el mundo en dirección opuesta a la cámara
2. **Rotación**: Rota el mundo alrededor del punto de la cámara
3. **Escala (Zoom)**: Aumenta o reduce el tamaño visible
4. **Traslación al centro**: Centra la vista en la pantalla

---

## Ejemplos Completos

### Configuración para Pixel Art (Juegos Retro)

```csharp
spriteBatch.Begin(
    sortMode: SpriteSortMode.Deferred,
    blendState: BlendState.AlphaBlend,
    samplerState: SamplerState.PointClamp,  // ← Crítico para píxeles nítidos
    depthStencilState: DepthStencilState.None,
    rasterizerState: RasterizerState.CullCounterClockwise,
    effect: null,
    transformMatrix: cameraMatrix
);
```

### Configuración para Arte de Alta Resolución

```csharp
spriteBatch.Begin(
    sortMode: SpriteSortMode.Deferred,
    blendState: BlendState.AlphaBlend,
    samplerState: SamplerState.LinearClamp,  // Suavizado al escalar
    depthStencilState: DepthStencilState.None,
    rasterizerState: RasterizerState.CullCounterClockwise
);
```

### Configuración para Efectos de Luz/Partículas

```csharp
spriteBatch.Begin(
    sortMode: SpriteSortMode.BackToFront,
    blendState: BlendState.Additive,  // ← Los colores se suman
    samplerState: SamplerState.LinearClamp,
    depthStencilState: DepthStencilState.None,
    rasterizerState: RasterizerState.CullNone
);
```

### Configuración con Shader Personalizado

```csharp
myShader.Parameters["Time"].SetValue(gameTime.TotalGameTime.TotalSeconds);

spriteBatch.Begin(
    sortMode: SpriteSortMode.Immediate,  // ← Necesario para cambios de shader
    blendState: BlendState.AlphaBlend,
    samplerState: SamplerState.PointClamp,
    effect: myShader
);
```

---

## Resumen Rápido

| Parámetro | Valor Recomendado Pixel Art | Valor Recomendado HD |
|-----------|----------------------------|----------------------|
| `sortMode` | `Deferred` | `Deferred` |
| `blendState` | `AlphaBlend` | `AlphaBlend` |
| `samplerState` | **`PointClamp`** | `LinearClamp` |
| `depthStencilState` | `None` o `Default` | `None` o `Default` |
| `rasterizerState` | `CullCounterClockwise` | `CullCounterClockwise` |

---

## Referencias

- [Documentación Oficial MonoGame - SpriteBatch](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteBatch.html)
- [MonoGame - DepthStencilState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.DepthStencilState.html)
- [MonoGame - RasterizerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RasterizerState.html)
- [MonoGame - SamplerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SamplerState.html)
- [MonoGame - BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html)
