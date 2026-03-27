using System;

namespace Benchmarks.Core.Entity
{
    /// <summary>
    /// Clase padre de todas las entidades. En este engine, a las entidades se les llaman "Yotsuba".
    /// </summary>
    public struct Yotsuba(int id)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; }
        public int Components { get; set; } = 0;

        public bool HasComponent(YTBComponent component) => (Components & (int)component) != 0;
        public bool HasNotComponent(YTBComponent component) => !HasComponent(component);
        public void AddComponent(YTBComponent component) => Components |= (int)component;

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
        YTBModel3D = 1 << 14,
    }
}
