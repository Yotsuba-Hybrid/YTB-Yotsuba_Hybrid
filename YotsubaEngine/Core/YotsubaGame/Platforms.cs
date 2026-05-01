using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Core.YotsubaGame
{
    public enum Platforms
    {
        Windows_DX12,
        Windows_WPF_DX12,
        Avalonia_GL,
        Desktop_GL,
        Desktop_VK,
        Android,
        IOS,
        /// <summary>
        /// Plataforma web mediante KNI Blazor WebAssembly (WebGL).
        /// <para>Web platform via KNI Blazor WebAssembly (WebGL).</para>
        /// </summary>
        Web_BlazorGL
    }
}
