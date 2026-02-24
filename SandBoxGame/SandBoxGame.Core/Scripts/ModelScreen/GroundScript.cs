using FuelCell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts.ModelScreen
{
    [Script]
    /// <summary>
    /// Script que renderiza el terreno y controla la cámara de la escena 3D.
    /// <para>Script that renders the terrain and controls the 3D scene camera.</para>
    /// </summary>
    public class GroundScript : BaseScript, IKeyboardListener
    {

        private GameObject ground;
        private Camera gameCamera;
        /// <summary>
        /// Inicializa las instancias de terreno y cámara.
        /// <para>Initializes terrain and camera instances.</para>
        /// </summary>
        public GroundScript()
        {
            ground = new GameObject();
            gameCamera = new Camera();
        }
        /// <summary>
        /// Inicializa los objetos del juego y carga el modelo del terreno.
        /// <para>Initializes game objects and loads the terrain model.</para>
        /// </summary>
        public override void Initialize()
        {
            // Initialize the Game objects
           
            ground.Model = YTBGlobalState.ContentManager.Load<Model>("Models/ground");

            //BindKeyboard(YotsubaEngine.Core.Component.C_AGNOSTIC.ActionEntityInput.MoveUp, Microsoft.Xna.Framework.Input.Keys.W);
            base.Initialize();
        }

        float rotation = 0.0f;

        /// <summary>
        /// Actualiza la cámara y la rotación del terreno cada cuadro.
        /// <para>Updates the camera and terrain rotation each frame.</para>
        /// </summary>
        /// <param name="gametime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gametime)
        {
            if (rotation >= 3) rotation = 0f;
            Vector3 position = Vector3.Zero;
            gameCamera.Update(rotation += 0.01f, position, YTBGlobalState.GraphicsDevice.Viewport.AspectRatio);
            base.Update(gametime);
        }

        /// <summary>
        /// Dibuja el terreno en 3D durante el renderizado.
        /// <para>Draws the terrain in 3D during rendering.</para>
        /// </summary>
        /// <param name="gameTime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Draw3D(GameTime gameTime)
        {
            DrawTerrain(ground.Model);
            base.Draw3D(gameTime);
        }

        /// <summary>
        /// Limpia los recursos del script.
        /// <para>Cleans up script resources.</para>
        /// </summary>
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

        /// <summary>
        /// Maneja la entrada de teclado para ajustar la cámara.
        /// <para>Handles keyboard input to adjust the camera.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de teclado recibido.
        /// <para>Keyboard event received.</para>
        /// </param>
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
    /// <summary>
    /// Representa un objeto de juego con modelo y estado básico.
    /// <para>Represents a game object with a model and basic state.</para>
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// Modelo 3D asociado al objeto.
        /// <para>3D model associated with the object.</para>
        /// </summary>
        public Model Model { get; set; }
        /// <summary>
        /// Posición del objeto en el mundo.
        /// <para>Object position in the world.</para>
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Indica si el objeto está activo.
        /// <para>Indicates whether the object is active.</para>
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Esfera de colisión del objeto.
        /// <para>Object collision sphere.</para>
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        /// <summary>
        /// Inicializa el objeto con valores por defecto.
        /// <para>Initializes the object with default values.</para>
        /// </summary>
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
    /// <summary>
    /// Constantes de cámara para la escena.
    /// <para>Camera constants for the scene.</para>
    /// </summary>
    public class GameConstants
    {
        //camera constants
        /// <summary>
        /// Plano cercano de recorte de la cámara.
        /// <para>Camera near clipping plane.</para>
        /// </summary>
        public const float NearClip = 1.0f;
        /// <summary>
        /// Plano lejano de recorte de la cámara.
        /// <para>Camera far clipping plane.</para>
        /// </summary>
        public const float FarClip = 2000;
        /// <summary>
        /// Ángulo de visión de la cámara.
        /// <para>Camera field of view angle.</para>
        /// </summary>
        public const float ViewAngle = 45.0f;
    }
}

namespace FuelCell
{
    /// <summary>
    /// Cámara orbital simple para la escena 3D.
    /// <para>Simple orbital camera for the 3D scene.</para>
    /// </summary>
    public class Camera
    {

        /// <summary>
        /// Desplazamiento de la cabeza del avatar respecto al objetivo.
        /// <para>Avatar head offset relative to the target.</para>
        /// </summary>
        public Vector3 AvatarHeadOffset { get; set; }
        /// <summary>
        /// Desplazamiento del objetivo de la cámara.
        /// <para>Camera target offset.</para>
        /// </summary>
        public Vector3 TargetOffset { get; set; }
        /// <summary>
        /// Matriz de vista actual.
        /// <para>Current view matrix.</para>
        /// </summary>
        public Matrix ViewMatrix { get; set; }
        /// <summary>
        /// Matriz de proyección actual.
        /// <para>Current projection matrix.</para>
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }

        /// <summary>
        /// Inicializa la cámara con valores por defecto.
        /// <para>Initializes the camera with default values.</para>
        /// </summary>
        public Camera()
        {
            AvatarHeadOffset = new Vector3(0, 0, -185);
            TargetOffset = new Vector3(0, 5, 0);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Actualiza las matrices de vista y proyección de la cámara.
        /// <para>Updates the camera view and projection matrices.</para>
        /// </summary>
        /// <param name="avatarYaw">
        /// Rotación en Y del avatar.
        /// <para>Avatar yaw rotation.</para>
        /// </param>
        /// <param name="position">
        /// Posición del avatar.
        /// <para>Avatar position.</para>
        /// </param>
        /// <param name="aspectRatio">
        /// Relación de aspecto del viewport.
        /// <para>Viewport aspect ratio.</para>
        /// </param>
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
