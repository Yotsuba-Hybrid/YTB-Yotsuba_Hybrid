using System;
using System.Collections.Generic;
using System.Text;

namespace YotsubaEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]

    public class UIComponent(string name, string serializableName) : Attribute
    {
        public string VisibleName { get; } = name;

        public string SerializableName { get; } = serializableName;
    }
}
