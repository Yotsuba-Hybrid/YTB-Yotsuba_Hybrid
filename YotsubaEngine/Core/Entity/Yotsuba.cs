

using System;

namespace YotsubaEngine.Core.Entity
{
    /// <summary>
    /// Clase padre de todas las entidades. En este engine, a las entidades se les llaman "Yotsuba".
    /// <para>Base class for all entities. In this engine, entities are called "Yotsuba".</para>
    /// </summary>
    /// <param name="id">Identificador único de la entidad.<para>Unique entity identifier.</para></param>
    public struct Yotsuba(int id)
    {
        /// <summary>
        /// Identificador único de la entidad.
        /// <para>Unique entity identifier.</para>
        /// </summary>
        public int Id { get; set; } = id;

        /// <summary>
        /// Nombre de la entidad (puede ser mostrado en pantalla).
        /// <para>Entity name (can be displayed on screen).</para>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Bitmask que indica los componentes que tiene la entidad.
        /// <para>Bitmask indicating which components the entity has.</para>
        /// </summary>
        public int Components { get; set; } = 0;

        /// <summary>
        /// Método para verificar si la entidad tiene un componente específico.
        /// <para>Checks whether the entity has a specific component.</para>
        /// </summary>
        /// <param name="component">Componente a comprobar.<para>Component to check.</para></param>
        /// <returns>True si la entidad tiene el componente.<para>True if the entity has the component.</para></returns>
        public bool HasComponent(YTBComponent component) => (Components & (int)component) != 0;

        /// <summary>
        /// Método para verificar si la entidad no tiene un componente específico.
        /// <para>Checks whether the entity does not have a specific component.</para>
        /// </summary>
        /// <param name="component">Componente a comprobar.<para>Component to check.</para></param>
        /// <returns>True si la entidad no tiene el componente.<para>True if the entity does not have the component.</para></returns>
        public bool HasNotComponent(YTBComponent component) => !HasComponent(component);

        /// <summary>
        /// Método para agregar un componente a la entidad.
        /// <para>Adds a component to the entity.</para>
        /// </summary>
        /// <param name="component">Componente a agregar.<para>Component to add.</para></param>
        public void AddComponent(YTBComponent component) => Components |= (int)component;

        /// <summary>
        /// Método para remover un componente de la entidad.
        /// <para>Removes a component from the entity.</para>
        /// </summary>
        /// <param name="component">Componente a remover.<para>Component to remove.</para></param>
        public void RemoveComponent(YTBComponent component)
        {
            Components &= ~(int)component;
        }
    }

    /// <summary>
    /// Enumeración de componentes que puede tener una entidad.
    /// <para>Enumeration of components an entity can have.</para>
    /// </summary>
    [Flags]
    public enum YTBComponent : ushort
    {
        /// <summary>
        /// Componente de sprite.
        /// <para>Sprite component.</para>
        /// </summary>
        Sprite = 1 << 0,
        /// <summary>
        /// Componente de transformación.
        /// <para>Transform component.</para>
        /// </summary>
        Transform = 1 << 1,
        /// <summary>
        /// Componente de animación.
        /// <para>Animation component.</para>
        /// </summary>
        Animation = 1 << 2,
        /// <summary>
        /// Componente de cuerpo rígido.
        /// <para>Rigid body component.</para>
        /// </summary>
        Rigibody = 1 << 3,
        /// <summary>
        /// Componente de entrada.
        /// <para>Input component.</para>
        /// </summary>
        Input = 1 << 4,
        /// <summary>
        /// Componente de modelo 3D.
        /// <para>3D model component.</para>
        /// </summary>
        Model3D = 1 << 5,
        /// <summary>
        /// Componente de botón 2D.
        /// <para>2D button component.</para>
        /// </summary>
        Button2D = 1 << 6,
        /// <summary>
        /// Componente de cámara.
        /// <para>Camera component.</para>
        /// </summary>
        Camera = 1 << 7,
        /// <summary>
        /// Componente de scripting.
        /// <para>Scripting component.</para>
        /// </summary>
        Script = 1 << 8,
        /// <summary>
        /// Componente de mapa de tiles.
        /// <para>Tile map component.</para>
        /// </summary>
        TileMap = 1 << 9,
        /// <summary>
        /// Componente de fuente.
        /// <para>Font component.</para>
        /// </summary>
        Font = 1 << 10,
        /// <summary>
        /// Componente de shader.
        /// <para>Shader component.</para>
        /// </summary>
        Shader = 1 << 11,
        /// <summary>
        /// Componente de interfaz YTB.
        /// <para>YTB UI element component.</para>
        /// </summary>
        YTBUIElement = 1 << 12,
        /// <summary>
        /// Componente de almacenamiento de objetos 3D.
        /// <para>3D object storage component.</para>
        /// </summary>
        StorageObjects3D = 1 << 13,
        /// <summary>
        /// Componente de objeto 3D.
        /// <para>3D object component.</para>
        /// </summary>
        Object3D = 1 << 14,
    }
}
