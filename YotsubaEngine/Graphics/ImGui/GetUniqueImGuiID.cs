using ImGuiNET;
using System;
using System.Collections.Generic;

/// <summary>
/// Genera identificadores ImGui únicos dentro de un rango configurado.
/// <para>Generates unique ImGui identifiers within a configured range.</para>
/// </summary>
public sealed class GetUniqueImGuiID
{
    /// <summary>
    /// Obtiene un nuevo ID único de ImGui.
    /// <para>Gets a new unique ImGui ID.</para>
    /// </summary>
    /// <returns>Nuevo ID único. <para>New unique ID.</para></returns>
    public static int GetID() => Instance.Get();

    /// <summary>
    /// Obtiene un nuevo ID único de ImGui y lo devuelve por salida.
    /// <para>Gets a new unique ImGui ID and outputs it.</para>
    /// </summary>
    /// <param name="id">ID generado. <para>Generated ID.</para></param>
    /// <returns>Nuevo ID único. <para>New unique ID.</para></returns>
    public static int GetID(out int id) => id = Instance.Get();

    /// <summary>
    /// Libera un ID de ImGui previamente reservado.
    /// <para>Releases a previously reserved ImGui ID.</para>
    /// </summary>
    /// <param name="value">ID a liberar. <para>ID to release.</para></param>
    /// <returns>True si se liberó. <para>True if released.</para></returns>
    public static bool ReleaseID(int value) => Instance.Release(value);

    private static GetUniqueImGuiID _instance;
    /// <summary>
    /// Instancia única del generador de IDs.
    /// <para>Singleton instance of the ID generator.</para>
    /// </summary>
    public static GetUniqueImGuiID Instance { get => _instance == null ? _instance = new(1_000_000, 2_000_000) : _instance; }
    private static int _min;
    private static int _max;
    private static readonly Random _random = new();
    private static readonly HashSet<int> _used = new();

    /// <summary>
    /// Inicializa el generador con un rango de IDs.
    /// <para>Initializes the generator with an ID range.</para>
    /// </summary>
    /// <param name="min">Valor mínimo permitido. <para>Minimum allowed value.</para></param>
    /// <param name="max">Valor máximo permitido. <para>Maximum allowed value.</para></param>
    public GetUniqueImGuiID(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min no puede ser mayor que max");

        _min = min;
        _max = max;
    }

    /// <summary>
    /// Obtiene un número aleatorio no repetido.
    /// <para>Gets a non-repeated random number.</para>
    /// </summary>
    /// <returns>ID único. <para>Unique ID.</para></returns>
    public int Get()
    {
        if (_used.Count >= (_max - _min + 1))
            throw new InvalidOperationException("No hay números disponibles");

        int value;
        do
        {
            value = _random.Next(_min, _max + 1);
        }
        while (_used.Contains(value));

        _used.Add(value);
        ImGui.PushID(value);
        return value;
    }

    /// <summary>
    /// Libera un número para que pueda volver a usarse.
    /// <para>Releases a number so it can be used again.</para>
    /// </summary>
    /// <param name="value">Valor a liberar. <para>Value to release.</para></param>
    /// <returns>True si se liberó. <para>True if released.</para></returns>
    public bool Release(int value)
    {
        if (value < _min || value > _max)
            return false;

        ImGui.PopID();
        return _used.Remove(value);
    }

    /// <summary>
    /// Limpia todo el pool.
    /// <para>Clears the entire pool.</para>
    /// </summary>
    public void Reset()
    {
        _used.Clear();
    }
}
