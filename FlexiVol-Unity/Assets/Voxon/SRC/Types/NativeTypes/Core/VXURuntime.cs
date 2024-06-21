using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UnityEngine;

// UNITY VERSION OF C# RUNTIME

namespace Voxon
{
	public enum ColorMode
	{
		// Dual Color - Currently Disabled
		BG = 4,
		RG = 3,
		RB = 2,
		// Full Color
		RGB = 1,
		// Monochrome
		WHITE = 0,
		RED = -1,
		GREEN = -2,
		YELLOW = -3,
		BLUE = -4,
		MAGENTA = -5,
		CYAN = -6
	};

	public enum MENU_BUTTON_TYPE
	{
		SINGLE = 3, // single use button
		FIRST = 1, // first button connected
		MIDDLE = 0, // middle button can be multiple
		END = 2, // end of the string of connected buttons
		TOGGLE = 8, // special button that can toggle its text
		FILE_PICKER = 9, // file picker to load in a file type
	}

	public enum VOXON_RUNTIME_INTERFACE
	{
		LEGACY,
		EXTENDED,
		//VxL, 

	}

	/// <summary>
	/// Display state representation
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct voxie_disp_t
	{
		//settings for quadrilateral (keystone) compensation (use voxiedemo mode 2 to calibrate)
		public point2d keyst0, keyst1, keyst2, keyst3, keyst4, keyst5, keyst6, keyst7;
		public int colo_r, colo_g, colo_b; //initial values at startup for rgb color mode
		public int mono_r, mono_g, mono_b; //initial values at startup for mono mode
		public int mirrorx, mirrory;       //projector hardware flipping (I suggest avoiding this)
	}


	/// <summary>
	/// Representation of frame buffer
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct voxie_frame_t
	{
		public IntPtr f;              //Pointer to top-left-up of current frame to draw
		public IntPtr p;              //Number of bytes per horizontal line (x)
		public IntPtr fp;             //Number of bytes per 24-plane frame (1/3 of screen)
		public int x, y;               //Width and height of viewport
		public int flags;             //Tells whether color mode is selected
		public int drawplanes;         //Tells how many planes to draw
		public int x0, y0, x1, y1;     //Viewport extents
		public float xmul, ymul, zmul; //Transform for medium and high level graphics functions..
		public float xadd, yadd, zadd; //Transform is: actual_x = passed_x * xmul + xadd
		public tiletype f2d;
	}






	public class VXURuntime : IRuntimePromise, IRuntimePromiseExtended
	{
		private string _pluginFilePath = "";
		private const string PluginFileName = "C#-Runtime.dll";

		private string PluginTypeName = "Voxon.Runtime";
		private string FeatureDictionaryName = "GetFeatures";
		private VOXON_RUNTIME_INTERFACE TypeOfRuntime = VOXON_RUNTIME_INTERFACE.LEGACY;

		public string ActiveRuntime;

		private static Type _tClassType;
		private static object _runtime;

		private static Dictionary<string, MethodInfo> _features;

		public VXURuntime(VOXON_RUNTIME_INTERFACE _TypeOfRuntime)
		{

			TypeOfRuntime = _TypeOfRuntime;

			switch (TypeOfRuntime)
			{
				default:
				case VOXON_RUNTIME_INTERFACE.LEGACY:
					PluginTypeName = "Voxon.Runtime";
					FeatureDictionaryName = "GetFeatures";
					break;
				case VOXON_RUNTIME_INTERFACE.EXTENDED:
					PluginTypeName = "Voxon.RuntimeExtended";
					FeatureDictionaryName = "GetFeatures";
					break;

			}


			_features = new Dictionary<string, MethodInfo>();
			FindDll();

			if (!System.IO.File.Exists(_pluginFilePath))
			{
				Debug.LogWarning("C#-Runtime.dll not found in Runtime directory.\nPlease ensure Voxon Runtime is correctly installed");
				Windows.Error("C#-Runtime.dll not found in Runtime directory.\nPlease ensure Voxon Runtime is correctly installed");
				_runtime = null;
				Application.Quit();
			}

			Assembly asm = Assembly.LoadFrom(_pluginFilePath);
			_tClassType = asm.GetType(PluginTypeName);
			ActiveRuntime = PluginTypeName;
			if (_tClassType == null)
			{
				Debug.LogWarning("Voxon Runtime failed to load from local C#-Runtime.dll.");
				Windows.Error("Voxon Runtime failed to load from local C#-Runtime.dll.");
				_runtime = null;
				Application.Quit();
			}

			try
			{
				_runtime = Activator.CreateInstance(_tClassType);

				MethodInfo makeRequestMethod = _tClassType.GetMethod(FeatureDictionaryName);
				if (makeRequestMethod == null) return;

				var featureNames = (HashSet<string>)makeRequestMethod.Invoke(_runtime, null);

				foreach (string feature in featureNames)
				{
					_features.Add(feature, _tClassType.GetMethod(feature));
				}
			}
			catch (Exception e)
			{
				Windows.Alert($"Voxon Runtime Bridge failed to load.\nCheck your version of C#-bridge-interface.dll\n{e}");
				Application.Quit();
			}


		}

