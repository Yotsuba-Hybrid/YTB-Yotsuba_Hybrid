using FuelCell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts.ModelScreen
{
    [Script]
    public class GroundScript : BaseScript, IKeyboardListener
    {

        private GameObject ground;
        private Camera gameCamera;
        public GroundScript()
        {
            ground = new GameObject();
            gameCamera = new Camera();
        }
        public override void Initialize()
        {
            // Initialize the Game objects
           
            ground.Model = YTBGlobalState.ContentManager.Load<Model>("Models/ground");

            //BindKeyboard(YotsubaEngine.Core.Component.C_AGNOSTIC.ActionEntityInput.MoveUp, Microsoft.Xna.Framework.Input.Keys.W);
            base.Initialize();
        }

        float rotation = 0.0f;

        public override void Update(GameTime gametime)
        {
            if (rotation >= 3) rotation = 0f;
            Vector3 position = Vector3.Zero;
            gameCamera.Update(rotation += 0.01f, position, YTBGlobalState.GraphicsDevice.Viewport.AspectRatio);
            base.Update(gametime);
        }

        public override void Draw3D(GameTime gameTime)
        {
            DrawTerrain(ground.Model);
            base.Draw3D(gameTime);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        private void DrawTerrain(Model model)
        {

            var transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index];
                    // Use the matrices provided by the game camera
                    effect.View = gameCamera.ViewMatrix;
                    effect.Projection = gameCamera.ProjectionMatrix;
                }
                YTBGlobalState.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

                mesh.Draw();
            }
        }

        public void OnKeyboardInput(OnKeyBoardEvent @event)
        {
            if(@event.Key == Microsoft.Xna.Framework.Input.Keys.W)
            {
                gameCamera.TargetOffset += new Vector3(0, 10, 20);
            }else if(@event.Key == Microsoft.Xna.Framework.Input.Keys.S)
            {
                gameCamera.TargetOffset += new Vector3(0, -10, -20);

            }
        }
    }
}


namespace FuelCell
{
    public class GameObject
    {
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
        public bool IsActive { get; set; }
        public BoundingSphere BoundingSphere { get; set; }

        public GameObject()
        {
            Model = null;
            Position = Vector3.Zero;
            IsActive = false;
            BoundingSphere = new BoundingSphere();
        }
    }
}

namespace FuelCell
{
    public class GameConstants
    {
        //camera constants
        public const float NearClip = 1.0f;
        public const float FarClip = 2000;
        public const float ViewAngle = 45.0f;
    }
}

namespace FuelCell
{
    public class Camera
    {

        public Vector3 AvatarHeadOffset { get; set; }
        public Vector3 TargetOffset { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Camera()
        {
            AvatarHeadOffset = new Vector3(0, 0, -185);
            TargetOffset = new Vector3(0, 5, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        public void Update(float avatarYaw, Vector3 position, float aspectRatio)
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

            Vector3 transformedheadOffset = Vector3.Transform(AvatarHeadOffset, rotationMatrix);
            Vector3 transformedReference = Vector3.Transform(TargetOffset, rotationMatrix);

            Vector3 cameraPosition = position + transformedheadOffset;
            Vector3 cameraTarget = position + transformedReference;

            //Calculate the camera's view and projection matrices based on current values.
            ViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(GameConstants.ViewAngle),
                aspectRatio, GameConstants.NearClip, GameConstants.FarClip);
        }
    }
}