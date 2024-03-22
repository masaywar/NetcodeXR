using NetcodeXR.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

#if INSTALLED_ROOT_MOTION
using RootMotion.FinalIK;
#endif

namespace NetcodeXR
{
    public class BlendShapeSynchronizer : NetworkBehaviour
    {
        internal NetworkAvatar m_Avatar;
        public NetworkAvatar Avatar => m_Avatar;

        private WaitForSeconds m_BsWait;

        private void OnEnable()
        {
            if (!NetworkManager.IsListening)
            {
                return;
            }

            if (!IsSpawned)
                return;

            StartCoroutine(SyncBlendshapes());
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            uint tick = NetworkManager.Singleton.NetworkTickSystem.TickRate;
            m_BsWait = new WaitForSeconds(1 / tick);

            if (IsClient)
                StartCoroutine(SyncBlendshapes());
        }

        public IEnumerator SyncBlendshapes()
        {
            if (!IsOwner)
            {
                yield break;
            }

            while (m_Avatar == null)
            {
                yield return m_BsWait;
            }

            Dictionary<int, float[]> tempDict = new Dictionary<int, float[]>();
            bool bsSyncFlag = m_Avatar.FacialSkinnedMesh != null && m_Avatar.FacialSkinnedMesh.Length > 0;

            while (bsSyncFlag)
            {
                for (int k = 0; k < m_Avatar.FacialSkinnedMesh.Length; ++k)
                {
                    var val = m_Avatar.GetBlendshapesValue(k);

                    if (!tempDict.TryAdd(k, val))
                    {
                        tempDict[k] = null;
                        tempDict[k] = val;
                    }
                }

                byte[] data = tempDict.SerializeToByteArray();
                SyncBlendshapesRpc(data);

                yield return m_BsWait;
            }
        }

        [Rpc(SendTo.NotMe & SendTo.ClientsAndHost)]
        public void SyncBlendshapesRpc(byte[] data, RpcParams param = default)
        {
            var senderCliendId = param.Receive.SenderClientId;

            var playerObject = NetcodeXRManager.Instance.GetNetworkPlayerById(senderCliendId);

            if (playerObject.Avatar == null)
            {
                return;
            }

            var networkAvatar = playerObject.Avatar;
            var dict = data.DeserializeFromByteArray<Dictionary<int, float[]>>();

            int index = 0;

            foreach (var dictKey in dict.Keys)
            {
                var skinnedValueArray = dict[dictKey];

                for (int k = 0; k < skinnedValueArray.Length; ++k)
                {
                    var val = skinnedValueArray[k];
                    networkAvatar.FacialSkinnedMesh[index].SetBlendShapeWeight(k, val);
                }
            }
        }
    }
}

