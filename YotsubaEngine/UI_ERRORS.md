# Errores en Sistema de Abstracción UI

## 1. ImGui - Controles Duplicados

**Síntoma:** Se renderizan dos versiones de cada control, una con el texto del usuario y otra con "debug".

**Código relevante:**
- `Core/System/YotsubaEngineUI/EngineUISystem.cs` - Renderiza paneles ImGui para el editor del motor
- `Core/YotsubaGame/Scene.cs:302-310` - Lógica de draw para FormsSystem
- `Forms/FormsManager.cs:100-122` - Draw loop para ImGui

**Hipótesis:** EngineUISystem y FormsSystem ambos ejecutan Begin/End Layout para ImGui, causando renderizado duplicado.

---

## 2. Myra - Solo el padre se renderiza

**Síntoma:** Window se muestra pero Panel y sus hijos (Button, Label) no aparecen.

**Código relevante:**
- `Forms/Implementation/ChildManager.cs:20-27` - Agrega hijos al contenedor Myra
- `Forms/Implementation/ChildManager.cs:103-116` - GetMyraWidget() switch
- `Forms/Implementation/Window.cs:113-116` - GetMyraContainer() retorna Content
- `Forms/Implementation/Panel.cs:65-68` - GetMyraContainer() retorna MyraControl

**Logs esperados vs reales:**
```
// Esperado después de AddChild:
[ChildManager] Added Myra child Button to container Panel
[ChildManager] Added Myra child Label to container Panel
[ChildManager] Added Myra child Panel to container Window

// Real: Solo aparece
[FormsManager] Added Myra widget to Desktop: Window
[FormsManager] Added control to root: Window
```

**Posibles causas:**
- `ChildManager.AddChildToContainer()` no se llama correctamente
- `GetMyraContainer()` retorna null o contenedor inválido
- Posicionamiento de widgets dentro del contenedor padre

---

## 3. GumUI - No renderiza nada

**Síntoma:** Ningún control aparece, pero logs muestran que se agregan al Root.

**Código relevante:**
- `Forms/Implementation/Managers/GumManager.cs:44-47` - EndFrame llama GumService.Default.Draw()
- `Forms/FormsManager.cs:229-246` - ConnectToGum()
- `Forms/Implementation/Button.cs:22-26` - GumControl con Width/Height definidos

**Posibles causas:**
- Posición de controles no se aplica correctamente
- `GumService.Default.Root` es null o no inicializado
- Falta configuración de visibilidad en controles Gum
- Los controles necesitan estar en un Canvas específico

---

## Flujo de Debug Sugerido

1. **ImGui:** Verificar si Begin/End Layout se llama dos veces agregando logs en `ImGuiManager.cs`

2. **Myra:** Agregar breakpoint en `ChildManager.AddChildToContainer()` para verificar que se llama con Panel y Window

3. **Gum:** Verificar valor de `GumService.Default.Root` después de inicialización