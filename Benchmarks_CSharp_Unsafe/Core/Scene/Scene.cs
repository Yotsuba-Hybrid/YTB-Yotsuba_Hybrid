using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Benchmarks.Core.Entity;
using Benchmarks.Core.Systems;
using Benchmarks.Core.Types;

namespace Benchmarks.Core.Scene
{
    /// <summary>
    /// Fully unmanaged scene struct allocated on native heap.
    /// All systems are value-type structs with pointer-based EntityManager access.
    /// </summary>
    public unsafe struct SceneStruct
    {
        public nint EmPtr; // EntityManager*
        public AnimationSystem2D AnimationSystem2D;
        public ButtonSystem2D ButtonSystem2D;
        public PhysicsSystem2D PhysicsSystem2D;
        public RenderSystem2D RenderSystem2D;
        public CameraSystem CameraSystem;
        public InputSystem InputSystem;
        public ScriptSystem ScriptSystem;
        public TileMapSystem2D TilemapSystem;
        public FontSystem2D FontSystem2D;
        public SystemBuilder SystemBuilder;

        public static nint Create()
        {
            SceneStruct* ptr = (SceneStruct*)NativeMemory.AllocZeroed((nuint)sizeof(SceneStruct));
            ptr->EmPtr = (nint)EntityManager.Create();
            ptr->AnimationSystem2D = default;
            ptr->ButtonSystem2D = default;
            ptr->PhysicsSystem2D = default;
            ptr->RenderSystem2D = default;
            ptr->CameraSystem = default;
            ptr->InputSystem = default;
            ptr->ScriptSystem = default;
            ptr->TilemapSystem = default;
            ptr->FontSystem2D = default;
            ptr->SystemBuilder = default;
            return (nint)ptr;
        }

        public static void Destroy(nint scenePtr)
        {
            if (scenePtr == 0) return;
            SceneStruct* ptr = (SceneStruct*)scenePtr;
            ptr->Dispose();
            NativeMemory.Free(ptr);
        }

        public void Initialize()
        {
            EntityManager* em = (EntityManager*)EmPtr;
            AnimationSystem2D.InitializeSystem(em);
            ButtonSystem2D.InitializeSystem(em);
            PhysicsSystem2D.InitializeSystem(em);
            RenderSystem2D.InitializeSystem(em);
            CameraSystem.InitializeSystem(em);
            InputSystem.InitializeSystem(em);
            ScriptSystem.InitializeSystem(em);
            TilemapSystem.InitializeSystem(em);
            FontSystem2D.InitializeSystem(em);
            SystemBuilder.InitializeSystem(em);

            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                ScriptSystem.SharedEntityInitialize(ref entity);
                FontSystem2D.SharedEntityInitialize(ref entity);
            }
        }

        public void Update(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)EmPtr;
            InputSystem.UpdateSystem(gameTime);

            if (em == null || em->YotsubaEntities.Count == 0) return;
            PhysicsSystem2D.UpdateSystem(gameTime);
            ButtonSystem2D.UpdateSystem(gameTime);
            AnimationSystem2D.UpdateSystem(gameTime);
            CameraSystem.UpdateSystem(gameTime);
            SystemBuilder.UpdateSystem(gameTime);

            Yotsuba* entities = em->YotsubaEntities.Ptr;
            int count = em->YotsubaEntities.Count;
            for (int i = 0; i < count; i++)
            {
                ref Yotsuba entity = ref Unsafe.Add(ref Unsafe.AsRef<Yotsuba>(entities), i);
                ScriptSystem.SharedEntityForEachUpdate(ref entity, gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            EntityManager* em = (EntityManager*)EmPtr;
            if (em == null || em->YotsubaEntities.Count == 0) return;

            Unsafe.SkipInit(out Rectangle cameraBounds);
            cameraBounds.X = 0;
            cameraBounds.Y = 0;
            cameraBounds.Width = 1920;
            cameraBounds.Height = 1080;

            TilemapSystem.UpdateSystem(gameTime, cameraBounds);
            RenderSystem2D.UpdateSystem(gameTime);
            FontSystem2D.DrawSystem(gameTime);

            ScriptSystem.DrawSystem3D(gameTime);
            SystemBuilder.Render3D(gameTime);

            ScriptSystem.DrawSystem2D(gameTime);
            SystemBuilder.Render2D(gameTime);
        }

        public void Dispose()
        {
            ScriptSystem.Clear();
            SystemBuilder.Dispose();
            AnimationSystem2D.Dispose();
            ButtonSystem2D.Dispose();
            PhysicsSystem2D.Dispose();
            CameraSystem.Dispose();
            InputSystem.Dispose();
            FontSystem2D.Dispose();

            if (EmPtr != 0)
            {
                EntityManager.Destroy((EntityManager*)EmPtr);
                EmPtr = 0;
            }
        }
    }
}
