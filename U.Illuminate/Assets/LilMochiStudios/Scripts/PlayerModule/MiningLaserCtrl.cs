using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace LilMochiStudios.PlayerModule {
    public class MiningLaserCtrl : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private float m_LaserRange = 10f;

        private bool m_Clicking;
        private Camera m_Camera;

        void Start() {
            m_Camera = Camera.main;
        }

        private void Update() {
            RotateToMousePosition();

            if (Input.GetMouseButtonDown(0)) {
                m_Clicking = true;
            } else if (Input.GetMouseButton(0) && m_Clicking) {
                Clicking();
            } else if (Input.GetMouseButtonUp(0) && m_Clicking) {
                m_Clicking = false;
            }


            Debug.DrawRay(transform.position, transform.right, Color.red);
        }

        private void Clicking() {

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.right);
            Physics.Raycast(ray, out hit, m_LaserRange);

            if (hit.collider == null) return;

            States.MiningState.OnMiningLaserContact?.Invoke(hit.point);

            //Vector3 worldPoint = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log($"Hit Point: {hit.point}\nWorldPoint: {worldPoint}");
        }

        private void RotateToMousePosition() {
            Vector2 mousePosition = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionToMouse = mousePosition - (Vector2)transform.position;

            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
