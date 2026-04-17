using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine_3D.Components
{
    public struct ModelComponent3D
    {
        private Model _model { get; set; }

        // Arrays para manejar los datos de los huesos de forma segura
        private Matrix[] _bindPoseOriginal;     // Las poses puras (solo lectura)
        private Matrix[] _transformacionesActuales; // Las transformaciones del frame actual
        private Matrix[] _matricesAbsolutas;    // El resultado final para la GPU

        private int _indiceBrazo;
        public ModelComponent3D(Model model)
        {
            _model = model;
            int cantidadHuesos = _model.Bones.Count;

            // 1. Inicializar los arrays para no generar basura (Garbage Collection) en el Update
            _bindPoseOriginal = new Matrix[cantidadHuesos];
            _transformacionesActuales = new Matrix[cantidadHuesos];
            _matricesAbsolutas = new Matrix[cantidadHuesos];

            _model.CopyBoneTransformsTo(_bindPoseOriginal);

            // 3. Clonar la pose inicial a nuestras transformaciones actuales
            _bindPoseOriginal.CopyTo(_transformacionesActuales, 0);

            // 4. Cachear el índice del hueso para no buscar por string en el Update
            _indiceBrazo = _model.Bones["Arm_R"].Index;
        }


        public void Update(GameTime gameTime)
        {
            float tiempo = (float)gameTime.TotalGameTime.TotalSeconds;

            // --- LA MATEMÁTICA PROCEDURAL ---
            // Vamos a hacer que el brazo salude oscilando como un péndulo usando una onda senoidal.
            // Math.Sin devuelve un valor entre -1 y 1. Lo multiplicamos por 1.5 para mayor amplitud.
            float anguloRotacion = (float)Math.Sin(tiempo * 5f) * 1.5f;

            // 1. Obtener la matriz local intacta de este hueso
            Matrix matrizOriginal = _bindPoseOriginal[_indiceBrazo];

            // 2. DESCOMPOSICIÓN TRS (Vital para no deformar la malla)
            Vector3 escalaOriginal;
            Quaternion rotacionOriginal;
            Vector3 posicionOriginal;
            matrizOriginal.Decompose(out escalaOriginal, out rotacionOriginal, out posicionOriginal);

            // 3. Crear la nueva rotación (ej. rotar en el eje Z local del hombro)
            Quaternion rotacionProcedural = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, anguloRotacion);

            // 4. Combinar las rotaciones (En Quaternions, esto se hace multiplicando)
            // OJO: El orden de multiplicación importa. 
            Quaternion rotacionFinal = rotacionOriginal * rotacionProcedural;

            // 5. RECONSTRUIR LA MATRIZ: Escala -> Rotación -> Traslación
            _transformacionesActuales[_indiceBrazo] =
                Matrix.CreateScale(escalaOriginal) * Matrix.CreateFromQuaternion(rotacionFinal) * Matrix.CreateTranslation(posicionOriginal);

            // 6. Inyectar todas las matrices modificadas de vuelta al modelo
            _model.CopyBoneTransformsFrom(_transformacionesActuales);
        }

        public void Draw(Matrix view, Matrix projection, Matrix worldJugador)
        {
            // 7. Calcular las matrices globales (Cinemática Directa) basándose en las nuevas poses
            _model.CopyAbsoluteBoneTransformsTo(_matricesAbsolutas);

            // 8. Dibujar el modelo (Bucle experto)
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // Multiplicamos la matriz global del hueso por la matriz del mundo de tu entidad
                    effect.World = _matricesAbsolutas[mesh.ParentBone.Index] * worldJugador;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }

}
