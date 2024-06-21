using UnityEngine;
// ReSharper disable CheckNamespace

public static class Point3dExtension{
    public static Vector3 ToVector3(this Voxon.point3d p3d)
    {
        return new Vector3(p3d.x, p3d.y, p3d.z);
    }

    public static Voxon.point3d Assign(this Voxon.point3d p3d, Vector3 v3)
    {
        p3d.x = v3.x;
        p3d.y = -v3.z;
        p3d.z = -v3.y;
        
        return p3d; 
    }
}
