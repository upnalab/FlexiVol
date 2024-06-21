using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class MeshRegister : Singleton<MeshRegister> {
		[FormerlySerializedAs("Register")][SerializeField]
		private Dictionary<string, RegisteredMesh> register;

		[FormerlySerializedAs("cshader_main")] public ComputeShader cshaderMain;
        public int kernelHandle;

        public static bool Active;

        public bool RENAME_MESHES = false;
        public static int MeshUniqueCount;

		public void Enable()
		{
			this.OnEnable();
		}

        private void OnEnable()
        {
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log("MeshRegister.cs - OnEnable called");

			Init();

            IFormatter formatter = new BinaryFormatter();


			string scene_path = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
			string scene_directory = Path.GetDirectoryName(scene_path).Replace("Assets/","");
			string scene_filename = Path.GetFileNameWithoutExtension(scene_path);

			string meshRegisterPath = $"{Application.dataPath}/StreamingAssets/{scene_directory}/{scene_filename}-Meshes.bin";


			if (!File.Exists(meshRegisterPath))
			{
				Active = true;
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes)  Debug.Log("MeshRegister.cs - No pre-generated meshes found");
				return;
			}
			// Debug.Log("Loading MeshRegistered");

			using (var s = new FileStream(meshRegisterPath, FileMode.Open))
			{
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Debug)
                    try
                    {
                        byte[] md_count_count_buf = new byte[sizeof(int)];
                        s.Read(md_count_count_buf, 0, sizeof(int));
                        int md_count = BitConverter.ToInt32(md_count_count_buf, 0);
                        MeshData loaded_mesh;
                        byte[] md_size_buf, md_buffer;
                        int packet_size;
                        for (int i = 0; i < md_count; i++)
                        {
                            md_size_buf = new byte[sizeof(int)];
                            s.Read(md_size_buf, 0, sizeof(int));
                            packet_size = System.BitConverter.ToInt32(md_size_buf, 0);
                            md_buffer = new byte[packet_size];
                            s.Read(md_buffer, 0, packet_size);
                            loaded_mesh = MeshData.fromByteArray(md_buffer);
                            register.Add(loaded_mesh.name, new RegisteredMesh(ref loaded_mesh));
                        }
                    }
                    catch (SerializationException e) { 
                        if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) Debug.Log("MesgRegister.cs - Failed to serialize. Reason: " + e.Message); 
                        throw;
                    }
			}

            Active = true;

        }

        private void Init()
        {
            register = new Dictionary<string, RegisteredMesh>();

            if (!Resources.Load("VCS"))
            {
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) Debug.Log("MesgRegister.cs - Failed to load VCS");
            }
            cshaderMain = (ComputeShader)Resources.Load("VCS");
            kernelHandle = cshaderMain.FindKernel("CSMain");
        }

		public RegisteredMesh get_registed_mesh(string mesh_name)
		{
			if (register.ContainsKey(mesh_name))
			{
				return register[(mesh_name)];
			}

			return null;
		}

        // function to check that mesh name doesn't already exist...
        public bool CheckMeshNameExists(string meshName)
        {
            if (register.ContainsKey(meshName)) return true;
            return false;
        }

        public RegisteredMesh get_registed_mesh(ref Mesh mesh)
        {
#if UNITY_EDITOR
			if(!mesh.name.StartsWith("Assets/"))
			{
				string path = UnityEditor.AssetDatabase.GetAssetPath(mesh);
				path += $":{mesh.name}";

				if (!path.StartsWith("Library"))
				{
					if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Debug)  Debug.LogWarning($"MeshRegister.cs - ({mesh.name}){path} is not preprocessed!");
					mesh.name = path;
				}
				
			}
#endif
			if (register == null)
            {
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes)  Debug.Log("MeshRegister.cs - Initialising Register");
                Init();
            }

            if (register.ContainsKey(mesh.name))
            {
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Debug)   Debug.Log($"MeshRegister.cs - Looking Up Mesh: {mesh.name}");
				RegisteredMesh rm = register[(mesh.name)];
                rm.Increment();

                return rm;
            }
            else
            {
             
				var rm = new RegisteredMesh(ref mesh);
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log($"MeshRegister.cs - Building Mesh: {mesh.name}");
                register.Add(mesh.name, rm);

                return rm;
            }
        }

        public bool drop_mesh(ref Mesh mesh)
        {
            if (register == null || !register.ContainsKey(mesh.name)) return false;
        
            RegisteredMesh rt = register[mesh.name];
            rt.Decrement();
            if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log($"MeshRegister.cs - drop_mesh() dropping mesh: {mesh.name}");

            /*
        if (!rt.isactive() && false)
        {
            register.Remove(mesh.name);
            rt.destroy();
        }
        */
            return true;
        }

        public int Length()
        {
            return register.Count;
        }

        public void Clear()
        {
            if (register == null)
                return;

            while (register.Count > 0)
            {
                RemoveRegister(register.ElementAt(0).Key);
            }
        }

		public string[] Keys()
		{
			return register.Keys.ToArray();
		}

        new void OnApplicationQuit()
        {
            Active = false;
            Clear();
            base.OnApplicationQuit();
        }

        new void OnDestroy()
        {
            Active = false;
            Clear();
            base.OnDestroy();
        }

        private void RemoveRegister(string meshName)
        {
            if (!register.ContainsKey(meshName)) return;
        
            RegisteredMesh rt = register[meshName];
            register.Remove(meshName);
            rt.Destroy();
        }

#if UNITY_EDITOR
        public MeshData[] PackMeshes()
        {
            var rMs = new MeshData[register.Count];
            var idx = 0;
            foreach(RegisteredMesh rm in register.Values)
            {
                rMs[idx] = rm.GetMeshData();
                idx++;
            }

			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes)  Debug.Log($"MeshRegister.cs - PackMeshes() RM Count: {rMs.Length}");

            return rMs;
        }
#endif
    }
}
