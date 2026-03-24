using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace SandBoxGame.Core.Localization;

/// <summary>
/// Manages localization settings for the game, including retrieving supported cultures and setting the current culture for localization.
/// </summary>
internal class LocalizationManager
{
    /// <summary>
    /// Código de cultura predeterminado.
    /// <para>Default culture code.</para>
    /// </summary>
    public const string DEFAULT_CULTURE_CODE = "en-EN";

    /// <summary>
    /// Recupera la lista de culturas compatibles según los recursos de idioma disponibles.
    /// <para>Retrieves a list of supported cultures based on available language resources.</para>
    /// </summary>
    /// <returns>
    /// Lista de <see cref="CultureInfo"/> con las culturas soportadas.
    /// <para>List of <see cref="CultureInfo"/> objects representing supported cultures.</para>
    /// </returns>
    /// <remarks>
    /// This method iterates through all specific cultures defined in the satellite assemblies and attempts to load the corresponding resource set.
    /// If a resource set is found for a particular culture, that culture is added to the list of supported cultures. The invariant culture
    /// is always included in the returned list as it represents the default (non-localized) resources.
    /// </remarks>
    public static List<CultureInfo> GetSupportedCultures()
    {
        // Only check cultures for which we have .resx files.
        // Iterating ALL CultureInfo.GetCultures() generates hundreds of
        // FileNotFoundException on Android and severely impacts startup.
        string[] knownCultures = { "es-ES", "fr-FR" };

        List<CultureInfo> supportedCultures = new List<CultureInfo>();
        Assembly assembly = Assembly.GetExecutingAssembly();
        ResourceManager resourceManager = new ResourceManager("SandBoxGame.Core.Localization.Resources", assembly);

        foreach (string code in knownCultures)
        {
            try
            {
                var culture = new CultureInfo(code);
                var resourceSet = resourceManager.GetResourceSet(culture, true, false);
                if (resourceSet != null)
                {
                    supportedCultures.Add(culture);
                }
            }
            catch (MissingManifestResourceException)
            {
            }
        }

        // Always add the default (invariant) culture - the base .resx file
        supportedCultures.Add(CultureInfo.InvariantCulture);

        return supportedCultures;
    }

    /// <summary>
    /// Establece la cultura actual del juego según el código indicado.
    /// <para>Sets the game's current culture based on the provided culture code.</para>
    /// </summary>
    /// <param name="cultureCode">
    /// Código de cultura (por ejemplo, "en-US", "fr-FR") a establecer.
    /// <para>Culture code (e.g., "en-US", "fr-FR") to set.</para>
    /// </param>
    /// <remarks>
    /// This method modifies the <see cref="Thread.CurrentThread.CurrentCulture"/> and <see cref="Thread.CurrentThread.CurrentUICulture"/> properties,
    /// which affect how dates, numbers, and other culture-specific values are formatted, as well as how localized resources are loaded.
    /// </remarks>
    public static void SetCulture(string cultureCode)
    {
        if (string.IsNullOrEmpty(cultureCode))
            throw new ArgumentNullException(nameof(cultureCode), "A culture code must be provided.");

        // Create a CultureInfo object from the culture code
        CultureInfo culture = new CultureInfo(cultureCode);

        // Set the current culture and UI culture for the current thread
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
