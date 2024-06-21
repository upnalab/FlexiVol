using UnityEngine;
// ReSharper disable CheckNamespace

public static class Vector3Extension{

	public static Voxon.point3d ToPoint3d(this Vector3 v3)
    {
        Voxon.point3d p3d = new Voxon.point3d {x = v3.x, y = -v3.z, z = -v3.y};
        return p3d; 
    }

    public static Voxon.poltex ToPoltex(this Vector3 v3)
    {
        Voxon.poltex pt = new Voxon.poltex() {x = v3.x, y = -v3.z, z = -v3.y};
        return pt;
    }
}
