using NUnit.Framework;
using UnityEngine;

namespace Voxon.tests
{
    public class TextureTests {
        [Test]
        public void CreateTTexture()
        {
            Texture2D testTexture = TestObjects.Texture();
            Assert.IsNotNull(testTexture, "Texture not generated");
        }

        [Test]
        public void CorrectWidth()
        {
            Texture2D testTexture = TestObjects.Texture();
            Assert.AreEqual(256, testTexture.width);
        }

        [Test]
        public void CorrectHeight()
        {
            Texture2D testTexture = TestObjects.Texture();
            Assert.AreEqual(256, testTexture.height);
        }
    }
}
