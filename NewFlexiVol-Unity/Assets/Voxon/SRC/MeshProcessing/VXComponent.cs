using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;




namespace Voxon
{
    /// <summary>
    /// VXComponent is capable of being rendered on the volumetric display.
    /// 
    /// </summary>
    public class VXComponent : MonoBehaviour, IDrawable
    {
        public bool enable = true;
        private bool NoMeshRenderFound = true;
        public string MeshPath = "";

        private Renderer _rend;
        private SkinnedMeshRenderer _smRend;

        // Mesh Structure Structure
        private Mesh _baseMesh;
        private Mesh _umesh;         // Objects mesh
        private Material[] _umaterials; // Object's Materials

        // Mesh Data
        private RegisteredMesh _mesh;
        private poltex[] _vt;   // List of vertices

        private int _submeshN = 0;      // Count of Submeshes part of mesh

        [FormerlySerializedAs("_flags")] public Flags VoxonRenderStyle = Flags.SURFACES;
        private int _drawFlags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer
        private int[] _cols;

        private string _tagWas = "";
        // Texture
        tiletype[] _textures;

        [Tooltip("If enabled the Material will be able to be recolored, offering a tint")]
        public bool enableLit = false;
        public bool enableOverSaturate = false;
        public bool CanExpire = false;

        private Matrix4x4 _transform;

        // Clean up
        [Tooltip("If enabled the VxComponent will be destroyed after the Time To Expire amount has passed")]
        public bool automaticallyExpire = false;
        public float TimeToExpire = 2.0f;

        private float TimeLeftAlive;





        // Use this for initialization
        void Start()
        {
            // Check if the current GameObject has a MeshRenderer component
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer == null && skinnedMeshRenderer == null)
            {
                string gameObjectName = gameObject.name;
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General) Debug.LogWarning($"VxComponent.cs - gameObject : {gameObjectName} has a VXComponent but no mesh renderer... Does this object need a VXComponent?");

                enable = false;
                NoMeshRenderFound = true;
                Destroy(this);
            }
            else
            {
                NoMeshRenderFound = false;
            }


            if (enable == false) return;
            TimeLeftAlive = TimeToExpire;

            _drawFlags = (int)VoxonRenderStyle | 1 << 3;

            if (GetComponent<VideoPlayer>() && GetComponent<VXVideoPlayer>() == null)
            {
                gameObject.AddComponent<VXVideoPlayer>();
            }

            try
            {
                try
                {
                    _rend = GetComponent<Renderer>();
                    _rend.enabled = true;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Except($"({name}) Failed to load suitable Renderer", e);
                    Destroy(this);
                }

                _smRend = GetComponent<SkinnedMeshRenderer>();
                if (_smRend)
                {
                    _baseMesh = _smRend.sharedMesh;
                    _umesh = new Mesh();

                    _smRend.BakeMesh(_umesh, true);
                    _umaterials = _smRend.materials;

                    _smRend.updateWhenOffscreen = true;
                }
                else
                {
                    _baseMesh = GetComponent<MeshFilter>().sharedMesh;

                    _umesh = GetComponent<MeshFilter>().sharedMesh;
                    _umaterials = GetComponent<MeshRenderer>().materials;
                }


                if (_umesh == null)
                {
                    ExceptionHandler.Except($"({name}) Mesh: FAILED TO LOAD", new NullReferenceException());
                }

                if (_umaterials.Length != _umesh.subMeshCount)
                {
                    ExceptionHandler.Except(
                        $"ERROR: {name} has mismatching materials and submesh count. These need to be equal! Submesh past material count will be assigned first material", new Exception());
                }

                // Load Mesh
                load_meshes();

                // Load textures
                load_textures();

                VXProcess.Drawables.Add(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while building {gameObject.name}", e);
            }

        }

