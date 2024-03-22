using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = NetcodeXR.NetworkPlayer;

public class AnimTest : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Dance"))
        {
            var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();

            var avatar = playerObject.Avatar;

            var animator = avatar.GetComponent<Animator>();

            animator.SetTrigger("Dance");
        }
    }
}
