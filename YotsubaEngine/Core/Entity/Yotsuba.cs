

using System;

namespace YotsubaEngine.Core.Entity
{
    /// <summary>
    /// Clase padre de todas las entidades.
    /// En este engine, a las entidades se les llaman "Yotsuba".
    /// </summary>
    /// <param name="id">Identificador unico de la entidad</param>
    public class Yotsuba(int id)
    {
        /// <summary>
        /// Identificador unico de la entidad
        /// </summary>
        public int Id { get; set; } = id;

        /// <summary>
        /// Nombre de la entidad (puede ser mostrado en pantalla)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Bitmask que indica los componentes que tiene la entidad.
        /// </summary>
        public int Components { get; set; } = 0;

        /// <summary>
        /// Metodo para verificar si la entidad tiene un componente específico.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool HasComponent(YTBComponent component) => (Components & (int)component) != 0;

        /// <summary>
        /// Metodo para verificar si la entidad no tiene un componente específico.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool HasNotComponent(YTBComponent component) => !HasComponent(component);

        /// <summary>
        /// Metodo para agregar un componente a la entidad.
        /// </summary>
        /// <param name="component"></param>
        public void AddComponent(YTBComponent component) => Components |= (int)component;

        /// <summary>
        /// Metodo para remover un componente de la entidad.
        /// </summary>
        /// <param name="component"></param>
        public void RemoveComponent(YTBComponent component)
        {
            Components &= ~(int)component;
        }
    }

    [Flags]
    public enum YTBComponent : ushort
    {
        Sprite = 1 << 0,
        Transform = 1 << 1,
        Animation = 1 << 2,
        Rigibody = 1 << 3,
        Input = 1 << 4,
        Model3D = 1 << 5,
        Button2D = 1 << 6,
        Camera = 1 << 7,
        Script = 1 << 8,
        TileMap = 1 << 9,
        Font = 1 << 10,
        Shader = 1 << 11,
        YTBUIElement = 1 << 12,
        StorageObjects3D = 1 << 13,
        Object3D = 1 << 14,
    }
}
