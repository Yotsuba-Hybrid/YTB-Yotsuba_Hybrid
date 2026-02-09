# 🎮 YotsubaEngine - Sistema de Movimiento y Físicas

Este documento explica cómo añadir movimiento automático a una entidad usando el sistema **WASDControl** integrado con el **PhysicsSystem2D**.

## 📋 Tabla de Contenidos

1. [Introducción](#introducción)
2. [Configuración Rápida](#configuración-rápida)
3. [Modos de Juego](#modos-de-juego)
4. [Personalización de Físicas](#personalización-de-físicas)
5. [Animaciones](#animaciones)
6. [Controles Soportados](#controles-soportados)
7. [Eventos del Sistema](#eventos-del-sistema)

---

## Introducción

YotsubaEngine provee un sistema de movimiento completamente automatizado que:

- ✅ Distingue entre juegos **TopDown** (vista superior) y **Platform** (plataformas)
- ✅ Maneja **salto** automático en modo Platform
- ✅ Implementa **caída rápida** al presionar abajo en el aire
- ✅ Aplica **gravedad** automática en modo Platform
- ✅ Detecta colisiones y estado **grounded** (en suelo)
- ✅ Soporta **WASD**, **flechas de dirección** y **GamePad**
- ✅ Verifica animaciones disponibles antes de asignarlas

---

## Configuración Rápida

### Paso 1: Crear una Entidad con los Componentes Necesarios

```csharp
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;

// Crear la entidad
Yotsuba player = new Yotsuba(0);
player.Name = "Player";

// Añadir componentes obligatorios
entityManager.AddEntity(ref player);
entityManager.AddTransformComponent(player, new TransformComponent());
entityManager.AddRigidbodyComponent(player, new RigidBodyComponent2D(GameType.Platform, MassLevel.Collision));
entityManager.AddInputComponent(player, CreateInputComponent());
```

### Paso 2: Configurar el InputComponent

```csharp
private InputComponent CreateInputComponent()
{
    var input = new InputComponent();
    input.AddInput(InputInUse.HasKeyboard);
    input.AddInput(InputInUse.HasGamepad);
    
    // Mapear teclas WASD
    input.KeyBoard.Add(ActionEntityInput.MoveUp, Keys.W);
    input.KeyBoard.Add(ActionEntityInput.MoveDown, Keys.S);
    input.KeyBoard.Add(ActionEntityInput.MoveLeft, Keys.A);
    input.KeyBoard.Add(ActionEntityInput.MoveRight, Keys.D);
    input.KeyBoard.Add(ActionEntityInput.Jump, Keys.Space);
    
    // Mapear GamePad
    input.GamePad.Add(ActionEntityInput.MoveUp, Buttons.DPadUp);
    input.GamePad.Add(ActionEntityInput.MoveDown, Buttons.DPadDown);
    input.GamePad.Add(ActionEntityInput.MoveLeft, Buttons.DPadLeft);
    input.GamePad.Add(ActionEntityInput.MoveRight, Buttons.DPadRight);
    input.GamePad.Add(ActionEntityInput.Jump, Buttons.A);
    
    return input;
}
```

### Paso 3: Registrar en WASDControl (en tu Script)

```csharp
[Script]
public class PlayerScript : BaseScript
{
    public override void Initialize()
    {
        base.Initialize();
        
        // Obtener referencia al WASD Control
        var wasdControl = YTBGlobalState.YTB_WASD_Movement;
        
        // Registrar la entidad - ¡Listo!
        wasdControl.AddEntity(Entity);
    }
}
```

**¡Eso es todo!** Tu entidad ahora tiene movimiento completo con físicas.

---

## Modos de Juego

### GameType.TopDown (Vista Superior)

Ideal para juegos como Zelda, RPGs con vista de pájaro.

```csharp
new RigidBodyComponent2D(GameType.TopDown, MassLevel.Collision)
```

**Comportamiento:**
- W/↑ = Mover arriba
- S/↓ = Mover abajo
- A/← = Mover izquierda
- D/→ = Mover derecha

### GameType.Platform (Plataformas)

Ideal para juegos como Mario, Metroid.

```csharp
new RigidBodyComponent2D(GameType.Platform, MassLevel.Collision)
```

**Comportamiento:**
- W/↑/Space = **Saltar** (solo si está en el suelo)
- S/↓ = **Caída rápida** (si está en el aire) o **Agacharse** (si está en el suelo)
- A/← = Mover izquierda
- D/→ = Mover derecha
- **Gravedad automática** se aplica constantemente

---

## Personalización de Físicas

Modifica los parámetros del `RigidBodyComponent2D` para ajustar el comportamiento:

```csharp
ref var rigidBody = ref entityManager.Rigidbody2DComponents[player.Id];

// Velocidad de movimiento
rigidBody.SPEED = 1.5f;           // Velocidad base
rigidBody.TOP_SPEED = 5.0f;       // Velocidad máxima horizontal

// Físicas de plataforma
rigidBody.Gravity = 0.5f;         // Fuerza de gravedad (default: 0.5)
rigidBody.JumpForce = -12.0f;     // Fuerza de salto (negativo = hacia arriba)
rigidBody.MaxFallSpeed = 15.0f;   // Velocidad terminal de caída
rigidBody.FastFallMultiplier = 2.5f; // Multiplicador de caída rápida
```

### Ejemplos de Configuración

**Personaje Ligero (saltos altos, caída lenta):**
```csharp
rigidBody.Gravity = 0.3f;
rigidBody.JumpForce = -15.0f;
rigidBody.MaxFallSpeed = 8.0f;
```

**Personaje Pesado (saltos bajos, caída rápida):**
```csharp
rigidBody.Gravity = 0.8f;
rigidBody.JumpForce = -8.0f;
rigidBody.MaxFallSpeed = 20.0f;
```

---

## Animaciones

El sistema cambia automáticamente las animaciones según el estado. Solo asegúrate de que tu entidad tenga las animaciones configuradas:

### Animaciones Requeridas (Opcionales)

| AnimationType | Cuándo se usa |
|---------------|---------------|
| `idle` | Cuando la entidad está quieta |
| `walk` | Cuando la entidad se mueve horizontalmente |
| `jump` | Cuando la entidad está en el aire (Platform) |
| `crouch` | Cuando presiona abajo en el suelo (Platform) |

**Nota:** El sistema verifica automáticamente si la animación existe antes de asignarla. Si no existe, usa `idle` como fallback.

### Configurar Animaciones

```csharp
// Crear componente de animación
var animComp = new AnimationComponent2D();

// Añadir animaciones
animComp.AddAnimation(AnimationType.idle, idleAnimation);
animComp.AddAnimation(AnimationType.walk, walkAnimation);
animComp.AddAnimation(AnimationType.jump, jumpAnimation);  // Opcional
animComp.AddAnimation(AnimationType.crouch, crouchAnimation);  // Opcional

entityManager.AddAnimationComponent(player, animComp);
```

---

## Controles Soportados

### Teclado

| Acción | Teclas |
|--------|--------|
| Mover Arriba | W, ↑ |
| Mover Abajo | S, ↓ |
| Mover Izquierda | A, ← |
| Mover Derecha | D, → |
| Saltar | Space, W, ↑ |

### GamePad

| Acción | Botones |
|--------|---------|
| Mover | D-Pad, Left Stick |
| Saltar | A (Xbox), X (PlayStation) |

---

## Eventos del Sistema

El sistema publica eventos que puedes escuchar para reaccionar a cambios de estado:

```csharp
// En tu script Initialize()
EventManager.Instance.Subscribe<OnEntityGroundedEvent>(OnLanded);
EventManager.Instance.Subscribe<OnEntityAirborneEvent>(OnBecameAirborne);
EventManager.Instance.Subscribe<OnEntityJumpEvent>(OnJumped);
EventManager.Instance.Subscribe<OnCollitionEvent>(OnCollision);

private void OnLanded(OnEntityGroundedEvent evt)
{
    if (evt.EntityId == Entity.Id)
    {
        // La entidad acaba de aterrizar
        PlayLandingSound();
    }
}

private void OnJumped(OnEntityJumpEvent evt)
{
    if (evt.EntityId == Entity.Id)
    {
        // La entidad acaba de saltar
        PlayJumpSound();
    }
}
```

### Eventos Disponibles

| Evento | Descripción |
|--------|-------------|
| `OnEntityGroundedEvent` | La entidad aterrizó en el suelo |
| `OnEntityAirborneEvent` | La entidad dejó el suelo |
| `OnEntityJumpEvent` | La entidad saltó |
| `OnCollitionEvent` | Hubo una colisión |

---

## Ejemplo Completo

```csharp
using Microsoft.Xna.Framework.Input;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame.Scripting;

[Script]
public class PlayerController : BaseScript
{
    public override void Initialize()
    {
        base.Initialize();
        
        // Configurar físicas personalizadas
        ref var rigidBody = ref GetRigidBodyComponent(Entity);
        rigidBody.TOP_SPEED = 4.0f;
        rigidBody.JumpForce = -14.0f;
        rigidBody.Gravity = 0.6f;
        
        // Registrar en el sistema de movimiento
        YTBGlobalState.YTB_WASD_Movement.AddEntity(Entity);
    }
    
    public override void Cleanup()
    {
        // Desregistrar al limpiar
        YTBGlobalState.YTB_WASD_Movement.RemoveEntity(Entity);
        base.Cleanup();
    }
}
```

---

## 🛠️ Solución de Problemas

### La entidad no se mueve
- Verifica que tenga `InputComponent` con teclas mapeadas
- Verifica que tenga `RigidBodyComponent2D`
- Verifica que esté registrada en `WASDControl.AddEntity()`

### No hay gravedad
- Asegúrate de usar `GameType.Platform`, no `TopDown`
- Verifica que `Gravity > 0`

### Las animaciones no cambian
- Verifica que tenga `AnimationComponent2D`
- El sistema usa fallback a `idle` si no encuentra la animación

### El salto no funciona
- Solo funciona en modo `Platform`
- Solo funciona si `IsGrounded == true`
- Verifica que `JumpForce` sea negativo (hacia arriba)

---

*YotsubaEngine © 2024 - Documentación de Sistema de Movimiento*
