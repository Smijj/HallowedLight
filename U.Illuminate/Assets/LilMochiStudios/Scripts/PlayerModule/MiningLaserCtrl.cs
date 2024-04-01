using LilMochiStudios.TerrainModule;
using LilMochiStudios.TerrainModule.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace LilMochiStudios.PlayerModule {
    public class MiningLaserCtrl : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private float m_LaserRange = 10f;
        [SerializeField] private float m_LaserWidth = 0.01f;
        [SerializeField] private LayerMask m_DestructableLayerMask;
        
        [SerializeField] private float m_SFXDelayMin = 0.3f;
        [SerializeField] private float m_SFXDelayMax = 0.6f;
        [ReadOnly, SerializeField] private float m_SFXDelay;
        [ReadOnly, SerializeField] private float m_SFXDelayCounter = 0;
        
        [Header("Elements")]
        [SerializeField] private LineRenderer m_LaserLine;
        [SerializeField] private Transform m_LaserTransform;
        [SerializeField] private Transform m_PlayerTransform;
        [SerializeField] private ParticleSystem m_LaserHitPartcleSysPrefab;
        [SerializeField] private ParticleSystem m_LaserHoldPartcleSysPrefab;
        [SerializeField] private ParticleSystem m_LaserHoldPartcleSys;

        private bool m_Clicking;
        private Camera m_Camera;


        void Start() {
            m_Camera = Camera.main;
            if (!m_LaserTransform) m_LaserTransform = this.transform;

            if (m_LaserLine) {
                m_LaserLine.SetPosition(0, Vector3.zero);
                m_LaserLine.SetPosition(1, Vector3.zero);
                m_LaserLine.startWidth = m_LaserWidth;
                m_LaserLine.endWidth = m_LaserWidth;
            }
        }

        private void Update() {
            if (PauseMenuCtrl.Paused) return;

            RotateToMousePosition();

            if (Input.GetMouseButtonDown(0)) {
                m_Clicking = true;
                
                // Sfx
                m_SFXDelay = Random.Range(m_SFXDelayMin, m_SFXDelayMax);
                m_SFXDelayCounter = m_SFXDelay;     // Play sfx straight away when player clicks

                // Vfx
                m_LaserHoldPartcleSys = Instantiate(m_LaserHoldPartcleSysPrefab);

            } else if (Input.GetMouseButton(0) && m_Clicking) {
                Clicking();
            } else if (Input.GetMouseButtonUp(0) && m_Clicking) {
                m_Clicking = false;
                DeactivateLaser();

                Destroy(m_LaserHoldPartcleSys.gameObject);
            }
        }

        private void Clicking() {

            RaycastHit hit;
            Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out hit, 50f);
            
            // Has the player clicked on something destructable?
            if (hit.collider == null) {
                DeactivateLaser();
                return;
            }
            // Is that destructable thing within range of the player?
            if (Vector3.Distance(hit.point, m_PlayerTransform.position) > m_LaserRange) {
                DeactivateLaser();
                return;
            }

            // Draw laser line
            ActivateLaser(hit.point);

            m_LaserHoldPartcleSys.transform.position = (Vector2)hit.point;

            // Invoke mining event
            DestructableState.OnDestructableContact?.Invoke(hit.point);

            // SFX
            if (m_SFXDelayCounter >= m_SFXDelay) {
                AudioManager.Instance.PlaySFX("LaserHit");
                m_SFXDelayCounter = 0;
                m_SFXDelay = Random.Range(m_SFXDelayMin, m_SFXDelayMax);

                // Spawn particle effects
                Instantiate(m_LaserHitPartcleSysPrefab, (Vector2)hit.point, Quaternion.identity);
            }
            m_SFXDelayCounter += Time.deltaTime;
        }

        private void RotateToMousePosition() {
            Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionToMouse = mousePosition - (Vector2)m_LaserTransform.position;

            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            m_LaserTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void ActivateLaser(Vector3 laserTargetInWorldSpace) {
            Vector2 laserTarget = m_LaserLine.transform.InverseTransformPoint(laserTargetInWorldSpace);
            m_LaserLine.SetPosition(1, laserTarget);

            // Position and turn on point light

            
        }

        private void DeactivateLaser() {
            m_LaserLine.SetPosition(1, Vector3.zero);

            // turn off point light
        }

    }
}
