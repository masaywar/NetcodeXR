using Unity.Netcode;

namespace NetcodeXR
{
    public class NetworkVisibility : NetworkBehaviour
    {
        public bool IsNetworkVisibleTo(ulong clientId) => NetworkObject.IsNetworkVisibleTo(clientId);
        public void NetworkShow(ulong clientId) => NetworkObject.NetworkShow(clientId);
        public void NetworkHide(ulong clientId) => NetworkObject.NetworkHide(clientId);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!NetcodeXRManager.Instance.Visibility)
            {
                return;
            }

            if (IsServer)
                NetcodeXRManager.Instance.VisibilityManager.AddNetworkObject(this);
        }

        public override void OnNetworkDespawn()
        {

            if (!NetcodeXRManager.Instance.Visibility)
            {
                return;
            }

            if (IsServer)
                NetcodeXRManager.Instance.VisibilityManager.RemoveNetworkObject(this);

            base.OnNetworkDespawn();
        }
    }
}
