using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LilMochiStudios.PlayerModule {
    public class FlowFieldGridCtrl : MonoBehaviour
    {
        public Vector2Int GridSize;
        public float CellRadius = 0.5f;
        public FlowField CurrentFlowField;

        [SerializeField] private Transform m_GridTarget;
        [ReadOnly, SerializeField] private Vector3 m_GridTargetLastPos;
        [SerializeField] private Cell m_DestinationCell;

        private void Start() {
            InitializeFlowField();
        }
        private void InitializeFlowField() {
            m_GridTargetLastPos = m_GridTarget.position;
            
            CurrentFlowField = new FlowField(CellRadius, GridSize);
            CurrentFlowField.CreateGrid(m_GridTarget);

            int gridPosX = Mathf.FloorToInt(GridSize.x / 2);
            int gridPosY = Mathf.FloorToInt(GridSize.y / 2);
            m_DestinationCell = CurrentFlowField.Grid[gridPosX, gridPosY];

            CurrentFlowField.InitializeFlowField(m_DestinationCell);
        }

        private void Update() {
            if (m_GridTarget.position != m_GridTargetLastPos) {
                m_GridTargetLastPos = m_GridTarget.position;

                CurrentFlowField.UpdateFlowField(m_DestinationCell);
            }

            // Debuging 
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    Cell currentCell = CurrentFlowField.Grid[x, y];
                    Debug.DrawRay(currentCell.WorldPosition, (Vector2)currentCell.BestDirection.Vector, Color.white);
                }
            }
        }
        
        

        private void OnDrawGizmos() {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.green;

            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    Vector3 drawPos = CurrentFlowField.Grid[x, y].WorldPosition;
                    Gizmos.DrawSphere(drawPos, CellRadius / 4);
#if UNITY_EDITOR
                    //Handles.Label(drawPos + Vector3.forward, CurrentFlowField.Grid[x, y].BestCost.ToString());
                    Handles.Label(drawPos + Vector3.forward, $"x:{x}, y:{y}");
#endif
                }
            }
        }
    }
}
