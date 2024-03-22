using UnityEditor;

namespace NetcodeXR.NetcodeXREditor
{
    [CustomEditor(typeof(NetworkInteractable))]
    public class NetworkInteractableCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
}
