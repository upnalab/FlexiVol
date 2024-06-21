using UnityEngine;
// ReSharper disable CheckNamespace

public static class PoltexExtension{
    public static Vector3 ToVector3(this Voxon.poltex p3d)
    {
        return new Vector3(p3d.x, -p3d.z, -p3d.y);
    }
}
