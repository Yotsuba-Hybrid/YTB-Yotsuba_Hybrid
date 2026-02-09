# FontDragSystem - Documentación

## Descripción General

`FontDragSystem` es un sistema de debug que permite mover entidades de texto (con `FontComponent2D`) arrastrándolas con el mouse mientras se mantiene presionada la combinación de teclas `Ctrl + Shift + Click Izquierdo`.

## Características

- ✅ Solo disponible en modo `DEBUG`
- ✅ Compatible con AOT y JIT
- ✅ Multiplataforma
- ✅ Transforma coordenadas de pantalla a mundo usando la cámara
- ✅ Guarda automáticamente los cambios en el archivo `.ytb`
- ✅ No interfiere con el `DragAndDropSystem` existente

## Uso

### Activar el arrastre de texto

1. **Combinación de teclas**: Mantén presionado `Ctrl + Shift`
2. **Click izquierdo**: Haz click sobre el texto que deseas mover
3. **Arrastra**: Mueve el mouse mientras mantienes presionado el botón
4. **Suelta**: Al soltar el botón o las teclas, la posición se guarda automáticamente

### Diferencias con DragAndDropSystem

| Característica | DragAndDropSystem | FontDragSystem |
|----------------|-------------------|----------------|
| Activación | `Ctrl + Click Izquierdo` | `Ctrl + Shift + Click Izquierdo` |
| Objetivo | Cualquier entidad con Transform | Solo entidades con FontComponent2D |
| Bounding Box | Basado en tamaño del sprite | Basado en medida del texto (MeasureString) |

## Implementación Técnica

### Arquitectura

```
Scene.cs
  ├── FontSystem2D (renderiza texto)
  └── FontDragSystem (maneja interacción)
        └── Referencia a FontSystem2D (para medir texto)
```

### Flujo de Ejecución

1. **Inicialización** (`Scene.cs`):
   ```csharp
   FontSystem2D.InitializeSystem(EntityManager);
   FontDragSystem.InitializeSystem(EntityManager);
   FontDragSystem.SetFontSystem(FontSystem2D); // Pasar referencia
   ```

2. **Update Loop** (`Scene.cs`):
   ```csharp
   Parallel.ForEach(entities, entity => {
       // ... otros sistemas
       FontDragSystem.SharedEntityForEachUpdate(entity, gameTime);
   });
   ```

3. **Detección de Colisión**:
   - Obtiene el `SpriteFont` desde `FontSystem2D.Fuentes`
   - Mide el texto con `font.MeasureString(texto)`
   - Calcula el bounding box: `(posX, posY, anchoEscalado, altoEscalado)`
   - Verifica si el mouse está dentro del rectángulo

4. **Transformación de Coordenadas**:
   ```csharp
   // Obtener matriz de vista de la cámara
   Matrix viewMatrix = camera.Get2DViewMatrix(camPos, screenCenter, zoom);
   
   // Invertir para obtener World Space
   Matrix inverseView = Matrix.Invert(viewMatrix);
   
   // Transformar posición del mouse
   Vector2 worldMousePos = Vector2.Transform(screenMousePos, inverseView);
   ```

5. **Actualizar Posición**:
   - Mientras se mantiene presionado: `transform.SetPosition(newPos.X, newPos.Y, newPos.Z)`
   - Al soltar: `GuardarCambiosEnEscena()` actualiza el archivo `.ytb`

### Guardado de Cambios

El sistema actualiza automáticamente el archivo `.ytb` de la escena actual:

```csharp
private void GuardarCambiosEnEscena(Yotsuba entity, TransformComponent transform)
{
    // 1. Obtener escena actual
    var currentScene = gameInfo.Scene.FirstOrDefault(x => x.Name == sceneName);
    
    // 2. Encontrar la entidad en la definición de la escena
    var entityData = currentScene.Entities.FirstOrDefault(x => x.Name == entity.Name);
    
    // 3. Actualizar la propiedad Position del TransformComponent
    UpdateProperty(transformComponentData, "Position", 
                   $"{transform.Position.X},{transform.Position.Y},{transform.Position.Z}");
}
```

