using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetcodeXR.NetcodeXREditor
{
    [CreateAssetMenu(menuName = "NetcodeXR/Create Default Scene Setting Asset")]
    public class DefaultNetcodeXRSetting : NetcodeXRProjectConfigScriptableObject
    {

        [Header("XR Interaction Toolkits")]
        [SerializeField] private GameObject defaultXROrigin;

        [Header("Netcode")]
        [SerializeField] private GameObject defaultNetworkManager;
        [SerializeField] private GameObject defaultEventSystem;
        [SerializeField] private GameObject defaultNetcodeXRManager;

        [Header("Scene")]
        [SerializeField] private GameObject m_DefaultObjectPool;
        [SerializeField] private GameObject m_Plane;
        [SerializeField] private GameObject m_Interactable;
        [SerializeField] private Vector3 m_InitInteractableLocation;

        public override void CreateScene(bool isServer)
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();

            var objects = currentScene.GetRootGameObjects();

            foreach (var obj in objects)
            {
                DestroyImmediate(obj);
            }

            ConvertToPrefabInstanceSettings convertToPrefabInstanceSettings = new ConvertToPrefabInstanceSettings();
            convertToPrefabInstanceSettings.changeRootNameToAssetName = true;

            if (!isServer)
            {
                var xrorigin = Instantiate(defaultXROrigin, Vector3.zero, Quaternion.identity);
                PrefabUtility.ConvertToPrefabInstance(xrorigin, defaultXROrigin, convertToPrefabInstanceSettings, InteractionMode.AutomatedAction);
                var plane = Instantiate(m_Plane, Vector3.zero, Quaternion.identity);
                var interactable = Instantiate(m_Interactable, m_InitInteractableLocation, Quaternion.identity);

                var light = new GameObject("Directional Light");
                light.transform.position = new Vector3(0, 3, 0);
                light.transform.rotation = Quaternion.Euler(50, -30, 0);

                var comp = light.AddComponent<Light>();

                comp.type = LightType.Directional;
                comp.lightmapBakeType = LightmapBakeType.Mixed;
                comp.shadows = LightShadows.Soft;

                RenderSettings.sun = comp;

                comp.color = new Color(1, 0.996f, 0.8392f, 1);
            }

            var networkManager = Instantiate(defaultNetworkManager);
            PrefabUtility.ConvertToPrefabInstance(networkManager, defaultNetworkManager, convertToPrefabInstanceSettings, InteractionMode.AutomatedAction);
            Instantiate(defaultEventSystem);

            var netcodeXRManager = Instantiate(defaultNetcodeXRManager);
            PrefabUtility.ConvertToPrefabInstance(netcodeXRManager, defaultNetcodeXRManager, convertToPrefabInstanceSettings, InteractionMode.AutomatedAction);

            var objectPool = Instantiate(m_DefaultObjectPool);
            PrefabUtility.ConvertToPrefabInstance(objectPool, m_DefaultObjectPool, convertToPrefabInstanceSettings, InteractionMode.AutomatedAction);

            EditorSceneManager.MarkSceneDirty(currentScene);
        }
    }
}
