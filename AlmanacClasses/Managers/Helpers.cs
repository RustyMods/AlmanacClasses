namespace AlmanacClasses.Managers;

public static class Helpers
{
    public static bool HasFlagFast(this Heightmap.Biome value, Heightmap.Biome flag)
    {
        return (value & flag) != 0;
    }
}