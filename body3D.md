# Body3D: colision 3D por geometria visible (sin bloquear huecos vacios)

Esto responde exactamente a tu caso: **suelo con columnas y espacios vacios transitables**.

---

## 1) Lo importante: que NO debes usar como colision final

No uses como colision final:
- BoundingBox global del modelo completo
- BoundingSphere global del modelo completo

Porque eso encapsula volumen vacio.  
En tu ejemplo (300x300x300 con columnas), un volumen global bloquearia el paso entre columnas aunque visualmente haya hueco.

---

## 2) Lo que SI necesitas: colision por malla (triangulos)

Para colisionar "solo lo visible/solido", debes colisionar contra la **geometria real** del modelo:

1. Convertir el `Model` a triangulos de colision (triangle soup).
2. Guardar eso en un `CollisionMesh`.
3. Hacer colision en dos fases:
   - **Broad phase**: AABB/BVH para descartar rapido.
   - **Narrow phase**: test triangulo-triangulo (o capsula-triangulo para jugador).

Asi, en espacios vacios sin triangulos, **no hay colision**.

---

## 3) Pipeline recomendado para YotsubaEngine

### Paso A: componente de colision 3D separado del render

No uses `ModelComponent3D` directo para fisica cada frame.  
Crea algo como:

```csharp
public struct CollisionMeshComponent3D
{
    public CollisionMesh Mesh;      // Triangulos locales + BVH
    public bool IsStatic;           // Mundo o dinamico
    public bool IsTrigger;
}
```

### Paso B: construir `CollisionMesh` al cargar contenido

En `YTBFileToGameData` (al parsear entidad), cuando veas `ModelComponent3D`, tambien puedes generar y cachear una malla de colision:

```csharp
CollisionMesh mesh = CollisionMeshBuilder.FromModel(model);
```

Hazlo una sola vez (cache por `ModelPath`) para no recalcular.

### Paso C: crear `PhysicsSystem3D`

En `Scene.Update()`:
1. Integracion de velocidades.
2. Broad phase (AABB world o BVH root bounds).
3. Narrow phase (tests exactos).
4. Resolver penetracion y velocidad.

---

## 4) Extraccion de triangulos desde `Model` (MonoGame)

La idea es leer:
- `ModelMeshPart.VertexBuffer`
- `ModelMeshPart.IndexBuffer`
- aplicar `bone transform` por mesh

y guardar triangulos en local model space.

```csharp
public static CollisionMesh FromModel(Model model)
{
    List<Triangle> triangles = new();
    Matrix[] bones = new Matrix[model.Bones.Count];
    model.CopyAbsoluteBoneTransformsTo(bones);

    foreach (ModelMesh mesh in model.Meshes)
    {
        Matrix meshToModel = bones[mesh.ParentBone.Index];

        foreach (ModelMeshPart part in mesh.MeshParts)
        {
            // 1) Leer posiciones de vertices del part
            // 2) Leer indices del part (ushort o uint)
            // 3) Armar triangulos (i0,i1,i2)
            // 4) Transformar cada vertice con meshToModel
            // 5) triangles.Add(...)
        }
    }

    return CollisionMesh.Build(triangles); // calcula AABB y BVH
}
```

> Nota: este builder se ejecuta en carga, no cada frame.

---

## 5) Deteccion entre dos modelos 3D (exacta, sin volumen vacio)

## 5.1 Broad phase

Primero descarta rapido:

```csharp
if (!a.WorldAabb.Intersects(b.WorldAabb))
    return false;
```

## 5.2 Narrow phase con BVH + triangulos

Solo si el broad phase pasa:

```csharp
bool IntersectsModelModel(CollisionBody3D a, CollisionBody3D b)
{
    return IntersectsNodes(a.Mesh.Root, a.WorldMatrix, b.Mesh.Root, b.WorldMatrix);
}
```

`IntersectsNodes(...)`:
1. Test AABB de nodo A vs nodo B.
2. Si no intersectan -> `false`.
3. Si ambos son hoja -> test triangulo-triangulo en mundo.
4. Si no -> bajar recursivamente a hijos.

Con esto no comparas "todos con todos"; solo ramas cercanas.

---

## 6) Caso "suelo con columnas": por que esto si funciona

Si el suelo/columnas se representa por triangulos:
- El hueco entre columnas **no tiene triangulos**.
- Entonces el jugador/modelo puede pasar por ahi.
- Solo colisiona al tocar triangulos de columnas/suelo.

Exactamente lo que necesitas.

---

## 7) Recomendacion practica para personaje (mejor que modelo-modelo puro)

Para personaje que se mueve:
- Usa **capsula** (o cilindro) para el jugador.
- Colisiona capsula vs triangulos del escenario.

Esto evita inestabilidad y costo de triangulo-triangulo para un actor dinamico.

Flujo:
1. Sweep de capsula con `desiredMove`.
2. Encontrar primer impacto con triangulos.
3. Corregir posicion.
4. Proyectar velocidad en tangente (slide).

---

## 8) Rendimiento: como hacerlo viable

Imprescindible:
- Cachear `CollisionMesh` por `ModelPath`.
- BVH por modelo (construido al cargar).
- Layer masks (ej. Player vs World, no Player vs Deco).
- Evitar test exacto si distancia > umbral.

No recomendado:
- Triangulo-triangulo bruto entre todos los triangulos de ambos modelos cada frame.

---

## 9) Integracion concreta en tu engine (estado actual)

Hoy en YotsubaEngine:
- Render 3D y camara ya estan (`RenderSystem3D`, `CameraComponent3D`).
- `RigidBodyComponent3D` existe pero no esta cableado.
- No hay `PhysicsSystem3D` activo.

Para habilitar esto:
1. Agregar storage ECS para colision 3D (`EntityManager`).
2. Parsear componente 3D real en `YTBFileToGameData` (no saltarlo).
3. Crear `PhysicsSystem3D` con broad+narrow.
4. Llamarlo desde `Scene.Update()`.

---

## 10) Resumen corto

Si quieres colisionar solo parte visible/solida y respetar huecos:
- usa **colision por triangulos** (idealmente con BVH),
- no uses volumen global como colision final,
- para personajes, capsula vs malla estatica es la opcion mas estable.
