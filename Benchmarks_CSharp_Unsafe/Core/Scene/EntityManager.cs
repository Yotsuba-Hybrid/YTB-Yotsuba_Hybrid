using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Benchmarks.Core.Collections;
using Benchmarks.Core.Components;
using Benchmarks.Core.Entity;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Fully unmanaged EntityManager using NativeMemory-backed YTB arrays.
    /// Allocated on native heap via Create(). All access via pointer.
    /// </summary>
    public unsafe struct EntityManager : IDisposable
    {
        public YTB<Yotsuba> YotsubaEntities;
        public YTB<TransformComponent> TransformComponents;
        public YTB<SpriteComponent2D> Sprite2DComponents;
        public YTB<AnimationComponent2D> Animation2DComponents;
        public YTB<RigidBodyComponent2D> Rigidbody2DComponents;
        public YTB<InputComponent> InputComponents;
        public YTB<ModelComponent3D> ModelComponents3D;
        public YTB<TileMapComponent2D> TileMapComponent2Ds;
        public YTB<ButtonComponent2D> Button2DComponents;
        public YTB<ScriptComponent> ScriptComponents;
        public YTB<FontComponent2D> Font2DComponents;
        public YTB<ShaderComponent> ShaderComponents;
        public YTB<YTBModelComponent3D> YtbModelComponents;

        public int CameraEntityId;
        public float CameraZoom;

        /// <summary>
        /// Allocates EntityManager on native heap. Returns stable pointer.
        /// </summary>
        public static EntityManager* Create()
        {
            EntityManager* ptr = (EntityManager*)NativeMemory.AllocZeroed((nuint)sizeof(EntityManager));
            ptr->YotsubaEntities = YTB<Yotsuba>.Create();
            ptr->TransformComponents = YTB<TransformComponent>.Create();
            ptr->Sprite2DComponents = YTB<SpriteComponent2D>.Create();
            ptr->Animation2DComponents = YTB<AnimationComponent2D>.Create();
            ptr->Rigidbody2DComponents = YTB<RigidBodyComponent2D>.Create();
            ptr->InputComponents = YTB<InputComponent>.Create();
            ptr->ModelComponents3D = YTB<ModelComponent3D>.Create();
            ptr->TileMapComponent2Ds = YTB<TileMapComponent2D>.Create();
            ptr->Button2DComponents = YTB<ButtonComponent2D>.Create();
            ptr->ScriptComponents = YTB<ScriptComponent>.Create();
            ptr->Font2DComponents = YTB<FontComponent2D>.Create();
            ptr->ShaderComponents = YTB<ShaderComponent>.Create();
            ptr->YtbModelComponents = YTB<YTBModelComponent3D>.Create();
            ptr->CameraEntityId = -1;
            ptr->CameraZoom = 1.0f;
            return ptr;
        }

        public static void Destroy(EntityManager* ptr)
        {
            if (ptr == null) return;
            ptr->Dispose();
            NativeMemory.Free(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntity(ref Yotsuba entity)
        {
            int Index = YotsubaEntities.Add(entity);
            YotsubaEntities[Index].Id = Index;
            entity.Id = Index;

            int ci = 0;
            TransformComponents.Add(default, out ci);
            Sprite2DComponents.Add(default, out ci);
            Animation2DComponents.Add(default, out ci);
            Rigidbody2DComponents.Add(default, out ci);
            InputComponents.Add(default, out ci);
            ModelComponents3D.Add(default, out ci);
            Button2DComponents.Add(default, out ci);
            ScriptComponents.Add(default, out ci);
            TileMapComponent2Ds.Add(default, out ci);
            Font2DComponents.Add(default, out ci);
            ShaderComponents.Add(default, out ci);
            YtbModelComponents.Add(default, out ci);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTransformComponent(Yotsuba entity, TransformComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Transform);
            TransformComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddSpriteComponent(Yotsuba entity, SpriteComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Sprite);
            Sprite2DComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddAnimationComponent(Yotsuba entity, AnimationComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Animation);
            Animation2DComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRigidbodyComponent(Yotsuba entity, RigidBodyComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Rigibody);
            Rigidbody2DComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddInputComponent(Yotsuba entity, InputComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Input);
            InputComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddButtonComponent2D(Yotsuba entity, ButtonComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Button2D);
            Button2DComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddShaderComponent2D(Yotsuba entity, ShaderComponent component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Shader);
            ShaderComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddScriptComponent(Yotsuba entity, ScriptComponent script)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Script);
            ScriptComponents.Set((uint)entity.Id, script);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFontComponent2D(Yotsuba entity, FontComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Font);
            Font2DComponents.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTileMapComponent(Yotsuba entity, TileMapComponent2D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.TileMap);
            TileMapComponent2Ds.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddModelComponent3D(Yotsuba entity, ModelComponent3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.Model3D);
            ModelComponents3D.Set((uint)entity.Id, component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddYTBObject3D(Yotsuba entity, YTBModelComponent3D component)
        {
            YotsubaEntities[entity.Id].AddComponent(YTBComponent.YTBModel3D);
            YtbModelComponents.Set((uint)entity.Id, component);
        }

        public void Dispose()
        {
            YotsubaEntities.Dispose();
            TransformComponents.Dispose();
            Sprite2DComponents.Dispose();
            Animation2DComponents.Dispose();
            Rigidbody2DComponents.Dispose();
            InputComponents.Dispose();
            ModelComponents3D.Dispose();
            TileMapComponent2Ds.Dispose();
            Button2DComponents.Dispose();
            ScriptComponents.Dispose();
            Font2DComponents.Dispose();
            ShaderComponents.Dispose();
            YtbModelComponents.Dispose();
        }
    }
}
