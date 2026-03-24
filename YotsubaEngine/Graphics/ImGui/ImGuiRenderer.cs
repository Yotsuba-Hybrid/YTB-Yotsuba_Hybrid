using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.ImNodes;
using Hexa.NET.ImPlot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



namespace YotsubaEngine.Graphics.ImGuiNet
{
    /// <summary>
    /// Renderizador de ImGui para MonoGame.
    /// <para>ImGui renderer for MonoGame.</para>
    /// </summary>
    /// <remarks>
    /// Autores originales del paquete MonoGame.ImGuiNet.
    /// <para>Original authors of the MonoGame.ImGuiNet package.</para>
    /// Author("Package MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://contrib.rocks/image?repo=Mezo-hx/MonoGame.ImGuiNet", "09/2025"),
    /// Author("https://github.com/Mezo-hx/MonoGame.ImGuiNet/graphs/contributors", "09/2025")
    /// </remarks>
    public class ImGuiRenderer
    {
        private Game _game;

        // Graphics
        private GraphicsDevice _graphicsDevice;

        private BasicEffect _effect;
        private RasterizerState _rasterizerState;

        private byte[] _vertexData;
        private VertexBuffer _vertexBuffer;
        private int _vertexBufferSize;

        private byte[] _indexData;
        private IndexBuffer _indexBuffer;
        private int _indexBufferSize;

        // Textures
        private Dictionary<ImTextureID, Texture2D> _loadedTextures = new Dictionary<ImTextureID, Texture2D>();

        private int _textureId;
        private ImTextureID _fontTextureId;

        // Input
        private int _scrollWheelValue;
        private int _horizontalScrollWheelValue;
        private readonly float WHEEL_DELTA = 120;
        private Keys[] _allKeys = Enum.GetValues<Keys>();

        // Native backend (Android)
        private bool _useNativeBackend;

        /// <summary>
        /// Inicializa el renderizador de ImGui.
        /// <para>Initializes the ImGui renderer.</para>
        /// </summary>
        /// <param name="game">Instancia del juego. <para>Game instance.</para></param>
        public ImGuiRenderer(Game game)
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            _game = game ?? throw new ArgumentNullException(nameof(game));
            _graphicsDevice = game.GraphicsDevice;

            _loadedTextures = new Dictionary<ImTextureID, Texture2D>();

            _rasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            SetupInput();
        }

        #region ImGuiRenderer

        /// <summary>
        /// Crea una textura y carga los datos de fuentes de ImGui; debe llamarse antes de renderizar.
        /// <para>Creates a texture and loads ImGui font data; call before rendering.</para>
        /// </summary>
        public virtual unsafe void RebuildFontAtlas()
        {
            // Get font texture from ImGui
            var io = ImGui.GetIO();
            byte* pixelData;
            int width, height, bytesPerPixel;
            io.Fonts.GetTexDataAsRGBA32(&pixelData, &width, &height, &bytesPerPixel);

            // Copy the data to a managed array
            var pixels = new byte[width * height * bytesPerPixel];
            Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

            // Create and register the texture as an XNA texture
            var tex2d = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
            tex2d.SetData(pixels);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (!_fontTextureId.IsNull) UnbindTexture(_fontTextureId);

            // Bind the new texture to an ImGui-friendly id
            _fontTextureId = BindTexture(tex2d);

            // Let ImGui know where to find the texture
            io.Fonts.SetTexID(_fontTextureId);
            io.Fonts.ClearTexData();
        }

        /// <summary>
        /// Initializes the native ImGui OpenGL3 backend for Android rendering.
        /// Must be called after fonts are configured but instead of RebuildFontAtlas().
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InitNativeBackend()
        {
            _useNativeBackend = true;
            var context = ImGui.GetCurrentContext();
            ImGuiImplOpenGL3.SetCurrentContext(context);
            ImGuiImplOpenGL3.Init("#version 300 es");
            ImGuiImplOpenGL3.CreateFontsTexture();
        }

        /// <summary>
        /// Crea un puntero a una textura para usarlo con ImGui (por ejemplo, <see cref="ImGui.Image" />).
        /// <para>Creates a texture pointer for ImGui calls (for example, <see cref="ImGui.Image" />).</para>
        /// </summary>
        /// <param name="texture">Textura a registrar. <para>Texture to register.</para></param>
        /// <returns>Identificador de textura para ImGui. <para>Texture identifier for ImGui.</para></returns>
        public virtual ImTextureID BindTexture(Texture2D texture)
        {
            var id = new ImTextureID(_textureId++);

            _loadedTextures.Add(id, texture);

            return id;
        }

        /// <summary>
        /// Elimina un puntero de textura creado previamente y libera la referencia.
        /// <para>Removes a previously created texture pointer and releases its reference.</para>
        /// </summary>
        /// <param name="textureId">Identificador de textura a liberar. <para>Texture identifier to unbind.</para></param>
        public virtual void UnbindTexture(ImTextureID textureId)
        {
            _loadedTextures.Remove(textureId);
        }

        /// <summary>
        /// Configura ImGui para un nuevo frame; debe llamarse al inicio del frame.
        /// <para>Sets up ImGui for a new frame; call at frame start.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public virtual void BeginLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateInput();

