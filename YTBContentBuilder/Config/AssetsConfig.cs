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



            contentCollection.Include<WildcardRule>("*");
                

            /// Copy
            //contentCollection.IncludeCopy<RegexRule>(".txt");
            contentCollection.IncludeCopy<RegexRule>(".ytb");  // Archivos del engine
            contentCollection.IncludeCopy<RegexRule>(".ttf");
            contentCollection.IncludeCopy<RegexRule>(".cs");   // Scripts del juego
            contentCollection.IncludeCopy<RegexRule>(".tmx");  // Mapas de Tiled
            contentCollection.IncludeCopy<RegexRule>(".xml");  // Mapas de Tiled



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
