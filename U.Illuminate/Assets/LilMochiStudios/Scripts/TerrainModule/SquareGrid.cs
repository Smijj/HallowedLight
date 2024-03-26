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
    private List<Vector2> m_UVs;

    private float m_IsoValue;
    private float m_GridScale;

    public SquareGrid(int size, float gridScale, float isoValue) {

        this.Squares = new Square[size, size];
        this.m_Vertices = new List<Vector3>();
        this.m_Triangles = new List<int>();
        this.m_UVs = new List<Vector2>();
        this.m_IsoValue = isoValue;
        this.m_GridScale = gridScale;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                // Create a position based on the xy of the grid amd the GridScale, then centre it
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
        this.m_UVs.Clear();

        int triangleStartIndex = 0;

        for (int y = 0; y < this.Squares.GetLength(1); y++) {
            for (int x = 0; x < this.Squares.GetLength(0); x++) {
                Square currentSquare = this.Squares[x, y];

                float[] values = new float[4];
                values[0] = grid[x + 1, y + 1];  // Top Right
                values[1] = grid[x + 1, y];      // Bottom Right
                values[2] = grid[x, y];          // Bottom Left
                values[3] = grid[x, y + 1];      // Top Left

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

                // Offset UVs so that textures applied to the terrain are correctly displayed
                Vector2[] uvArray = currentSquare.GetUVs();
                for (int i = 0; i < uvArray.Length; i++) {
                    uvArray[i] /= this.Squares.GetLength(0);
                    uvArray[i] += Vector2.one * this.m_GridScale / 2;
                }

                // Add offset UV array into SquareGrids UV list
                this.m_UVs.AddRange(uvArray);
            }
        }
    }

    public Vector3[] GetVertices() {
        return this.m_Vertices.ToArray();
    }
    public int[] GetTriangles() {
        return this.m_Triangles.ToArray();
    }
    public Vector2[] GetUVs() {
        return this.m_UVs.ToArray();
    }
}
