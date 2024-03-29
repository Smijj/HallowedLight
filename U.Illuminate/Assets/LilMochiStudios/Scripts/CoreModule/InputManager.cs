using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.CoreModule {
    public class InputManager : MonoBehaviour
    {

        [Header("Debug")]
        [SerializeField] private bool m_Clicking;

        [Header("Actions")]
        public static System.Action<Vector3> OnTouching;

        private Camera m_Camera;


        void Start() {
            m_Camera = Camera.main;
        }

        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                m_Clicking = true;
            }
            else if (Input.GetMouseButton(0) && m_Clicking) {
                Clicking();
            }
            else if (Input.GetMouseButtonUp(0) && m_Clicking) {
                m_Clicking = false;
            }

        }

        private void Clicking() {

            RaycastHit hit;
            Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out hit, 50f);

            if (hit.collider == null) return;

            //OnTouching?.Invoke(hit.point);
            PlayerModule.States.MiningState.OnMiningLaserContact?.Invoke(hit.point);

            //Vector3 worldPoint = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log($"Hit Point: {hit.point}\nWorldPoint: {worldPoint}");
        }
    }
}
