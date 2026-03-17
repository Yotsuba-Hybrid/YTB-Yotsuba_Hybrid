// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiTypes.cs – Tipos fundamentales de valor para el pipeline de UI.
//
// DECISIONES DE RENDIMIENTO:
// • Todos los tipos son structs para garantizar localidad de caché y evitar
//   presión sobre el GC.  Se usan StructLayout.Sequential para que la CPU
//   pueda leer campos contiguos en una sola línea de caché (64 bytes).
// • Los operadores aritméticos están marcados AggressiveInlining para que el
//   JIT los emita como instrucciones escalares en lugar de CALL.
// • GuiRect usa campos públicos en lugar de propiedades para eliminar la
//   indirección de get/set en bucles de layout hot-path.
// ============================================================================

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.IMGUI
{
    // ========================================================================
    // GuiId – Identificador numérico de 32 bits para controles IMGUI.
    // Envuelve un uint producido por FNV-1a.  El wrapper evita confusiones
    // entre IDs de la UI y otros enteros del motor.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct GuiId : IEquatable<GuiId>
    {
        public static readonly GuiId None = new(0);

        public readonly uint Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiId(uint value) => Value = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GuiId other) => Value == other.Value;

        public override bool Equals(object? obj) => obj is GuiId other && Equals(other);
        public override int GetHashCode() => (int)Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GuiId a, GuiId b) => a.Value == b.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GuiId a, GuiId b) => a.Value != b.Value;

        public override string ToString() => $"GuiId(0x{Value:X8})";
    }

    // ========================================================================
    // GuiRect – Rectángulo axis-aligned definido por posición + tamaño.
    //
    // Usa campos públicos (no propiedades) para que el JIT pueda acceder
    // directamente al offset del campo sin indirección.  En un tight loop de
    // layout esto marca diferencia medible.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct GuiRect
    {
        public float X;
        public float Y;
        public float W;
        public float H;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiRect(float x, float y, float w, float h)
        {
            X = x; Y = y; W = w; H = h;
        }

        public float Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => X + W;
        }

        public float Bottom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Y + H;
        }

        /// <summary>
        /// Test de contención punto-en-rectángulo.  Sin ramificaciones usando
        /// aritmética booleana: cada comparación devuelve 0/1 y se combinan con AND.
        /// El JIT de .NET 8+ suele convertir esto en CMOVcc o SETCC evitando branch mispredicts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(float px, float py)
        {
            // Forma branchless: el JIT emite SETcc + AND para cada condición
            return (px >= X) & (py >= Y) & (px < X + W) & (py < Y + H);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(Vector2 p) => Contains(p.X, p.Y);

        /// <summary>
        /// Expande (o contrae con valores negativos) el rect por un padding uniforme.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GuiRect Expand(float padding)
        {
            return new GuiRect(X - padding, Y - padding, W + padding * 2f, H + padding * 2f);
        }

        /// <summary>
        /// Devuelve la intersección de dos rectángulos (clip region).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly GuiRect Intersect(GuiRect other)
        {
            float x1 = MathF.Max(X, other.X);
            float y1 = MathF.Max(Y, other.Y);
            float x2 = MathF.Min(Right, other.Right);
            float y2 = MathF.Min(Bottom, other.Bottom);
            float w = MathF.Max(0, x2 - x1);
            float h = MathF.Max(0, y2 - y1);
            return new GuiRect(x1, y1, w, h);
        }

        public override readonly string ToString() => $"Rect({X},{Y},{W},{H})";
    }

    // ========================================================================
    // GuiColor32 – Color empaquetado en 32 bits (ABGR nativo de MonoGame).
    //
    // Almacena el color como uint para copias rápidas a vertex buffers.
    // La conversión desde/hacia Microsoft.Xna.Framework.Color es inline.
    // ========================================================================
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public readonly struct GuiColor32 : IEquatable<GuiColor32>
    {
        [FieldOffset(0)] public readonly uint PackedValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiColor32(uint packed) => PackedValue = packed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiColor32(byte r, byte g, byte b, byte a = 255)
        {
            // MonoGame Color usa orden ABGR internamente.
            PackedValue = (uint)(r | (g << 8) | (b << 16) | (a << 24));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ToXnaColor() => new(PackedValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiColor32 FromXna(Color c) => new(c.PackedValue);

        /// <summary>
        /// Interpolación lineal entre dos colores, componente a componente.
        /// Usa desempaquetado entero para evitar conversiones float.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiColor32 Lerp(GuiColor32 a, GuiColor32 b, float t)
        {
            uint ta = (uint)(t * 255f);
            uint ita = 255u - ta;
            uint r = ((a.PackedValue & 0xFF) * ita + (b.PackedValue & 0xFF) * ta) >> 8;
            uint g = (((a.PackedValue >> 8) & 0xFF) * ita + ((b.PackedValue >> 8) & 0xFF) * ta) >> 8;
            uint bl = (((a.PackedValue >> 16) & 0xFF) * ita + ((b.PackedValue >> 16) & 0xFF) * ta) >> 8;
            uint al = (((a.PackedValue >> 24) & 0xFF) * ita + ((b.PackedValue >> 24) & 0xFF) * ta) >> 8;
            return new GuiColor32((uint)(r | (g << 8) | (bl << 16) | (al << 24)));
        }

        public bool Equals(GuiColor32 other) => PackedValue == other.PackedValue;
        public override bool Equals(object? obj) => obj is GuiColor32 o && Equals(o);
        public override int GetHashCode() => (int)PackedValue;

        // ---- Colores predefinidos usados frecuentemente por el tema por defecto ----
        public static readonly GuiColor32 White = new(255, 255, 255, 255);
        public static readonly GuiColor32 Black = new(0, 0, 0, 255);
        public static readonly GuiColor32 Transparent = new(0, 0, 0, 0);
    }

    // ========================================================================
    // GuiDrawVert – Vértice individual compatible con VertexPositionColorTexture.
    //
    // Usa exactamente la misma disposición de memoria que VPCT de MonoGame
    // (Position Vector3 + Color + TexCoord Vector2 = 24 bytes) para permitir
    // copia directa al DynamicVertexBuffer sin conversión.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GuiDrawVert
    {
        public Vector3 Position;   // 12 bytes
        public Color Color;        // 4  bytes
        public Vector2 TexCoord;   // 8  bytes
        // Total: 24 bytes – coincide con VertexPositionColorTexture

        /// <summary>
        /// VertexDeclaration compartida.  Se crea una sola vez y se reutiliza
        /// en todas las llamadas de dibujo, evitando asignaciones por frame.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration = VertexPositionColorTexture.VertexDeclaration;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiDrawVert(float x, float y, Color color, float u, float v)
        {
            Position = new Vector3(x, y, 0f);
            Color = color;
            TexCoord = new Vector2(u, v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GuiDrawVert(Vector2 pos, Color color, Vector2 uv)
        {
            Position = new Vector3(pos, 0f);
            Color = color;
            TexCoord = uv;
        }
    }

    // ========================================================================
    // GuiDrawCmd – Comando de dibujado individual dentro de un DrawList.
    //
    // Cada comando describe un rango de triángulos en los buffers de
    // vértices/índices y la textura asociada.  Esto permite al renderer
    // agrupar (batch) llamadas con la misma textura y minimizar cambios
    // de estado en la GPU.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct GuiDrawCmd
    {
        /// <summary>Número de índices que abarca este comando (siempre múltiplo de 3).</summary>
        public int IndexCount;

        /// <summary>Offset dentro del index buffer global del DrawList.</summary>
        public int IndexOffset;

        /// <summary>Offset dentro del vertex buffer global del DrawList.</summary>
        public int VertexOffset;

        /// <summary>Textura a bindear para este batch. null = textura blanca 1×1.</summary>
        public Texture2D? Texture;

        /// <summary>Rectángulo de clip en coordenadas de pantalla.</summary>
        public GuiRect ClipRect;
    }

    // ========================================================================
    // GuiLayoutEntry – Entrada en el stack de layout.
    //
    // Cada Window/Panel/Group empuja una entrada al stack.  Los controles
    // hijo consumen espacio desde CursorY y avanzan el cursor según su alto.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct GuiLayoutEntry
    {
        /// <summary>Región completa disponible para este contenedor.</summary>
        public GuiRect Bounds;

        /// <summary>Posición X actual del cursor de layout (margen izquierdo + indent).</summary>
        public float CursorX;

        /// <summary>Posición Y actual del cursor de layout (avanza hacia abajo).</summary>
        public float CursorY;

        /// <summary>Ancho disponible para controles hijos.</summary>
        public float ContentWidth;

        /// <summary>Padding interno del contenedor.</summary>
        public float Padding;

        /// <summary>Espacio vertical entre controles consecutivos.</summary>
        public float ItemSpacing;

        /// <summary>Nivel de indentación actual (en píxeles).</summary>
        public float IndentX;

        /// <summary>ID del contenedor propietario (para resolución jerárquica de IDs).</summary>
        public GuiId OwnerId;
    }

    // ========================================================================
    // GuiMouseState / GuiInputState – Estado de entrada snapshot por frame.
    //
    // Se captura al inicio de cada frame y permanece inmutable durante todo
    // el procesamiento de la UI, garantizando coherencia temporal.
    // ========================================================================
    [StructLayout(LayoutKind.Sequential)]
    public struct GuiMouseState
    {
        public Vector2 Position;
        public bool LeftDown;
        public bool LeftPressed;    // Transición: no-pulsado → pulsado este frame
        public bool LeftReleased;   // Transición: pulsado → no-pulsado este frame
        public bool RightDown;
        public bool RightPressed;
        public bool RightReleased;
        public float ScrollDelta;   // Delta de rueda (normalizado)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GuiKeyboardState
    {
        /// <summary>Buffer circular de caracteres escritos este frame (para TextInput).</summary>
        public unsafe fixed char TypedChars[32];
        public int TypedCharCount;

        public bool BackspacePressed;
        public bool DeletePressed;
        public bool EnterPressed;
        public bool TabPressed;
        public bool EscapePressed;
        public bool LeftArrowPressed;
        public bool RightArrowPressed;
        public bool HomePressed;
        public bool EndPressed;
        public bool CtrlDown;
        public bool ShiftDown;

        /// <summary>
        /// Añade un carácter al buffer de tecleo.  No aloca: escribe en el buffer fijo.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushChar(char c)
        {
            if (TypedCharCount < 32)
            {
                fixed (char* buf = TypedChars)
                {
                    buf[TypedCharCount++] = c;
                }
            }
        }
    }

    // ========================================================================
    // GuiStyle – Tema visual global.  Valores por defecto inspirados en
    // Dear ImGui "Dark" theme.  Todos los campos son públicos para permitir
    // personalización directa sin overhead de propiedades.
    // ========================================================================
    public struct GuiStyle
    {
        // ---- Dimensiones ----
        public float WindowPadding;
        public float ItemSpacing;
        public float ItemInnerSpacing;
        public float FramePadding;
        public float IndentSpacing;
        public float ScrollbarWidth;
        public float ControlHeight;
        public float WindowTitleBarHeight;
        public float FontSize;

        // ---- Colores ----
        public GuiColor32 WindowBackground;
        public GuiColor32 TitleBarBackground;
        public GuiColor32 TitleBarActive;
        public GuiColor32 FrameBackground;
        public GuiColor32 FrameBackgroundHovered;
        public GuiColor32 FrameBackgroundActive;
        public GuiColor32 ButtonNormal;
        public GuiColor32 ButtonHovered;
        public GuiColor32 ButtonActive;
        public GuiColor32 CheckMark;
        public GuiColor32 SliderGrab;
        public GuiColor32 SliderGrabActive;
        public GuiColor32 TextColor;
        public GuiColor32 TextDisabled;
        public GuiColor32 BorderColor;
        public GuiColor32 ProgressBarFill;
        public GuiColor32 HeaderNormal;
        public GuiColor32 HeaderHovered;
        public GuiColor32 HeaderActive;
        public GuiColor32 DropdownBackground;

        /// <summary>
        /// Crea un tema oscuro por defecto (inspirado en Dear ImGui Dark).
        /// </summary>
        public static GuiStyle Default()
        {
            return new GuiStyle
            {
                WindowPadding = 8f,
                ItemSpacing = 4f,
                ItemInnerSpacing = 4f,
                FramePadding = 4f,
                IndentSpacing = 21f,
                ScrollbarWidth = 14f,
                ControlHeight = 22f,
                WindowTitleBarHeight = 26f,
                FontSize = 14f,

                WindowBackground     = new GuiColor32(15, 15, 15, 240),
                TitleBarBackground   = new GuiColor32(10, 10, 10, 255),
                TitleBarActive       = new GuiColor32(40, 73, 122, 255),
                FrameBackground      = new GuiColor32(20, 21, 23, 138),
                FrameBackgroundHovered = new GuiColor32(40, 40, 45, 200),
                FrameBackgroundActive = new GuiColor32(50, 50, 58, 220),
                ButtonNormal         = new GuiColor32(35, 60, 100, 255),
                ButtonHovered        = new GuiColor32(45, 75, 120, 255),
                ButtonActive         = new GuiColor32(15, 45, 85, 255),
                CheckMark            = new GuiColor32(100, 175, 255, 255),
                SliderGrab           = new GuiColor32(80, 130, 200, 255),
                SliderGrabActive     = new GuiColor32(100, 160, 240, 255),
                TextColor            = new GuiColor32(220, 220, 220, 255),
                TextDisabled         = new GuiColor32(128, 128, 128, 255),
                BorderColor          = new GuiColor32(60, 60, 70, 128),
                ProgressBarFill      = new GuiColor32(80, 150, 230, 255),
                HeaderNormal         = new GuiColor32(35, 60, 100, 200),
                HeaderHovered        = new GuiColor32(45, 75, 120, 200),
                HeaderActive         = new GuiColor32(15, 45, 85, 200),
                DropdownBackground   = new GuiColor32(25, 25, 28, 250),
            };
        }
    }
}
