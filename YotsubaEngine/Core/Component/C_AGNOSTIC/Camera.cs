using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YTBMath;
using YotsubaEngine.Exceptions;
using static YotsubaEngine.Exceptions.GameWontRun;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Cámara base que define matrices de vista y proyección para renderizado.
    /// <para>Base camera that defines view and projection matrices for rendering.</para>
    /// </summary>
    public abstract class Camera
    {
        /// <summary>
        /// Constructor de la cámara. Inicializa posición, ángulo de visión y planos de recorte.
        /// <para>Camera constructor. Initializes position, view angle, and clipping planes.</para>
        /// </summary>
        /// <param name="cameraPosition">Posición inicial de la cámara.<para>Initial camera position.</para></param>
        /// <param name="angleView">Ángulo de visión en grados.<para>Field of view angle in degrees.</para></param>
        /// <param name="near">Plano de recorte cercano.<para>Near clipping plane.</para></param>
        /// <param name="far">Plano de recorte lejano.<para>Far clipping plane.</para></param>
        public Camera(Vector3 cameraPosition, float angleView, float near, float far)
        {
            SetRenderPoint(cameraPosition, Vector3.One);
            SetAngleView(angleView);
            SetDistanceRender(near, far);
        }

        /// <summary>
        /// Posición de la cámara en el espacio 3D.
        /// <para>Camera position in 3D space.</para>
        /// </summary>
        protected Vector3 CameraPosition { get; set; }

        /// <summary>
        /// Matriz de vista que indica hacia dónde mira la cámara.
        /// <para>View matrix indicating where the camera is looking.</para>
        /// </summary>
        protected Matrix RenderPoint { get; set; }

        /// <summary>
        /// Matriz de vista que indica hacia dónde mira la cámara.
        /// <para>View matrix indicating where the camera is looking.</para>
        /// </summary>
        public Matrix ViewMatrix => RenderPoint;
        /// <summary>
        /// Matriz de parámetros de renderizado de la cámara, utilizada para definir la proyección y el recorte.
        /// <para>Camera render parameters matrix, used for projection and clipping.</para>
        /// </summary>
        protected Matrix RenderParams { get; set; }
        /// <summary>
        /// Matriz de parámetros de renderizado de la cámara, utilizada para definir la proyección y el recorte.
        /// <para>Camera render parameters matrix, used for projection and clipping.</para>
        /// </summary>
        public Matrix ProjectionMatrix => RenderParams;

        /// <summary>
        /// Ángulo de visión de la cámara. Mientras más alto, más abierto el campo de visión.
        /// <para>Camera field of view angle. Higher values mean a wider field of view.</para>
        /// </summary>
        private float AngleView { get; set; }

        /// <summary>
        /// Establece la matriz de vista de la cámara.
        /// <para>Sets the camera view matrix.</para>
        /// </summary>
        /// <param name="renderPoint">Matriz de vista.<para>View matrix.</para></param>
        private void SetRenderPoint(Matrix renderPoint)
        {
            RenderPoint = renderPoint;
        }

        /// <summary>
        /// Establece la posición y el objetivo de la cámara en 3D. El vector "up" es Vector3.Up por defecto.
        /// <para>Sets the camera position and target in 3D. The "up" vector is Vector3.Up by default.</para>
        /// </summary>
        /// <param name="cameraPosition">Posición de la cámara.<para>Camera position.</para></param>
        /// <param name="targetPosition">Punto objetivo.<para>Target position.</para></param>
        public void SetRenderPoint(Vector3 cameraPosition, Vector3 targetPosition)
        {
            CameraPosition = cameraPosition;
            SetRenderPoint(Matrix.CreateLookAt(cameraPosition, targetPosition, Vector3.Up));
        }

        /// <summary>
        /// Establece la posición, objetivo y vector "up" de la cámara en 3D.
        /// <para>Sets the camera position, target, and "up" vector in 3D.</para>
        /// </summary>
        /// <param name="cameraPosition">Posición de la cámara.<para>Camera position.</para></param>
        /// <param name="targetPosition">Punto objetivo.<para>Target position.</para></param>
        /// <param name="up">Vector "arriba".<para>Up vector.</para></param>
        public void SetRenderPoint(Vector3 cameraPosition, Vector3 targetPosition, Vector3 up)
        {
            CameraPosition = cameraPosition;
            SetRenderPoint(Matrix.CreateLookAt(cameraPosition, targetPosition, up));
        }

        /// <summary>
        /// Establece el ángulo de visión en radianes.
        /// <para>Sets the field of view angle in radians.</para>
        /// </summary>
        /// <remarks>Utiliza este método si ya dispones del valor del ángulo en radianes y deseas aplicarlo directamente a la cámara. / Use this method if you already have the angle in radians and want to apply it directly.</remarks>
        /// <param name="angleRadians">Ángulo a establecer, en radianes.<para>Angle to set, in radians.</para></param>
        public void SetAngleViewRadians(float angleRadians)
        {
            AngleView = angleRadians;
        }

        /// <summary>
        /// Establece el ángulo de visión en grados.
        /// <para>Sets the field of view angle in degrees.</para>
        /// </summary>
        /// <remarks>El ángulo especificado se convierte de grados a radianes antes de aplicarse. Utiliza este método para actualizar la orientación de la vista a partir de un valor en grados. / The specified angle is converted from degrees to radians before being applied. Use this method to update the view orientation based on degree input.</remarks>
        /// <param name="angleGrades">Ángulo a establecer, en grados. Normalmente debe estar entre 0 y 360.<para>Angle to set, in degrees. Typically must be between 0 and 360.</para></param>
        public void SetAngleView(float angleGrades)
        {
            SetAngleViewRadians(MathHelper.ToRadians(angleGrades));
        }

        /// <summary>
        /// Establece los planos de recorte cercano y lejano para el renderizado basado en distancia.
        /// <para>Sets the near and far clipping planes for distance-based rendering.</para>
        /// </summary>
        /// <param name="near">Distancia al plano de recorte cercano. Debe ser mayor que cero y menor que el valor de <paramref name="far"/>.<para>The distance to the near clipping plane. Must be greater than zero and less than the value of <paramref name="far"/>.</para></param>
        /// <param name="far">Distancia al plano de recorte lejano. Debe ser mayor que el valor de <paramref name="near"/>.<para>The distance to the far clipping plane. Must be greater than the value of <paramref name="near"/>.</para></param>
        public void SetDistanceRender(float near, float far)
        {
            // Validar que AngleView sea válido antes de crear la proyección
            if (AngleView <= 0 || AngleView >= MathHelper.Pi || near <= 0 || far <= near)
                return;

            float aspect = YTBGlobalState.GraphicsDevice.Viewport.AspectRatio;
            if (aspect <= 0)
                return;

            RenderParams = Matrix.CreatePerspectiveFieldOfView(AngleView, aspect, near, far);
        }

        /// <summary>
        /// Actualiza la posición de la cámara para que mire hacia un nuevo objetivo.
        /// <para>Updates the camera position to look at a new target.</para>
        /// </summary>
        /// <param name="target">Nuevo objetivo.<para>New target.</para></param>
        public void Update(Vector3 target)
        {
            SetRenderPoint(CameraPosition, target);
        }

        /// <summary>
        /// Invierte una matriz dada.
        /// <para>Inverts a given matrix.</para>
        /// </summary>
        /// <param name="matrix">Matriz a invertir.<para>Matrix to invert.</para></param>
        /// <returns>Matriz invertida.<para>Inverted matrix.</para></returns>
        protected Matrix Invert(Matrix matrix)
        {
            return Matrix.Invert(matrix);
        }
    }


    /// <summary>
    /// Componente de cámara 3D que sigue a una entidad específica en el mundo del juego.
    /// <para>3D camera component that follows a specific entity in the game world.</para>
    /// </summary>
    public class CameraComponent3D : Camera
    {

        /// <summary>
        /// Entidad a la que seguirá la cámara.
        /// <para>Entity that the camera will follow.</para>
        /// </summary>
        public Yotsuba EntityToFollow { get; set; }

        /// <summary>
        /// Referencia al administrador de entidades.
        /// <para>Reference to the entity manager.</para>
        /// </summary>
        private EntityManager EntityManager { get; set; }

        /// <summary>
        /// Desfase de la cámara relativa a la posición de la entidad a seguir. Por ejemplo, para mirar por encima del jugador (0, 15, 5).
        /// <para>Camera offset relative to the entity to follow. For example, to look above the player (0, 15, 5).</para>
        /// </summary>
        public Vector3 OffsetCamera { get; set; } = new Vector3(0, 50, -100);

        /// <summary>
        /// Indica si la cámara aplica orbitado alrededor del objetivo.
        /// <para>Indicates whether the camera applies orbiting around the target.</para>
        /// </summary>
        public bool UseOrbit { get; set; }

        /// <summary>
        /// Rotación horizontal de la órbita en radianes.
        /// <para>Horizontal orbit rotation in radians.</para>
        /// </summary>
        public float OrbitYaw { get; private set; }

        /// <summary>
        /// Rotación vertical de la órbita en radianes.
        /// <para>Vertical orbit rotation in radians.</para>
        /// </summary>
        public float OrbitPitch { get; private set; }

        /// <summary>
        /// Constructor de CameraComponent3D. Inicializa la cámara y la referencia al EntityManager.
        /// <para>CameraComponent3D constructor. Initializes the camera and the EntityManager reference.</para>
        /// </summary>
        /// <param name="entityManager">Administrador de entidades a usar.<para>Entity manager to use.</para></param>
        /// <param name="cameraPosition">Posición inicial de la cámara.<para>Initial camera position.</para></param>
        /// <param name="angleView">Ángulo de visión en grados.<para>Field of view angle in degrees.</para></param>
        /// <param name="nearRender">Plano de recorte cercano.<para>Near clipping plane.</para></param>
        /// <param name="farRender">Plano de recorte lejano.<para>Far clipping plane.</para></param>
        public CameraComponent3D(EntityManager entityManager, Vector3 cameraPosition, float angleView, float nearRender, float farRender) 
            : base(cameraPosition, angleView, nearRender, farRender)
        {
            EntityManager = entityManager;
        }

        /// <summary>
        /// Vector derecho relativo a la cámara.
        /// <para>Right vector relative to the camera.</para>
        /// </summary>
        public Vector3 Right => Invert(RenderPoint).Right;
        /// <summary>
        /// Vector izquierdo relativo a la cámara.
        /// <para>Left vector relative to the camera.</para>
        /// </summary>
        public Vector3 Left => Invert(RenderPoint).Left;
        /// <summary>
        /// Vector trasero relativo a la cámara.
        /// <para>Backward vector relative to the camera.</para>
        /// </summary>
        public Vector3 Back => Invert(RenderPoint).Backward;
        /// <summary>
        /// Vector frontal relativo a la cámara.
        /// <para>Forward vector relative to the camera.</para>
        /// </summary>
        public Vector3 Front => Invert(RenderPoint).Forward;
        /// <summary>
        /// Vector hacia abajo relativo a la cámara.
        /// <para>Down vector relative to the camera.</para>
        /// </summary>
        public Vector3 Down => Invert(RenderPoint).Down;
        /// <summary>
        /// Vector hacia arriba relativo a la cámara.
        /// <para>Up vector relative to the camera.</para>
        /// </summary>
        public Vector3 Up => Invert(RenderPoint).Up;

        /// <summary>
        /// Actualiza la posición de la cámara para seguir a la entidad objetivo.
        /// <para>Updates the camera position to follow the target entity.</para>
        /// </summary>
        public void Update()
        {
//-:cnd:noEmit
#if YTB
            if (EntityToFollow.HasNotComponent(YTBComponent.Transform))
                _ = new GameWontRun("La entidad a seguir de la camara 3D no tiene un TransformComponent", YTBErrors.EntityFollowCameraIsNotAppropiate);

            // Cámara libre del engine: solo escribe las matrices de render sin tocar CameraPosition
            if (YTBGlobalState.EngineShortcutsMode && YTBGlobalState.FreeCameraInitialized)
            {
                Vector3 pos = YTBGlobalState.FreeCameraPosition;
                float yaw = YTBGlobalState.FreeCameraYaw;
                float pitch = YTBGlobalState.FreeCameraPitch;

                Vector3 forward = new Vector3(
                    (float)(Math.Cos(pitch) * Math.Sin(yaw)),
                    (float)Math.Sin(pitch),
                    (float)(Math.Cos(pitch) * Math.Cos(yaw))
                );

                RenderPoint = Matrix.CreateLookAt(pos, pos + forward, Vector3.Up);
                return;
            }
#endif
//+:cnd:noEmit
            ref TransformComponent positionEntity = ref EntityManager.TransformComponents[EntityToFollow];

            Vector3 offset = UseOrbit ? GetOrbitOffset() : OffsetCamera;
            Vector3 cameraPosition = positionEntity.Position + offset + new Vector3(YTBGlobalState.OffsetCamera, 0);

            SetRenderPoint(cameraPosition, positionEntity.Position);
        }

        /// <summary>
        /// Establece los ángulos de órbita de la cámara en radianes.
        /// <para>Sets the camera orbit angles in radians.</para>
        /// </summary>
        /// <param name="yawRadians">Rotación horizontal en radianes.<para>Horizontal rotation in radians.</para></param>
        /// <param name="pitchRadians">Rotación vertical en radianes.<para>Vertical rotation in radians.</para></param>
        public void SetOrbitAngles(float yawRadians, float pitchRadians)
        {
            OrbitYaw = yawRadians;
            OrbitPitch = MathHelper.Clamp(pitchRadians, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
            UseOrbit = true;
        }

        /// <summary>
        /// Incrementa los ángulos de órbita de la cámara en radianes.
        /// <para>Increments the camera orbit angles in radians.</para>
        /// </summary>
        /// <param name="yawDeltaRadians">Delta de rotación horizontal.<para>Horizontal rotation delta.</para></param>
        /// <param name="pitchDeltaRadians">Delta de rotación vertical.<para>Vertical rotation delta.</para></param>
        public void AddOrbitAngles(float yawDeltaRadians, float pitchDeltaRadians)
        {
            SetOrbitAngles(OrbitYaw + yawDeltaRadians, OrbitPitch + pitchDeltaRadians);
        }

        /// <summary>
        /// Obtiene el offset orbitado según los ángulos actuales.
        /// <para>Gets the orbited offset based on the current angles.</para>
        /// </summary>
        /// <returns>Offset calculado para la órbita.<para>Calculated orbit offset.</para></returns>
        public Vector3 GetOrbitOffset()
        {
            var rotation = Matrix.CreateFromYawPitchRoll(OrbitYaw, OrbitPitch, 0f);
            return Vector3.Transform(OffsetCamera, rotation);
        }

        /// <summary>
        /// Ayudante para dibujar un modelo 3D con la cámara actual.
        /// <para>Helper to draw a 3D model with the current camera.</para>
        /// </summary>
        /// <param name="model3D">Modelo 3D a dibujar.<para>3D model to draw.</para></param>
        /// <param name="transformComponent">Componente de transformación.<para>Transform component.</para></param>
        /// <param name="shaderComponent">Componente de shader opcional.<para>Optional shader component.</para></param>
        /// <param name="entityId">ID de la entidad para verificar selección en modo engine.<para>Entity ID for engine mode selection check.</para></param>
        public void DrawModel(ModelComponent3D model3D, ref TransformComponent transformComponent, ShaderComponent? shaderComponent = null, int entityId = -1)
        {
            var transforms = new Matrix[model3D.Model.Bones.Count];
            model3D.Model.CopyAbsoluteBoneTransformsTo(transforms);

//-:cnd:noEmit
#if YTB
            bool isSelected = YTBGlobalState.EngineShortcutsMode && entityId != -1 && YTBGlobalState.SelectedModel3DEntityIds.Contains(entityId);
#endif
//+:cnd:noEmit

            if (shaderComponent.HasValue && shaderComponent.Value.IsActive)
            {

                foreach (ModelMesh mesh in model3D.Model.Meshes)
                {
                    var meshTransform = transforms[mesh.ParentBone.Index];

                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = shaderComponent.Value.Effect;

                    }

                    mesh.Draw();
                }

            }
            else 
            {

                foreach (ModelMesh mesh in model3D.Model.Meshes)
                {

                    foreach (BasicEffect e in mesh.Effects)
                    {

                        float yaw = MathHelper.ToRadians(transformComponent.Rotation);
                        Matrix world = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateRotationY(yaw)
                            * Matrix.CreateTranslation(transformComponent.Position);

                        
//-:cnd:noEmit
#if YTB 
                        if (isSelected)
                        {
                            e.LightingEnabled = true;
                            e.EmissiveColor = new Vector3(0.5f, 0.0f, 0.0f);

                        }
                        else
                        {
                            e.EmissiveColor = Vector3.Zero;
                            e.LightingEnabled = false;
                        }
#endif
//+:cnd:noEmit

                        e.World = world;
                        e.View = RenderPoint;
                        e.Projection = RenderParams;
                        e.EnableDefaultLighting();
                        e.PreferPerPixelLighting = true;

                    }

                    mesh.Draw();

                }
            }
        }


        /// <summary>
        /// Construye una matriz de vista 2D con las transformaciones de la cámara.
        /// <para>Builds a 2D view matrix with camera transforms.</para>
        /// </summary>
        /// <param name="screenCenter">Centro de la pantalla.<para>Screen center.</para></param>
        /// <param name="zoom">Nivel de zoom.<para>Zoom level.</para></param>
        /// <param name="rotation">Rotación en radianes.<para>Rotation in radians.</para></param>
        /// <returns>La matriz de vista.<para>The view matrix.</para></returns>
        public Matrix Get2DViewMatrix(Vector2 screenCenter, float zoom = 1f, float rotation = 0f)
        {
            return
          Matrix.CreateTranslation(new Vector3(-YTBCartessian.Vector3ToVector2(CameraPosition), 0f)) *
          Matrix.CreateRotationZ(rotation) *
          Matrix.CreateScale(zoom, zoom, 1f) *
          Matrix.CreateTranslation(new Vector3(screenCenter, 0f));
        }

        /// <summary>
        /// Construye una matriz de vista 2D con posición y transformaciones personalizadas.
        /// <para>Builds a 2D view matrix with a custom position and transforms.</para>
        /// </summary>
        /// <param name="position">Posición de la cámara 2D.<para>2D camera position.</para></param>
        /// <param name="screenCenter">Centro de la pantalla.<para>Screen center.</para></param>
        /// <param name="zoom">Nivel de zoom.<para>Zoom level.</para></param>
        /// <param name="rotation">Rotación en radianes.<para>Rotation in radians.</para></param>
        /// <returns>La matriz de vista.<para>The view matrix.</para></returns>
        public Matrix Get2DViewMatrix(Vector2 position, Vector2 screenCenter, float zoom = 1f, float rotation = 0f)
        {
            return
          Matrix.CreateTranslation(new Vector3(-position, 0f)) *
          Matrix.CreateRotationZ(rotation) *
          Matrix.CreateScale(zoom, zoom, 1f) *
          Matrix.CreateTranslation(new Vector3(screenCenter, 0f));
        }
    }

}