## Consideraciones de Rendimiento

### Optimizaciones Aplicadas

1. **Early Exit**: Verifica componentes requeridos antes de calcular
2. **Caching**: Mantiene referencia al `FontSystem2D` en vez de buscarlo cada frame
3. **Lazy Calculation**: Solo calcula bounding box si la entidad tiene `FontComponent2D`
4. **Single Entity Drag**: Solo una entidad de texto puede arrastrarse a la vez

### Impacto en Performance

- **Update Loop**: O(n) donde n = número de entidades con `FontComponent2D`
- **Memory**: 2 campos de estado (`int?` + `Vector2`) ≈ 12 bytes
- **Guardado**: Solo ocurre al soltar (no en cada frame)

## Limitaciones Conocidas

1. **Texto en UI vs Mundo**: Actualmente solo soporta texto en coordenadas del mundo (afectado por cámara)
2. **Origin**: Asume origin en `Vector2.Zero` (esquina superior izquierda)
3. **Multi-line**: No considera textos multi-línea correctamente
4. **Rotación**: El bounding box no rota con el texto

## Extensiones Futuras

### Posibles Mejoras

- [ ] Soporte para texto en UI (sin transformación de cámara)
- [ ] Bounding box rotado para texto con `Transform.Rotation != 0`
- [ ] Snapping a grid opcional
- [ ] Undo/Redo para posicionamiento
- [ ] Vista previa del bounding box durante el arrastre

### Ejemplo de Uso con Snapping

```csharp
// En FontDragSystem.FontEntityDrag(), después de calcular newPos:
if (EnableSnapping)
{
    float gridSize = 16f;
    newPos.X = MathF.Round(newPos.X / gridSize) * gridSize;
    newPos.Y = MathF.Round(newPos.Y / gridSize) * gridSize;
}
transform.SetPosition(newPos.X, newPos.Y, transform.Position.Z);
```

## Debugging

### Cómo verificar que funciona

1. **Compilar en modo DEBUG**: `dotnet build -c Debug`
2. **Ejecutar el juego**
3. **Crear una entidad con FontComponent2D**
4. **Presionar** `Ctrl + Shift` y hacer click sobre el texto
5. **Observar**: El texto debe seguir al mouse
6. **Verificar archivo .ytb**: La posición debe actualizarse al soltar

### Logs Útiles

Agregar en `FontEntityDrag()` para debug:
```csharp
if (_draggedFontEntityId == entity.Id)
{
    Console.WriteLine($"Dragging text '{fontComponent.Texto}' to {newPos}");
}
```

## AOT Compatibility

Este sistema es **100% compatible con AOT** porque:

- ✅ No usa reflexión dinámica
- ✅ No genera código en runtime
- ✅ Todas las llamadas son estáticas o devirtualizadas
- ✅ Usa tipos primitivos y structs
- ✅ Compatible con Native AOT de .NET

## Testing

### Test Manual

```
1. Abrir YotsubaEngine en modo DEBUG
2. Cargar escena con texto
3. Ctrl + Shift + Click Izquierdo sobre texto
4. Verificar que el texto se mueve
5. Soltar mouse
6. Recargar escena
7. Verificar que la nueva posición persiste
```

### Test de Regresión

- [ ] No debe interferir con `DragAndDropSystem`
- [ ] No debe afectar texto sin `TransformComponent`
- [ ] No debe crashear si no hay cámara
- [ ] Debe funcionar con zoom de cámara
- [ ] Debe funcionar con offset de cámara

## Referencias

- **Código fuente**: `YotsubaEngine/Core/System/YotsubaEngineCore/FontDragSystem.cs`
- **Integración**: `YotsubaEngine/Core/YotsubaGame/Scene.cs`
- **Sistema relacionado**: `YotsubaEngine/Core/System/YotsubaEngineCore/DragAndDropSystem.cs`
- **Renderizado de texto**: `YotsubaEngine/Core/System/S_2D/FontSystem2D.cs`

---

**Última actualización**: 2025-01-20  
**Versión**: 1.0  
**Autor**: Yotsuba (AI Assistant)
