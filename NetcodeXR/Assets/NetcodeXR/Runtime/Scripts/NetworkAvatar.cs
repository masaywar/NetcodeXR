using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using NetcodeXR.Utility;
using System.Collections;
using System.Threading.Tasks;

#if INSTALLED_ROOT_MOTION
using RootMotion.FinalIK;
using VRIK = RootMotion.FinalIK.VRIK;
#else 
using UnityEngine.Animations.Rigging;
#endif

namespace NetcodeXR
{
    #if !INSTALLED_ROOT_MOTION
    [Serializable]
    public class VRIK
    {
        [SerializeField]
        private Rig m_Rig;
        
        public Rig Rig => m_Rig;
    } 
    #endif
  
    public class NetworkAvatar : NetworkBehaviour
    {
        [SerializeField]
        private VRIK m_IK = null;

        public VRIK IK 
        {
            get 
            {
                if(m_IK == null)
                {
                    #if INSTALLED_ROOT_MOTION 
                    m_IK = GetComponent<VRIK>();
                    #else
                    return null;
                    #endif
                }

                return m_IK;
            }
            set => m_IK = value;
        }

        [SerializeField]
        private SkinnedMeshRenderer[] m_FacialSkinnedMesh;
        public SkinnedMeshRenderer[] FacialSkinnedMesh => m_FacialSkinnedMesh;         

        public void SetAllIKTargetRig(SolverTargetSynchronizer controller)
        {
            foreach(var solver in controller.Solvers)
            {
                SetIKTargetRig(solver.TargetTag, solver.SolverTarget);
            }
        }

        private void SetIKTargetRig(EIKTargetTag targetTag, Transform target)
        {
            #if INSTALLED_ROOT_MOTION
            switch (targetTag)
            {
                case EIKTargetTag.SPINE_HEAD:  
                    IK.solver.spine.headTarget = target;
                    break;
                case EIKTargetTag.SPINE_PELVIS: 
                    IK.solver.spine.pelvisTarget = target;
                    break;
                case EIKTargetTag.SPINE_CHEST: 
                    IK.solver.spine.chestGoal = target;
                    break;
                case EIKTargetTag.LEFTARM_HAND: 
                    IK.solver.leftArm.target = target;
                    break;
                case EIKTargetTag.LEFTARM_BENDING: 
                    IK.solver.leftArm.bendGoal = target;
                    break;
                case EIKTargetTag.RIGHTARM_HAND:
                    IK.solver.rightArm.target = target;
                    break;
                case EIKTargetTag.RIGHTARM_BENDING:
                    IK.solver.rightArm.bendGoal = target;
                    break;
                case EIKTargetTag.LEFTLEG_FOOT: 
                    IK.solver.leftLeg.target = target;
                    break;
                case EIKTargetTag.LEFTLEG_BENDING: 
                    IK.solver.leftLeg.bendGoal = target;
                    break;
                case EIKTargetTag.RIGHTLEG_FOOT:
                    IK.solver.rightLeg.target = target;
                    break;
                case EIKTargetTag.RIGHTLEG_BENDING: 
                    IK.solver.rightLeg.bendGoal = target;
                    break;
            }
            #endif
        }

        public float[] GetBlendshapesValue(int index)
        {
            NetcodeXRLog.Assert(m_FacialSkinnedMesh.Length > index);

            var targetMesh = m_FacialSkinnedMesh[index];
            int count = targetMesh.sharedMesh.blendShapeCount;

            float[] temp = new float[count];

            for(int k=0; k<count; ++k)
            {
                temp[k] = targetMesh.GetBlendShapeWeight(k);
            }

            return temp;
        }

        public override async void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            var player = await LoadPlayerAsync();
            
            player.SetAvatar(this);
            SetAllIKTargetRig(player.SolverTargetSynchronizer);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private async Task<NetworkPlayer> LoadPlayerAsync()
        {
            while(true)
            {
                if(!NetcodeXRManager.Instance.PlayerSpawnedObjects.TryGetValue(OwnerClientId, out var player))
                {
                    await Task.Delay(20);
                    continue;
                }

                return player;
            }
        }
    }
}
