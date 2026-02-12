using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;

namespace SandBoxGame.Core.Scripts
{
    [Script]
    /// <summary>
    /// Script que permite controlar la cámara y el movimiento en modo 2.5D.
    /// <para>Script that controls camera and movement in 2.5D mode.</para>
    /// </summary>
    public class Dimencion25 : BaseScript, IKeyboardListener
    {

        private CameraComponent3D CameraComponent3D;
        /// <summary>
        /// Inicializa los controles de entrada y la cámara.
        /// <para>Initializes input bindings and the camera.</para>
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            GetInputComponent().KeyBoard.Add(ActionEntityInput.RotateCameraLeft, Microsoft.Xna.Framework.Input.Keys.Left);
            GetInputComponent().KeyBoard.Add(ActionEntityInput.RotateCameraRight, Microsoft.Xna.Framework.Input.Keys.Right);
            GetInputComponent().KeyBoard.Add(ActionEntityInput.RotateCameraUp, Microsoft.Xna.Framework.Input.Keys.Up);
            GetInputComponent().KeyBoard.Add(ActionEntityInput.RotateCameraDown, Microsoft.Xna.Framework.Input.Keys.Down);
            CameraComponent3D = EntityManager.Camera;
        }

        /// <summary>
        /// Maneja la entrada de teclado para mover la cámara.
        /// <para>Handles keyboard input to move the camera.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de teclado recibido.
        /// <para>Keyboard event received.</para>
        /// </param>
        public void OnKeyboardInput(OnKeyBoardEvent @event)
        {
            ref TransformComponent transformComponent = ref GetTransformComponent();
            float speed = 3f;
            float turnSpeed = MathHelper.ToRadians(2f);
            switch (@event.Key)
            {
                case Microsoft.Xna.Framework.Input.Keys.W:
                    transformComponent.Position += CameraComponent3D.Front * speed;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.S:
                    transformComponent.Position += CameraComponent3D.Back * speed;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.A:
                    transformComponent.Position += CameraComponent3D.Left * speed;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.D:
                    transformComponent.Position += CameraComponent3D.Right * speed;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Left:
                    CameraComponent3D.AddOrbitAngles(-turnSpeed, 0f);
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Right:
                    CameraComponent3D.AddOrbitAngles(turnSpeed, 0f);
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Up:
                    CameraComponent3D.AddOrbitAngles(0f, -turnSpeed);
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Down:
                    CameraComponent3D.AddOrbitAngles(0f, turnSpeed);
                    break;
            }
        }

        /// <summary>
        /// Actualiza el script cada cuadro.
        /// <para>Updates the script each frame.</para>
        /// </summary>
        /// <param name="gametime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

       
    }
}
