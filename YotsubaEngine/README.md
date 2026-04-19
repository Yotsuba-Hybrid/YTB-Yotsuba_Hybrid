# Documentación del proyecto

### Concepto
> Trabajar con modelos 3D no es algo tan complicado como se piensa. Realmente es super intuitivo
y fácil de aprender. Todo se manipula mediante la clase "Model".

## Uso

### Cargar un modelo
```csharp
//Para cargar un modelo 3D, es tan sencillo como hacer esto:
Model model = ContentManager.Load<Model>("NombreDelModelo");
```

### o, Sencillamente usar los helpers de los Scripts:
```csharp
Model model = Load<Model>("NombreDelModelo");
```

### Y solo faltaria asignarselo a la entidad que lo va a usar, y el engine se encargara de dibujarlo (Debe tener un transformComponent):
```csharp
Model model = Load<Model>("NombreDelModelo");
ModelComponent modelComponent = new(model);
EntityManager.AddModelComponent3D(Entity, modelComponent); //Entity viene de la clase de la que heredan todos los scripts, el "BaseScript".
```

### Para acceder a los Huesos del modelo y manipularlos individualmente, segun se organizo en Blender, u otro software 3D:

- Accediendo por el nombre del hueso:
```csharp
Model model = Load<Model>("NombreDelModelo");

// De esta manera se pueden hacer animaciones dinamicas desde el codigo para modelos 3D.
model.Bones["NombreDelHueso"].Transform = Matrix.CreateRotationX(MathHelper.ToRadians(45)); //Ejemplo de rotar un hueso 45 grados en el eje X.

```

- O por su indice:
```csharp
// Rotar la cabeza del personaje 45 grados en el eje Y continuamente
int headIndex = myModel.Bones["Head"].Index;
myModel.Bones[headIndex].Transform *= Matrix.CreateRotationY(0.1f);
```
