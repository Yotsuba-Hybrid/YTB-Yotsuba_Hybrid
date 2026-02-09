using ImGuiNET;
using System;
using System.Collections.Generic;

/// <summary>
/// Generates unique ImGui identifiers within a configured range.
/// Genera identificadores ImGui únicos dentro de un rango configurado.
/// </summary>
public sealed class GetUniqueImGuiID
{
    /// <summary>
    /// Gets a new unique ImGui ID.
    /// Obtiene un nuevo ID único de ImGui.
    /// </summary>
    public static int GetID() => Instance.Get();

    /// <summary>
    /// Gets a new unique ImGui ID and outputs it.
    /// Obtiene un nuevo ID único de ImGui y lo devuelve por salida.
    /// </summary>
    /// <param name="id">Generated ID. ID generado.</param>
    public static int GetID(out int id) => id = Instance.Get();

    /// <summary>
    /// Releases a previously reserved ImGui ID.
    /// Libera un ID de ImGui previamente reservado.
    /// </summary>
    /// <param name="value">ID to release. ID a liberar.</param>
    public static bool ReleaseID(int value) => Instance.Release(value);

    private static GetUniqueImGuiID _instance;
    /// <summary>
    /// Singleton instance of the ID generator.
    /// Instancia única del generador de IDs.
    /// </summary>
    public static GetUniqueImGuiID Instance { get => _instance == null ? _instance = new(1_000_000, 2_000_000) : _instance; }
    private static int _min;
    private static int _max;
    private static readonly Random _random = new();
    private static readonly HashSet<int> _used = new();

    public GetUniqueImGuiID(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min no puede ser mayor que max");

        _min = min;
        _max = max;
    }

    /// <summary>
    /// Obtiene un número aleatorio no repetido
    /// </summary>
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
    /// Libera un número para que pueda volver a usarse
    /// </summary>
    public bool Release(int value)
    {
        if (value < _min || value > _max)
            return false;

        ImGui.PopID();
        return _used.Remove(value);
    }

    /// <summary>
    /// Limpia todo el pool
    /// </summary>
    public void Reset()
    {
        _used.Clear();
    }
}
