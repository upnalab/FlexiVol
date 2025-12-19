using NUnit.Framework;
using UnityEngine;

namespace Voxon.tests
{
    public class GameObjectTests {

        [Test]
        public void CreateTGameObject()
        {
            GameObject testObject = TestObjects.GameObject();
            Assert.IsNotNull(testObject, "GameObject not generated");
        }

        [Test]
        public void GameObjectHasMeshFilter()
        {
            GameObject testObject = TestObjects.GameObject();
            var meshFilter = testObject.GetComponent<MeshFilter>();

            Assert.IsNotNull(meshFilter, "MeshFilter not generated");
        }

        [Test]
        public void GameObjectHasMesh()
        {
            GameObject testObject = TestObjects.GameObject();
            testObject.GetComponent<MeshFilter>();
            Mesh mesh = testObject.GetComponent<MeshFilter>().sharedMesh;

            Assert.IsNotNull(mesh, "Mesh not generated");
        }

        [Test]
        public void GameObjectHasSingleSubmesh()
        {
            GameObject testObject = TestObjects.GameObject();
            testObject.GetComponent<MeshFilter>();
            Mesh mesh = testObject.GetComponent<MeshFilter>().sharedMesh;

            Assert.AreEqual(1, mesh.subMeshCount);
        }
    }
}
