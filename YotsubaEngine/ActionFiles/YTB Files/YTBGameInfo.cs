using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Modelo del archivo del juego que contiene escenas y entidades.
    /// <para>Represents the game file model containing scenes and entities.</para>
    /// </summary>
    internal class YTBGameInfo
    {
        [JsonPropertyName("scene")]
        /// <summary>
        /// Obtiene o establece la lista de escenas.
        /// <para>Gets or sets the list of scenes.</para>
        /// </summary>
        public List<YTBScene> Scene { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Obtiene la cantidad de escenas.
        /// <para>Gets the number of scenes.</para>
        /// </summary>
        public int ScenesCount => Scene.Count;

    }

    /// <summary>
    /// Representa una escena en el archivo del juego.
    /// <para>Represents a scene entry in the game file.</para>
    /// </summary>
    internal class YTBScene
    {
        [JsonPropertyName("name")]
        /// <summary>
        /// Obtiene o establece el nombre de la escena.
        /// <para>Gets or sets the scene name.</para>
        /// </summary>
        public string Name { get; set; }

        [JsonPropertyName("entities")]
        /// <summary>
        /// Obtiene o establece la lista de entidades en la escena.
        /// <para>Gets or sets the list of entities in the scene.</para>
        /// </summary>
        public List<YTBEntity> Entities { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Obtiene la cantidad de entidades.
        /// <para>Gets the number of entities.</para>
        /// </summary>
        public int EntitiesCount => Entities.Count;
    }

    /// <summary>
    /// Representa una entidad en el archivo del juego.
    /// <para>Represents an entity entry in the game file.</para>
    /// </summary>
    internal class YTBEntity
    {
        [JsonPropertyName("name")]
        /// <summary>
        /// Obtiene o establece el nombre de la entidad.
        /// <para>Gets or sets the entity name.</para>
        /// </summary>
        public string Name { get; set; }

        [JsonPropertyName("components")]
        /// <summary>
        /// Obtiene o establece los componentes de la entidad.
        /// <para>Gets or sets the entity components.</para>
        /// </summary>
        public List<YTBComponents> Components { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Obtiene la cantidad de componentes.
        /// <para>Gets the number of components.</para>
        /// </summary>
        public int ComponentsCount => Components.Count;
    }

    /// <summary>
    /// Representa un componente dentro de una entidad.
    /// <para>Represents a component entry within an entity.</para>
    /// </summary>
    public class YTBComponents : IEquatable<YTBComponents>
    {
        [JsonPropertyName("ComponentName")]
        /// <summary>
        /// Obtiene o establece el nombre del componente.
        /// <para>Gets or sets the component name.</para>
        /// </summary>
        public string ComponentName { get; set; }

        [JsonPropertyName("properties")]
        /// <summary>
        /// Obtiene o establece las propiedades del componente.
        /// <para>Gets or sets the component properties.</para>
        /// </summary>
        public List<Tuple<string, string>> Propiedades { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Obtiene la cantidad de propiedades.
        /// <para>Gets the number of properties.</para>
        /// </summary>
        public int PropiedadesCount => Propiedades.Count;

        /// <summary>
        /// Determina si otro componente es igual a esta instancia.
        /// <para>Determines whether another component is equal to this instance.</para>
        /// </summary>
        /// <param name="other">Otro componente. <para>Other component.</para></param>
        /// <returns>True si es igual. <para>True if equal.</para></returns>
        public bool Equals(YTBComponents other)
        {
            if (other is null) return false;
            if (!string.Equals(ComponentName, other.ComponentName, StringComparison.Ordinal))
                return false;

            if (PropiedadesCount != other.PropiedadesCount)
                return false;

            for (int i = 0; i < PropiedadesCount; i++)
            {
                if (!Propiedades[i].Item1.Equals(other.Propiedades[i].Item1, StringComparison.Ordinal)
                 || !Propiedades[i].Item2.Equals(other.Propiedades[i].Item2, StringComparison.Ordinal))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determina si otro objeto es igual a esta instancia.
        /// <para>Determines whether another object is equal to this instance.</para>
        /// </summary>
        /// <param name="obj">Objeto a comparar. <para>Object to compare.</para></param>
        /// <returns>True si es igual. <para>True if equal.</para></returns>
        public override bool Equals(object obj) => Equals(obj as YTBComponents);

        /// <summary>
        /// Devuelve el codigo hash del componente.
        /// <para>Returns a hash code for the component.</para>
        /// </summary>
        /// <returns>Codigo hash. <para>Hash code.</para></returns>
        public override int GetHashCode()
        {
            int hash = ComponentName?.GetHashCode() ?? 0;

            foreach (var p in Propiedades)
            {
                hash = HashCode.Combine(hash, p.Item1?.GetHashCode() ?? 0, p.Item2?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

}
