using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRIKBuilder
{
    public class VRIKFootSolver : MonoBehaviour
    {
        public bool isMovingForward;

        [SerializeField] LayerMask m_TerrainLayer = default;
        [SerializeField] Transform m_Body = default;
        [SerializeField] VRIKFootSolver m_OtherFoot = default;
        [SerializeField] float m_Speed = 4;
        [SerializeField] float m_StepDistance = .2f;
        [SerializeField] float m_StepLength = .2f;
        [SerializeField] float sideStepLength = .1f;

        [SerializeField] float m_StepHeight = .3f;
        [SerializeField] Vector3 n_FootOffset = default;

        public Vector3 footRotOffset;
        public float footYPosOffset = 0.1f;

        public float rayStartYOffset = 0;
        public float rayLength = 1.5f;
        
        float footSpacing;
        Vector3 oldPosition, currentPosition, newPosition;
        Vector3 oldNormal, currentNormal, newNormal;
        float lerp;

        private void Start()
        {
            footSpacing = transform.localPosition.x;
            currentPosition = newPosition = oldPosition = transform.position;
            currentNormal = newNormal = oldNormal = transform.up;
            lerp = 1;
        }

        // Update is called once per frame

        void Update()
        {
            transform.position = currentPosition + Vector3.up * footYPosOffset;
            transform.localRotation = Quaternion.Euler(footRotOffset);

            Ray ray = new Ray(m_Body.position + (m_Body.right * footSpacing) + Vector3.up * rayStartYOffset, Vector3.down);

            Debug.DrawRay(m_Body.position + (m_Body.right * footSpacing) + Vector3.up * rayStartYOffset, Vector3.down);
                
            if (Physics.Raycast(ray, out RaycastHit info, rayLength, m_TerrainLayer.value))
            {
                if (Vector3.Distance(newPosition, info.point) > m_StepDistance && !m_OtherFoot.IsMoving() && lerp >= 1)
                {
                    lerp = 0;
                    Vector3 direction = Vector3.ProjectOnPlane(info.point - currentPosition,Vector3.up).normalized;

                    float angle = Vector3.Angle(m_Body.forward, m_Body.InverseTransformDirection(direction));

                    isMovingForward = angle < 50 || angle > 130;

                    if(isMovingForward)
                    {
                        newPosition = info.point + direction * m_StepLength + n_FootOffset;
                        newNormal = info.normal;
                    }
                    else
                    {
                        newPosition = info.point + direction * sideStepLength + n_FootOffset;
                        newNormal = info.normal;
                    }

                }
            }

            if (lerp < 1)
            {
                Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
                tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * m_StepHeight;

                currentPosition = tempPosition;
                currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
                lerp += Time.deltaTime * m_Speed;
            }
            else
            {
                oldPosition = newPosition;
                oldNormal = newNormal;
            }
        }

        private void OnDrawGizmos()
        {

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(newPosition, 0.1f);
        }



        public bool IsMoving()
        {
            return lerp < 1;
        }

    }
}
