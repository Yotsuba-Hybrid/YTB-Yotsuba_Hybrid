# Compilación de Shaders en MonoGame Content Pipeline

## Introducción

Los archivos `.fx` (shaders) necesitan ser compilados por el Content Pipeline de MonoGame para convertirse en archivos `.xnb` que pueden ser cargados en tiempo de ejecución.

## Pasos para Compilar Shaders

### Opción 1: Usando MGCB Editor (Recomendado)

1. **Abrir el MGCB Editor:**
   ```bash
   mgcb-editor-linux Content.mgcb
   # o en Windows:
   mgcb-editor Content.mgcb
   # o en macOS:
   mgcb-editor-mac Content.mgcb
   ```

2. **Agregar los archivos de shader:**
   - Click derecho en la carpeta raíz del Content
   - Selecciona "Add" > "Existing Item"
   - Navega a `YotsubaEngine/Graphics/Shaders/`
   - Selecciona todos los archivos `.fx`:
     - `Grayscale.fx`
     - `Saturation.fx`
     - `ColorTint.fx`
     - `BrightnessContrast.fx`
     - `Transition.fx`

3. **Configurar cada shader:**
   - Selecciona cada archivo `.fx`
   - En el panel de propiedades, verifica:
     - **Importer:** EffectImporter
     - **Processor:** EffectProcessor
     - **Build Action:** Build

4. **Organizar en carpeta Shaders:**
   - En MGCB Editor, crea una carpeta virtual llamada "Shaders"
   - Arrastra los archivos `.fx` a esta carpeta

5. **Compilar:**
   - Menú "Build" > "Build" (o presiona F6)
   - Verifica que no haya errores de compilación

### Opción 2: Usando línea de comandos

```bash
# Navegar a la carpeta del Content
cd YotsubaEngine/SandBoxGame/SandBoxGame.Content

# Construir el contenido
dotnet mgcb Content.mgcb
```

### Opción 3: Agregar al archivo .mgcb manualmente

Edita el archivo `Content.mgcb` y agrega las siguientes líneas:

```
#begin Shaders/Grayscale.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:Shaders/Grayscale.fx

#begin Shaders/Saturation.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:Shaders/Saturation.fx

#begin Shaders/ColorTint.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:Shaders/ColorTint.fx

#begin Shaders/BrightnessContrast.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:Shaders/BrightnessContrast.fx

#begin Shaders/Transition.fx
/importer:EffectImporter
/processor:EffectProcessor
/processorParam:DebugMode=Auto
/build:Shaders/Transition.fx
```

## Estructura de Carpetas Recomendada

```
SandBoxGame.Content/
├── Content.mgcb
├── Shaders/
│   ├── Grayscale.fx
│   ├── Saturation.fx
│   ├── ColorTint.fx
│   ├── BrightnessContrast.fx
│   └── Transition.fx
└── (otros assets)
```

Después de compilar, los archivos se generarán en:

```
bin/Content/Shaders/
├── Grayscale.xnb
├── Saturation.xnb
├── ColorTint.xnb
├── BrightnessContrast.xnb
└── Transition.xnb
```

## Copiar Shaders al Proyecto de Contenido

Los archivos `.fx` deben estar en el proyecto de contenido. Hay dos opciones:

### Opción A: Copiar manualmente
```bash
# Copiar desde YotsubaEngine al proyecto de contenido
cp YotsubaEngine/Graphics/Shaders/*.fx SandBoxGame.Content/Shaders/
```

### Opción B: Usar links simbólicos (recomendado para desarrollo)
```bash
# En Linux/macOS
ln -s ../../YotsubaEngine/Graphics/Shaders/*.fx SandBoxGame.Content/Shaders/

# En Windows (cmd como administrador)
mklink /D SandBoxGame.Content\Shaders ..\..\YotsubaEngine\Graphics\Shaders
```

### Opción C: Agregar al .csproj (automático)

Edita `SandBoxGame.Content.csproj` y agrega:

```xml
<ItemGroup>
  <None Include="../../YotsubaEngine/Graphics/Shaders/*.fx">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Verificación

Para verificar que los shaders se compilaron correctamente:

1. Ejecuta el juego
2. Intenta cargar un shader:
   ```csharp
   var shader = contentManager.Load<Effect>("Shaders/Grayscale");
   ```
3. Si no hay errores, el shader está listo para usarse

## Solución de Problemas

### Error: "Could not load Shaders/Grayscale"
- Verifica que el archivo `.fx` esté en el Content Pipeline
- Asegúrate de que se compiló correctamente (verifica la carpeta `bin/Content/Shaders/`)
- Revisa que el nombre del asset sea correcto (sin extensión en el código)

### Error de compilación en el shader
- Revisa la sintaxis HLSL del archivo `.fx`
- Asegúrate de usar las macros correctas (`#if OPENGL` para OpenGL, etc.)
- Verifica que las funciones y estructuras sean compatibles con el profile seleccionado

### Shader no se ve en la pantalla
- Verifica que el shader esté siendo aplicado al `SpriteBatch.Begin()`
- Revisa los parámetros del shader (saturation, brightness, etc.)
- Asegúrate de que la entidad tenga el componente `ShaderComponent2D` activo

## Compatibilidad Multiplataforma

Los shaders usan condicionales para adaptarse a diferentes plataformas:

- **DirectX (Windows):** `vs_4_0_level_9_1` / `ps_4_0_level_9_1`
- **OpenGL (Linux/macOS/Mobile):** `vs_3_0` / `ps_3_0`

MonoGame se encarga automáticamente de seleccionar el profile correcto según la plataforma.

## Integración con YotsubaEngine

Una vez compilados, los shaders se cargan automáticamente con:

```csharp
// Inicializar el ShaderManager
ShaderManager.Initialize(contentManager);

// Cargar shader
var saturationShader = ShaderManager.LoadShader("Shaders/Saturation");

// Usar helper de YotsubaGraphicsManager
var shaderComponent = YotsubaGraphicsManager.SaturationShaderGenerator(contentManager, 0.5f);
```

## Referencias

- [MonoGame Content Pipeline](https://docs.monogame.net/articles/getting_started/4_adding_content.html)
- [Effect Processing](https://docs.monogame.net/articles/content/using_mgcb_editor.html)
- [HLSL Shader Model 3.0](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-sm3)
