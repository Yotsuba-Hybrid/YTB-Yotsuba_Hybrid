using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace YotsubaEngine.Graphics.Shaders
{
    /// <summary>
    /// Gestor centralizado de shaders con carga, caché y acceso a efectos, compatible con AOT y multiplataforma.
    /// <para>Centralized shader manager for loading, caching, and accessing effects, AOT and cross-platform compatible.</para>
    /// </summary>
    public static class ShaderManager
    {
        private static readonly Dictionary<string, Effect> _loadedShaders = new();
        private static ContentManager _contentManager;

        /// <summary>
        /// Inicializa el ShaderManager con el ContentManager antes de cargar shaders.
        /// <para>Initializes the ShaderManager with the ContentManager before loading shaders.</para>
        /// </summary>
        /// <param name="contentManager">ContentManager del juego. <para>Game ContentManager.</para></param>
        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        }

        /// <summary>
        /// Carga un shader desde el contenido compilado (.xnb) y usa caché si ya existe.
        /// <para>Loads a shader from compiled content (.xnb) and uses cache if it already exists.</para>
        /// </summary>
        /// <param name="shaderPath">Ruta al shader sin extensión (ej: "Shaders/Grayscale"). <para>Shader path without extension (e.g., "Shaders/Grayscale").</para></param>
        /// <returns>Efecto cargado. <para>Loaded effect.</para></returns>
        public static Effect LoadShader(string shaderPath)
        {
            if (_contentManager == null)
            {
                throw new InvalidOperationException("ShaderManager no ha sido inicializado. Llama a Initialize() primero.");
            }

            if (string.IsNullOrWhiteSpace(shaderPath))
            {
                throw new ArgumentException("La ruta del shader no puede estar vacía.", nameof(shaderPath));
            }

            // Si ya está en caché, devolverlo
            if (_loadedShaders.TryGetValue(shaderPath, out Effect cachedEffect))
            {
                return cachedEffect;
            }

            // Cargar el shader
            Effect effect = _contentManager.Load<Effect>(shaderPath);
            _loadedShaders[shaderPath] = effect;

            return effect;
        }

        /// <summary>
        /// Obtiene un shader ya cargado desde la caché.
        /// <para>Gets a shader already loaded from cache.</para>
        /// </summary>
        /// <param name="shaderPath">Ruta al shader. <para>Shader path.</para></param>
        /// <returns>Efecto si está cargado, null si no existe. <para>Effect if loaded, null if missing.</para></returns>
        public static Effect GetShader(string shaderPath)
        {
            _loadedShaders.TryGetValue(shaderPath, out Effect effect);
            return effect;
        }

        /// <summary>
        /// Verifica si un shader está cargado en caché.
        /// <para>Checks whether a shader is loaded in cache.</para>
        /// </summary>
        /// <param name="shaderPath">Ruta al shader. <para>Shader path.</para></param>
        /// <returns>True si está cargado. <para>True if loaded.</para></returns>
        public static bool IsShaderLoaded(string shaderPath)
        {
            return _loadedShaders.ContainsKey(shaderPath);
        }

        /// <summary>
        /// Limpia todos los shaders cargados; útil al cambiar de escena.
        /// <para>Clears all loaded shaders; useful when changing scenes.</para>
        /// </summary>
        public static void UnloadAll()
        {
            _loadedShaders.Clear();
        }

        /// <summary>
        /// Descarga un shader específico de la caché.
        /// <para>Unloads a specific shader from cache.</para>
        /// </summary>
        /// <param name="shaderPath">Ruta al shader. <para>Shader path.</para></param>
        public static void UnloadShader(string shaderPath)
        {
            _loadedShaders.Remove(shaderPath);
        }

        /// <summary>
        /// Crea un clon independiente de un shader para permitir parámetros únicos por entidad.
        /// <para>Creates an independent shader clone to allow per-entity parameters.</para>
        /// </summary>
        /// <remarks>
        /// Úsalo con cuidado; cada clon consume memoria adicional.
        /// <para>Use with care; each clone consumes additional memory.</para>
        /// </remarks>
        /// <param name="shaderPath">Ruta al shader base. <para>Base shader path.</para></param>
        /// <returns>Nuevo efecto clonado. <para>New cloned effect.</para></returns>
        public static Effect CloneShader(string shaderPath)
        {
            Effect originalEffect = LoadShader(shaderPath);
            return originalEffect.Clone();
        }
    }
}
