
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeXR
{
    [RequireComponent(typeof(SolverTargetSynchronizer))]
    public partial class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        private SolverTargetSynchronizer m_SolverTargetSynchronizer;

        public SolverTargetSynchronizer SolverTargetSynchronizer => m_SolverTargetSynchronizer;


        [SerializeField]
        private BlendShapeSynchronizer m_BlendshapeSynchronizer;

        public BlendShapeSynchronizer BlendShapeSynchronizer => m_BlendshapeSynchronizer;

        internal NetworkAvatar m_Avatar;
        public NetworkAvatar Avatar => m_Avatar;

        private void Awake()
        {
            m_SolverTargetSynchronizer = GetComponent<SolverTargetSynchronizer>();
            m_BlendshapeSynchronizer = GetComponent<BlendShapeSynchronizer>();
        }

        public void SetAvatar(NetworkAvatar networkAvatar)
        {
            m_BlendshapeSynchronizer.m_Avatar = networkAvatar;
            m_SolverTargetSynchronizer.m_Avatar = networkAvatar;
            m_Avatar = networkAvatar;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetcodeXRManager.Instance.PlayerSpawnedObjects.Add(OwnerClientId, this);

            if (IsOwner && NetcodeXRManager.Instance.SpawnAvatarAtStart)
            {
                SpawnAvatar();
            }

            if (IsClient)
                StartCoroutine(FollowXROrigin());
        }

        public override void OnNetworkDespawn()
        {
            NetcodeXRManager.Instance.PlayerSpawnedObjects.Remove(OwnerClientId);

            base.OnNetworkDespawn();
        }

        private IEnumerator FollowXROrigin()
        {
            var wait = new WaitForFixedUpdate();

            while (true)
            {
                var localPosition = LocalPlayer.Instance.transform.position;
                transform.position = localPosition;

                yield return wait;
            }
        }

        public void SpawnAvatar(NetworkAvatar inAvatar = null)
        {
            if (inAvatar == null)
                inAvatar = NetcodeXRManager.Instance.defaultAvatar;

            if (IsHost)
            {
                LocalPlayer.Instance.transform.GetPositionAndRotation(out var spawnPosition, out var spawnRotation);
                NetworkAvatar avatar = Instantiate(inAvatar, spawnPosition, spawnRotation);
                NetworkObject avatarNO = avatar.GetComponent<NetworkObject>();

                avatarNO.SpawnWithOwnership(NetworkObject.OwnerClientId);
                avatar.SetAllIKTargetRig(m_SolverTargetSynchronizer);
            }
            else if (IsClient)
            {
                var tempTf = LocalPlayer.Instance.transform;
                SpawnAvatarRpc(tempTf.position, tempTf.rotation.eulerAngles);
            }
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void SpawnAvatarRpc(Vector3 initPosition, Vector3 initRotation, RpcParams param = default)
        {
            var clientId = param.Receive.SenderClientId;
            var player = NetcodeXRManager.Instance.GetNetworkPlayerById(clientId);

            NetworkAvatar avatar = Instantiate(NetcodeXRManager.Instance.defaultAvatar, initPosition, Quaternion.Euler(initRotation));
            NetworkObject avatarNO = avatar.GetComponent<NetworkObject>();

            avatarNO.SpawnWithOwnership(clientId);

            var avatarController = player.GetComponent<SolverTargetSynchronizer>();

            avatar.SetAllIKTargetRig(avatarController);
        }
    }
}
