#if YTB
using System;
using System.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Exceptions;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    public partial struct ScriptComponent
    {
        /// <summary>
        /// Render UI editor para "Scripts": combo con todos los scripts registrados.
        /// Mantiene el formato compuesto "CSHARP&amp;:&amp;route&amp;;&amp;".
        /// </summary>
        public static void RenderScriptsUI(IUIRenderContext ctx, string raw)
        {
            try
            {
                string mainScript = (raw ?? "").Split("&;&")[0];
                string scriptType = mainScript.Contains("&:&") ? mainScript.Split("&:&")[0] : "CSHARP";

                if (!Enum.TryParse(typeof(ScriptComponentType), scriptType, out var _))
                    return;

                string scriptPath = mainScript.Contains("&:&") && mainScript.Split("&:&").Length > 1
                    ? mainScript.Split("&:&")[1]
                    : "";

                var scripts = ctx.AllScripts.ToArray();
                int indexOfPredet = string.IsNullOrEmpty(scriptPath)
                    ? 0
                    : Array.IndexOf(scripts, scriptPath);
                if (indexOfPredet < 0) indexOfPredet = 0;

                if (ImGui.Combo("Guiones##Scripts", ref indexOfPredet, scripts, scripts.Length))
                {
                    ctx.UpdateProperty("Scripts", scriptType + "&:&" + scripts[indexOfPredet] + "&;&");
                }
            }
            catch (Exception ex)
            {
                new GameWontRun(ex, GameWontRun.YTBErrors.ScriptHasError);
            }
        }
    }
}
#endif
