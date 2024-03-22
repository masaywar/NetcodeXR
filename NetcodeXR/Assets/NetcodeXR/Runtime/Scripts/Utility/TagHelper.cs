using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NetcodeXR.Utility
{
    public static class TagHelper
    {
#if UNITY_EDITOR
        public static void AddTag(params string[] inTags)
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject so = new SerializedObject(asset[0]);
                SerializedProperty tags = so.FindProperty("tags");

                foreach (var tag in inTags)
                {
                    for (int i = 0; i < tags.arraySize; ++i)
                    {
                        if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                        {
                            return;     // Tag already present, nothing to do.
                        }
                    }

                    tags.InsertArrayElementAtIndex(0);
                    tags.GetArrayElementAtIndex(0).stringValue = tag;
                }

                so.ApplyModifiedProperties();
                so.Update();
            }
        }
#endif

#if UNITY_EDITOR
        public static bool IsTagDefined(string inTag)
        {
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            if ((asset != null) && (asset.Length > 0))
            {
                SerializedObject so = new SerializedObject(asset[0]);
                SerializedProperty tags = so.FindProperty("tags");

                for (int i = 0; i < tags.arraySize; ++i)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == inTag)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
#endif
    }
}
