# YTB-Yotsuba Hybrid

Repositorio monolítico que contiene:

- **YotsubaEngine**: motor de juego (ECS) construido sobre MonoGame.
- **SandBoxGame**: juego/laboratorio de ejemplo para probar el motor en múltiples plataformas.
- **YTBContentBuilder**: utilidades para pipeline de contenido.

> Si quieres empezar por el motor, ve directo a [`YotsubaEngine/README.md`](YotsubaEngine/README.md).

## Estructura principal

```text
.
├── YotsubaEngine/        # Motor ECS + subsistemas 2D/3D/UI/script/audio
├── SandBoxGame/          # Proyecto de ejemplo multiplataforma
└── YTBContentBuilder/    # Herramientas de construcción de contenido
```

## Requisitos

- SDK de **.NET 10.0** (preview según el entorno).
- Dependencias NuGet de MonoGame/ImGui/Gum (restauradas por `dotnet restore`).

## Comandos rápidos

```bash
# Restaurar dependencias del motor
 dotnet restore YotsubaEngine/YotsubaEngine.csproj

# Compilar motor
 dotnet build YotsubaEngine/YotsubaEngine.csproj -c Debug

# Compilar sandbox (DesktopGL)
 dotnet build SandBoxGame/SandBoxGame.DesktopGL/SandBoxGame.DesktopGL.csproj -c Debug
```

## Estado actual de la documentación

- Documentación principal del engine: [`YotsubaEngine/README.md`](YotsubaEngine/README.md)
- Mapa de árbol de archivos: [`YotsubaEngine/filetree.md`](YotsubaEngine/filetree.md)
- Casos de uso: [`YotsubaEngine/UserCases`](YotsubaEngine/UserCases)
- Guía de shaders: [`YotsubaEngine/Graphics/Shaders/README_SHADERS.md`](YotsubaEngine/Graphics/Shaders/README_SHADERS.md)

---

Si quieres, puedo crear en un siguiente cambio una **guía “from zero to playable scene in 10 minutes”** con pasos exactos y snippets listos para copiar/pegar.
