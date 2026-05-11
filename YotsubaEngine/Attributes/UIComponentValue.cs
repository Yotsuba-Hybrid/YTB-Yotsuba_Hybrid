#nullable enable
using System;

namespace YotsubaEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class UIComponentValue(string name, string serializableName, string descripcion, string textoErrorValorInvalido, string? ValueConverterForParse = null, string? ValueConverterForRead = null, params string[]? legacySerializableNames) : Attribute
    {
        public string VisibleName { get; } = name;
        public string SerializableName { get; } = serializableName;
        public string Descripcion { get; } = descripcion;
        public string TextoErrorValorInvalido { get; } = textoErrorValorInvalido;

        //Search via Reflection in component struct/class
        public string? NameMethodValueConverterForParse { get; } = ValueConverterForParse;

        //Search via Reflection in component struct/class
        public string? NameMethodValueConverterForRead { get; } = ValueConverterForRead;

        // Para versiones legacy del codigo, si el SerializableName no existe, buscar entre los LegacySerializableNames a ver cual coincide.
        public string[] LegacySerializableNames = legacySerializableNames ?? [];

        private object Convert(object value)
        {
            return value;
        }
    }
}
