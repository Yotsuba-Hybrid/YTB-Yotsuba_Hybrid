using System;

namespace YotsubaEngine.Attributes
{
    /// <summary>
    /// Marca componentes que se exponen al editor visual (EntityManagerUI) y al pipeline de serialización del .ytb.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class UIComponent(string name, string serializableName) : Attribute
    {
        public string VisibleName { get; } = name;

        public string SerializableName { get; } = serializableName;

        public bool IsClass { get; set; }
    }
}
