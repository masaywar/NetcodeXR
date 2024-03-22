using Unity.Netcode.Components;
using UnityEngine;


namespace NetcodeXR
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        private void Start()
        {
            Animator = GetComponent<Animator>();
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}

