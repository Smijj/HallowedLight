using LilMochiStudios.PlayerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private FlowFieldGridCtrl m_GridCtrl;
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private int m_NumberOfUnits;
    [SerializeField] private float m_MoveSpeed;

    [SerializeField] private List<GameObject> m_Units = new List<GameObject>();


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SpawnUnits();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            DestroyUnits();
        }
    }

    private void FixedUpdate() {
        
        if (m_GridCtrl == null || m_GridCtrl.CurrentFlowField == null) return;

        foreach (var unit in m_Units) {
            Cell cellBelow = m_GridCtrl.CurrentFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 moveDirection = (Vector2)cellBelow.BestDirection.Vector;
            Rigidbody unitRB = unit.GetComponent<Rigidbody>();
            unitRB.velocity = moveDirection * m_MoveSpeed;
        }

    }


    private void SpawnUnits() {
        Vector2Int gridSize = m_GridCtrl.GridSize;
        float nodeRadius = m_GridCtrl.CellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        int colMask = LayerMask.GetMask("Destructable");
        Vector3 newPos;
        for (int i = 0; i < m_NumberOfUnits; i++) {
            GameObject newUnit = Instantiate(m_EnemyPrefab, transform);
            m_Units.Add(newUnit);

            //do {
                newPos = new Vector3(Random.Range(0, maxSpawnPos.x), Random.Range(0, maxSpawnPos.y), 0);
                newUnit.transform.position = newPos;
            //} while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);

        }
    }

    private void DestroyUnits() {
        foreach (var unit in m_Units) {
            Destroy(unit);
        }
        m_Units.Clear();
    }
}
