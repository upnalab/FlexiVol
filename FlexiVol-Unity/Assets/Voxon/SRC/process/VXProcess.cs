//#define DEBUG_VXPROCESS // Debug for internal

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


namespace Voxon
{

    public enum VXLaunchType
    {
        Standard = 0,    // Standard                         - Run VXApp with all features. Warning VoxieBox.dll is unstable when its using its own sound engine.  
        Unity = 1,    // VxUnity App                      - Run VXApp without the VoxieBox.dll sound engine (Unity VxApps use Unity's sound engine 
        UnityEditor = 3,    // Testing VxUnity App within Unity - Disables Voxon Audio, exclusive mouse, the shutdown button and window buttons  
    };


    public enum VXProcessReportLevel
    {
        None = 0,               // Log no errors
        General = 1,            // Show only important warnings / errors for typical users for the plug in
        Processes = 2,          // General Reports + Show when various processes are called or other logs related to the process of using the plugin
        Debug = 3               // Process Reports + Extra debug information
    };


    public class VXProcess : Singleton<VXProcess>
    {

        #region types
        public enum RecordingType { SINGLE_FRAME, ANIMATION };
        #endregion

        #region timer
        double startTime = 0;
        #endregion
        // Create a Stopwatch instance
        Stopwatch stopwatch = new Stopwatch();
        TimeSpan DurationOfDrawCalls;
        public const string BuildDate = "20240509";
        public const string Version = "2.0AL";  // Special Legacy Plug-in Update 
        // DYNAMIC RUNTIME  
        public static VXURuntime Runtime;

        #region inspector
        [Tooltip("Choose which target platform to build for")]
        public VOXON_RUNTIME_INTERFACE VXInterface = VOXON_RUNTIME_INTERFACE.EXTENDED;

        [Header("Reporting")]

       public VXProcessReportLevel VXUReportingLevel = VXProcessReportLevel.General;

        [Header("Program Flow")]

#if DEBUG_VXPROCESS 
        public bool crashBugIt = false;
        public bool PreCrashBugIt = false;
        public bool StopItTest = false;
#else
        private bool crashBugIt = false;
        private bool PreCrashBugIt = false;
        private bool StopItTest = false;
#endif



        [Header("Simulator")]

        [FormerlySerializedAs("_guidelines")]
        [Tooltip("Renders an outline of the capture volume on the display screen")]
        public bool guidelines;

        [Tooltip("Disable to hide version information on touch screen panel")]
        public bool show_info = true;
        public int show_info_xPos = 20;
        public int show_info_yPos = 350;

        [Header("Camera")]
        [Tooltip("Enable / Disable sending the capture volume to the VXRuntime ")]
        public bool active = true;

        [Tooltip("When enabled skips all the graphical calls being sent to Volumetric display - to check performance without draw calls")]
        public bool bypassDraws = false;

        [Header("Start")]
        [Tooltip("Enable runtime applying VXGameobjects to all objects on load")]
        public bool apply_vx_on_load = true;

