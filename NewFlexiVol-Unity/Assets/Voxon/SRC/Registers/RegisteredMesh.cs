using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class RegisteredMesh {
        // Mesh Data
        public poltex[] vertices;
        public string name;
        [FormerlySerializedAs("vertex_count")] public int vertexCount;        // Number of vertices
        [FormerlySerializedAs("submesh_count")] public int submeshCount;      // Count of Submeshes part of mesh
        public int[][] indices;
        [FormerlySerializedAs("index_counts")] public int[] indexCounts;

        // Shader operations
        ComputeBuffer _cbufferIVertices;
        ComputeBuffer _cbufferIUvs;
        ComputeBuffer _cbufferOPoltex;
        public int counter = 1;     // If instantiated we expect at least 1, so set that as default value

        bool _loaded;

        // Use this for initialization
        public RegisteredMesh(ref Mesh mesh)
        {
            try
            {
                // Debug.Log($"Processing Mesh Data: {mesh.name}");
                name = mesh.name; // this means 2 meshes with same name will cause an error

                // Name would be : "Mesh_<randomnumber>_<realtimeSinceStartup> on add
                /*
                System.Random rnd = new System.Random(); // adding some random numbers to the end to ensure 
                int rand = rnd.Next(0, 100);
                mesh.name = string.Concat(mesh.name, "_");
                mesh.name = string.Concat(mesh.name, rand);
                mesh.name = string.Concat(mesh.name, "_");
                mesh.name = string.Concat(mesh.name, Time.realtimeSinceStartup);
                
        
                name = mesh.name;
                */

                // New version -- add VX instance and remove it afterwards 
                // this solution renames the mesh - better to only rename it with there is a duplicate.

                // function to check that mesh name doesn't already exist...
                if (MeshRegister.Instance.CheckMeshNameExists(mesh.name) == true )
                {
                    int index = mesh.name.IndexOf("_VXURegID_");
                    if (index >= 0)
                    {
                        name = name.Substring(0, index);
                    }
                    int num = MeshRegister.MeshUniqueCount;
                    MeshRegister.MeshUniqueCount++;
                    mesh.name = string.Concat(name, "_VXURegID_");
                    mesh.name = string.Concat(name, num);
                  
                }

          
                         
  

                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log($"RegisteredMesh.cs = Processing Mesh Data: {mesh.name}");

                // Mesh Divisions
                submeshCount = mesh.subMeshCount;

                // Vertices
                vertexCount = mesh.vertexCount;

                vertices = new poltex[vertexCount];
                load_poltex(ref mesh);

                // UVs
                var tmpUvs = new List<Vector2>();
                mesh.GetUVs(0, tmpUvs);

                // Mesh may not have uvs; default to 0,0 to ensure we have values
                if (tmpUvs.Count < mesh.vertexCount)
                {
                    tmpUvs.AddRange(Enumerable.Repeat(Vector2.zero, mesh.vertexCount - tmpUvs.Count));
                }

                for (int idx = vertexCount - 1; idx >= 0; --idx)
                {
                    vertices[idx].u = tmpUvs[idx].x;
                    vertices[idx].v = tmpUvs[idx].y;
                }

                // Indexes
                indices = new int[submeshCount][];
                indexCounts = new int[submeshCount];

                // Triangles
                rearrange_indices(ref mesh);

				if(vertexCount > 0) { 
					// Set up output compute buffer; the assigned data array will change per instance
					_cbufferOPoltex = new ComputeBuffer(vertexCount, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);

					_cbufferIUvs = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Default);
					_cbufferIUvs.SetData(tmpUvs.ToArray());
					_cbufferIVertices = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Default);
					_cbufferIVertices.SetData(mesh.vertices);
				} else
				{
					Debug.LogWarning($"{mesh.name} has 0 vertices and is likely dynamic. Compute buffers will not be initialised");
				}

			}
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error building Mesh {name}", e);
            }
        }

        public RegisteredMesh(ref MeshData meshData)
        {
            try
            {
                //  name = meshData.name; // this means 2 meshes with same name will cause an error

                // Name would be : "Mesh_<randomnumber>_<realtimeSinceStartup> on add
                // function to check that mesh name doesn't already exist...
                if (MeshRegister.Instance.CheckMeshNameExists(meshData.name) == true)
                {
                    int index = meshData.name.IndexOf("_VXURegID_");
                    if (index >= 0)
                    {
                        meshData.name = meshData.name.Substring(0, index);
                    }
                    int num = MeshRegister.MeshUniqueCount;
                    MeshRegister.MeshUniqueCount++;
                    meshData.name = string.Concat(meshData.name, "_VXURegID_");
                    meshData.name = string.Concat(meshData.name, num);

                }


                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log($"RegisteredMesh.cs = Processing Mesh Data: {meshData.name}");

                // Mesh Divisions
                submeshCount = meshData.submeshCount;

                // Vertices
                vertexCount = meshData.vertexCount;
                vertices = meshData.vertices;

                // Indexes
                indices = meshData.indices;
                indexCounts = meshData.indexCounts;

                var tmpUvs = new List<Vector2>();
                var tmpVerts = new List<Vector3>();
                foreach (poltex vert in meshData.vertices)
                {
                    tmpUvs.Add(new Vector2(vert.u, vert.v));
                    tmpVerts.Add(new Vector3(vert.x, vert.y, vert.z));
                }

                // Set up output buffer; the assigned data array will change per instance
                _cbufferOPoltex = new ComputeBuffer(vertexCount, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);

                _cbufferIUvs = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Default);
                _cbufferIUvs.SetData(tmpUvs.ToArray());

                _cbufferIVertices = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Default);
                _cbufferIVertices.SetData(tmpVerts.ToArray());

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error building Mesh {name}", e);
            }
        }
        public void Increment()
        {
            counter++;
        }

        public void Decrement()
        {
            counter--;
        }

        public bool Isactive()
        {
            return counter > 0;
        }

        public static poltex build_poltex(Vector3 pos, Vector2 uv, int col)
        {
            var t = new poltex
            {
                x = pos.x,
                y = pos.y,
                z = pos.z,
                u = uv.x,
                v = uv.y,
                col = col
            };
            return t;
        }

        public static void update_baked_mesh(SkinnedMeshRenderer smRend, ref Mesh mesh)
        {
            smRend.BakeMesh(mesh, true);
        }

        /** Privates **/
        private void load_poltex(ref Mesh mesh)
        {
            // Set Source Vertices
            int uvLength = mesh.uv.Length;

            int colour = mesh.colors.Length > 0 ? mesh.colors[0].ToInt() : 0xFFFFFF;

            Vector3[] verts = mesh.vertices;
            Vector2[] uvs = mesh.uv;

            for (int idx = vertexCount - 1; idx >= 0; --idx)
            {
                vertices[idx] = build_poltex(verts[idx], uvLength == 0 ? Vector2.zero : uvs[idx], colour);
            }

        }

        private void rearrange_indices(ref Mesh mesh)
        {
            /* Triangles are 3 idx and our array requires -1 delimiting, 
        /  So we need to make room for all tris (count) + a -1 at the end of each (count / 3)
        */
            try
            {
                for (var submesh = 0; submesh < submeshCount; submesh++)
                {
                    int indicesCount = mesh.GetTriangles(submesh).Length;
                    indexCounts[submesh] = indicesCount + (indicesCount / 3);

                    indices[submesh] = new int[indexCounts[submesh]]; // Number of Poly Indices

                    // Set up indices
                    int[] triMe = mesh.GetTriangles(submesh);

                    var outIdx = 0;
                    for (var i = 0; i < indicesCount; i += 3, outIdx += 4)
                    {
                        // Copy internal array to output array
                        indices[submesh][0 + outIdx] = triMe[i + 0];
                        indices[submesh][1 + outIdx] = triMe[i + 1];
                        indices[submesh][2 + outIdx] = triMe[i + 2];

                        // flag end of triangle
                        indices[submesh][3 + outIdx] = -1;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Translating Triangles {name}", e);
            }
        }

        /// <summary>  
        ///  Compute Shader call. Set up Kernel, define transform values and dispatches GPU threads
        ///  Currently only sends thin batches; should see to increase this in future.
        ///  </summary>
        public void compute_transform_gpu(Matrix4x4 _combinedMatrix, ref poltex[] vt, ref Mesh mesh)
        {
            try
            {
				MeshRegister.Instance.cshaderMain.SetMatrix("_combinedMatrix", _combinedMatrix);
				// MeshRegister.Instance.cshaderMain.SetMatrix("_localToWorld", _localToWorld);
				// MeshRegister.Instance.cshaderMain.SetMatrix("_activeCamera", _activeCamera);

				_cbufferIVertices.SetData(mesh.vertices);

                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "in_vertices", _cbufferIVertices);
                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "in_uvs", _cbufferIUvs);
                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "output", _cbufferOPoltex);

                // Slight Magic Number; Aiming to create 256 int blocks
                int blocks = (vertexCount + 256 - 1) / 256;

                // cshader_main.Dispatch(kernelHandle, blocks, subblocks, 1);
                MeshRegister.Instance.cshaderMain.Dispatch(MeshRegister.Instance.kernelHandle, blocks, 1, 1);
            
                _cbufferOPoltex.GetData(vt);

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Computing Transform {name}", e);
            }
        }

        public void compute_transform_cpu(Matrix4x4 component, ref poltex[] vt)
        {
            Vector4 inV = Vector4.one;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {
                inV.x = vertices[idx].x;
                inV.y = vertices[idx].y;
                inV.z = vertices[idx].z;
            
                inV = component * inV;
                
                // Generate Poltex from Vec4, and add U/V values
                vt[idx] = inV.ToPoltex(vertices[idx].u, vertices[idx].v);
            }

        }

		~RegisteredMesh()
		{
			if (_cbufferIUvs.IsValid())
			{
				_cbufferIUvs.Release();
				_cbufferIUvs.Dispose();
				_cbufferIUvs = null;
			}
			if (_cbufferIVertices.IsValid())
			{
				_cbufferIVertices.Release();
				_cbufferIVertices.Dispose();
				_cbufferIVertices = null;
			}
			if (_cbufferOPoltex.IsValid())
			{
				_cbufferOPoltex.Release();
				_cbufferOPoltex.Dispose();
				_cbufferOPoltex = null;
			}
		}

		public void Destroy()
        {
            if (_cbufferIUvs.IsValid())
            {
                _cbufferIUvs.Release();
                _cbufferIUvs.Dispose();
                _cbufferIUvs = null;
            }
            if (_cbufferIVertices.IsValid())
            {
                _cbufferIVertices.Release();
                _cbufferIVertices.Dispose();
                _cbufferIVertices = null;
            }
            if (_cbufferOPoltex.IsValid())
            {
                _cbufferOPoltex.Release();
                _cbufferOPoltex.Dispose();
                _cbufferOPoltex = null;
            }
        }

#if UNITY_EDITOR
        public MeshData GetMeshData()
        {
            var meshData = new MeshData
            {
                indexCounts = indexCounts,
                indices = indices,
                name = name,
                submeshCount = submeshCount,
                vertexCount = vertexCount,
                vertices = vertices
            };

            return meshData;
        }
#endif
    }
}
