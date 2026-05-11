#if YTB
namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Datos de subtextura parseados desde un XML de atlas.
    /// </summary>
    public class SubtextureInfo
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>
    /// Datos de animación parseados desde un XML de atlas.
    /// </summary>
    public class AnimationInfo
    {
        public string Name { get; set; }
        public int Delay { get; set; }
    }
}
#endif
