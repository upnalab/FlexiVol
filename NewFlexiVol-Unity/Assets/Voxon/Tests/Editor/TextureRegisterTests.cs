using NUnit.Framework;
using UnityEngine;

namespace Voxon.tests
{
    public class TextureRegisterTests
    {
        [Test]
        public void AccessTextureRegister()
        {
            TextureRegister tr = TextureRegister.Instance;
            Assert.IsNotNull(tr, "Texture Register not available");
            tr.ClearRegister();
        }

        [Test]
        public void GetTile()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);

            Assert.IsNotNull(tt, "Tile Type not generated on initial request");
            tr.ClearRegister();
        }

        [Test]
        public void DropTileUnloading()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);
            bool dropSuccessful = tr.drop_tile(ref mat);

            Assert.True(dropSuccessful, "Tile was not unloaded from library");
            tr.ClearRegister();
        }

        [Test]
        public void DropTileDecrement()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            // Initial load
            tr.get_tile(ref mat);

            // Additional load to increase "active" materials
            tr.get_tile(ref mat);

            bool dropSuccessful = tr.drop_tile(ref mat);

            Assert.False(dropSuccessful, "Tile was incorrectly unloaded from library");
            tr.ClearRegister();
        }

        [Test]
        public void DropTileMissing()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            bool dropSuccessful = tr.drop_tile(ref mat);

            Assert.False(dropSuccessful, "Unloaded Tile was unloaded from library. Should be impossible.");
            tr.ClearRegister();
        }

        [Test]
        public void GetTexturePointer()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);

            Assert.AreNotEqual(0, tt.first_pixel);
            tr.ClearRegister();
        }

        [Test]
        public void GetHeight()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);

            Assert.AreEqual(256, (System.Int32)tt.height);
            tr.ClearRegister();
        }

        [Test]
        public void GetWidth()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);

            Assert.AreEqual(256, (System.Int32)tt.width);
            tr.ClearRegister();
        }

        [Test]
        public void GetPitch()
        {
            TextureRegister tr = TextureRegister.Instance;
            Texture2D mat = (Texture2D)TestObjects.Material().mainTexture;

            tiletype tt = tr.get_tile(ref mat);

            Assert.AreEqual(1024, (System.Int32)tt.pitch);
            tr.ClearRegister();
        }
    }
}
