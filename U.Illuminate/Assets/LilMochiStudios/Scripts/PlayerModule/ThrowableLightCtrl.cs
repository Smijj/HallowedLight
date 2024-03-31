using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableLightCtrl : MonoBehaviour
{
    [SerializeField] private float m_ThrowForce = 5f;
    [SerializeField] private float m_LightDuration = 30f;
    [ReadOnly, SerializeField] private float m_LightDurationCounter = 0;
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] private Collider m_Collider;

    //[SerializeField] private int m_MaxBounces = 4;
    //[ReadOnly, SerializeField] private int m_BounceCounter = 0;
    //private bool m_IsStuck = false;

    private System.Action<ThrowableLightCtrl> m_LightDestroyedCallback;

    private void Start() {
        if (!m_Rb) m_Rb = GetComponent<Rigidbody>();
        if (!m_Collider) m_Collider = GetComponent<Collider>();
    }
    private void Update() {
        if (m_LightDuration.Equals(-1)) return;

        m_LightDurationCounter += Time.deltaTime;   
        if (m_LightDurationCounter > m_LightDuration) {
            DestroyLight();
        }
    }

    //private void OnCollisionEnter(Collision collision) {
    //    if (m_IsStuck) return;

    //    m_BounceCounter++;

    //    if (m_BounceCounter >= m_MaxBounces) {
    //        // disable collider and rigidbody the first time the light touches something, simulating sticky light!
    //        if (m_Collider) m_Collider.enabled = false;
    //        if (m_Rb) {
    //            m_Rb.useGravity = false;
    //            m_Rb.freezeRotation = true;
    //            m_Rb.velocity = Vector3.zero;
    //        }

    //        m_IsStuck = true;
    //    }
    //}


    public void SetLightDestroyedCallback(System.Action<ThrowableLightCtrl> callback) {
        m_LightDestroyedCallback = callback;
    }

    public void AddForce(Vector3 forceDirection) {
        m_Rb.AddForce(forceDirection * m_ThrowForce, ForceMode.Impulse);
    }

    private void DestroyLight() {
        // destroy light callback
        m_LightDestroyedCallback?.Invoke(this);

        // destroy light
        Destroy(gameObject);
    }

}
