using UnityEngine;

namespace Voxon.tests
{
    public static class TestObjects {

        public static GameObject GameObject()
        {
            Vector3[] vertices = {
                new Vector3(1f, -1f, -1f),
                new Vector3(1f, -1f, 1f),
                new Vector3(-1f, -1f, 1f),
                new Vector3(-1f, -1f, -1f),
                new Vector3(1f, 0.5f, 1f),
                new Vector3(-1f, 0.5f, 1f)
            };

            int[] triangles = {
                1, 3, 0,
                4, 2, 1,
                3, 2, 5,
                1, 0, 4,
                5, 0, 3,
                1, 2, 3,
                4, 5, 2,
                5, 4, 0 };

            Vector2[] uvs =
            {
                new Vector2(0.292869f, 0.000141f),
                new Vector2(0.585597f, 0.000141f),
                new Vector2(0.585597f, 0.292869f),
                new Vector2(0.585596f, 0.585597f),
                new Vector2(0.292869f, 0.292869f),
                new Vector2(0.000141f, 0.292869f),
            };

            var mesh = new Mesh();
            var go = new GameObject();
            var mf = go.AddComponent<MeshFilter>();

            mesh.name = "tGameObject.Name";
            mf.mesh = mesh;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            return go;
        }

        public static Material Material()
        {
            int mainTextureId = Shader.PropertyToID("_MainTex");

            var mat = new Material(Shader.Find("Standard")) {color = Color.red};

            mat.SetTexture(mainTextureId, Texture());

            return mat;
        }

        public static Texture2D Texture()
        {
            const int width = 256;
            const int height = 256;

            const int wBlocks = 8;
            const int hBlocks = 8;

            var texture = new Texture2D(width, height);

            for(var widthIdx = 0; widthIdx < width; ++widthIdx)
            {
                for (var heightIdx = 0; heightIdx < height; ++heightIdx)
                {
                    texture.SetPixel(widthIdx, heightIdx,
                        ((widthIdx / wBlocks) + (heightIdx / hBlocks)) % 2 == 0 ? Color.black : Color.white);
                }
            }

            texture.Apply();

            return texture;
        }
    }
}
