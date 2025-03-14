﻿using UnityEditor;

namespace Voxon.Editor
{
    public static class TagHelper
    {
        public static void AddTag(string tag)
        {
            UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if ((asset == null) || (asset.Length <= 0)) return;
        
            var so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (var i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return;     // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }
}