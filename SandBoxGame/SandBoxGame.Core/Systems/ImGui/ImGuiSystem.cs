#if !YTB

using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Hexa.NET.ImNodes;
using Hexa.NET.ImPlot;
using System.IO;
using System.Text;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics.ImGuiNet;

namespace SandBoxGame.Core.Systems.ImGui
{
    public class ImGuiSystem : IRenderSystem
    {
        public static ImGuiRenderer GuiRenderer { get; set; }

        public override void InitializeSystem(EntityManager entities)
        {
            GuiRenderer = new ImGuiRenderer(YTBGlobalState.Game);

            if (YTBGlobalState.IsDesktop)
            {

                // ImGui setup (fonts, theme)
                var io = Hexa.NET.ImGui.ImGui.GetIO();
                Hexa.NET.ImGui.ImGui.GetIO().BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
                io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

                const string outputFontsDir = "Fonts";
                string fuentePrincipal = Path.Combine(outputFontsDir, "LibertinusMath-Regular.ttf");
                string fuenteIconos = Path.Combine(outputFontsDir, "NerdFontsSymbolsOnly.ttf");

                unsafe
                {

                    io.Fonts.AddFontFromFileTTF(fuentePrincipal, YTBGlobalState.Platform == Platforms.Avalonia_GL ? 20.0f : 24.0f, null, io.Fonts.GetGlyphRangesDefault());
                    // 1. Cargar la fuente principal (texto)

                    // 2. Crear la configuración para la fuente de íconos
                    ImFontConfigPtr config = Hexa.NET.ImGui.ImGui.ImFontConfig();
                    config.MergeMode = true;
                    config.PixelSnapH = true;

                    // 3. Definir el rango de caracteres para Nerd Fonts.
                    uint[] iconRanges = new uint[]
                    {
                        0xE000, 0xF8FF,
                        0
                    };

                    fixed (uint* rangePtr = iconRanges)
                    {
                        byte[] fuenteBytes = Encoding.UTF8.GetBytes(fuenteIconos);

                        fixed (byte* fb = fuenteBytes)
                        {


                            // Pass rangePtr instead of GetGlyphRangesDefault()
                            io.Fonts.AddFontFromFileTTF(fb, YTBGlobalState.Platform == Platforms.Avalonia_GL ? 20.0f : 24.0f, config, rangePtr);
                        }
                    }

                    config.Destroy();
                }

                io.FontGlobalScale = 1;

                // 6. Construir la textura final (Obligatorio después de añadir fuentes)
                io.Fonts.Build();
            }

            var style = Hexa.NET.ImGui.ImGui.GetStyle();

            if (YTBGlobalState.IsAndroid)
            {
                GuiRenderer.InitNativeBackend();
            }
            else if (!YTBGlobalState.IsIOS)
            {
                GuiRenderer.RebuildFontAtlas();
            }
            var guiContext = Hexa.NET.ImGui.ImGui.GetCurrentContext();

            if (YTBGlobalState.IsDesktop)
            {
                ImPlot.SetImGuiContext(guiContext);
                var plotContext = ImPlot.CreateContext();
                ImPlot.SetCurrentContext(plotContext);

                ImNodes.SetImGuiContext(guiContext);
                var nodesContext = ImNodes.CreateContext();
                ImNodes.SetCurrentContext(nodesContext);
                var editorCtx = ImNodes.EditorContextCreate();
                ImNodes.EditorContextSet(editorCtx);
                ImNodes.StyleColorsDark(ImNodes.GetStyle());

                // Le pasamos el contexto principal de ImGui a ImGuizmo para que sepa dónde dibujar
                ImGuizmo.SetImGuiContext(guiContext);
            }


            base.InitializeSystem(entities);
        }
    }
}
#endif