		private void FindDll()
		{
			if (_pluginFilePath != "") return;

			if (File.Exists(PluginFileName))
			{
				_pluginFilePath = PluginFileName;
				return;
			}

#if NET_4_6
			RegistryKey dll = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon");
			if (dll != null)
			{
				_pluginFilePath =
					$"{(string)Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon")?.GetValue("Path")}{PluginFileName}";
				return;
			}
#endif

			string voxon_path = Environment.GetEnvironmentVariable("VOXON_RUNTIME", EnvironmentVariableTarget.User);

			if (File.Exists($"{voxon_path}{PluginFileName}"))
			{
				_pluginFilePath = $"{voxon_path}{PluginFileName}";
				return;
			}
			else
			{
				Debug.Log($"{voxon_path}{PluginFileName} Doesn't Exist");

			}

			string[] paths = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User)?.Split(';');

			if (paths == null) return;

			foreach (string path in paths)
			{
				if (!File.Exists($"{path}\\{PluginFileName}")) continue;

				_pluginFilePath = $"{path}\\{PluginFileName}";
			}
		}


		public double GetTime()
		{

			return (double)_features["GetTime"].Invoke(_runtime, null);
		}

		// TODO is this right?

		public HashSet<string> GetFeatures()
		{
			return (HashSet<string>)_features["DrawPolygon"].Invoke(_runtime, null);
		}

		public void DrawBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ, int fill, int colour)
		{
			point3d min = new point3d(minX, minY, minZ);
			point3d max = new point3d(maxX, maxY, maxZ);

			DrawBox(ref min, ref max, fill, colour);

		}



		public void DrawBox(ref point3d min, ref point3d max, int fill, int colour)
		{
			_features["DrawBox"].Invoke(_runtime, parameters: new object[] { min, max, fill, colour });
		}

		public void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int fillMode, int col)
		{
			var paras = new object[] { pp, pr, pd, pf, fillMode, col };
			_features["DrawCube"].Invoke(_runtime, paras);
		}

		public void DrawGuidelines()
		{
			_features["DrawGuidelines"].Invoke(_runtime, null);
		}

		public void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int colorkey, int minHeight, int flags)
		{
			var paras = new object[] { texture, pp, pr, pd, pf, colorkey, minHeight, flags };
			_features["DrawHeightmap"].Invoke(_runtime, paras);
		}

		public void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, int col, byte[] text)
		{
			var paras = new object[] { pp, pr, pd, col, text };
			_features["DrawLetters"].Invoke(_runtime, paras);
		}

		public void DrawLine(float x0, float y0, float z0, float x1, float y1, float z1, int col)
		{
			point3d lineStart = new point3d(x0, y0, z0);
			point3d lineEnd = new point3d(x1, y1, z1);
			DrawLine(ref lineStart, ref lineEnd, col);
		}

		public void DrawLine(ref point3d min, ref point3d max, int col)
		{
			var paras = new object[] { min, max, col };
			_features["DrawLine"].Invoke(_runtime, paras);
		}

		public void DrawPolygon(pol_t[] pt, int ptCount, int col)
		{
			var paras = new object[] { pt, ptCount, col };
			_features["DrawPolygon"].Invoke(_runtime, paras);
		}

		public void DrawSphere(float x, float y, float z, float radius, int issol, int colour)
		{
			point3d position = new point3d(x, y, z);

			var paras = new object[] { position, radius, issol, colour };
			_features["DrawSphere"].Invoke(_runtime, paras);
		}

		public void DrawSphere(ref point3d position, float radius, int issol, int colour)
		{
			var paras = new object[] { position, radius, issol, colour };
			_features["DrawSphere"].Invoke(_runtime, paras);
		}

		public void DrawSphereBulk(poltex[] vertices, float radius)
		{
			var paras = new object[] { vertices, radius };
			_features["DrawSphereBulk"].Invoke(_runtime, paras);
		}

		public void DrawSphereBulkCnt(poltex[] vertices, float radius, int count)
		{
			var paras = new object[] { vertices, radius, count };
			_features["DrawSphereBulkCnt"].Invoke(_runtime, paras);
		}

		public void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags)
		{
			var paras = new object[] { texture, vertices, verticeCount, indices, indiceCount, flags };
			_features["DrawTexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawLitTexturedMesh(ref tiletype texture, poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags, int ambient_color = 0x040404)
		{
			var paras = new object[] { texture, vertices, verticeCount, indices, indiceCount, flags, ambient_color };
			_features["DrawLitTexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawUntexturedMesh(poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags, int colour)
		{
			var paras = new object[] { vertices, verticeCount, indices, indiceCount, flags, colour };
			_features["DrawUntexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawVoxel(float x, float y, float z, int col)
		{
			point3d pos = new point3d(x, y, z);
			DrawVoxel(ref pos, col);

		}

		public void DrawVoxel(ref point3d position, int col)
		{
			var paras = new object[] { position, col };
			_features["DrawVoxel"].Invoke(_runtime, paras);
		}

		public void DrawVoxelBatch(ref point3d[] positions, int voxel_count, int colour)
		{
			// colour
			var paras = new object[] { positions, voxel_count, colour };
			_features["DrawVoxelBatch"].Invoke(_runtime, paras);
		}

		public void DrawVoxels(ref point3d[] positions, int voxel_count, ref int[] colours)
		{
			var paras = new object[] { positions, voxel_count, colours };
			_features["DrawVoxels"].Invoke(_runtime, paras);
		}

		public void SetFlag(int value, int flag)
		{
			var paras = new object[] { value, flag };
			_features["SetFlag"].Invoke(_runtime, paras);
		}

		public int GetFlags()
		{
			return (int)_features["GetFlags"].Invoke(_runtime, null);
		}

		public bool IsFlagSet(int flagID)
		{
			var paras = new object[] { flagID };
			return (bool)_features["IsFlagSet"].Invoke(_runtime, paras);
		}

		public void Report(String reportType, int posX, int posY)
		{

			var paras = new object[] { posX, posY };
			_features["Report" + reportType].Invoke(_runtime, paras);
		}


		public void FrameEnd()
		{
			_features["FrameEnd"].Invoke(_runtime, null);
		}

		public bool FrameStart()
		{
			return (bool)_features["FrameStart"].Invoke(_runtime, null);
		}

		public float[] GetAspectRatio()
		{
			return (float[])_features["GetAspectRatio"].Invoke(_runtime, null);

		}

		public float GetAxis(int axis, int player)
		{
			var paras = new object[] { axis, player };
			return (float)_features["GetAxis"].Invoke(_runtime, paras);
		}

		public bool GetButton(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButton"].Invoke(_runtime, paras);

		}

		public bool GetButtonDown(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonDown"].Invoke(_runtime, paras);
		}

		public bool GetButtonUp(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonUp"].Invoke(_runtime, paras);
		}



		// return true if Key is pressed. Use VX Key codes enum VX_KEY
		public bool GetKey(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKey"].Invoke(_runtime, paras);
		}
		// returns true if Key is just pressed. Use Voxon Key codes enum VX_KEY
		public bool GetKeyDown(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyDown"].Invoke(_runtime, paras);

		}
		// returns the amount of time a has been pressed, odes enum VX_KEY
		public double GetKeyDownTime(int keycode)
		{
			var paras = new object[] { keycode };
			return (double)_features["GetKeyDownTime"].Invoke(_runtime, paras);

		}

		public int GetKeyState(int keycode)
		{
			var paras = new object[] { keycode };
			return (int)_features["GetKeyState"].Invoke(_runtime, paras);
		}

		public bool GetKeyUp(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyUp"].Invoke(_runtime, paras);
		}


		// Some chaining to make it more friendly to use

		public bool GetKey(VX_KEYS keycode)
		{
			return (bool)GetKey((int)keycode);
		}

		public bool GetKeyDown(VX_KEYS keycode)
		{
			return (bool)GetKeyDown((int)keycode);
		}

		public double GetKeyDownTime(VX_KEYS keycode)
		{
			return (double)GetKeyDownTime((int)keycode);

		}
		public int GetKeyState(VX_KEYS keycode)
		{
			return (int)GetKeyState((int)keycode);
		}
		public bool GetKeyUp(VX_KEYS keycode)
		{
			return (bool)GetKeyUp((int)keycode);
		}









		public bool GetMouseButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButton"].Invoke(_runtime, paras);
		}

		public bool GetMouseButtonDown(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButtonDown"].Invoke(_runtime, paras);
		}

		public float[] GetMousePosition()
		{
			return (float[])_features["GetMousePosition"].Invoke(_runtime, null);
		}

		public float[] GetSpaceNavPosition()
		{
			return (float[])_features["GetSpaceNavPosition"].Invoke(_runtime, null);
		}

		public float[] GetSpaceNavRotation()
		{
			return (float[])_features["GetSpaceNavRotation"].Invoke(_runtime, null);
		}

		public bool GetSpaceNavButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetSpaceNavButton"].Invoke(_runtime, paras);
		}

		public float GetVolume()
		{
			return (float)_features["GetVolume"].Invoke(_runtime, null);
		}

		public void Initialise(int type)
		{
			var paras = new object[] { type };
			_features["Initialise"].Invoke(_runtime, paras);
		}

		public bool isInitialised()
		{
			return (bool)_features["isInitialised"].Invoke(_runtime, null);
		}

		public bool isLoaded()
		{
			return (bool)_features["isLoaded"].Invoke(_runtime, null);
		}

		public void Load()
		{
			_features["Load"].Invoke(_runtime, null);
		}

		public void LogToFile(string msg)
		{
			var paras = new object[] { msg };
			_features["LogToFile"].Invoke(_runtime, paras);
		}

		public void LogToScreen(int x, int y, string text)
		{
			var paras = new object[] { x, y, text };
			_features["LogToScreen"].Invoke(_runtime, paras);
		}

		public void LogToScreenExt(int x, int y, int colFG, int colBG, string text)
		{
			var paras = new object[] { x, y, colFG, colBG, text };
			_features["LogToScreenExt"].Invoke(_runtime, paras);
		}

		public void SetAspectRatio(float aspx, float aspy, float aspz)
		{
			var paras = new object[] { aspx, aspy, aspz };
			_features["SetAspectRatio"].Invoke(_runtime, paras);
		}

		public void SetColorMode(int colour)
		{
			var paras = new object[] { colour };
			_features["SetColorMode"].Invoke(_runtime, paras);
		}

		public int GetColorMode()
		{
			return (int)_features["isInitialised"].Invoke(_runtime, null);
		}

		public void SetDisplayColor(ColorMode color)
		{
			SetColorMode((int)color);
		}

		public ColorMode GetDisplayColor()
		{
			return (ColorMode)GetColorMode();
		}

		public void Shutdown()
		{
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes)
				Debug.Log("Runtime.cs - Shutdown() function called... preparing Runtime.cs");

			_features["Shutdown"].Invoke(_runtime, null);
		}

		public void Unload()
		{
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes)
				Debug.Log("Runtime.cs - Unload() function called... Unloading DLL");

			_features["Unload"].Invoke(_runtime, null);
		}

		public long GetDLLVersion()
		{
			return (long)_features["GetDLLVersion"].Invoke(_runtime, null);
		}

		public string GetSDKVersion()
		{
			return (string)_features["GetSDKVersion"].Invoke(_runtime, null);
		}

		public void SetDotSize(int dotSize)
		{
			var paras = new object[] { dotSize };
			_features["SetDotSize"].Invoke(_runtime, paras);
		}

		public void SetDotSize(int rValue,int gValue,int  bValue)
		{
			int dotSize =    (int)((rValue << 16) | (gValue << 8) | (bValue));
			SetDotSize(dotSize);
		}


		public int GetDotSize()
		{
			return (int)_features["GetDotSize"].Invoke(_runtime, null);
		}

		public void SetGamma(float gamma)
		{
			var paras = new object[] { gamma };
			_features["SetGamma"].Invoke(_runtime, paras);
		}

		public float GetGamma()
		{
			return (float)_features["GetGamma"].Invoke(_runtime, null);
		}

		public void SetDensity(float density)
		{
			var paras = new object[] { density };
			_features["SetDensity"].Invoke(_runtime, paras);
		}

		public float GetDensity()
		{
			return (float)_features["GetDensity"].Invoke(_runtime, null);
		}

		public void DisableNormalLighting()
		{
			_features["DisableNormalLighting"].Invoke(_runtime, null);
		}

		public void SetNormalLighting(float x, float y, float z)
		{
			var paras = new object[] { x, y, z };
			_features["SetNormalLighting"].Invoke(_runtime, paras);
		}

		public bool SetDisplay2D(int hibernateLeds = -1)
		{
			var paras = new object[] { hibernateLeds };
			return (bool)_features["SetDisplay2D"].Invoke(_runtime, paras);
		}

		public bool SetDisplay3D()
		{

			return (bool)_features["SetDisplay3D"].Invoke(_runtime, null);
		}

		#region Clipshape Controls
		public int GetClipShape()
		{
			//	return _features.ContainsKey("GetClipShape") && (int) _features["GetClipShape"].Invoke(_runtime, null);
			return (int)_features["GetClipShape"].Invoke(_runtime, null);
		}

		public void SetClipShape(int newValue)
		{
			if (_features.ContainsKey("SetClipShape"))
			{
				var paras = new object[] { newValue };
				_features["SetClipShape"].Invoke(_runtime, paras);
			}
		}

		public float GetExternalRadius()
		{
			if (_features.ContainsKey("GetExternalRadius"))
			{
				return (float)_features["GetExternalRadius"].Invoke(_runtime, null);
			}

			return 0.0f;
		}

		public void SetExternalRadius(float radius)
		{
			if (_features.ContainsKey("SetExternalRadius"))
			{
				var paras = new object[] { radius };
				_features["SetExternalRadius"].Invoke(_runtime, paras);
			}
		}

		public float GetInternalRadius()
		{
			if (_features.ContainsKey("GetInternalRadius"))
			{
				return (float)_features["GetInternalRadius"].Invoke(_runtime, null);
			}

			return 0.0f;
		}

		public void SetInternalRadius(float radius)
		{
			if (_features.ContainsKey("SetInternalRadius"))
			{
				var paras = new object[] { radius };
				_features["SetInternalRadius"].Invoke(_runtime, paras);
			}
		}

		#endregion

		#region Menu Controls
		public void MenuReset(MenuUpdateHandler menuUpdate, IntPtr userdata)
		{
			var paras = new object[] { menuUpdate, userdata };
			_features["MenuReset"].Invoke(_runtime, paras);
		}

		public void MenuAddTab(string text, int x, int y, int width, int height)
		{
			var paras = new object[] { text, x, y, width, height };
			_features["MenuAddTab"].Invoke(_runtime, paras);
		}

		public void MenuAddText(int id, string text, int x, int y, int width, int height, int colour)
		{
			var paras = new object[] { id, text, x, y, width, height, colour };
			_features["MenuAddText"].Invoke(_runtime, paras);
		}

		public void MenuAddButton(int id, string text, int x, int y, int width, int height, int colour, int position)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, position };
			_features["MenuAddButton"].Invoke(_runtime, paras);
		}

		public void MenuAddVerticleSlider(int id, string text, int x, int y, int width, int height, int colour, double initial_value,
			double min, double max, double minor_step, double major_step)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, initial_value, min, max, minor_step, major_step };
			_features["MenuAddVerticleSlider"].Invoke(_runtime, paras);
		}

		public void MenuAddHorizontalSlider(int id, string text, int x, int y, int width, int height, int colour, double initial_value,
			double min, double max, double minor_step, double major_step)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, initial_value, min, max, minor_step, major_step };
			_features["MenuAddHorizontalSlider"].Invoke(_runtime, paras);
		}

		public void MenuAddEdit(int id, string text, int x, int y, int width, int height, int colour, bool hasFollowupButton = false)
		{
			var paras = new object[] { id, text, x, y, width, height, colour, hasFollowupButton };
			_features["MenuAddEdit"].Invoke(_runtime, paras);
		}

		public void MenuUpdateItem(int id, string st, int down, double v)
		{
			var paras = new object[] { id, st, down, v };
			_features["MenuUpdateItem"].Invoke(_runtime, paras);
		}
		#endregion

		#region Emulator
		public float SetEmulatorHorizontalAngle(float radians)
		{
			var paras = new object[] { radians };
			return (float)_features["SetEmulatorHorizontalAngle"].Invoke(_runtime, paras);
		}

		public float SetEmulatorVerticalAngle(float radians)
		{
			var paras = new object[] { radians };
			return (float)_features["SetEmulatorVerticalAngle"].Invoke(_runtime, paras);
		}

		public float SetEmulatorDistance(float distance)
		{
			var paras = new object[] { distance };
			return (float)_features["SetEmulatorDistance"].Invoke(_runtime, paras);
		}

		public float GetEmulatorHorizontalAngle()
		{
			return (float)_features["GetEmulatorHorizontalAngle"].Invoke(_runtime, null);
		}

		public float GetEmulatorVerticalAngle()
		{
			return (float)_features["GetEmulatorVerticalAngle"].Invoke(_runtime, null);
		}

		public float GetEmulatorDistance()
		{
			return (float)_features["GetEmulatorDistance"].Invoke(_runtime, null);
		}

		public void StartRecording(string filename, int vps)
		{
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General) {
			  Debug.Log($"Runtime.cs - StartRecording() function called writing: {filename}");
			}

				var paras = new object[] { filename, vps };
			_features["StartRecording"].Invoke(_runtime, paras);
		}

		public void EndRecording()
		{
			if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General)
			{
				Debug.Log($"Runtime.cs - EndRecording() function called");
			}

			Debug.Log("EndRecording");
			_features["EndRecording"].Invoke(_runtime, null);
		}

		public void GetVCB(string filename, int vps)
		{
			var paras = new object[] { filename, vps };
			_features["GetVCB"].Invoke(_runtime, paras);
		}

		#endregion


		/*
		 * EXTENDED BRIDGE FUNCTIONS
		 */
		#region ExtendedRuntime
		private bool isInterfaceExtended()
		{
			if (this.TypeOfRuntime == VOXON_RUNTIME_INTERFACE.EXTENDED) return true;
			else return false;
		}

		string NotSupportedBridgeMsg = "Function is not supported with this Voxon x Unity Interface";

		//NEW
		public IntPtr GetVoxieHandle()
		{
			if (!isInterfaceExtended())
			{
				Debug.LogWarning(NotSupportedBridgeMsg);
				return (IntPtr)null;
			}

			return (IntPtr)_features["GetVoxieHandle"].Invoke(_runtime, null);
		}

		public voxie_wind_t GetVoxieWindow()
		{
			voxie_wind_t wind = new voxie_wind_t();
			if (!isInterfaceExtended())
			{
				Debug.LogWarning(NotSupportedBridgeMsg);
				return wind;
			}

			object obj = _features["GetVoxieWindow"].Invoke(_runtime, null);
			wind = (voxie_wind_t)obj;

			return wind;
		}

		// VoxieExtended
		public void UpdateVoxieWindow()
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }
			_features["UpdateVoxieWindow"].Invoke(_runtime, null);
		}

		// VoxieExtended
		public void ReplaceVoxieWindow(ref voxie_wind_t voxieWind)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }
			var paras = new object[] { voxieWind };
			_features["ReplaceVoxieWindow"].Invoke(_runtime, paras);
		}

		public void SetView(float x0, float y0, float z0, float x1, float y1, float z1)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }
			var paras = new object[] { x0, y0, z0, x1, y1, z1 };
			_features["SetView"].Invoke(_runtime, paras);
		}

		public void SetMaskPlane(float x0, float y0, float z0, float nx, float ny, float nz)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x0, y0, z0, nx, ny, nz };
			_features["SetMaskPlane"].Invoke(_runtime, paras);
		}

		public void MountZip(string fileName)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { fileName };
			_features["MountZip"].Invoke(_runtime, paras);
		}

		public void FreeFromCache(string fileName)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { fileName };
			_features["FreeFromCache"].Invoke(_runtime, paras);
		}

		public int PlaySound(string fileName, int sourceChannel, int volumeLeft, int volumeRight, float playBackRate)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return -1; }
			var paras = new object[] { fileName, sourceChannel, volumeLeft, volumeRight, playBackRate };
			return (int)_features["PlaySound"].Invoke(_runtime, paras);
		}

		public void PlaySoundUpdate(int handle, int sourceChannel, int volumeLeft, int volumeRight, float playBackRate)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { handle, sourceChannel, volumeLeft, volumeRight, playBackRate };
			_features["PlaySoundUpdate"].Invoke(_runtime, paras);
		}

		public int DrawSprite(string fileName, ref point3d pos, ref point3d rVec, ref point3d dVec, ref point3d fVec, int colour)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return 0; }

			var paras = new object[] { fileName, pos, rVec, dVec, fVec, colour };
			return (int)_features["DrawSprite"].Invoke(_runtime, paras);
		}

		public int DrawSpriteExtended(string fileName, ref point3d pos, ref point3d rVec, ref point3d dVec, ref point3d fVec, int colour, float forcescale, float fdrawratio = 1, int flags = 0)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return 0; }

			var paras = new object[] { fileName, pos, rVec, dVec, fVec, colour, forcescale, fdrawratio, flags };
			return (int)_features["DrawSpriteExtended"].Invoke(_runtime, paras);
		}

		public void DrawCone(ref point3d startPoint, float startPointradius, ref point3d endPoint, float endPointRadius, int fillmode, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			DrawCone(startPoint.x, startPoint.y, startPoint.z, startPointradius, endPoint.x, endPoint.y, endPoint.z, endPointRadius, fillmode, col);
		}

		public void DrawCone(float x0, float y0, float z0, float r0, float x1, float y1, float z1, float r1, int fillmode, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x0, y0, z0, r0, x1, y1, z1, r1, fillmode, col };
			_features["DrawCone"].Invoke(_runtime, paras);
		}

		public int KeyRead()
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return 0; }
			return (int)_features["KeyRead"].Invoke(_runtime, null);
		}

		public void ScreenDrawPix(int x, int y, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }
			var paras = new object[] { x, y, col };
			_features["ScreenDrawPix"].Invoke(_runtime, paras);
		}

		public void ScreenDrawHLine(int x0, int x1, int y, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x0, x1, y, col };
			_features["ScreenDrawHLine"].Invoke(_runtime, paras);
		}

		public void ScreenDrawLine(int x0, int y0, int x1, int y1, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x0, y0, x1, y1, col };
			_features["ScreenDrawLine"].Invoke(_runtime, paras);
		}

		public void ScreenDrawCircle(int xc, int yc, int r, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { xc, yc, r, col };
			_features["ScreenDrawCircle"].Invoke(_runtime, paras);
		}

		public void ScreenDrawRectangleFill(int x0, int y0, int x1, int y1, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x0, y0, x1, y1, col };
			_features["ScreenDrawRectangleFill"].Invoke(_runtime, paras);
		}

		public void ScreenDrawCircleFill(int x, int y, int r, int col)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { x, y, r, col };
			_features["ScreenDrawCircleFill"].Invoke(_runtime, paras);
		}

		public void ScreenDrawTile(ref tiletype source, int xpos, int ypos)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { source, xpos, ypos };
			_features["ScreenDrawTile"].Invoke(_runtime, paras);
		}

		public void ShutdownAdv(int uninitType = 0)
		{
			if (!isInterfaceExtended()) { Debug.LogWarning(NotSupportedBridgeMsg); return; }

			var paras = new object[] { uninitType };
			_features["ShutdownAdv"].Invoke(_runtime, paras);
		}
		#endregion
	}
}
