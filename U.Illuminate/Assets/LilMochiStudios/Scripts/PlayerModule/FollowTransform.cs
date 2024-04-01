using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.PlayerModule {
    public class FollowTransform : MonoBehaviour {

        [SerializeField] private Transform m_FollowPointTransform;
        [SerializeField] private bool m_ExcludeZAxis = false;
        [SerializeField] private float m_MaxMoveSpeed = 10;
        [SerializeField] private float m_SmoothTime = 0.3f;

        private Vector3 m_CurrentVelocityV3;
        private Vector2 m_CurrentVelocityV2;

        private void Update() {
            // Follows the target transform with a slight delay
            if (m_ExcludeZAxis) {
                Vector3 newPosition = new Vector3(m_FollowPointTransform.position.x, m_FollowPointTransform.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref m_CurrentVelocityV3, m_SmoothTime, m_MaxMoveSpeed);
            } else {
                transform.position = Vector2.SmoothDamp(transform.position, m_FollowPointTransform.position, ref m_CurrentVelocityV2, m_SmoothTime, m_MaxMoveSpeed);
            }
        }
    }
}
