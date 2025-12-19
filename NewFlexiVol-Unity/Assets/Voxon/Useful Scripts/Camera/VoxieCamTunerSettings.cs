using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxieCamTunerSettings : System.Object {

    public string filename = "";

    // CamTunerSettings
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float scaX, scaY, scaZ;

    public void setFilename(string go)
    {
        filename = go + "_VoxieCamTunerSettings.json";
    }

    public void save()
    {

        if (filename == "") return;

        string dataAsJson = JsonUtility.ToJson(this, true);
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();
        }

        File.WriteAllText(filePath, dataAsJson);
    }

    public void load()
    {
        if (filename == "") return;

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        string jsonString = File.ReadAllText(filePath);

        VoxieCamTunerSettings tmpCS = JsonUtility.FromJson<VoxieCamTunerSettings>(jsonString);



        posX = tmpCS.posX;
        posY = tmpCS.posY;
        posZ = tmpCS.posZ;
        rotX = tmpCS.rotX;
        rotY = tmpCS.rotY;
        rotZ = tmpCS.rotZ;
        rotW = tmpCS.rotW;
        scaX = tmpCS.scaX;
        scaY = tmpCS.scaY;
        scaZ = tmpCS.scaZ;

    }
}
