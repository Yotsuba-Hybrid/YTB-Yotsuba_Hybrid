# YTBGumUI - API de UI para YotsubaEngine

Esta documentación describe cómo usar la integración de GumUI en YotsubaEngine a través de la API simplificada `YTBGum`.

## Tabla de Contenidos

1. [Introducción](#introducción)
2. [Instalación y Setup](#instalación-y-setup)
3. [Controles Básicos](#controles-básicos)
4. [Layouts y Contenedores](#layouts-y-contenedores)
5. [Estilos y Utilidades](#estilos-y-utilidades)
6. [Ejemplos Completos](#ejemplos-completos)
7. [Referencia de API](#referencia-de-api)

---

## Introducción

YTBGumUI es una capa de abstracción sobre GumUI que simplifica la creación de interfaces de usuario en juegos desarrollados con YotsubaEngine y MonoGame. Proporciona una API fluida y fácil de usar para crear botones, etiquetas, cajas de texto, sliders y más.

### Características principales:
- **API simplificada**: Métodos factory para crear controles con una sola línea de código
- **Compatibilidad AOT**: Totalmente compatible con compilación Ahead-of-Time
- **Multiplataforma**: Funciona en Windows, Linux, macOS, iOS y Android
- **Fluent API**: Encadenamiento de métodos para configuración rápida

---

## Instalación y Setup

### Paso 1: Inicializar GumUI

En tu clase de juego, inicializa GumUI en el método `Initialize`:

```csharp
using YotsubaEngine.Core.System.GumUI;

public class MiJuego : Game
{
    protected override void Initialize()
    {
        // Inicializar GumUI con configuración por defecto (V3)
        YTBGum.Initialize(this);
        
        base.Initialize();
    }
}
```

### Paso 2: Actualizar y Dibujar

Asegúrate de llamar `Update` y `Draw` en los métodos correspondientes:

```csharp
protected override void Update(GameTime gameTime)
{
    // Actualizar GumUI (procesa input de mouse/teclado)
    YTBGum.Update(gameTime);
    
    base.Update(gameTime);
}

protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);
    
    // Dibujar la UI después de limpiar el fondo
    YTBGum.Draw();
    
    base.Draw(gameTime);
}
```

---

## Controles Básicos

### Button (Botón)

Crea botones interactivos con texto y eventos click:

```csharp
// Botón simple
var boton = YTBGum.AddButton("¡Clickeame!", 
    onClick: () => Console.WriteLine("¡Click!"),
    x: 100, 
    y: 50);

// Botón con tamaño personalizado
var botonGrande = YTBGum.AddButton("Botón Grande", 
    onClick: MiMetodo,
    x: 100, 
    y: 120,
    width: 200, 
    height: 60);
```

### Label (Etiqueta)

Muestra texto estático:

```csharp
// Etiqueta simple
var etiqueta = YTBGum.AddLabel("¡Hola Mundo!", x: 50, y: 20);

// Etiqueta centrada en pantalla
var titulo = YTBGum.AddLabel("Mi Juego", 
    x: YTBGum.CenterX - 50, 
    y: 10);
```

### TextBox (Caja de Texto)

Input de texto editable:

```csharp
// TextBox básico
var nombreInput = YTBGum.AddTextBox(
    placeholder: "Escribe tu nombre...",
    x: 100,
    y: 100,
    width: 250);

// TextBox con evento de cambio
var emailInput = YTBGum.AddTextBox(
    placeholder: "email@ejemplo.com",
    x: 100,
    y: 150,
    width: 250,
    onTextChanged: (texto) => Console.WriteLine($"Email: {texto}"));
```

### PasswordBox (Caja de Contraseña)

Input de contraseña con caracteres ocultos:

```csharp
var password = YTBGum.AddPasswordBox(
    x: 100,
    y: 200,
    width: 250);
```

### CheckBox (Casilla de Verificación)

Control para opciones booleanas:

```csharp
// CheckBox básico
var musicaCheck = YTBGum.AddCheckBox(
    text: "Habilitar Música",
    isChecked: true,
    x: 100,
    y: 250);

// CheckBox con evento
var sonidoCheck = YTBGum.AddCheckBox(
    text: "Habilitar Sonido",
    isChecked: true,
    x: 100,
    y: 280,
    onCheckedChanged: (estado) => ToggleSonido(estado));
```

### RadioButton (Botón de Radio)

Opciones mutuamente excluyentes:

```csharp
// Grupo de dificultad
var facil = YTBGum.AddRadioButton("Fácil", "dificultad", x: 100, y: 320);
var normal = YTBGum.AddRadioButton("Normal", "dificultad", x: 100, y: 350);
var dificil = YTBGum.AddRadioButton("Difícil", "dificultad", x: 100, y: 380);

// Con evento
var modoA = YTBGum.AddRadioButton("Modo A", "modo", 
    x: 100, y: 420,
    onCheckedChanged: (seleccionado) => {
        if (seleccionado) ActivarModoA();
    });
```

### ComboBox (Lista Desplegable)

Selección de una opción de una lista:

```csharp
var resoluciones = new[] { "1920x1080", "1280x720", "800x600" };
var comboResolucion = YTBGum.AddComboBox(
    items: resoluciones,
    x: 100,
    y: 460,
    width: 180,
    onSelectionChanged: (indice) => CambiarResolucion(indice));
```

### Slider (Control Deslizante)

Control para seleccionar valores numéricos:

```csharp
// Slider de volumen (0-100)
var volumenSlider = YTBGum.AddSlider(
    min: 0,
    max: 100,
    value: 75,
    x: 100,
    y: 500,
    width: 200,
    onValueChanged: (valor) => SetVolumen((float)valor / 100));
```

### ListBox (Lista)

Lista de elementos seleccionables:

```csharp
var niveles = new[] { "Nivel 1 - Tutorial", "Nivel 2 - Bosque", "Nivel 3 - Cueva" };
var listaNiveles = YTBGum.AddListBox(
    items: niveles,
    x: 100,
    y: 540,
    width: 250,
    height: 120,
    onSelectionChanged: (indice) => CargarNivel(indice));
```

### ScrollViewer (Visor con Scroll)

Contenedor con barras de desplazamiento:

```csharp
var scrollViewer = YTBGum.AddScrollViewer(
    x: 400,
    y: 100,
    width: 300,
    height: 200);
```

---

## Layouts y Contenedores

### StackPanel (Panel de Apilado)

Organiza controles vertical u horizontalmente:

```csharp
// Panel vertical
var panelVertical = YTBGum.AddVerticalStack(x: 50, y: 50);

// Agregar controles al panel
var btn1 = YTBGumControls.CreateButton("Opción 1").AddTo(panelVertical);
var btn2 = YTBGumControls.CreateButton("Opción 2").AddTo(panelVertical);
var btn3 = YTBGumControls.CreateButton("Opción 3").AddTo(panelVertical);

// Panel horizontal
var panelHorizontal = YTBGum.AddHorizontalStack(x: 50, y: 300);
```

### Panel (Contenedor Simple)

Contenedor básico para agrupar controles:

```csharp
var panel = YTBGum.AddPanel(
    x: 100,
    y: 100,
    width: 400,
    height: 300);
```

---

## Estilos y Utilidades

### Colores Predefinidos

YTBGumStyles proporciona colores temáticos:

```csharp
using YotsubaEngine.Core.System.GumUI;

// Tema oscuro
Color primario = YTBGumStyles.DarkPrimary;     // RGB(45, 45, 48)
Color secundario = YTBGumStyles.DarkSecondary;  // RGB(62, 62, 66)
Color acento = YTBGumStyles.DarkAccent;        // RGB(0, 122, 204)

// Tema claro
Color primarioClaro = YTBGumStyles.LightPrimary;
Color secundarioClaro = YTBGumStyles.LightSecondary;
Color acentoClaro = YTBGumStyles.LightAccent;

// Colores semánticos
Color exito = YTBGumStyles.Success;    // Verde
Color advertencia = YTBGumStyles.Warning; // Naranja
Color error = YTBGumStyles.Error;      // Rojo
Color info = YTBGumStyles.Info;        // Azul
```

### Extension Methods para Estilizado

```csharp
// Tamaño y posición
var boton = YTBGum.AddButton("Test")
    .WithSize(150, 50)
    .WithPosition(100, 200);

// Visibilidad
boton.Hide();     // Ocultar
boton.Show();     // Mostrar
boton.ToggleVisibility(); // Alternar

// Habilitación
boton.Disable();  // Deshabilitar
boton.Enable();   // Habilitar
```

### Propiedades de Pantalla

```csharp
float ancho = YTBGum.ScreenWidth;
float alto = YTBGum.ScreenHeight;
float centroX = YTBGum.CenterX;
float centroY = YTBGum.CenterY;
```

---

## Ejemplos Completos

### Ejemplo 1: Menú Principal Simple

```csharp
using YotsubaEngine.Core.System.GumUI;

public class MenuPrincipal
{
    public void CrearMenu()
    {
        // Título
        YTBGum.AddLabel("MI JUEGO", x: YTBGum.CenterX - 80, y: 50);
        
        // Botones del menú
        float y = 150;
        YTBGum.AddButton("Nuevo Juego", () => IniciarJuego(), 
            x: YTBGum.CenterX - 100, y: y, width: 200);
        
        YTBGum.AddButton("Continuar", () => ContinuarJuego(), 
            x: YTBGum.CenterX - 100, y: y + 60, width: 200);
        
        YTBGum.AddButton("Opciones", () => AbrirOpciones(), 
            x: YTBGum.CenterX - 100, y: y + 120, width: 200);
        
        YTBGum.AddButton("Salir", () => SalirJuego(), 
            x: YTBGum.CenterX - 100, y: y + 180, width: 200);
    }
    
    private void IniciarJuego() { /* ... */ }
    private void ContinuarJuego() { /* ... */ }
    private void AbrirOpciones() { /* ... */ }
    private void SalirJuego() { /* ... */ }
}
```

### Ejemplo 2: Pantalla de Login

```csharp
using YotsubaEngine.Core.System.GumUI;

public class PantallaLogin
{
    private TextBox _usuarioInput;
    private PasswordBox _passwordInput;
    
    public void CrearUI()
    {
        float x = 200;
        float y = 100;
        
        // Etiquetas e inputs
        YTBGum.AddLabel("Usuario:", x: x, y: y);
        _usuarioInput = YTBGum.AddTextBox("", x: x, y: y + 30, width: 250);
        
        YTBGum.AddLabel("Contraseña:", x: x, y: y + 80);
        _passwordInput = YTBGum.AddPasswordBox(x: x, y: y + 110, width: 250);
        
        // Recordar usuario
        YTBGum.AddCheckBox("Recordar usuario", isChecked: false, x: x, y: y + 160);
        
        // Botones
        YTBGum.AddButton("Iniciar Sesión", () => Login(), 
            x: x, y: y + 210, width: 120);
        
        YTBGum.AddButton("Registrarse", () => Registrar(), 
            x: x + 140, y: y + 210, width: 110);
    }
    
    private void Login()
    {
        string usuario = _usuarioInput.Text;
        // Validar credenciales...
    }
    
    private void Registrar() { /* ... */ }
}
```

### Ejemplo 3: Panel de Opciones de Audio

```csharp
using YotsubaEngine.Core.System.GumUI;

public class OpcionesAudio
{
    private float _volumenMusica = 75;
    private float _volumenEfectos = 100;
    
    public void CrearUI()
    {
        // Crear panel vertical para opciones
        var panel = YTBGum.AddVerticalStack(x: 50, y: 50);
        
        // Título
        YTBGumControls.CreateLabel("OPCIONES DE AUDIO", x: 0, y: 0).AddTo(panel);
        
        // Volumen música
        YTBGumControls.CreateLabel("Volumen Música:", 0, 0).AddTo(panel);
        YTBGumControls.CreateSlider(0, 100, _volumenMusica, 0, 0, 200,
            onValueChanged: (v) => { _volumenMusica = (float)v; })
            .AddTo(panel);
        
        // Volumen efectos
        YTBGumControls.CreateLabel("Volumen Efectos:", 0, 0).AddTo(panel);
        YTBGumControls.CreateSlider(0, 100, _volumenEfectos, 0, 0, 200,
            onValueChanged: (v) => { _volumenEfectos = (float)v; })
            .AddTo(panel);
        
        // Opciones de audio
        YTBGumControls.CreateCheckBox("Habilitar Música", true).AddTo(panel);
        YTBGumControls.CreateCheckBox("Habilitar Efectos", true).AddTo(panel);
        YTBGumControls.CreateCheckBox("Subtítulos", false).AddTo(panel);
        
        // Botón aplicar
        YTBGumControls.CreateButton("Aplicar", () => AplicarCambios()).AddTo(panel);
    }
    
    private void AplicarCambios()
    {
        // Aplicar volúmenes...
    }
}
```

### Ejemplo 4: Selector de Personaje

```csharp
using YotsubaEngine.Core.System.GumUI;

public class SelectorPersonaje
{
    private int _personajeSeleccionado = -1;
    
    public void CrearUI()
    {
        // Lista de personajes
        var personajes = new[] {
            "Guerrero - Fuerza alta",
            "Mago - Magia alta", 
            "Arquero - Destreza alta",
            "Ladrón - Velocidad alta"
        };
        
        YTBGum.AddLabel("SELECCIONA TU PERSONAJE", x: 100, y: 30);
        
        YTBGum.AddListBox(
            items: personajes,
            x: 100,
            y: 70,
            width: 300,
            height: 200,
            onSelectionChanged: (indice) => {
                _personajeSeleccionado = indice;
                MostrarPreview(indice);
            });
        
        // Dificultad
        YTBGum.AddLabel("Dificultad:", x: 100, y: 290);
        YTBGum.AddRadioButton("Fácil", "dif", x: 100, y: 320);
        YTBGum.AddRadioButton("Normal", "dif", x: 100, y: 350);
        YTBGum.AddRadioButton("Difícil", "dif", x: 100, y: 380);
        
        // Botón confirmar
        YTBGum.AddButton("¡Comenzar Aventura!", 
            onClick: () => IniciarJuego(),
            x: 100, y: 430, width: 200);
    }
    
    private void MostrarPreview(int indice) { /* ... */ }
    private void IniciarJuego() { /* ... */ }
}
```

---

## Referencia de API

### YTBGum (Facade Principal)

| Método | Descripción |
|--------|-------------|
| `Initialize(Game game)` | Inicializa GumUI con versión V3 |
| `Initialize(Game game, DefaultVisualsVersion version)` | Inicializa con versión específica |
| `Update(GameTime gameTime)` | Actualiza la UI |
| `Draw()` | Dibuja la UI |
| `Clear()` | Elimina todos los controles |

### YTBGumControls (Factory de Controles)

| Método | Parámetros | Retorno |
|--------|------------|---------|
| `CreateButton` | text, onClick, x, y, width, height | `Button` |
| `AddButton` | (mismos) | `Button` (agregado a root) |
| `CreateLabel` | text, x, y | `Label` |
| `AddLabel` | (mismos) | `Label` (agregado a root) |
| `CreateTextBox` | placeholder, x, y, width, onTextChanged | `TextBox` |
| `AddTextBox` | (mismos) | `TextBox` (agregado a root) |
| `CreatePasswordBox` | x, y, width | `PasswordBox` |
| `AddPasswordBox` | (mismos) | `PasswordBox` (agregado a root) |
| `CreateCheckBox` | text, isChecked, x, y, onCheckedChanged | `CheckBox` |
| `AddCheckBox` | (mismos) | `CheckBox` (agregado a root) |
| `CreateRadioButton` | text, groupName, x, y, onCheckedChanged | `RadioButton` |
| `AddRadioButton` | (mismos) | `RadioButton` (agregado a root) |
| `CreateComboBox` | items, x, y, width, onSelectionChanged | `ComboBox` |
| `AddComboBox` | (mismos) | `ComboBox` (agregado a root) |
| `CreateSlider` | min, max, value, x, y, width, onValueChanged | `Slider` |
| `AddSlider` | (mismos) | `Slider` (agregado a root) |
| `CreateListBox` | items, x, y, width, height, onSelectionChanged | `ListBox` |
| `AddListBox` | (mismos) | `ListBox` (agregado a root) |
| `CreateScrollViewer` | x, y, width, height | `ScrollViewer` |
| `AddScrollViewer` | (mismos) | `ScrollViewer` (agregado a root) |

### YTBGumLayouts (Contenedores)

| Método | Descripción |
|--------|-------------|
| `CreateVerticalStack(x, y)` | Crea StackPanel vertical |
| `AddVerticalStack(x, y)` | Crea y agrega StackPanel vertical |
| `CreateHorizontalStack(x, y)` | Crea StackPanel horizontal |
| `AddHorizontalStack(x, y)` | Crea y agrega StackPanel horizontal |
| `CreatePanel(x, y, width, height)` | Crea Panel contenedor |
| `AddPanel(x, y, width, height)` | Crea y agrega Panel |

### YTBGumStyles (Utilidades de Estilo)

| Extensión | Descripción |
|-----------|-------------|
| `.WithSize(width, height)` | Establece tamaño |
| `.WithPosition(x, y)` | Establece posición |
| `.WithWidth(width)` | Establece ancho |
| `.WithHeight(height)` | Establece alto |
| `.Show()` | Hace visible |
| `.Hide()` | Oculta |
| `.ToggleVisibility()` | Alterna visibilidad |
| `.Enable()` | Habilita |
| `.Disable()` | Deshabilita |

---

## Notas Adicionales

### Compatibilidad con AOT

Todos los controles y métodos de YTBGumUI son compatibles con compilación AOT (Ahead-of-Time), lo que significa que funcionan correctamente en plataformas como iOS y consolas que no permiten JIT.

### Rendimiento

- Los métodos `Add*` agregan controles directamente al root de Gum
- Para UIs complejas, usa `Create*` y luego agrega a un contenedor padre
- Evita crear/destruir controles en el loop de Update; prefiere mostrar/ocultar

### Más Información

Para documentación más detallada sobre GumUI, visita:
- [GumUI Documentation](https://docs.flatredball.com/gum)
- [MonoGame Gum Tutorial](https://docs.monogame.net/articles/tutorials/building_2d_games/20_implementing_ui_with_gum/)
