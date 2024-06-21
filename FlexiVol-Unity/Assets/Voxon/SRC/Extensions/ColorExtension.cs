using UnityEngine;
// ReSharper disable CheckNamespace

public static class ColorExtension {
    public static int ToInt(this Color col)
    {
        Color32 a = col;
        return (a.r << 16) | (a.g << 8) | a.b;
    }

    public static int ToInt(this Color32 col)
    {
        return (col.r << 16) | (col.g << 8) | col.b;
    }
}
