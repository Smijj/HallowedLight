using LilMochiStudios.CoreModule;
using LilMochiStudios.TerrainModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LilMochiStudios.PlayerModule {
    public class PlayerCtrl : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_MoveSpeed = 1;
        [SerializeField] private float m_JumpForce = 3;
        [SerializeField] private int m_MaxJumps = 2;
        [ReadOnly, SerializeField] private int m_JumpCounter = 0;

        [Header("Light Settings")]
        [SerializeField] private int m_MaxActiveLights = 4;
        [SerializeField] private MaterialDataSO m_LightMaterial;
        [SerializeField] private ThrowableLightCtrl m_LightPrefab;
        [SerializeField] private Transform m_LightsParent;
        [SerializeField] private List<ThrowableLightCtrl> m_ActiveLights = new List<ThrowableLightCtrl>();

        private Rigidbody m_Rb;
        private Animator m_Anim;
        private Vector2 m_Input;
        private Camera m_Camera;

        private bool m_FacingRight = true;


        private void Start() {
            m_Rb = GetComponent<Rigidbody>();
            m_Anim = GetComponent<Animator>();
            m_Camera = Camera.main;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
            }

            if (Input.GetMouseButtonDown(1)) {
                ThrowLight();
            }

            if (Input.mousePosition.x <= Screen.width/2) {
                m_FacingRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
            } else {
                transform.localScale = new Vector3(1, 1, 1);
                m_FacingRight = true;
            }

            
        }

        private void FixedUpdate() {
            HandleMovement();
        }

        private void OnCollisionEnter(Collision collision) {
            m_JumpCounter = 0;
        }

        private void HandleMovement() {
            m_Input.x = Input.GetAxisRaw("Horizontal");
            m_Input.y = Input.GetAxisRaw("Vertical");

            if (m_Input.x < -0.1f || m_Input.x > 0.1f) {    
                Vector3 moveDirection = new Vector3(m_Input.normalized.x * m_MoveSpeed, 0, 0);
                m_Rb.AddForce(moveDirection, ForceMode.Impulse);
                m_Anim.Play("PlayerMovement");
            } else {
                m_Anim.Play("PlayerIdle");
            }
        }

        private void Jump() {
            if (m_JumpCounter < m_MaxJumps) {
                m_Rb.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
                m_JumpCounter++;
            }
        }

        private void ThrowLight() {
            if (m_ActiveLights.Count >= m_MaxActiveLights) return;

            // Check if there is any Illumanium to throw
            QuotaManager.QuotaItem quotaItem = CoreModule.States.QuotaState.OnGetQuotaItem?.Invoke(m_LightMaterial);
            if (quotaItem == null || quotaItem.Collected <= 0) return;

            // Instantiate prefab
            ThrowableLightCtrl light = Instantiate(m_LightPrefab, transform.position + Vector3.up, Quaternion.identity, m_LightsParent);
            m_ActiveLights.Add(light);
            light.SetLightDestroyedCallback(OnLightDestroyed);

            // Give object force in the direction of the mouse
            Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;
            light.AddForce(directionToMouse);

            // Reduce quota for illumanium
            CoreModule.States.QuotaState.OnRemoveFromQuota?.Invoke(m_LightMaterial);

        }

        private void OnLightDestroyed(ThrowableLightCtrl destroyedLight) {
            m_ActiveLights.Remove(destroyedLight);
        }
    }
}