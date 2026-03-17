// =============================================================================
// GuiHash.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Sistema de hashing de IDs para controles de UI.
//
// DECISIÓN DE DISEÑO – FNV-1a 32-bit
// ------------------------------------
// Se eligió FNV-1a frente a otras alternativas (MurmurHash, xxHash) porque:
//  1. Cálculo en un solo pase, sin necesidad de SIMD ni pre-procesamiento de bloques.
//  2. Excelente distribución (efecto avalancha) para strings cortos (<32 chars),
//     exactamente el caso de uso de etiquetas de UI ("Button##id", "Slider_Speed").
//  3. Patrón de ramificación JIT-friendly: el bucle interior es tan simple que el
//     JIT de .NET lo elide completamente en binarios Release (inlining + unrolling).
//  4. Sin estado externo, sin seeds complejos, sin tablas de lookup.
//
// Patrón "Label##HiddenID" (idéntico a Dear ImGui):
//  - La parte visible ("Label") se usa para el texto renderizado.
//  - La parte oculta ("HiddenID") se usa para el hash → sin colisión de IDs
//    cuando múltiples controles comparten la misma etiqueta visible.
// =============================================================================

using System;
using System.Runtime.CompilerServices;

namespace YotsubaEngine.Graphics.Gui;

/// <summary>
/// Utilidades de hashing ultra-rápido (FNV-1a) para la generación de IDs
/// de controles UI. Todos los métodos son estáticos e inlineables.
/// </summary>
internal static class GuiHash
{
    // FNV-1a 32-bit: constantes oficiales del algoritmo.
    // offset_basis = 2166136261 (0x811C9DC5)
    // prime        = 16777619   (0x01000193)
    private const uint FNV_OFFSET_BASIS = 2166136261u;
    private const uint FNV_PRIME = 16777619u;

    // ID reservado que significa "ningún elemento".
    // Nunca se puede generar mediante hashing porque 0 no es un resultado válido
    // de FNV-1a para strings no vacíos (la probabilidad es 1/2^32, despreciable).
    internal const uint NONE = 0u;

    // ─────────────────────────────────────────────────────────────────────────
    // HASHING CORE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Calcula el hash FNV-1a de un span de caracteres. Cero alocaciones.
    /// La ruta caliente (ASCII puro) usa un único cast (byte)char sin ramas extra.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Hash(ReadOnlySpan<char> text)
    {
        uint hash = FNV_OFFSET_BASIS;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c < 128)
            {
                // Ruta caliente ASCII: un solo byte, sin bifurcación adicional.
                hash = (hash ^ (byte)c) * FNV_PRIME;
            }
            else
            {
                // Ruta fría no-ASCII: hasheamos ambos bytes del codeunit UTF-16
                // para preservar unicidad en etiquetas con acentos o símbolos.
                hash = (hash ^ (byte)(c & 0xFF)) * FNV_PRIME;
                hash = (hash ^ (byte)(c >> 8))   * FNV_PRIME;
            }
        }
        // Garantizar que el resultado nunca sea NONE (0).
        // Si el hash es 0 lo desplazamos a 1 para mantener el invariante.
        return hash == NONE ? 1u : hash;
    }

    /// <summary>
    /// Hash FNV-1a con semilla. Se usa para encadenar el ID de un padre con
    /// el de sus hijos, generando identificadores jerárquicamente únicos.
    /// El seed típicamente es el ID del contexto de ventana activo.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint HashWithSeed(ReadOnlySpan<char> text, uint seed)
    {
        // Iniciamos el hash con la semilla en lugar del offset_basis.
        // Esto mezcla el estado del padre con el texto del hijo, produciendo
        // hashes completamente distintos para nodos hermanos con el mismo nombre.
        uint hash = seed;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c < 128)
                hash = (hash ^ (byte)c) * FNV_PRIME;
            else
            {
                hash = (hash ^ (byte)(c & 0xFF)) * FNV_PRIME;
                hash = (hash ^ (byte)(c >> 8))   * FNV_PRIME;
            }
        }
        return hash == NONE ? 1u : hash;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PARSING DE ETIQUETAS (patrón ImGui "Label##HiddenID")
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Separa una etiqueta completa en su parte visible y su parte de hashing.
    /// Ambos spans son slices del span original → cero alocaciones.
    /// </summary>
    /// <param name="fullLabel">Etiqueta completa, p.ej. "Velocidad##slider_vel".</param>
    /// <param name="displayLabel">Parte visible ("Velocidad").</param>
    /// <param name="hashLabel">Parte usada para el hash ("slider_vel").</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ParseLabel(
        ReadOnlySpan<char> fullLabel,
        out ReadOnlySpan<char> displayLabel,
        out ReadOnlySpan<char> hashLabel)
    {
        // Búsqueda branchless-friendly: IndexOf usa SIMD interno en .NET 8+
        int sep = fullLabel.IndexOf("##", StringComparison.Ordinal);
        if (sep >= 0)
        {
            displayLabel = fullLabel[..sep];
            hashLabel    = fullLabel[(sep + 2)..];
        }
        else
        {
            displayLabel = fullLabel;
            hashLabel    = fullLabel;
        }
    }

    /// <summary>
    /// Calcula el ID de un control dada su etiqueta completa y el ID del
    /// contexto padre (para jerarquía). Combina ParseLabel + HashWithSeed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ComputeId(ReadOnlySpan<char> fullLabel, uint parentId)
    {
        ParseLabel(fullLabel, out _, out ReadOnlySpan<char> hashPart);
        return HashWithSeed(hashPart, parentId);
    }
}
