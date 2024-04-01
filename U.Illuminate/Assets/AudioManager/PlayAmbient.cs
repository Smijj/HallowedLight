using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbient : MonoBehaviour
{
    [SerializeField] private string m_TrackName;
    [Header("When Trigger is used, the track will start/stop on trigger enter/exit")]
    [SerializeField] private TriggerConditions m_TriggerCondition = TriggerConditions.Trigger;

    private void Start() {
        if (m_TriggerCondition == TriggerConditions.Start) {
            AudioManager.Instance.PlayAmbient(m_TrackName);
        } else {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) {
#if LI_Debug
                Debug.Log("There is no Collider.");
#endif
                return;
            }
            collider.isTrigger = true;
        }
    }

    private void OnDisable() {
        AudioManager.Instance.StopAmbient(m_TrackName);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;

        if (m_TriggerCondition == TriggerConditions.Trigger) {
            AudioManager.Instance.PlayAmbient(m_TrackName);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;

        if (m_TriggerCondition == TriggerConditions.Trigger) {
            AudioManager.Instance.StopAmbient(m_TrackName);
        }
    }
}
