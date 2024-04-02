using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public struct SquareGrid {
        
        private Square[,] m_Squares;
        public float[,] GridData;
        public float TotalGridDataValue;

        private List<Vector2> m_Vertices;
        private List<int> m_Triangles;
        private List<Vector2> m_UVs;

        private float m_IsoValue;
        private float m_GridScale;
        private int m_GridSize;

        public SquareGrid(int gridSize, float gridScale, float isoValue) {

            int squareGridSize = gridSize - 1;

            this.m_Squares = new Square[squareGridSize, squareGridSize];
            this.GridData = new float[gridSize, gridSize];

            this.m_Vertices = new List<Vector2>();
            this.m_Triangles = new List<int>();
            this.m_UVs = new List<Vector2>();

            this.m_IsoValue = isoValue;
            this.m_GridScale = gridScale;
            this.m_GridSize = gridSize;
            this.TotalGridDataValue = 0;
            
            GenerateGridData(gridSize);

            for (int y = 0; y < squareGridSize; y++) {
                for (int x = 0; x < squareGridSize; x++) {
                    // Create a position using x,y based on the size of the Grid and the GridScale, This is also then centered
                    Vector2 squarePosition = GetWorldPositionFromGridPosition(x, y, squareGridSize);
                    this.m_Squares[x, y] = new Square(squarePosition, gridScale);
                }
            }
        }

        private void GenerateGridData(int gridSize) {
            float gridValue = 0;
            for (int y = 0; y < gridSize; y++) {
                for (int x = 0; x < gridSize; x++) {
                    //GridData[x, y] = m_IsoValue + 0.1f;
                    GridData[x, y] = m_IsoValue * 2;
                    gridValue += GridData[x, y] - m_IsoValue;
                }
            }
            TotalGridDataValue = gridValue;
        }
        public void GenerateCircleGridData(int gridSize, float circleSize = 3) {
            float gridValue = 0;
            for (int y = 0; y < gridSize; y++) {
                for (int x = 0; x < gridSize; x++) {

                    // Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                    float distance = Vector2.Distance(new Vector2Int(gridSize / 2, gridSize / 2), new Vector2Int(x, y));
                    float factor = Mathf.Exp(-distance * circleSize / gridSize) * (gridSize * m_GridScale) / 6;
                    //Debug.Log($"Position: {x}, {y}. Factor: {factor}");

                    GridData[x, y] = factor;
                    if (factor > m_IsoValue) gridValue += GridData[x, y] - m_IsoValue;

                }
            }
            TotalGridDataValue = gridValue;
        }
        public void GenerateRandomShapeGridData(int gridSize, float shapeSize = 3) {
            float gridValue = 0;

            int randomVarianceRange = Random.Range(0, gridSize / Random.Range(2, 5));
            int randomX = Random.Range((gridSize / 2) - randomVarianceRange / 2, (gridSize / 2) + randomVarianceRange / 2);
            int randomY = Random.Range((gridSize / 2) - randomVarianceRange / 2, (gridSize / 2) + randomVarianceRange / 2);
            Vector2Int centerPoint = new Vector2Int(randomX, randomY);

            for (int y = 0; y < gridSize; y++) {
                for (int x = 0; x < gridSize; x++) {

                    // Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                    Vector2Int currentPoint = new Vector2Int(Random.Range(x - randomVarianceRange, x + randomVarianceRange), Random.Range(y - randomVarianceRange, y + randomVarianceRange));
                    float distance = Vector2.Distance(centerPoint, currentPoint);
                    float factor = Mathf.Exp(-distance * Random.Range(shapeSize - shapeSize / 2, shapeSize * 2) / gridSize) * (gridSize * m_GridScale) / 5;
                    //Debug.Log($"Position: {x}, {y}. Factor: {factor}");

                    GridData[x, y] = factor;
                    if (factor > m_IsoValue) gridValue += GridData[x, y] - m_IsoValue;
                }
            }
            TotalGridDataValue = gridValue;
        }
        public void GenerateRandomGridData(int gridSize) {
            for (int y = 0; y < gridSize; y++) {
                for (int x = 0; x < gridSize; x++) {
                    GridData[x, y] = Random.Range(0f, m_IsoValue * 2);
                }
            }
        }

        public void Update() {
            this.m_Vertices.Clear();
            this.m_Triangles.Clear();
            this.m_UVs.Clear();

            int triangleStartIndex = 0;

            for (int y = 0; y < this.m_Squares.GetLength(1); y++) {
                for (int x = 0; x < this.m_Squares.GetLength(0); x++) {
                    Square currentSquare = this.m_Squares[x, y];

                    float[] values = new float[4];
                    values[0] = GridData[x + 1, y + 1];  // Top Right
                    values[1] = GridData[x + 1, y];      // Bottom Right
                    values[2] = GridData[x, y];          // Bottom Left
                    values[3] = GridData[x, y + 1];      // Top Left

                    // Calculate the Vertices and triangles for the current square
                    currentSquare.Triangulate(m_IsoValue, values);

                    // Add the current Squares Vertices to the SquareGrid's vertice list
                    this.m_Vertices.AddRange(currentSquare.GetVertices());

                    // Add the current Square's Triangles to the SquareGrid's triangle list, this involve incrementing the triangles vertice index value by the triangleStartIndex (which is just the sum of all the vertices already added)
                    int[] currentSquareTriangles = currentSquare.GetTriangles();
                    for (int i = 0; i < currentSquareTriangles.Length; i++) {
                        currentSquareTriangles[i] += triangleStartIndex;
                    }
                    this.m_Triangles.AddRange(currentSquareTriangles);
                    triangleStartIndex += currentSquare.GetVertices().Length;

                    // Add UV array into SquareGrids UV list
                    this.m_UVs.AddRange(currentSquare.GetUVs());
                }
            }
        }

        public Vector2[] GetVertices() {
            return this.m_Vertices.ToArray();
        }
        /// <summary>
        /// Returns the vertices of the SquareGrid as a Vector3 Array.
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetVerticesV3() {
            List<Vector3> verticesV3 = new List<Vector3>();
            for (int i = 0; i < this.m_Vertices.Count; i++) {
                verticesV3.Add((Vector3)this.m_Vertices[i]);
            }
            return verticesV3.ToArray();
        }
        public Vector2[] GetEdgeVertices() {
            List<Vector2> edgeVertices = new List<Vector2>();

            for (int y = 0; y < m_GridSize; y++) {
                for (int x = 0; x < m_GridSize; x++) {
                    if (GridData[x, y] < m_IsoValue) continue;  // Isn't part of an edge

                    // Check grid positions adjacent points if they are less then the iso value, if so this is an edge vertice.
                    if (!IsValidGridPosition(x - 1, y, m_GridSize) || 
                        GridData[x - 1, y] < m_IsoValue ||
                        !IsValidGridPosition(x, y + 1, m_GridSize) ||
                        GridData[x, y + 1] < m_IsoValue ||
                        !IsValidGridPosition(x + 1, y, m_GridSize) ||
                        GridData[x + 1, y] < m_IsoValue ||
                        !IsValidGridPosition(x, y - 1, m_GridSize) ||
                        GridData[x, y - 1] < m_IsoValue) 
                        {
                        edgeVertices.Add(GetWorldPositionFromGridPosition(x, y, m_GridSize));
                    }
                }
            }

            return edgeVertices.ToArray();
        }
        public int[] GetTriangles() {
            return this.m_Triangles.ToArray();
        }
        public Vector2[] GetUVs() {
            return this.m_UVs.ToArray();
        }
        private bool IsValidGridPosition(int x, int y, int gridSize) {
            return x >= 0f && x < gridSize && y >= 0f && y < gridSize;
        }
        private Vector2 GetWorldPositionFromGridPosition(int x, int y, float gridSize) {
            Vector2 worldPosition = new Vector2(x, y) * m_GridScale;
            worldPosition.x -= (gridSize * m_GridScale) / 2 - m_GridScale / 2;
            worldPosition.y -= (gridSize * m_GridScale) / 2 - m_GridScale / 2;
            return worldPosition;
        }
    }
}
