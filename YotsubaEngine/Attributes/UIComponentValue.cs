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
    public class UIComponentValue(
        string name,
        string serializableName,
        string descripcion,
        string textoErrorValorInvalido,
        string? ValueConverterForParse = null,
        string? ValueConverterForRead = null,
        string defaultValue = "",
        string inactiveValue = "",
        params string[]? legacySerializableNames) : Attribute
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
        /// Valor por defecto que se asigna a esta propiedad cuando se añade el componente a una entidad recién creada.
        /// Reemplaza a las plantillas hardcodeadas que antes vivían en EntityYTBXmlTemplate.
        /// </summary>
        public string DefaultValue { get; } = defaultValue;

        /// <summary>
        /// Valor "inactivo": si todas las propiedades del componente coinciden con su <c>InactiveValue</c>,
        /// el engine considera el componente como no-presente y lo salta al cargar la entidad
        /// (reemplaza la antigua comprobación contra <c>EntityYTBXmlTemplate.GenerateNew()</c>).
        /// </summary>
        public string InactiveValue { get; } = inactiveValue;

        /// <summary>
        /// Si el <see cref="SerializableName"/> no existe en el .ytb, se busca entre estos nombres legacy para mantener compatibilidad.
        /// </summary>
        public string[] LegacySerializableNames = legacySerializableNames ?? [];
    }
}
