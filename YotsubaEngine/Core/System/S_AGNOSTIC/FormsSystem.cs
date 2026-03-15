using Microsoft.Xna.Framework;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Forms;
using System;
using global::System.Diagnostics;

namespace YotsubaEngine.Core.System.S_AGNOSTIC
{
    public class FormsSystem : ISystem
    {
        public EntityManager EntityManager { get; set; }

        public void InitializeSystem(EntityManager entityManager)
        {
            Debug.WriteLine("[FormsSystem] InitializeSystem called");
            Console.WriteLine("[FormsSystem] InitializeSystem called");
            
            EntityManager = entityManager;
            
            try
            {
                FormsManager.Instance.Initialize();
                Debug.WriteLine("[FormsSystem] FormsManager initialized successfully");
                Console.WriteLine("[FormsSystem] FormsManager initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormsSystem] ERROR initializing FormsManager: {ex.Message}");
                Console.WriteLine($"[FormsSystem] ERROR initializing FormsManager: {ex.Message}");
            }
        }

        public void UpdateSystem(GameTime gameTime)
        {
            FormsManager.Instance.Update(gameTime);
        }

        public void DrawSystem(GameTime gameTime)
        {
            FormsManager.Instance.Draw(gameTime);
        }

        public void Dispose()
        {
            FormsManager.Reset();
        }
    }
}