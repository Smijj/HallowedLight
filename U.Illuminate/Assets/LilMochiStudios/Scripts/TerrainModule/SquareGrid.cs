using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public struct SquareGrid
{
    public Square[,] Squares;

    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;

    private float m_IsoValue;

    public SquareGrid(int size, float gridScale, float isoValue) {

        this.Squares = new Square[size, size];
        this.m_Vertices = new List<Vector3>();
        this.m_Triangles = new List<int>();
        this.m_IsoValue = isoValue;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                Vector2 squarePosition = new Vector2(x, y) * gridScale;
                squarePosition.x -= (size * gridScale) / 2 - gridScale / 2;
                squarePosition.y -= (size * gridScale) / 2 - gridScale / 2;

                this.Squares[x, y] = new Square(squarePosition, gridScale);
            }
        }
    }

    public void Update(float[,] grid) {
        this.m_Vertices.Clear();
        this.m_Triangles.Clear();

        int triangleStartIndex = 0;

        for (int y = 0; y < this.Squares.GetLength(1); y++) {
            for (int x = 0; x < this.Squares.GetLength(0); x++) {
                Square currentSquare = this.Squares[x, y];

                float[] values = new float[4];
                values[0] = grid[x + 1, y + 1];  // Top Right
                values[1] = grid[x + 1, y];      // Bottom Right
                values[2] = grid[x, y];          // Bottom Left
                values[3] = grid[x, y + 1];      // Top Left

                currentSquare.Triangulate(m_IsoValue, values);
                this.m_Vertices.AddRange(currentSquare.GetVertices());

                int[] currentSquareTriangles = currentSquare.GetTriangles();
                for (int i = 0; i < currentSquareTriangles.Length; i++) {
                    currentSquareTriangles[i] += triangleStartIndex;
                }
                this.m_Triangles.AddRange(currentSquareTriangles);

                triangleStartIndex += currentSquare.GetVertices().Length;
            }
        }
    }

    public Vector3[] GetVertices() {
        return this.m_Vertices.ToArray();
    }
    public int[] GetTriangles() {
        return this.m_Triangles.ToArray();
    }
}
