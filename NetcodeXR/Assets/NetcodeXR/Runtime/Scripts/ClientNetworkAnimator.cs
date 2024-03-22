using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;


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

