using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.PlayerModule {
    public class FlowField
    {
        public Cell[,] Grid { get; private set; }
        public Vector2Int GridSize { get; private set; }
        public float CellRadius { get; private set; }
        public Cell DestinationCell { get; private set; }

        private float m_CellDiameter;
        private float m_IsoValue;
        private Transform m_FollowTarget;

        public FlowField(float _cellRadius, Vector2Int _gridSize) {
            CellRadius = _cellRadius;
            m_CellDiameter = CellRadius * 2;
            GridSize = _gridSize;
            m_IsoValue = (float)TerrainModule.States.TerrainState.OnGetIsoValue?.Invoke();
        }

        public void InitializeFlowField(Cell destinationCell) {
            UpdateCostField();
            CreateIntegrationField(destinationCell);
            CreateFlowField();
        }

        public void UpdateFlowField(Cell destinationCell) {
            UpdateGrid();
            UpdateCostField();
            CreateIntegrationField(destinationCell);
            CreateFlowField();
        }

        public void CreateGrid(Transform followTarget) {
            Grid = new Cell[GridSize.x, GridSize.y];
            m_FollowTarget = followTarget;

            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    Vector3 worldPosition = m_FollowTarget.position + new Vector3(m_CellDiameter * x - (GridSize.x * m_CellDiameter / 2) + CellRadius,
                        m_CellDiameter * y - (GridSize.y * m_CellDiameter / 2) + CellRadius, 0);

                    Grid[x, y] = new Cell(worldPosition, new Vector2Int(x, y));
                }
            }
        }

        private void UpdateGrid() {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    Vector3 worldPosition = m_FollowTarget.position + new Vector3(m_CellDiameter * x - (GridSize.x * m_CellDiameter / 2) + CellRadius,
                        m_CellDiameter * y - (GridSize.y * m_CellDiameter / 2) + CellRadius, 0);
                    
                    Grid[x, y].UpdateWorldPosition(worldPosition);
                }
            }
        }

        private void UpdateCostField() {
            foreach (Cell cell in Grid) {
                float cellIsoValue = (float)TerrainModule.States.TerrainState.OnGetGridIsoValueFromWorldPos?.Invoke(cell.WorldPosition);

                if (cellIsoValue >= m_IsoValue) {
                    cell.SetCost(255);
                } else {
                    cell.SetCost(1);
                }
                cell.BestCost = ushort.MaxValue;
                cell.BestDirection = GridDirection.None;
            }
        }

        private void CreateIntegrationField(Cell destinationCell) {
            DestinationCell = destinationCell;

            DestinationCell.Cost = 0;
            DestinationCell.BestCost = 0;

            Queue<Cell> cellsToCheck = new Queue<Cell>();

            cellsToCheck.Enqueue(destinationCell);

            while(cellsToCheck.Count > 0) {
                Cell currentCell = cellsToCheck.Dequeue();
                List<Cell> currentNeighbours = GetNeighbourCells(currentCell.GridIndex, GridDirection.CardinalDirections);

                foreach (var currentNeighbour in currentNeighbours) {
                    if (currentNeighbour.Cost == byte.MaxValue) continue;
                    if (currentNeighbour.Cost + currentCell.BestCost < currentNeighbour.BestCost) {
                        currentNeighbour.BestCost = (ushort)(currentNeighbour.Cost + currentCell.BestCost);
                        cellsToCheck.Enqueue(currentNeighbour);
                    }
                }
            }
        }

        private void CreateFlowField() {
            foreach (var currentCell in Grid) {
                List<Cell> currentNeighbours = GetNeighbourCells(currentCell.GridIndex, GridDirection.AllDirections);

                int bestCost = currentCell.BestCost;

                foreach (var currentNeighbour in currentNeighbours) {
                    if (currentNeighbour.BestCost < bestCost) {
                        bestCost = currentNeighbour.BestCost;
                        currentCell.BestDirection = GridDirection.GetDirectionFromV2I(currentNeighbour.GridIndex - currentCell.GridIndex);
                    }
                }
            }
        }

        private List<Cell> GetNeighbourCells(Vector2Int nodeIndex, List<GridDirection> directions) {
            List<Cell> neighbourCells = new List<Cell>();

            foreach (var currentDirection in directions) {
                Cell newNeighbour = GetCellsAtRelativePos(nodeIndex, currentDirection);
                if (newNeighbour != null) {
                    neighbourCells.Add(newNeighbour);
                }
            }
            return neighbourCells;
        } 

        private Cell GetCellsAtRelativePos(Vector2Int orignPos, Vector2Int relativePos) {
            Vector2Int finalPos = orignPos + relativePos;
            if (finalPos.x < 0 || finalPos.x >= GridSize.x || finalPos.y < 0 || finalPos.y >= GridSize.y) {
                return null;
            }
            return Grid[finalPos.x, finalPos.y];
        }

        public Cell GetCellFromWorldPos(Vector3 worldPos) {
            worldPos = m_FollowTarget.transform.InverseTransformPoint(worldPos);

            Vector2Int gridPos = new Vector2Int();
            gridPos.x = Mathf.Clamp(Mathf.FloorToInt(worldPos.x / m_CellDiameter + (GridSize.x / 2) - CellRadius), 0, GridSize.x - 1);
            gridPos.y = Mathf.Clamp(Mathf.FloorToInt(worldPos.y / m_CellDiameter + (GridSize.y / 2) - CellRadius), 0, GridSize.y - 1);
            //gridPos.y = Mathf.FloorToInt(worldPos.y / m_CellDiameter + (GridSize.y / 2) - CellRadius);

            return Grid[gridPos.x, gridPos.y];
        }
    }
}
