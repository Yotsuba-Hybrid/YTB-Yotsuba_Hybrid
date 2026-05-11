#nullable enable
using System;

namespace YotsubaEngine.Attributes
{
    /// <summary>
    /// Marca una propiedad/campo/parámetro de un componente como serializable y editable en la UI del engine.
    /// El EntityManagerUI itera estos miembros vía reflexión (solo en #if YTB) y dibuja un control automático según el tipo.
    /// El YTBContentBuilder genera código estático en tiempo de compilación para el parseo del .ytb (sin reflexión en runtime).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public class UIComponentValue(string name, string serializableName, string descripcion, string textoErrorValorInvalido, string? ValueConverterForParse = null, string? ValueConverterForRead = null, params string[]? legacySerializableNames) : Attribute
    {
        public string VisibleName { get; } = name;
        public string SerializableName { get; } = serializableName;
        public string Descripcion { get; } = descripcion;
        public string TextoErrorValorInvalido { get; } = textoErrorValorInvalido;

        /// <summary>
        /// Nombre de un método estático del componente con firma <c>static object Method(string raw)</c>.
        /// Usado por el YTBContentBuilder para generar la llamada al parser custom (sin reflexión en runtime).
        /// </summary>
        public string? NameMethodValueConverterForParse { get; } = ValueConverterForParse;

        /// <summary>
        /// Nombre de un método estático del componente con firma <c>static void Method(IUIRenderContext ctx, string raw)</c>.
        /// Usado por EntityManagerUI (vía delegate JIT-cacheado) para UI compleja como combos pareados o dropdowns con datos externos.
        /// </summary>
        public string? NameMethodValueConverterForRead { get; } = ValueConverterForRead;

        /// <summary>
        /// Si el <see cref="SerializableName"/> no existe en el .ytb, se busca entre estos nombres legacy para mantener compatibilidad.
        /// </summary>
        public string[] LegacySerializableNames = legacySerializableNames ?? [];
    }
}
