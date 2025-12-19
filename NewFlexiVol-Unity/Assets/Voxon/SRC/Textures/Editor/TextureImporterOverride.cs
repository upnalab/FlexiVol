using UnityEditor;

namespace Voxon.Editor
{
    class TextureImporterOverride : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            var textureImporter = assetImporter as TextureImporter;
            if (textureImporter != null) textureImporter.isReadable = true;
        }
    }
}