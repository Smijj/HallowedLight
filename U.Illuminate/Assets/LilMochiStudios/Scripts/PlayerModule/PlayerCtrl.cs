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

        private Rigidbody rb;
        private Vector2 m_Input;


        private void Start() {
            rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Jump();
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
                rb.AddForce(moveDirection, ForceMode.Impulse);
            }
        }

        private void Jump() {
            if (m_JumpCounter < m_MaxJumps) {
                rb.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
                m_JumpCounter++;
            }
        }
    }
}