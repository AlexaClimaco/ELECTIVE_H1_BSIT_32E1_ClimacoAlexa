using ResolutionsApi.Models;

namespace ResolutionsApi.Data;

public static class ResolutionStore
{
    // In-memory list of resolutions
    public static List<Resolution> Resolutions { get; set; } = new List<Resolution>();

    // Auto-increment ID
    private static int _nextId = 1;
    public static int NextId => _nextId++;
}
