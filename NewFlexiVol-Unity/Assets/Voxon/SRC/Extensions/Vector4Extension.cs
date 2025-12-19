using UnityEngine;
// ReSharper disable CheckNamespace

public static class Vector4Extension
{

    public static Voxon.point3d ToPoint3d(this Vector4 v4)
    {
        Voxon.point3d p3d = new Voxon.point3d {x = v4.x, y = -v4.z, z = -v4.y};

        return p3d;
    }
    
    public static Voxon.poltex ToPoltex(this Vector4 v4)
    {
        Voxon.poltex p3d = new Voxon.poltex() {x = v4.x, y = -v4.z, z = -v4.y};

        return p3d;
    }
    
    public static Voxon.poltex ToPoltex(this Vector4 v4, float u, float v)
    {
        Voxon.poltex p3d = new Voxon.poltex() {x = v4.x, y = -v4.z, z = -v4.y, u = u, v = v};

        return p3d;
    }
}