        [FormerlySerializedAs("_editor_camera")]
        [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
        [SerializeField]
        private VXCamera editorCamera;

        [Header("Performance")]
        [Tooltip("Apply a fixed framerate for a consistent performance")]
        public bool fixedFrameRate = false;

        [Tooltip("Target framerate when rendering and recording")]
        public int TargetFrameRate = 15;

        [Header("Recording")]
        [Tooltip("Path of recorded frame data. Use for static playback")]
        public string recordingPath = "C:\\Voxon\\Media\\MyCaptures\\framedata";

        [Tooltip("Activate recording on project load")]
        public bool recordOnLoad = false;

        [Tooltip("Capture all vcb into single zip, or as individual frames")]
        public RecordingType recordingStyle = RecordingType.ANIMATION;

        #endregion

        private Vector3 normalLighting = new Vector3();
        private bool lightingUpdated = false;

        #region drawables
        public static List<IDrawable> Drawables = new List<IDrawable>();
        public static List<VXGameObject> Gameobjects = new List<VXGameObject>();
        #endregion

        #region internal_vars
        private Int64 current_frame = 0;
        private string _dll_version = "";
        private VolumetricCamera _camera = new VolumetricCamera();
        static List<string> _logger = new List<string>();
        bool is_closing_VXProcess = false;
        bool is_recording = false;
        bool hasInited = false;
        double AvgVPS = 0;
        double HoldBreathTime = 0;

        #endregion




        #region public_vars
        [Header("Logging")]
        public int _logger_max_lines = 10;

        #endregion
        #region getters_setters

        public bool IsClosingVXProcess()
        {
            return is_closing_VXProcess;
        }
        public VXCamera Camera
        {
            get => _camera?.Camera;
            set => _camera.Camera = value;
        }

        public Matrix4x4 Transform => _camera.Transform;

        public Vector3 EulerAngles
        {
            get => _camera.EulerAngles;
            set => _camera.EulerAngles = value;
        }

        public bool HasChanged => _camera.HasChanged;

        public Vector3 NormalLight
        {
            get => normalLighting;
            set
            {
                lightingUpdated = true;
                normalLighting = value;
            }
        }

        #endregion

        #region unity_functions

        // Function to delay a Voxon Breath update... (incase you want Unity to do something first)
        public void HoldBreath(double time)
        {
            HoldBreathTime = Time.timeAsDouble + time;
        }

        private void Awake()
        {


            if (fixedFrameRate)
            {
                QualitySettings.vSyncCount = 0;  // VSync must be disabled
                Application.targetFrameRate = TargetFrameRate;
                Time.captureFramerate = TargetFrameRate;
            }

            current_frame = -1; // We haven't started our first frame yet
            Drawables.Clear();
            Gameobjects.Clear();
        }

        public bool TryToFindCamera()
        {
            if (_camera.Camera == null)
            {

                // maybe the editor camera has been set but the scene has lost it?
                if (editorCamera != null)
                {
                    _camera.Camera = editorCamera;
                    return true;
                }

                string scriptNameToSearch = "VXCamera"; // Name of the script you want to find

                // Find all GameObjects in the scene
                GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();

                // List to store GameObjects with the specified script attached
                List<GameObject> objectsWithScript = new List<GameObject>();

                // Iterate through each GameObject
                foreach (GameObject go in gameObjects)
                {

                    // Check if the GameObject has the specified script attached
                    if (go.GetComponent(scriptNameToSearch) != null)
                    {
                        // Add the GameObject to the list if it has the script attached
                        _camera.Camera = go.GetComponent<VXCamera>();
                        return true;
                    }
                }


            }

            return false;

        }




        public void Start()
        {
#if UNITY_EDITOR
            // Enables VXProcess logging only for editor. 
            PlayerPrefs.SetInt("Voxon_VXProcessReportingLevel", (int)VXUReportingLevel);
#else
            PlayerPrefs.SetInt("Voxon_VXProcessReportingLevel", 0 );
#endif

            if (PreCrashBugIt)
            {
                Runtime = new VXURuntime(VXInterface);

                for (int i = 0; i < 10; i++)
                {
                    Runtime.Shutdown();
                    Runtime.Unload();
                    Runtime.Load();
                    Runtime.Initialise(1);
                }
                active = false;
                CloseVXProcess();
                return;
            }

            Camera = editorCamera;
            int LaunchVXAppType = (int)VXLaunchType.Unity;
            startTime = Time.timeAsDouble;


            // Should VX Load?
            if (!active)
            {
                return;
            }
            else if (_camera.Camera == null)
            {
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.General) UnityEngine.Debug.Log("No Volumetric Camera Assigned - Attempting to find one in the scene");

                if (TryToFindCamera() == false)
                {
                    UnityEngine.Debug.LogError("No Volumetric Camera Assigned - Please attach a VX Camera in Process Manager (Voxon -> Process)");

                    active = false;
                    return;
                }
            }

            if (Runtime == null)
            {
                Runtime = new Voxon.VXURuntime(VXInterface);
            }
            // Load DLL
            if (!Runtime.isLoaded())
            {

                Runtime.Load();
#if UNITY_EDITOR
                LaunchVXAppType = (int)VXLaunchType.UnityEditor;
#endif
                hasInited = true;



                //type = 0;
                Runtime.Initialise(LaunchVXAppType);
                _dll_version = Runtime.GetDLLVersion().ToString().Substring(0, 8);


                if (StopItTest)
                {
                    active = false;
                    CloseVXProcess();
                    return;
                }


                if (crashBugIt)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Runtime.Shutdown();
                        Runtime.Unload();
                        Runtime.Load();
                        Runtime.Initialise(LaunchVXAppType);
                    }
                    active = false;
                    CloseVXProcess();
                }

            }
            else
            {

                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.General) UnityEngine.Debug.LogWarning("VXRuntime DLL was already loaded... Were you loading a new scene? Otherwise this might be a symptom of the VxProcess.cs being call multiple times");
            }


            if (apply_vx_on_load)
            {
                // Load all existing drawable components
                Renderer[] pack = FindObjectsOfType<Renderer>();
                foreach (Renderer piece in pack)
                {
                    if (piece.gameObject.GetComponent<ParticleSystem>())
                    {

                    }
                    else if (piece.gameObject.GetComponent<LineRenderer>() && !piece.gameObject.GetComponent<Line>())
                    {
                        piece.gameObject.AddComponent<Line>();
                    }
                    else
                    {
                        GameObject parent = piece.transform.root.gameObject;
                        if (!parent.GetComponent<VXGameObject>())
                        {
                            Gameobjects.Add(parent.AddComponent<VXGameObject>());
                        }
                    }

                }
            }

            if (recordOnLoad)
            {
                is_recording = true;
                if (recordingStyle == RecordingType.ANIMATION)
                {
                    Voxon.VXProcess.Runtime.StartRecording(recordingPath, TargetFrameRate);
                }
            }
        }


        void Update()
        {

            if (HoldBreathTime > Time.timeAsDouble)
            {
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.Processes)  UnityEngine.Debug.Log("VxProcess - Holding Breath");
                return;
            }

            if (!active || Runtime == null || HoldBreathTime > Time.timeAsDouble)
            {
                return;
            }

            startTime = Time.timeAsDouble;

            current_frame++;

            bool isBreathing = false;
            isBreathing = Runtime.FrameStart();


            // A camera must always be active while in process
            if (_camera != null && _camera.Camera == null)
            {
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.General) UnityEngine.Debug.LogWarning("VxProcess - No Active VXCamera / Volumetric Camera has been set the VXProcess! - attempting to find one in the scene.");
                if (TryToFindCamera() == false)
                {
                    active = false;
                    this.CloseVXProcess();
                    return;
                }
            }

            if (guidelines)
                Runtime.DrawGuidelines();

            if (show_info)
            {

                AvgVPS += (Time.deltaTime - AvgVPS) * .1;
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos, 0xff4000, -1, $"Voxon X Unity Plugin {VXProcess.Version}                       VPS: ");
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos, 0xffffff, -1, $"                                                       {(1 / AvgVPS):F2}");

                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 10, 0xffffff, -1, $"Voxon Runtime version:");
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 10, 0x00ff00, -1, $"                                                    {_dll_version}");

                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 20, 0xffffff, -1, $"Plugin build date: ");
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 20, 0x00ff00, -1, $"                                                    {BuildDate}");

                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 30, 0x00ffff, -1, $"Compatible with Unity versions ");
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 30, 0xffff00, -1, $"                                                     >= 2020");

                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 40, 0xffff00, -1, $"             {Drawables.Count}               {Gameobjects.Count}              {DurationOfDrawCalls}");
                Runtime.LogToScreenExt(show_info_xPos, show_info_yPos + 40, 0xff00ff, -1, $"IDrawables:     GameObjects:     Duration:");

            }


            // TODO if Loaded Camera Animation - > Set Camera Transform
            _camera?.LoadCameraAnim();

            if (_camera != null && _camera.HasChanged)
            {

                _camera?.ForceUpdate();
            }

            if (bypassDraws == false)
            {
                // TODO If Loaded Capture Playback -> Set Capture Frame Else Draw
                Draw();
            }



            // TODO Save Camera Pos
            _camera?.SaveCameraAnim();
            // TODO Save Frame
            if (is_recording && recordingStyle == RecordingType.SINGLE_FRAME)
            {
                Runtime.GetVCB(recordingPath, TargetFrameRate);
            }

            _camera?.ClearUpdated();


            AudioListener.volume = Runtime.GetVolume();




            // VX quit command; TODO this should be by choice
            if (Runtime.GetKey(0x1) || !isBreathing)
            {
                CloseVXProcess();

                _camera?.Camera.CloseAnimator();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
				    Application.OpenURL(webplayerQuitURL);
#else
                        Application.Quit();
#endif
            }

            Runtime.FrameEnd();



        }
        /// Summary ///
        /* Running this function closes the VXProcess. 
         * Having a single function to handle unloading the DLL makes it easier to manage 
         * 
         * 
         */
        public void CloseVXProcess()
        {

            if (is_closing_VXProcess == false)
            {
                if (hasInited == false) return;
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.Processes) UnityEngine.Debug.Log("VXProcess.cs - CloseVXProcess() called, VXProcess and Runtime shutting down.");
                is_closing_VXProcess = true;
                HoldBreathTime = Time.timeAsDouble + 10;
                CloseRuntime();

                _camera?.Camera.CloseAnimator();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
				Application.OpenURL(webplayerQuitURL);
#else
                Application.Quit();
#endif
            }
        }
        // Nothing should call this other than by VXProcess::CloseVXProcess() -- maybe embed this into CloseVXPRocess
        private void CloseRuntime()
        {
            if (is_closing_VXProcess == false)
            {
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.Debug) UnityEngine.Debug.LogWarning("VXProcess.cs - CloseRuntime() was called outside of calling CloseVXProcess() this could make the system unstable call CloseVXProcess() first");
                return;
            }

            if (Runtime != null)
            {
                if (is_recording && recordingStyle == RecordingType.ANIMATION)
                {
                    is_recording = false;
                    Runtime.EndRecording();
                }

                Runtime.Shutdown();
                Runtime.Unload();
            }
        }


        private new void OnApplicationQuit()
        {
            if(Runtime != null && Runtime.isInitialised() == true) {
                CloseVXProcess();

            }

            try
            {
                Runtime.Unload();

            }
            catch
            {
                if ((int)VXUReportingLevel >= (int)VXProcessReportLevel.Debug) UnityEngine.Debug.Log("VXRuntime wasn't initialized, no need to unload it.");
            }

            base.OnApplicationQuit();

        // Workaround to get around Unity Editor becoming unstable after a few times the VxU plugin is used. -- not used
        //
        //#if UNITY_EDITOR
        //
        //           LayoutUtility.SaveLayoutToAsset("Assets/Voxon/Layout/VxLayout.wlt");
        //            LayoutUtility.LoadLayoutFromAsset("Assets/Voxon/Layout/VxLayout.wlt");
        //
        //#endif

        }





