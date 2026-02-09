using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Effect;
using MonoGame.Framework.Content.Pipeline.Builder;


namespace YotsubaEngine.YTBContentBuilder
{
    public class AssetsConfig 
    {
        public static void Apply(ContentCollection contentCollection)
        {

            

            /// Models
            contentCollection.Include<RegexRule>(".fbx");
            contentCollection.Include<RegexRule>(".obj");
            contentCollection.Include<RegexRule>(".x");

            /// Images
            contentCollection.Include<RegexRule>(".png");
            contentCollection.Include<RegexRule>(".jpg");
            contentCollection.Include<RegexRule>(".jpeg");
            contentCollection.Include<RegexRule>(".bmp");

            /// Audio
            contentCollection.Include<RegexRule>(".wav");
            contentCollection.Include<RegexRule>(".mp3");
            contentCollection.Include<RegexRule>(".ogg");

            ///Video
            contentCollection.Include<RegexRule>(".mp4");

            /// Text
            contentCollection.Include<RegexRule>(".spritefont");
            contentCollection.IncludeCopy<RegexRule>(".ttf");
            contentCollection.IncludeCopy<RegexRule>(".xml");

            /// Copy
            //contentCollection.IncludeCopy<RegexRule>(".txt");
            contentCollection.IncludeCopy<RegexRule>(".ytb");  // Archivos del engine
            contentCollection.IncludeCopy<RegexRule>(".ttf");
            contentCollection.IncludeCopy<RegexRule>(".cs");   // Scripts del juego
            contentCollection.IncludeCopy<RegexRule>(".tmx");  // Mapas de Tiled

            /// Effects
            contentCollection.Include<RegexRule>(".fx");

            // exclude bin / obj paths
            contentCollection.Exclude<RegexRule>("bin/");
            contentCollection.Exclude<RegexRule>("obj/");
            contentCollection.Exclude<WildcardRule>("*.mgcb");
            contentCollection.Exclude<WildcardRule>("*.contentproj");
            contentCollection.Exclude<WildcardRule>("*.xnb");
            contentCollection.Exclude<WildcardRule>("*.mgcontent");
        }
    }

}