        private void Update()
        {
            if (automaticallyExpire || CanExpire)
            {
                TimeLeftAlive -= Time.deltaTime;
                if (TimeLeftAlive <= 0)
                {
                    DestroyImmediate(this);
                }
            }
        }

        public void Refresh()
        {
            TimeLeftAlive = TimeToExpire;
        }

        public void set_flag(int flag)
        {
            VoxonRenderStyle = (Flags)flag;
            _drawFlags = flag | 1 << 3;
        }

        // Use destroy to free gpu data
        private void OnDestroy()
        {

            if (NoMeshRenderFound) return;

            try
            {
                bool meshDropped = false;

                // Remove ourselves from Draw cycle
                if (VXProcess.Runtime != null)
                {
                    VXProcess.Drawables.Remove(this);
                    meshDropped = true;
                }

                // Free Mesh
                if (_umesh && MeshRegister.Active)
                    MeshRegister.Instance.drop_mesh(ref _umesh);

                for (var submesh = 0; submesh < _submeshN; submesh++)
                {
                    if (_umaterials[submesh].mainTexture)
                    {
                        Texture2D tmpText2D = _umaterials[submesh].mainTexture as Texture2D;
                        TextureRegister.Instance.drop_tile(ref tmpText2D);
                    }
                }


                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.Processes)
                    Debug.Log($"VXComponent {this._mesh.name} destroyed. Was it dropped from the VXProcess.Drawables? {meshDropped}");




            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel") >= (int)VXProcessReportLevel.General) Debug.LogError($"Error while Destroying {gameObject.name}, Reason:   {e}");
#else
                Debug.LogError($"Error while Destroying {gameObject.name}, Reason:   {e}");
#endif
            }

        }

        /// <summary>  
        ///  Draw the drawable mesh; Uses Capture Volume's transform to determine if play space has changed
        ///  Animated meshes are set to redraw every frame while statics only redrawn on them or the volume
        ///  changing transform.
        ///  </summary>
        public void Draw()
        {
            if (enable == false) return;
            if (VXProcess.Runtime == null || VXProcess.Instance.active == false) return;



            try
            {
                if (!gameObject.activeInHierarchy || CompareTag("VoxieHide") || !enabled)
                {
                    // Debug.Log($"{gameObject.name}: Skipping");
                    return;
                }


                if (_smRend)
                {
                    RegisteredMesh.update_baked_mesh(_smRend, ref _umesh);
                }

                if (_smRend || VXProcess.Instance.HasChanged || transform.hasChanged || _tagWas == "VoxieHide")
                {
                    BuildMesh();
                }

                for (var idx = 0; idx < _mesh.submeshCount; idx++)
                {

                    if (_umaterials[idx].HasProperty("_MainTex") && _umaterials[idx].mainTexture != null)
                    {
                        if (enableLit)
                        {
                            int blendColor = _rend.materials[idx].color.ToInt();
                            if (!enableOverSaturate)
                            {
                                blendColor = colorHexDivide(blendColor, 4);
                            }

                            VXProcess.Runtime.DrawLitTexturedMesh(ref _textures[idx], _vt, _mesh.vertexCount, _mesh.indices[idx], _mesh.indexCounts[idx], _drawFlags, blendColor);
                        }
                        else
                        {
                            VXProcess.Runtime.DrawTexturedMesh(ref _textures[idx], _vt, _mesh.vertexCount, _mesh.indices[idx], _mesh.indexCounts[idx], _drawFlags);
                        }

                    }
                    else
                    {
                        if (enableLit)
                        {
                            Color32 unlit = _rend.materials[idx].color;
                            int blendColor = unlit.ToInt();
                            if (!enableOverSaturate)
                            {
                                blendColor = colorHexDivide(blendColor, 4);
                            }
                            VXProcess.Runtime.DrawUntexturedMesh(_vt, _mesh.vertexCount, _mesh.indices[idx], _mesh.indexCounts[idx], _drawFlags, blendColor);

                        }
                        else
                        {
                            VXProcess.Runtime.DrawUntexturedMesh(_vt, _mesh.vertexCount, _mesh.indices[idx], _mesh.indexCounts[idx], _drawFlags, _rend.materials[idx].color.ToInt());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Drawing {gameObject.name}", e);
            }

            _tagWas = tag;

        }

        /// <summary>  
        ///  Generates relevant transform for mesh type against capture volume transform
        ///  Passes to the Compute Shader for processing
        ///  </summary>
        private void BuildMesh()
        {
            try
            {
                // Set Model View transform
                Matrix4x4 matrix = transform.localToWorldMatrix;
                matrix = VXProcess.Instance.Transform * matrix;

                if (_smRend)
                {
                    //_smRend.
                    _mesh.compute_transform_gpu(matrix, ref _vt, ref _umesh);
                }
                else
                {
                    _mesh.compute_transform_cpu(matrix, ref _vt);
                }

                transform.hasChanged = false;
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Building Mesh {gameObject.name}", e);
            }
        }

        private void load_meshes()
        {
            if (MeshPath != "")
            {
                _baseMesh.name = MeshPath;
            }
            Profiler.BeginSample("Load Meshes");
            try
            {
                _mesh = MeshRegister.Instance.get_registed_mesh(ref _baseMesh);
                _vt = new poltex[_mesh.vertexCount];

                BuildMesh();
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Loading Mesh: {gameObject.name}", e);
            }
            Profiler.EndSample();
        }

        private void load_textures()
        {
            Profiler.BeginSample("Load Textures");
            try
            {
                _textures = new tiletype[_mesh.submeshCount];
                for (var submesh = 0; submesh < _mesh.submeshCount; submesh++)
                {
                    if (_umaterials[submesh].HasProperty("_MainTex") && _umaterials[submesh].mainTexture)
                    {
                        if (GetComponent<VXRenderTarget>())
                        {
                            Texture2D tempTex2D = GetComponent<VXRenderTarget>()._textureBuffer;
                        }
                        else
                        {
                            Texture2D tmpText2D = _umaterials[submesh].mainTexture as Texture2D;
                            _textures[submesh] = TextureRegister.Instance.get_tile(ref tmpText2D);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Loading Textures: {gameObject.name}", e);
            }
            Profiler.EndSample();
        }

        public void RefreshDynamicTexture(ref Texture2D dynamic_texture)
        {
            for (var submesh = 0; submesh < _mesh.submeshCount; submesh++)
            {
                if (_umaterials[submesh].HasProperty("_MainTex")
                    && _umaterials[submesh].mainTexture
                    && _umaterials[submesh].mainTexture.name == dynamic_texture.name)
                {
                    _textures[submesh] = TextureRegister.Instance.refresh_tile(ref dynamic_texture);
                }
            }
        }

        public void LoadRenderTexture(ref RenderTexture dynamic_texture)
        {
            for (var submesh = 0; submesh < _mesh.submeshCount; submesh++)
            {
                if (_umaterials[submesh].HasProperty("_MainTex")
                    && _umaterials[submesh].mainTexture
                    && _umaterials[submesh].mainTexture.name == dynamic_texture.name)
                {
                    Texture2D texture2D = new Texture2D(dynamic_texture.width, dynamic_texture.height);
                    Graphics.ConvertTexture(dynamic_texture, texture2D);
                    _textures[submesh] = TextureRegister.Instance.refresh_tile(ref texture2D);
                }
            }
        }

        int colorHexDivide(int _colour, int _divideAmount)
        {
            if (_divideAmount == -1) return _colour;
            int b, g, r;

            b = (_colour & 0xFF);
            g = (_colour >> 8) & 0xFF;
            r = (_colour >> 16) & 0xFF;


            if (_divideAmount == 0) return 0;

            b /= _divideAmount;
            g /= _divideAmount;
            r /= _divideAmount;

            return (r << 16) | (g << 8) | (b);
        }

    }


}