#endregion

#region drawing
        private void Draw()
        {
            if (lightingUpdated)
            {
               // if ((int)VXProcessReportingLevel >= (int)VXProcessReportLevel.Processes) UnityEngine.Debug.Log($"VXRuntime normal lighting updated: {normalLighting}");
                Runtime.SetNormalLighting(normalLighting.x, normalLighting.y, normalLighting.z);
                lightingUpdated = false;

            }

            if (show_info)
            {
                stopwatch.Start();
            }
            foreach (IDrawable go in Drawables)
            {
                go.Draw();
            }

            if (show_info)
            {
                stopwatch.Stop();
                DurationOfDrawCalls = stopwatch.Elapsed;
                stopwatch.Reset();
            }

            while (_logger.Count > _logger_max_lines)
            {
                _logger.RemoveAt(0);
            }

            for (var idx = 0; idx < _logger.Count; idx++)
            {
                Runtime.LogToScreen(0, 64 + (idx * 8), _logger[idx]);
            }


        }

        public static void add_log_line(string str)
        {
            _logger.Add(str);
        }
#endregion

#region computing_transforms
        public static void ComputeTransform(ref Matrix4x4 targetWorld, ref Vector3[] vertices, ref point3d[] outPoltex)
        {

            //UnityEngine.Debug.Log($"ComputingTransform 1 called");

            if (vertices.Length != outPoltex.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(outPoltex));
            }

            if (Instance == null) return;

            // Build Camera transform
            Matrix4x4 matrix = Instance.Transform * targetWorld;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {

                var inV = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

                inV = matrix * inV;

                outPoltex[idx].x = inV.x;
                outPoltex[idx].y = -inV.z;
                outPoltex[idx].z = -inV.y;
            }
        }

        private static void ComputeTransform(ref Matrix4x4 targetWorld, ref Vector3[] vertices, ref Vector2[] uvs, ref poltex[] outPoltex)
        {

            //UnityEngine.Debug.Log($"ComputingTransform 2 called");

            if (vertices.Length != outPoltex.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(outPoltex));
            }

            // Build Camera transform
            Matrix4x4 matrix = Instance.Transform * targetWorld;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {
                var inV = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

                inV = matrix * inV;

                outPoltex[idx].x = inV.x;
                outPoltex[idx].y = -inV.z;
                outPoltex[idx].z = -inV.y;
                outPoltex[idx].u = uvs[idx].x;
                outPoltex[idx].v = uvs[idx].y;
            }
        }

        public static void ComputeTransform(ref Matrix4x4 target, ref Vector3[] vertices, ref poltex[] outPoltex)
        {
            var uvs = new Vector2[vertices.Length];

            ComputeTransform(ref target, ref vertices, ref uvs, ref outPoltex);
        }
#endregion
    }
}
