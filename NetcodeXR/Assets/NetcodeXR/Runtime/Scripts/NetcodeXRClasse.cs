using System;
using UnityEngine;

namespace NetcodeXR
{
    public enum EIKTargetTag
    {
        SPINE_HEAD,
        SPINE_PELVIS,
        SPINE_CHEST,
        LEFTARM_HAND,
        LEFTARM_BENDING,
        RIGHTARM_HAND,
        RIGHTARM_BENDING,
        LEFTLEG_FOOT,
        LEFTLEG_BENDING,
        RIGHTLEG_FOOT,
        RIGHTLEG_BENDING,
        XRORIGIN_POSITION
    }


    [Serializable]
    public class IKTarget
    {
        [SerializeField, ReadOnly(true)]
        private EIKTargetTag m_SolverTarget;

        public EIKTargetTag SolverTarget => m_SolverTarget;

        [SerializeField, ReadOnly(true)]
        private Transform m_TargetTransform;
        public Transform TargetTransform => m_TargetTransform;
    }

    [Serializable]
    public class IKSolver
    {
        [SerializeField, ReadOnly(true)]
        private EIKTargetTag m_TargetTag;

        [SerializeField, ReadOnly(true)]
        private Transform m_SolverTarget = null;

        [SerializeField, ReadOnly(true)]
        private Transform m_FollowingTarget = null;

        public EIKTargetTag TargetTag => m_TargetTag;
        public Transform SolverTarget => m_SolverTarget;
        public Transform FollowingTarget
        {
            get => m_FollowingTarget;
            set => m_FollowingTarget = value;
        }

        public void UpdatePositionAndRotation()
        {
            if (m_SolverTarget == null || FollowingTarget == null)
                return;

            m_SolverTarget.SetPositionAndRotation(FollowingTarget.position, FollowingTarget.rotation);
        }
    }
}


