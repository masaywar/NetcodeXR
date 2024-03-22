using UnityEngine;

namespace NetcodeXR
{
    public partial class LocalPlayer : MonoBehaviour
    {
        private static LocalPlayer m_Instance = null;
        public static LocalPlayer Instance => m_Instance;

        [SerializeField]
        private ActionBasedControllerManager m_RightInteractorManager = null;
        public ActionBasedControllerManager RightInteractorManager => m_RightInteractorManager;

        [SerializeField]
        private ActionBasedControllerManager m_LeftInteractaorManager = null;
        public ActionBasedControllerManager LeftInteractorManager => m_LeftInteractaorManager;

        [Header("Network Player's Transform References")]
        [SerializeField]
        private IKTarget[] m_Solvers;

        public IKTarget[] Solvers => m_Solvers;

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
            }

            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            var netcodeXRManger = NetcodeXRManager.Instance;
            netcodeXRManger.forceGrabChanged += OnForceGrabChanged;
        }

        private void OnDisable()
        {
            var netcodeXRManger = NetcodeXRManager.Instance;

            if (netcodeXRManger != null)
                netcodeXRManger.forceGrabChanged -= OnForceGrabChanged;
        }

        private void OnForceGrabChanged(bool forceGrab)
        {
            m_RightInteractorManager.RayInteractor.useForceGrab = forceGrab;
            m_LeftInteractaorManager.RayInteractor.useForceGrab = forceGrab;
        }
    }
}
