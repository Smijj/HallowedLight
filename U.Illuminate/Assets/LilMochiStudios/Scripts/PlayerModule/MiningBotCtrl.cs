using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.PlayerModule {
    public class MiningBotCtrl : MonoBehaviour {

        [SerializeField] private Transform m_FollowPointTransform;
        [SerializeField] private float m_MaxMoveSpeed = 10;
        [SerializeField] private float m_SmoothTime = 0.3f;

        private Vector2 m_CurrentVelocity;

        private void Update() {
            // Follows the player with a slight delay
            transform.position = Vector2.SmoothDamp(transform.position, m_FollowPointTransform.position, ref m_CurrentVelocity, m_SmoothTime, m_MaxMoveSpeed);
        }
    }
}
