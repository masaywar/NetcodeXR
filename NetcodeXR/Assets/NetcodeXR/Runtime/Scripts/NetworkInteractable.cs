using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace NetcodeXR
{
    [RequireComponent(typeof(Rigidbody),
                     typeof(ClientNetworkTransform),
                     typeof(NetworkRigidbody))]
    public partial class NetworkInteractable : NetworkBehaviour
    {
        [SerializeField, ReadOnly(false)]
        private bool m_IsInteractable;
        public bool IsInteractable => m_IsInteractable;

        private void OnEnable()
        {

            SetupXRIInteractableEvent();
        }

        public void SetEnable(bool set)
        {
            var interactableScript = GetComponent<XRBaseInteractable>();
            if (interactableScript != null)
            { 
                interactableScript.enabled = set;
                m_IsInteractable = set;
            }
        }

        private void SetupXRIInteractableEvent()
        {
            var component = GetComponent<XRBaseInteractable>();

            if (component == null)
            {
                return;
            }

            component.selectEntered.AddListener(args =>
            {
                SetOwnershipOnSelectEnterRpc(NetworkObjectId, NetworkManager.LocalClientId);
            });

            component.selectExited.AddListener(args =>
            {
                SetOwnershipOnSelectExitRpc(NetworkObjectId, NetworkManager.LocalClientId);
            });
        }

        void OnValidate()
        {
            if (!TryGetComponent<XRBaseInteractable>(out var component))
            {

            }
        }

        [Rpc(SendTo.Everyone)]
        public void SetOwnershipOnSelectEnterRpc(ulong targetObjectId, ulong newOwnerId, RpcParams param = default)
        {
            var targetObject = NetworkManager.SpawnManager.SpawnedObjects[targetObjectId];

            if (IsServer)
            {
                targetObject.ChangeOwnership(newOwnerId);
            }

            if (newOwnerId != NetworkManager.LocalClientId)
            {
                targetObject.GetComponent<NetworkInteractable>().SetEnable(false);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void SetOwnershipOnSelectExitRpc(ulong targetObjectId, ulong newOwnerId, RpcParams param = default)
        {
            var targetObject = NetworkManager.SpawnManager.SpawnedObjects[targetObjectId];

            if (IsServer)
            {
                targetObject.ChangeOwnership(newOwnerId);
            }

            targetObject.GetComponent<NetworkInteractable>().SetEnable(true);
        }
    }
}
