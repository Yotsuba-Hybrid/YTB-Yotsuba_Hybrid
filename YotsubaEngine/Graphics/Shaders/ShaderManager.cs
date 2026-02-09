using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace YotsubaEngine.Graphics.Shaders
{
    /// <summary>
    /// Gestor centralizado de shaders. Maneja la carga, caché y acceso a efectos.
    /// Totalmente compatible con AOT y multiplataforma.
    /// </summary>
    public static class ShaderManager
    {
        private static readonly Dictionary<string, Effect> _loadedShaders = new();
        private static ContentManager _contentManager;

        /// <summary>
        /// Inicializa el ShaderManager con el ContentManager.
        /// Debe llamarse antes de cargar cualquier shader.
        /// </summary>
        /// <param name="contentManager">ContentManager del juego</param>
        public static void Initialize(ContentManager contentManager)
        {
            _contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        }

        /// <summary>
        /// Carga un shader desde el contenido compilado (.xnb).
        /// Si ya está cargado, devuelve la instancia en caché.
        /// </summary>
        /// <param name="shaderPath">Ruta al shader sin extensión (ej: "Shaders/Grayscale")</param>
        /// <returns>El efecto cargado</returns>
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
        /// </summary>
        /// <param name="shaderPath">Ruta al shader</param>
        /// <returns>El efecto si está cargado, null si no existe en caché</returns>
        public static Effect GetShader(string shaderPath)
        {
            _loadedShaders.TryGetValue(shaderPath, out Effect effect);
            return effect;
        }

        /// <summary>
        /// Verifica si un shader está cargado en caché.
        /// </summary>
        /// <param name="shaderPath">Ruta al shader</param>
        /// <returns>true si está cargado, false si no</returns>
        public static bool IsShaderLoaded(string shaderPath)
        {
            return _loadedShaders.ContainsKey(shaderPath);
        }

        /// <summary>
        /// Limpia todos los shaders cargados. Útil al cambiar de escena.
        /// </summary>
        public static void UnloadAll()
        {
            _loadedShaders.Clear();
        }

        /// <summary>
        /// Descarga un shader específico de la caché.
        /// </summary>
        /// <param name="shaderPath">Ruta al shader</param>
        public static void UnloadShader(string shaderPath)
        {
            _loadedShaders.Remove(shaderPath);
        }

        /// <summary>
        /// Crea un clon independiente de un shader para permitir parámetros únicos por entidad.
        /// IMPORTANTE: Usar con cuidado, cada clon consume memoria adicional.
        /// Solo clonar cuando necesites parámetros diferentes del shader base.
        /// Para la mayoría de casos, compartir el mismo Effect entre entidades es suficiente.
        /// </summary>
        /// <param name="shaderPath">Ruta al shader base</param>
        /// <returns>Un nuevo efecto clonado</returns>
        public static Effect CloneShader(string shaderPath)
        {
            Effect originalEffect = LoadShader(shaderPath);
            return originalEffect.Clone();
        }
    }
}
