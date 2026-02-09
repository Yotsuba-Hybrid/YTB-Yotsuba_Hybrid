using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Represents the game file model containing scenes and entities.
    /// Modelo del archivo del juego. Alli guarda el engine las escenas y entidades del juego.
    /// </summary>
    internal class YTBGameInfo
    {
        [JsonPropertyName("scene")]
        /// <summary>
        /// Gets or sets the list of scenes.
        /// Obtiene o establece la lista de escenas.
        /// </summary>
        public List<YTBScene> Scene { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Gets the number of scenes.
        /// Obtiene la cantidad de escenas.
        /// </summary>
        public int ScenesCount => Scene.Count;

    }

    /// <summary>
    /// Represents a scene entry in the game file.
    /// Representa una escena en el archivo del juego.
    /// </summary>
    internal class YTBScene
    {
        [JsonPropertyName("name")]
        /// <summary>
        /// Gets or sets the scene name.
        /// Obtiene o establece el nombre de la escena.
        /// </summary>
        public string Name { get; set; }

        [JsonPropertyName("entities")]
        /// <summary>
        /// Gets or sets the list of entities in the scene.
        /// Obtiene o establece la lista de entidades en la escena.
        /// </summary>
        public List<YTBEntity> Entities { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Gets the number of entities.
        /// Obtiene la cantidad de entidades.
        /// </summary>
        public int EntitiesCount => Entities.Count;
    }

    /// <summary>
    /// Represents an entity entry in the game file.
    /// Representa una entidad en el archivo del juego.
    /// </summary>
    internal class YTBEntity
    {
        [JsonPropertyName("name")]
        /// <summary>
        /// Gets or sets the entity name.
        /// Obtiene o establece el nombre de la entidad.
        /// </summary>
        public string Name { get; set; }

        [JsonPropertyName("components")]
        /// <summary>
        /// Gets or sets the entity components.
        /// Obtiene o establece los componentes de la entidad.
        /// </summary>
        public List<YTBComponents> Components { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Gets the number of components.
        /// Obtiene la cantidad de componentes.
        /// </summary>
        public int ComponentsCount => Components.Count;
    }

    /// <summary>
    /// Represents a component entry within an entity.
    /// Representa un componente dentro de una entidad.
    /// </summary>
    public class YTBComponents : IEquatable<YTBComponents>
    {
        [JsonPropertyName("ComponentName")]
        /// <summary>
        /// Gets or sets the component name.
        /// Obtiene o establece el nombre del componente.
        /// </summary>
        public string ComponentName { get; set; }

        [JsonPropertyName("properties")]
        /// <summary>
        /// Gets or sets the component properties.
        /// Obtiene o establece las propiedades del componente.
        /// </summary>
        public List<Tuple<string, string>> Propiedades { get; set; }

        [JsonIgnore]
        /// <summary>
        /// Gets the number of properties.
        /// Obtiene la cantidad de propiedades.
        /// </summary>
        public int PropiedadesCount => Propiedades.Count;

        /// <summary>
        /// Determines whether another component is equal to this instance.
        /// Determina si otro componente es igual a esta instancia.
        /// </summary>
        /// <param name="other">Other component. Otro componente.</param>
        /// <returns>True if equal. True si es igual.</returns>
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
        /// Determines whether another object is equal to this instance.
        /// Determina si otro objeto es igual a esta instancia.
        /// </summary>
        /// <param name="obj">Object to compare. Objeto a comparar.</param>
        /// <returns>True if equal. True si es igual.</returns>
        public override bool Equals(object obj) => Equals(obj as YTBComponents);

        /// <summary>
        /// Returns a hash code for the component.
        /// Devuelve el código hash del componente.
        /// </summary>
        /// <returns>Hash code. Código hash.</returns>
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
