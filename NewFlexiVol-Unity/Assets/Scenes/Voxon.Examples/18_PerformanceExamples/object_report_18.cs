using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public struct MeshTracker
{
    public int instanceCount;
    public int triangleCount;
};
public class object_report_18 : MonoBehaviour
{
    // Update is called once per frame
    private void Start()
    {
        VXProcess.Instance._logger_max_lines = 1;
    }

    void Update()
    {
        Dictionary<string, MeshTracker> objects = new Dictionary<string, MeshTracker>();
        
        VXComponent[] components  = FindObjectsOfType<VXComponent>();

        foreach (var comp in components)
        {
            MeshFilter mf = comp.gameObject.GetComponent<MeshFilter>();
            string meshName;
            int triangle_count = 0;

            if (mf != null)
            {
                meshName = mf.name;
                triangle_count = mf.sharedMesh.triangles.Length;
            }
            else
            {
                SkinnedMeshRenderer smr = comp.gameObject.GetComponent<SkinnedMeshRenderer>(); 
                meshName = smr.name;
                triangle_count = smr.sharedMesh.triangles.Length;
            }
             
            if (objects.ContainsKey(meshName))
            {
                MeshTracker mt = objects[meshName];
                mt.instanceCount += 1;
                objects[meshName] = mt;
            }
            else
            {
                objects[meshName] = new MeshTracker() {instanceCount = 1, triangleCount = triangle_count};
            }
        }

        string output = "";

        foreach (var comp in objects)
        {
            output += $"{comp.Value.instanceCount}   {comp.Key}: {comp.Value.triangleCount} triangles\n";
        }
        
        VXProcess.add_log_line(output);
        
    }
}