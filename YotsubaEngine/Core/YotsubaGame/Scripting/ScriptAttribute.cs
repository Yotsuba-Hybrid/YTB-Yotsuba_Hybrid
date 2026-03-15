using System;
using YotsubaEngine.Forms;

namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScriptAttribute : Attribute
    {
        public UILibrary UISystem { get; set; } = UILibrary.GumUI;

        public ScriptAttribute() { }
    }
}

namespace YotsubaEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ScriptAttribute : global::YotsubaEngine.Core.YotsubaGame.Scripting.ScriptAttribute
    {
        public ScriptAttribute() : base() { }
    }
}