            if (_useNativeBackend)
            {
                // Each native .so (cimgui, cimnodes, cimplot, libImGuiImpl) has ImGui
                // statically linked with its own GImGui global. Re-sync every frame
                // to ensure all libraries point to the same context.
                var ctx = ImGui.GetCurrentContext();
                ImGuiImplOpenGL3.SetCurrentContext(ctx);
                ImNodes.SetImGuiContext(ctx);
                ImPlot.SetImGuiContext(ctx);
                ImGuiImplOpenGL3.NewFrame();
            }
            ImGui.NewFrame();
        }

        /// <summary>
        /// Envía la geometría generada por ImGui al pipeline gráfico; llamar tras dibujar la UI.
        /// <para>Sends ImGui generated geometry to the graphics pipeline; call after drawing the UI.</para>
        /// </summary>
        public virtual void EndLayout()
        {
            ImGui.Render();

            if (_useNativeBackend)
            {
                ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());
            }
            else
            {
                unsafe { RenderDrawData(ImGui.GetDrawData()); }
            }
        }

        #endregion ImGuiRenderer

        #region Setup & Update

        /// <summary>
        /// Setup key input event handler.
        /// </summary>
        protected virtual void SetupInput()
        {
            if (!OperatingSystem.IsAndroid())
            {
                SetupDesktopTextInput();
            }
        }

        // Isolated in a separate method so the JIT never resolves the TextInput
        // member on Android, where GameWindow.TextInput does not exist.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SetupDesktopTextInput()
        {
            var io = ImGui.GetIO();
            _game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t') return;
                io.AddInputCharacter(a.Character);
            };
        }

        /// <summary>
        /// Updates the <see cref="Effect" /> to the current matrices and texture
        /// </summary>
        protected virtual Effect UpdateEffect(Texture2D texture)
        {
            _effect = _effect ?? new BasicEffect(_graphicsDevice);

            var io = ImGui.GetIO();

            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0f, -1f, 1f);
            _effect.TextureEnabled = true;
            _effect.Texture = texture;
            _effect.VertexColorEnabled = true;

            return _effect;
        }

        /// <summary>
        /// Sends XNA input state to ImGui
        /// </summary>
        protected virtual void UpdateInput()
        {
            var io = ImGui.GetIO();

            // Always set display size — must not be gated by IsActive
            io.DisplaySize = new System.Numerics.Vector2(
                _graphicsDevice.PresentationParameters.BackBufferWidth,
                _graphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

            if (!_game.IsActive) return;

            if (OperatingSystem.IsAndroid())
            {
                UpdateAndroidInput(io);
            }
            else
            {
                UpdateDesktopInput(io);
            }
        }

        private void UpdateDesktopInput(ImGuiIOPtr io)
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            io.AddMousePosEvent(mouse.X, mouse.Y);
            io.AddMouseButtonEvent(0, mouse.LeftButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(1, mouse.RightButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(2, mouse.MiddleButton == ButtonState.Pressed);
            io.AddMouseButtonEvent(3, mouse.XButton1 == ButtonState.Pressed);
            io.AddMouseButtonEvent(4, mouse.XButton2 == ButtonState.Pressed);

            io.AddMouseWheelEvent(
                (mouse.HorizontalScrollWheelValue - _horizontalScrollWheelValue) / WHEEL_DELTA,
                (mouse.ScrollWheelValue - _scrollWheelValue) / WHEEL_DELTA);
            _scrollWheelValue = mouse.ScrollWheelValue;
            _horizontalScrollWheelValue = mouse.HorizontalScrollWheelValue;

            foreach (var key in _allKeys)
            {
                if (TryMapKeys(key, out ImGuiKey imguikey))
                {
                    io.AddKeyEvent(imguikey, keyboard.IsKeyDown(key));
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void UpdateAndroidInput(ImGuiIOPtr io)
        {
            var touchState = TouchPanel.GetState();

            if (touchState.Count > 0)
            {
                var touch = touchState[0];
                io.AddMousePosEvent(touch.Position.X, touch.Position.Y);

                bool isPressed = touch.State == TouchLocationState.Pressed
                              || touch.State == TouchLocationState.Moved;
                io.AddMouseButtonEvent(0, isPressed);
            }
            else
            {
                io.AddMouseButtonEvent(0, false);
            }
        }

        private bool TryMapKeys(Keys key, out ImGuiKey imguikey)
        {
            //Special case not handed in the switch...
            //If the actual key we put in is "None", return none and true. 
            //otherwise, return none and false.
            if (key == Keys.None)
            {
                imguikey = ImGuiKey.None;
                return true;
            }

            imguikey = key switch
            {
                Keys.Back => ImGuiKey.Backspace,
                Keys.Tab => ImGuiKey.Tab,
                Keys.Enter => ImGuiKey.Enter,
                Keys.CapsLock => ImGuiKey.CapsLock,
                Keys.Escape => ImGuiKey.Escape,
                Keys.Space => ImGuiKey.Space,
                Keys.PageUp => ImGuiKey.PageUp,
                Keys.PageDown => ImGuiKey.PageDown,
                Keys.End => ImGuiKey.End,
                Keys.Home => ImGuiKey.Home,
                Keys.Left => ImGuiKey.LeftArrow,
                Keys.Right => ImGuiKey.RightArrow,
                Keys.Up => ImGuiKey.UpArrow,
                Keys.Down => ImGuiKey.DownArrow,
                Keys.PrintScreen => ImGuiKey.PrintScreen,
                Keys.Insert => ImGuiKey.Insert,
                Keys.Delete => ImGuiKey.Delete,
                >= Keys.D0 and <= Keys.D9 => ImGuiKey.Key0 + (key - Keys.D0),
                >= Keys.A and <= Keys.Z => ImGuiKey.A + (key - Keys.A),
                >= Keys.NumPad0 and <= Keys.NumPad9 => ImGuiKey.Keypad0 + (key - Keys.NumPad0),
                Keys.Multiply => ImGuiKey.KeypadMultiply,
                Keys.Add => ImGuiKey.KeypadAdd,
                Keys.Subtract => ImGuiKey.KeypadSubtract,
                Keys.Decimal => ImGuiKey.KeypadDecimal,
                Keys.Divide => ImGuiKey.KeypadDivide,
                >= Keys.F1 and <= Keys.F12 => ImGuiKey.F1 + (key - Keys.F1),
                Keys.NumLock => ImGuiKey.NumLock,
                Keys.Scroll => ImGuiKey.ScrollLock,
                Keys.LeftShift => ImGuiKey.ModShift,
                Keys.LeftControl => ImGuiKey.ModCtrl,
                Keys.LeftAlt => ImGuiKey.ModAlt,
                Keys.OemSemicolon => ImGuiKey.Semicolon,
                Keys.OemPlus => ImGuiKey.Equal,
                Keys.OemComma => ImGuiKey.Comma,
                Keys.OemMinus => ImGuiKey.Minus,
                Keys.OemPeriod => ImGuiKey.Period,
                Keys.OemQuestion => ImGuiKey.Slash,
                Keys.OemTilde => ImGuiKey.GraveAccent,
                Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
                Keys.OemCloseBrackets => ImGuiKey.RightBracket,
                Keys.OemPipe => ImGuiKey.Backslash,
                Keys.OemQuotes => ImGuiKey.Apostrophe,
                _ => ImGuiKey.None,
            };

            return imguikey != ImGuiKey.None;
        }

        #endregion Setup & Update

        #region Internals

        /// <summary>
        /// Gets the geometry as set up by ImGui and sends it to the graphics device
        /// </summary>
        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
            var lastViewport = _graphicsDevice.Viewport;
            var lastScissorBox = _graphicsDevice.ScissorRectangle;

            _graphicsDevice.BlendFactor = Color.White;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            // Setup projection
            _graphicsDevice.Viewport = new Viewport(0, 0, _graphicsDevice.PresentationParameters.BackBufferWidth, _graphicsDevice.PresentationParameters.BackBufferHeight);

            UpdateBuffers(drawData);

            RenderCommandLists(drawData);

            // Restore modified state
            _graphicsDevice.Viewport = lastViewport;
            _graphicsDevice.ScissorRectangle = lastScissorBox;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            // Expand buffers if we need more room
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                _vertexBuffer?.Dispose();

                _vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                _vertexBuffer = new VertexBuffer(_graphicsDevice, DrawVertDeclaration.Declaration, _vertexBufferSize, BufferUsage.None);
                _vertexData = new byte[_vertexBufferSize * DrawVertDeclaration.Size];
            }

            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                _indexBuffer?.Dispose();

                _indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, _indexBufferSize, BufferUsage.None);
                _indexData = new byte[_indexBufferSize * sizeof(ushort)];
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays.
            // Index values are pre-offset by vtxOffset so that baseVertex is not needed
            // at draw time — OpenGL ES < 3.2 does not support glDrawElementsBaseVertex.
            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdLists[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset * DrawVertDeclaration.Size])
                {
                    Buffer.MemoryCopy(cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.Size);
                }

                fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
                {
                    ushort* srcPtr = (ushort*)cmdList.IdxBuffer.Data;
                    ushort* dstPtr = (ushort*)idxDstPtr;
                    for (int i = 0; i < cmdList.IdxBuffer.Size; i++)
                    {
                        dstPtr[i] = (ushort)(srcPtr[i] + vtxOffset);
                    }
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            _vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * DrawVertDeclaration.Size);
            _indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
        {
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;

            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdLists[n];

                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ref ImDrawCmd drawCmd = ref cmdList.CmdBuffer.Data[cmdi];

                    if (drawCmd.ElemCount == 0)
                    {
                        continue;
                    }

                    if (!_loadedTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }

                    _graphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                    );

                    var effect = UpdateEffect(_loadedTextures[drawCmd.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        _graphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            (int)drawCmd.VtxOffset,
                            (int)drawCmd.IdxOffset + idxOffset,
                            (int)drawCmd.ElemCount / 3
                        );
                    }
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }
        }

        #endregion Internals
    }
}
