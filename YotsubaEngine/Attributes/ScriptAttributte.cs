using System;

namespace YotsubaEngine.Attributes
{
    /// <summary>
    /// Marca clases que se tratan como scripts del motor.
    /// <para>Marks classes that are treated as engine scripts.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScriptAttribute : Attribute
    {
    }
}
