using NUnit.Framework;
using UnityEngine;

namespace Voxon.tests
{
    public class RegisteredMeshTests
    {

        [Test]
        public void CreateRegisteredMesh()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);
            Assert.IsNotNull(testRm, "Registered Mesh was not created from Test Game Object");
        }

        [Test]
        public void HasName()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            Assert.AreEqual("tGameObject.Name", testRm.name);
        }

        [Test]
        public void CountsSelf()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            Assert.AreEqual(1, testRm.counter);
        }

        [Test]
        public void IncrementsCounter()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            RegisteredMesh testRm = new RegisteredMesh(ref mesh);
            int oldCounter = testRm.counter;
            testRm.Increment();

            // Test value has changed (incase we accidentally set the default value to 2)
            Assert.AreNotEqual(oldCounter, testRm.counter);

            // Test value is now 2
            Assert.AreEqual(2, testRm.counter);
        }

        [Test]
        public void DecrementsCounter()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            RegisteredMesh testRm = new RegisteredMesh(ref mesh);
            int oldCounter = testRm.counter;
            testRm.Decrement();

            // Test value has changed
            Assert.AreNotEqual(oldCounter, testRm.counter);

            // Test value is now 0
            Assert.AreEqual(0, testRm.counter);
        }

        [Test]
        public void RegisteredMeshIsActive()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);
            Assert.IsTrue(testRm.Isactive());
        }

        [Test]
        public void RegisteredMeshIsDeactivated()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh) {counter = 0};

            Assert.IsFalse(testRm.Isactive());
        }

        [Test]
        public void BuildPoltex()
        {
            poltex pol = RegisteredMesh.build_poltex(new Vector3(1, 2, 3), new Vector2(4, 5), 6);

            var expectedPol = new poltex
            {
                x = 1,
                y = 2,
                z = 3,
                u = 4,
                v = 5,
                col = 6
            };

            // Poltex build is direct mapping (no transforms into Voxon Space)
            Assert.AreEqual(expectedPol, pol);
        }

        [Test]
        public void LoadVertices()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            Vector3[] vertices = mesh.vertices;

            for (int idx = mesh.vertexCount - 1; idx >= 0; --idx)
            {
                Assert.AreEqual(vertices[idx].x, testRm.vertices[idx].x);
                Assert.AreEqual(vertices[idx].y, testRm.vertices[idx].y);
                Assert.AreEqual(vertices[idx].z, testRm.vertices[idx].z);
            }
        }

        [Test]
        public void LoadUvs()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var uvs = new System.Collections.Generic.List<Vector2>();
            mesh.GetUVs(0, uvs);

            for (int idx = mesh.uv.Length - 1; idx >= 0; --idx)
            {
                Assert.AreEqual(uvs[idx].x, testRm.vertices[idx].u);
                Assert.AreEqual(uvs[idx].y, testRm.vertices[idx].v);
            }
        }

        [Test]
        public void LoadIndices()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            int[] indices = mesh.GetIndices(0);
            int indicesLength = indices.Length;

            var outIdx = 0;
            for (var i = 0; i < indicesLength; i += 3, outIdx += 4)
            {
                // Copy internal array to output array
                Assert.AreEqual(indices[i + 0], testRm.indices[0][0 + outIdx]);
                Assert.AreEqual(indices[i + 1], testRm.indices[0][1 + outIdx]);
                Assert.AreEqual(indices[i + 2], testRm.indices[0][2 + outIdx]);

                // flag end of triangle
                Assert.AreEqual(-1, testRm.indices[0][3 + outIdx]);
            }
        }

        [Test]
        public void computeTransformCPU_Identity()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.identity;

            testRm.compute_transform_cpu(mat, ref vt);

            /* Original Vertices before VX1 Swap */
            /*
        Vector3[] expected_vertices = {
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(-1f, 0.5f, 1f)
        };
        */

            var expectedVertices = new[] {
                new Vector3(1f, 1f, 1f),
                new Vector3(1f, -1f, 1f),
                new Vector3(-1f, -1f, 1f),
                new Vector3(-1f, 1f, 1f),
                new Vector3(1f, -1f, -0.5f),
                new Vector3(-1f, -1f, -0.5f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: {0}", idx);
            }
        }

        [Test]
        public void computeTransformCPU_Scale()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Scale(new Vector3(2, 2, 2));

            testRm.compute_transform_cpu(mat, ref vt);

            Vector3[] expectedVertices = {
                new Vector3(2f, 2f, 2f),
                new Vector3(2f, -2f, 2f),
                new Vector3(-2f, -2f, 2f),
                new Vector3(-2f, 2f, 2f),
                new Vector3(2f, -2f, -1f),
                new Vector3(-2f, -2f, -1f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: {0}", idx);
            }
        }

        [Test]
        public void computeTransformCPU_Translate()
        {
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Translate(new Vector3(2, 2, 2));

            testRm.compute_transform_cpu(mat, ref vt);

            Vector3[] expectedVertices = {
                new Vector3(3f, -1f, -1f),
                new Vector3(3f, -3f, -1f),
                new Vector3(1f, -3f, -1f),
                new Vector3(1f, -1f, -1f),
                new Vector3(3f, -3f, -2.5f),
                new Vector3(1f, -3f, -2.5f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: {0}", idx);
            }
        }

        [Test]
        public void computeTransformCPU_Rotation()
        {
            // Perform a 90 degree rotation around the X axis
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)));

            testRm.compute_transform_cpu(mat, ref vt);

            Vector3[] expectedVertices = {
                new Vector3(1,-1,1),
                new Vector3(1,-1,-1),
                new Vector3(-1,-1,-1),
                new Vector3(-1,-1,1),
                new Vector3(1,0.5f,-1),
                new Vector3(-1,0.5f,-1),
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, 0.001f, "X failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, 0.001f, "Y failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, 0.001f, "Z failed on index: " + idx);
            }
        }

        [Test]
        public void computeTransformCPU_LocalMatrix()
        {
            // Perform a 90 degree rotation around the X axis
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.TRS(new Vector3(2, 2, 2), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), new Vector3(2, 2, 2));

            testRm.compute_transform_cpu(mat, ref vt);

            // Unity Rotation is done Right Handed
            Vector3[] expectedVertices = {
                new Vector3(4,0,-4),
                new Vector3(4,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,-4),
                new Vector3(4,-3,0),
                new Vector3(0,-3,0)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, 0.001f, $"X failed on index: {idx}");
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, 0.001f, $"Y failed on index: {idx}");
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, 0.001f, $"Z failed on index: {idx}");
            }
        }

        private void generateMeshRegister()
        {
            if (MeshRegister.Instance.cshaderMain != null) return;
        
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            MeshRegister.Instance.get_registed_mesh(ref mesh);
            MeshRegister.Instance.drop_mesh(ref mesh);
        }

        [Test]
        public void computeTransformGPU_Identity()
        {
            generateMeshRegister();
        
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.identity;

            testRm.compute_transform_gpu(mat, ref vt, ref mesh);

            /* Original Vertices before VX1 Swap
        Vector3[] expected_vertices = {
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(-1f, 0.5f, 1f)
        };
        */

            Vector3[] expectedVertices = {
                new Vector3(1f, 1f, 1f),
                new Vector3(1f, -1f, 1f),
                new Vector3(-1f, -1f, 1f),
                new Vector3(-1f, 1f, 1f),
                new Vector3(1f, -1f, -0.5f),
                new Vector3(-1f, -1f, -0.5f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: " + idx);
            }
        }

        [Test]
        public void computeTransformGPU_Scale()
        {
            generateMeshRegister();

            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Scale(new Vector3(2, 2, 2));

            testRm.compute_transform_gpu(mat, ref vt, ref mesh);

            Vector3[] expectedVertices = {
                new Vector3(2f, 2f, 2f),
                new Vector3(2f, -2f, 2f),
                new Vector3(-2f, -2f, 2f),
                new Vector3(-2f, 2f, 2f),
                new Vector3(2f, -2f, -1f),
                new Vector3(-2f, -2f, -1f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: " + idx);
            }
        }

        [Test]
        public void computeTransformGPU_Translate()
        {
            generateMeshRegister();

            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Translate(new Vector3(2, 2, 2));

			// TODO : No Camera Object
            testRm.compute_transform_gpu(mat, ref vt, ref mesh);

            Vector3[] expectedVertices = {
                new Vector3(3f, -1f, -1f),
                new Vector3(3f, -3f, -1f),
                new Vector3(1f, -3f, -1f),
                new Vector3(1f, -1f, -1f),
                new Vector3(3f, -3f, -2.5f),
                new Vector3(1f, -3f, -2.5f)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, "X failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, "Y failed on index: {0}", idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, "Z failed on index: {0}", idx);
            }
        }

        [Test]
        public void computeTransformGPU_Rotation()
        {
            generateMeshRegister();

            // Perform a 90 degree rotation around the X axis
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)));

			// TODO : No Camera Object
			testRm.compute_transform_gpu(mat, ref vt, ref mesh);

            Vector3[] expectedVertices = {
                new Vector3(1,-1,1),
                new Vector3(1,-1,-1),
                new Vector3(-1,-1,-1),
                new Vector3(-1,-1,1),
                new Vector3(1,0.5f,-1),
                new Vector3(-1,0.5f,-1),
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, 0.001f, $"X failed on index: {idx}");
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, 0.001f, $"Y failed on index: {idx}");
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, 0.001f, $"Z failed on index: {idx}");
            }
        }

        [Test]
        public void computeTransformGPU_LocalMatrix()
        {
            generateMeshRegister();

            // Perform a 90 degree rotation around the X axis
            GameObject go = TestObjects.GameObject();
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            var testRm = new RegisteredMesh(ref mesh);

            var vt = new poltex[mesh.vertexCount];

            Matrix4x4 mat = Matrix4x4.TRS(new Vector3(2, 2, 2), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), new Vector3(2, 2, 2));

			// TODO : No Camera Object
			testRm.compute_transform_gpu(mat, ref vt, ref mesh);

            // Unity Rotation is done Right Handed
            Vector3[] expectedVertices = {
                new Vector3(4,0,-4),
                new Vector3(4,0,0),
                new Vector3(0,0,0),
                new Vector3(0,0,-4),
                new Vector3(4,-3,0),
                new Vector3(0,-3,0)
            };

            for (var idx = 0; idx < mesh.vertexCount; idx++)
            {
                Assert.AreEqual(expectedVertices[idx].x, vt[idx].x, 0.001f, "X failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].y, vt[idx].y, 0.001f, "Y failed on index: " + idx);
                Assert.AreEqual(expectedVertices[idx].z, vt[idx].z, 0.001f, "Z failed on index: " + idx);
            }
        }
    }
}
