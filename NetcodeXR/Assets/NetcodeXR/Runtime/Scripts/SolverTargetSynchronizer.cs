using NetcodeXR.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


#if INSTALLED_ROOT_MOTION
#endif

namespace NetcodeXR
{
    public class SolverTargetSynchronizer : NetworkBehaviour
    {
        internal NetworkAvatar m_Avatar = null;

        public NetworkAvatar Avatar
        {
            get
            {
                return m_Avatar;
            }
        }

        [SerializeField]
        private IKSolver[] m_Solvers = null;
        public IKSolver[] Solvers => m_Solvers;

        private WaitForFixedUpdate m_TransformWait;

        private Dictionary<EIKTargetTag, IKSolver> m_IKSolversDict = new Dictionary<EIKTargetTag, IKSolver>();
        public Dictionary<EIKTargetTag, IKSolver> IKSolverDict => m_IKSolversDict;

        private void Awake()
        {
            m_TransformWait = new WaitForFixedUpdate();

            foreach (var k in m_Solvers)
            {
                m_IKSolversDict.Add(k.TargetTag, k);
            }
        }

        public void OnEnable()
        {
            if (!IsSpawned)
            {
                return;
            }

            if (IsOwner)
            {
                StartCoroutine(FollowTarget());
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                InitializeSolverTransform();
                OnEnable();
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private void InitializeSolverTransform()
        {
            if (!IsOwner)
                return;

            if (IsServer && !IsClient)
                return;

            foreach (var solver in m_Solvers)
            {
                foreach (var target in LocalPlayer.Instance.Solvers)
                {
                    if (solver.TargetTag == target.SolverTarget)
                    {
                        solver.FollowingTarget = target.TargetTransform;
                    }
                }
            }
        }

        private IEnumerator FollowTarget()
        {
            NetcodeXRLog.Assert(IsSpawned && IsOwner);

            while (true)
            {
                foreach (var solver in m_Solvers)
                {
                    solver.UpdatePositionAndRotation();
                }

                yield return m_TransformWait;
            }
        }

        public IKSolver GetSolver(EIKTargetTag targetTag)
        {
            return m_IKSolversDict[targetTag];
        }
    }
}

