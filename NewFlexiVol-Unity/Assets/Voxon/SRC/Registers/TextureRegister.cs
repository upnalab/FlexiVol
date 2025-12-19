using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;

namespace Voxon
{
    public class TextureRegister : Singleton<TextureRegister> {
		[SerializeField]
        private Dictionary<String, RegisteredTexture> _register;

		public static bool Active;

		public void Enable()
		{
			this.OnEnable();
		}

		private void OnEnable()
		{
			Init();

			IFormatter formatter = new BinaryFormatter();


			string scene_path = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
			string scene_directory = Path.GetDirectoryName(scene_path).Replace("Assets/", "");
			string scene_filename = Path.GetFileNameWithoutExtension(scene_path);

			string textureRegisterPath = $"{Application.dataPath}/StreamingAssets/{scene_directory}/{scene_filename}-Textures.bin";

			if (!File.Exists(textureRegisterPath))
			{
				Active = true;
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes) Debug.Log($"TextureRegister.cs - No pre-generated textures found ( looked in path = {textureRegisterPath} )");
				return;
			}

			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes)  Debug.Log($"TextureRegister.cs - Loading TextureRegistered path = {textureRegisterPath} ");

			using (var s = new FileStream(textureRegisterPath, FileMode.Open))
			{
				try
				{
					// Prep for all meshes;
					byte[] total_textures_buf = new byte[sizeof(int)];
					s.Read(total_textures_buf, 0, sizeof(int));
					int td_count = BitConverter.ToInt32(total_textures_buf, 0);

					TextureData loaded_texture;

					byte[] td_size_buf, td_buffer;
					int packet_size;

					for (int i = 0; i < td_count; i++)
					{
						td_size_buf = new byte[sizeof(int)];
						s.Read(td_size_buf, 0, sizeof(int));

						/*
						{ // Debug
							int start = 0;
							int end = sizeof(int);
							System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
							for (int idx = start; idx < end; idx++)
							{
								hex.AppendFormat("{0:x2}", md_size_buf[idx]);
							}
							Debug.Log($"\tSizeBytes: \t{hex.ToString()}");
						}
						*/

						packet_size = System.BitConverter.ToInt32(td_size_buf, 0);
						// Debug.Log(packet_size);
						td_buffer = new byte[packet_size];
						s.Read(td_buffer, 0, packet_size);

						/*
						{ // Debug
							int start = 0;
							int end = packet_size;
							System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
							for (int idx = start; idx < end; idx++)
							{
								hex.AppendFormat("{0:x2}", md_buffer[idx]);
							}
							Debug.Log($"Packet_bytes: \t{hex.ToString()}");
						}
						*/

						loaded_texture = TextureData.fromByteArray(td_buffer);
						_register.Add(loaded_texture.name, new RegisteredTexture(ref loaded_texture));
					}
				}
				catch (SerializationException e)
				{
					if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General)  Debug.Log("TextureRegister.cs - Failed to serialize. Reason: " + e.Message);
					throw;
				}
			}

			Active = true;

		}

		private void Init()
		{
			_register = new Dictionary<string, RegisteredTexture>();
		}

		public tiletype get_tile(string texture_name)
		{
			if (_register.ContainsKey(texture_name))
			{
				return _register[(texture_name)].Texture;
			}

			throw new KeyNotFoundException();
		}


		public tiletype get_tile( ref Texture2D in_texture)
		{
#if UNITY_EDITOR
			if (!in_texture.name.StartsWith("Assets/"))
			{
				string path = UnityEditor.AssetDatabase.GetAssetPath(in_texture);
				if (!path.StartsWith("Library") && path != "")
				{
					if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General)
						Debug.LogWarning($"TextureRegister.cs - ({in_texture.name}){path} is not preprocessed!");
					in_texture.name = path;
				}
			}
#endif

			if (_register == null)
            {
                _register = new Dictionary<string, RegisteredTexture>();
            }

			RegisteredTexture rt;
            if (_register.ContainsKey(in_texture.name))
            {
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes) 
					Debug.Log($"TextureRegister.cs - Looking Up Texture: {in_texture.name}");
				rt = _register[in_texture.name];
                rt.Counter++;
                _register[in_texture.name] = rt;

                return rt.Texture;
            }
            else
            {
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes) 
					Debug.Log($"TextureRegister.cs - Building Texture: {in_texture.name}"); 
				rt = new RegisteredTexture(ref in_texture);
				_register.Add(in_texture.name, rt);
                return rt.Texture;
            }
        }

        public bool drop_tile(ref Texture2D texture)
        {
            if (_register == null || !_register.ContainsKey(texture.name)) return false;

			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes) 
				Debug.Log($"TextureRegister.cs - drop_tile() Dropping Tile / Texture: {texture.name}");
			
			RegisteredTexture rt = _register[texture.name];
            rt.Counter--;

            if(rt.Counter <= 0)
            {
                _register.Remove(texture.name);
                Marshal.FreeHGlobal(rt.Texture.first_pixel);
                return true;
            }

            _register[texture.name] = rt;
            return false;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();

            ClearRegister();
        }

        private void RemoveRegister(string textureName)
        {
            if (!_register.ContainsKey(textureName)) return;
        
            RegisteredTexture rt = _register[textureName];
            _register.Remove(textureName);
            Marshal.FreeHGlobal(rt.Texture.first_pixel);
        }

        public void ClearRegister()
        {
            if (_register == null)
                return;

            while (_register.Count > 0)
            {
                RemoveRegister(_register.ElementAt(0).Key);
            }
        }
        
        public tiletype refresh_tile(ref Texture2D tex)
        {
            if(_register == null)
            {
				if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes) Debug.Log($"TextureRegister.cs - New Dictionary Created - refresh_tile()");
                _register = new Dictionary<string, RegisteredTexture>();
            }

            if (!_register.ContainsKey(tex.name))
            {
                return get_tile(ref tex);
            } else
            {
                return RefreshTexture(ref tex);
            }
        }
        
        tiletype RefreshTexture(ref Texture2D texture)
        {
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes)  Debug.Log($"TextureRegister.cs - Refreshing Texture: " + texture.name);
            var reorderedTextures = new Texture2D(texture.width, texture.height, TextureFormat.BGRA32, false);

            Color32[] tCol = texture.GetPixels32();
            reorderedTextures.SetPixels32(tCol);

            RegisteredTexture rt = _register[texture.name];
            Marshal.Copy(reorderedTextures.GetRawTextureData(), 0, rt.Texture.first_pixel, reorderedTextures.GetRawTextureData().Length);
            _register[texture.name] = rt;

            Destroy(reorderedTextures);
            return rt.Texture;
        }

		public int Length()
		{
			return _register.Count;
		}

		private void Clear()
		{
			if (_register == null)
				return;

			while (_register.Count > 0)
			{
				RemoveRegister(_register.ElementAt(0).Key);
			}
		}

		public string[] Keys()
		{
			return _register.Keys.ToArray();
		}

		public new void OnApplicationQuit()
		{
			Active = false;
			Clear();
			base.OnApplicationQuit();
		}

#if UNITY_EDITOR
		public TextureData[] PackMeshes()
		{
			if (_register == null) return new TextureData[0];

			var rTs = new TextureData[_register.Count];
			var idx = 0;
			foreach (RegisteredTexture rt in _register.Values)
			{
				rTs[idx] = rt.GetTextureData();
				idx++;
			}

			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes)  Debug.Log($"TextureRegister.cs - PackMeshes() called RT Count: {rTs.Length}");

			return rTs;
		}
#endif
	}
}