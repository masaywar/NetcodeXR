namespace NetcodeXR.NetcodeXREditor
{
    using NetcodeXR.Utility.Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ConditionalHideInInspectorAttribute))]
    public class ConditionalHideInInspectorDrawer : PropertyDrawer
    {
        ConditionalHideInInspectorAttribute Attribute;

        SerializedProperty ComparedField;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CanDraw(property) ? EditorGUI.GetPropertyHeight(property, label, true) : 0.0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (CanDraw(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool CanDraw(SerializedProperty property)
        {
            Attribute = attribute as ConditionalHideInInspectorAttribute;

            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);

            if (SymbolDefineHelper.IsDefinedSymbol(namedBuildTarget, Attribute.ComparedPropertyName))
            {
                return true;
            }

            return false;

        }
    }

}
