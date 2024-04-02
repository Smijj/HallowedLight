using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrack : MonoBehaviour {
    [SerializeField] private string m_TrackName;
    private TriggerConditions m_TriggerCondition = TriggerConditions.Trigger;
    [SerializeField] private bool m_DisableOnTrigger = true;

    private void Start() {
        if (m_TriggerCondition == TriggerConditions.Start) {
            AudioManager.Instance.PlayTrack(m_TrackName);
        } 
        else {
            Collider2D collider = GetComponent<Collider2D>();
            if(collider == null) {
#if LI_Debug
                Debug.Log("There is no Collider.");
#endif
                return;
            }
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;

        if (m_TriggerCondition == TriggerConditions.Trigger) {
            AudioManager.Instance.PlayTrack(m_TrackName);
            if (m_DisableOnTrigger) {
                gameObject.SetActive(false);
            }
        }
    }
}

