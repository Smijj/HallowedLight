using LilMochiStudios.TerrainModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {

    public class OreDropCtrl : MonoBehaviour
    {
        [Header("Ore Drop Settings")]
        [SerializeField] private float m_HangTime = 1.5f;
        [ReadOnly, SerializeField] private float m_HangTimeCounter = 0f;
        [SerializeField] private float m_MaxMoveSpeed = 20;
        [SerializeField] private float m_SmoothTime = 0.6f;
        [SerializeField] private float m_AbsorbRange = 0.5f;

        [Header("Elements")]
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        private SphereCollider m_Collider;
        private Rigidbody m_Rb;

        private Transform m_Target;
        private MaterialDataSO m_Material;
        private Vector2 m_CurrentVelocity;
        private bool m_Initialized = false;

        private void Start() {
            if (!m_Collider) m_Collider = GetComponent<SphereCollider>();
            if (!m_Rb) m_Rb = GetComponent<Rigidbody>();
        }

        public void Initialize(Transform target, MaterialDataSO material) {
            m_Target = target;
            m_Material = material;

            if (m_SpriteRenderer) m_SpriteRenderer.sprite = material.GetDropSprite();

            m_Initialized = true;
        }

        private void Update() {
            if (!m_Initialized) return;

            if (m_HangTimeCounter < m_HangTime) {
                m_HangTimeCounter += Time.deltaTime;
                return;
            }

            // Disable colliders and gravity when it flys towards target position, to prevent any gamebreaking collisions
            if (m_Collider.enabled) m_Collider.enabled = false;
            if (m_Rb.useGravity) {
                m_Rb.useGravity = false;
                m_Rb.velocity = Vector3.zero;
            }

            transform.position = Vector2.SmoothDamp(transform.position, m_Target.position, ref m_CurrentVelocity, m_SmoothTime, m_MaxMoveSpeed);

            if (Vector3.Distance(transform.position, m_Target.position) < m_AbsorbRange) {
                AddOreToQuota(m_Material);
            }
        }

        private void AddOreToQuota(MaterialDataSO material) {
            CoreModule.States.QuotaState.OnAddToQuota?.Invoke(material);
            Destroy(gameObject);
        }
    }
}
