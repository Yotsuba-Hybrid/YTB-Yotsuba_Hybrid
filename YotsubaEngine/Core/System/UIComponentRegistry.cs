#if YTB
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.System
{
    /// <summary>
    /// Registro central que descubre, en el arranque, todos los tipos con [UIComponent] y sus miembros con [UIComponentValue].
    /// Cachea metadata y delegates JIT-compilados para los métodos de render UI complejos, evitando MethodInfo.Invoke por frame.
    /// Solo compila en #if YTB (modo editor) — el juego final usa código estático generado por YTBContentBuilder.
    /// </summary>
    internal static class UIComponentRegistry
    {
        /// <summary>
        /// Un miembro serializable de un componente: puede ser PropertyInfo o FieldInfo.
        /// </summary>
        public readonly struct ComponentMember
        {
            public readonly MemberInfo Member;
            public readonly UIComponentValue Attribute;

            public ComponentMember(MemberInfo member, UIComponentValue attribute)
            {
                Member = member;
                Attribute = attribute;
            }

            public Type MemberType => Member switch
            {
                PropertyInfo pi => pi.PropertyType,
                FieldInfo fi => fi.FieldType,
                _ => typeof(object)
            };

            public string Name => Member.Name;
        }

        private static readonly Dictionary<string, Type> _componentTypesBySerializableName = new(StringComparer.Ordinal);
        private static readonly Dictionary<Type, UIComponent> _componentAttributes = new();
        private static readonly Dictionary<Type, ComponentMember[]> _membersByType = new();
        private static readonly Dictionary<(Type, string), Action<IUIRenderContext, string>> _renderDelegateCache = new();
        private static readonly Dictionary<Type, Action<IUIRenderContext>?> _extraControlsCache = new();

        static UIComponentRegistry()
        {
            // Escanea todos los assemblies cargados (incluye el del engine y posibles assemblies de juegos).
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray()!; }
                catch { continue; }

                foreach (var type in types)
                {
                    var ui = type.GetCustomAttribute<UIComponent>();
                    if (ui == null) continue;

                    _componentTypesBySerializableName[ui.SerializableName] = type;
                    _componentAttributes[type] = ui;

                    var members = new List<ComponentMember>();

                    foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        var attr = prop.GetCustomAttribute<UIComponentValue>();
                        if (attr != null)
                            members.Add(new ComponentMember(prop, attr));
                    }
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        var attr = field.GetCustomAttribute<UIComponentValue>();
                        if (attr != null)
                            members.Add(new ComponentMember(field, attr));
                    }

                    _membersByType[type] = members.ToArray();
                }
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de componentes registrados.
        /// </summary>
        public static IEnumerable<Type> AllComponentTypes => _componentAttributes.Keys;

        /// <summary>
        /// Obtiene todos los pares (Type, UIComponent) registrados.
        /// </summary>
        public static IEnumerable<KeyValuePair<Type, UIComponent>> AllComponents => _componentAttributes;

        /// <summary>
        /// Resuelve el Type de un componente por su nombre serializable. Retorna null si no se encontró.
        /// </summary>
        public static Type GetComponentType(string serializableName)
        {
            return _componentTypesBySerializableName.TryGetValue(serializableName, out var t) ? t : null;
        }

        /// <summary>
        /// Obtiene el atributo [UIComponent] de un tipo (o null si no lo tiene).
        /// </summary>
        public static UIComponent GetComponentAttribute(Type type)
        {
            return _componentAttributes.TryGetValue(type, out var a) ? a : null;
        }

        /// <summary>
        /// Obtiene los miembros con [UIComponentValue] de un tipo (vacío si no es un componente registrado).
        /// </summary>
        public static IReadOnlyList<ComponentMember> GetMembers(Type type)
        {
            return _membersByType.TryGetValue(type, out var arr) ? arr : Array.Empty<ComponentMember>();
        }

        /// <summary>
        /// Obtiene (con caché) un delegate JIT-compilado para un método estático de render UI con firma
        /// <c>static void Method(IUIRenderContext ctx, string raw)</c>. Retorna null si el método no existe o no compatible.
        /// </summary>
        public static Action<IUIRenderContext, string> GetRenderConverter(Type componentType, string methodName)
        {
            var key = (componentType, methodName);
            if (_renderDelegateCache.TryGetValue(key, out var cached))
                return cached;

            var method = componentType.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(IUIRenderContext), typeof(string) },
                modifiers: null);

            if (method == null)
            {
                _renderDelegateCache[key] = null;
                return null;
            }

            var del = (Action<IUIRenderContext, string>)Delegate.CreateDelegate(typeof(Action<IUIRenderContext, string>), method);
            _renderDelegateCache[key] = del;
            return del;
        }

        /// <summary>
        /// Obtiene (con caché) un delegate JIT-compilado para el método estático <c>RenderExtraControls(IUIRenderContext)</c>
        /// del componente, si está declarado. Retorna null si el componente no define controles extra.
        /// El EntityManagerUI lo invoca al final del render, después del loop de propiedades, para mostrar
        /// botones/sincronizaciones específicas (ej: "Sincronizar Size con Sprite" del Transform).
        /// </summary>
        public static Action<IUIRenderContext> GetExtraControls(Type componentType)
        {
            if (_extraControlsCache.TryGetValue(componentType, out var cached))
                return cached;

            var method = componentType.GetMethod(
                "RenderExtraControls",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(IUIRenderContext) },
                modifiers: null);

            if (method == null)
            {
                _extraControlsCache[componentType] = null;
                return null;
            }

            var del = (Action<IUIRenderContext>)Delegate.CreateDelegate(typeof(Action<IUIRenderContext>), method);
            _extraControlsCache[componentType] = del;
            return del;
        }
    }
}
#endif
