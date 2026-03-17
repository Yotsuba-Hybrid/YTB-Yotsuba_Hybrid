// ============================================================================
// YotsubaEngine – Immediate Mode GUI System
// GuiHash.cs – Sistema de hashing ultra-rápido basado en FNV-1a.
//
// PROPÓSITO:
// En un IMGUI cada control necesita un ID numérico estable que persista entre
// frames.  El ID se genera hasheando el label del control.  Para permitir
// dos controles con el mismo label visible, se soporta la sintaxis
// "LabelVisible##IdOculto" (igual que Dear ImGui).
//
// DECISIONES DE RENDIMIENTO:
// • FNV-1a de 32 bits: excelente dispersión, cero allocations, recorrido
//   lineal byte-a-byte, sin dependencias de datos entre iteraciones
//   (pipeline-friendly).
// • El procesamiento se hace sobre ReadOnlySpan<char> para evitar copias
//   de string.  Cuando es posible se opera directamente sobre los bytes
//   del char (UTF-16 LE) usando unsafe pointer casting.
// • Se ofrece un overload stackalloc-friendly para concatenar un ID seed
//   (ej. el ID del Window padre) con el label del control, creando IDs
//   jerárquicos sin allocar en el heap.
// ============================================================================

using System;
using System.Runtime.CompilerServices;

namespace YotsubaEngine.IMGUI
{
    public static class GuiHash
    {
        // Constantes FNV-1a de 32 bits
        // Ref: http://www.isthe.com/chongo/tech/comp/fnv/
        private const uint FnvOffsetBasis = 2166136261u;
        private const uint FnvPrime       = 16777619u;

        /// <summary>
        /// Separador que divide la parte visible de la parte identificadora.
        /// "Aceptar##btn_accept"  →  hash solo de "btn_accept".
        /// Si no contiene "##", se hashea el string completo.
        /// </summary>
        private const string HiddenIdSeparator = "##";

        // ================================================================
        // API principal
        // ================================================================

        /// <summary>
        /// Calcula el GuiId para un label de control, soportando "##" para IDs ocultos.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiId Hash(ReadOnlySpan<char> label)
        {
            // Buscar "##" en el span
            int sepIdx = FindSeparator(label);
            ReadOnlySpan<char> hashPart = sepIdx >= 0
                ? label[(sepIdx + 2)..]
                : label;

            return new GuiId(Fnv1a(hashPart));
        }

        /// <summary>
        /// Calcula un ID jerárquico combinando un seed (ej. ID del window padre)
        /// con el label del control hijo.  Esto permite tener controles con el
        /// mismo label en diferentes windows sin colisiones.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiId HashWithSeed(ReadOnlySpan<char> label, GuiId seed)
        {
            int sepIdx = FindSeparator(label);
            ReadOnlySpan<char> hashPart = sepIdx >= 0
                ? label[(sepIdx + 2)..]
                : label;

            return new GuiId(Fnv1aSeeded(hashPart, seed.Value));
        }

        /// <summary>
        /// Extrae solo la parte visible del label (antes de "##").
        /// Devuelve el span completo si no hay separador.
        /// Zero-allocation: no crea un nuevo string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> GetVisibleLabel(ReadOnlySpan<char> label)
        {
            int sepIdx = FindSeparator(label);
            return sepIdx >= 0 ? label[..sepIdx] : label;
        }

        // ================================================================
        // FNV-1a core – opera sobre los bytes crudos de los chars (UTF-16 LE).
        //
        // Procesar 2 bytes por char (en lugar de convertir a UTF-8 primero)
        // es más rápido y produce hashes igualmente bien distribuidos para
        // los codepoints típicos de UI (ASCII + Latin extendido).
        // ================================================================

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Fnv1a(ReadOnlySpan<char> data)
        {
            uint hash = FnvOffsetBasis;

            fixed (char* ptr = data)
            {
                byte* bytes = (byte*)ptr;
                int byteLen = data.Length * 2; // 2 bytes por char UTF-16

                for (int i = 0; i < byteLen; i++)
                {
                    hash ^= bytes[i];
                    hash *= FnvPrime;
                }
            }

            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Fnv1aSeeded(ReadOnlySpan<char> data, uint seed)
        {
            // Mezclar el seed con el offset basis para evitar que seed=0
            // produzca el mismo resultado que sin seed.
            uint hash = FnvOffsetBasis ^ seed;
            hash *= FnvPrime;

            fixed (char* ptr = data)
            {
                byte* bytes = (byte*)ptr;
                int byteLen = data.Length * 2;

                for (int i = 0; i < byteLen; i++)
                {
                    hash ^= bytes[i];
                    hash *= FnvPrime;
                }
            }

            return hash;
        }

        /// <summary>
        /// Busca la subcadena "##" en un span sin alocar.
        /// Retorna el índice del primer '#' o -1 si no se encuentra.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindSeparator(ReadOnlySpan<char> span)
        {
            // Búsqueda lineal simple; los labels de UI son cortos (< 64 chars)
            // así que el overhead de un algoritmo más sofisticado no compensa.
            for (int i = 0; i < span.Length - 1; i++)
            {
                if (span[i] == '#' && span[i + 1] == '#')
                    return i;
            }
            return -1;
        }

        // ================================================================
        // Utilidades para ID stacking (usado internamente por GuiContext)
        // ================================================================

        /// <summary>
        /// Combina dos IDs en uno nuevo.  Se usa para crear un ID de scope
        /// cuando se anidan windows/groups.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GuiId Combine(GuiId a, GuiId b)
        {
            // Mezcla tipo boost::hash_combine: buena dispersión, rápido
            uint h = a.Value;
            h ^= b.Value + 0x9E3779B9u + (h << 6) + (h >> 2);
            return new GuiId(h);
        }
    }
}
