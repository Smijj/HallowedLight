using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshGenerator.SquareGrid;

public class MeshGenerator : MonoBehaviour
{

    [SerializeField] private int m_MapSize = 10;
    [SerializeField] private int m_SquareSize = 1;
    [SerializeField] private SquareGrid m_SquareGrid;
    [SerializeField] private List<Vector3> m_Vertices;
    [SerializeField] private List<int> m_Triangles;

    private void Start() {
        GenerateMesh(GenerateGridData(m_MapSize), m_SquareSize);
    }
    private int[,] GenerateGridData(int size) {
        int[,] map = new int[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                map[x, y] = 1;
            }
        }
        return map;
    }

    public void GenerateMesh(int[,] map, float squareSize) {
        m_SquareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < m_SquareGrid.Squares.GetLength(0); x++) {
            for (int y = 0; y < m_SquareGrid.Squares.GetLength(1); y++) {
                TriangulateSquare(m_SquareGrid.Squares[x, y]);
            }
        }
    }

    private void TriangulateSquare(Square square) {
        switch (square.Configuration) {
            case 0:
            break;

            // 1 points
            case 1:
                MeshFromPoints(square.CentreBottom, square.BottomLeft, square.CentreLeft);
                break;
            case 2:
                MeshFromPoints(square.CentreRight, square.BottomRight, square.CentreBottom);
                break;
            case 4:
                MeshFromPoints(square.CentreTop, square.TopRight, square.CentreRight);
                break;
            case 8:
                MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreLeft);
                break;

            // 2 points
            case 3:
                MeshFromPoints(square.CentreRight, square.BottomRight, square.BottomLeft, square.CentreLeft);
                break;
            case 6:
                MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.CentreBottom);
                break;
            case 9:
                MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreBottom, square.BottomLeft);
                break;
            case 12:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreLeft);
                break;
           
            case 5:
                MeshFromPoints(square.CentreTop, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft, square.CentreLeft);
                break;
            case 10:
                MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.CentreBottom, square.CentreLeft);
                break;

            // 3 points
            case 7:
                MeshFromPoints(square.CentreTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CentreLeft);
                break;
            case 11:
                MeshFromPoints(square.TopLeft, square.CentreTop, square.CentreRight, square.BottomRight, square.BottomLeft);
                break;
            case 13:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CentreRight, square.CentreBottom, square.BottomLeft);
                break;
            case 14:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CentreBottom, square.CentreLeft);
                break;

            // 4 points
            case 15:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
                break;
        } 
    } 

    private void MeshFromPoints(params Node[] points) {
        AssignVertices(points);

        if (points.Length >= 3) {
            CreateTriangle(points[0], points[1], points[2]);
        }
        if (points.Length >= 4) {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5) {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6) {
            CreateTriangle(points[0], points[4], points[5]);
        }

        //for (int i = 0; i < points.Length - 2; i++)
        //    CreateTriangle(points[0], points[i + 1], points[i + 2]);
    }

    private void AssignVertices(Node[] points) {
        for (int i = 0; i < points.Length; i++) {
            if (points[i].VetexIndex == -1) {
                points[i].VetexIndex = m_Vertices.Count;
                m_Vertices.Add(points[i].Position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c) {
        m_Triangles.Add(a.VetexIndex);
        m_Triangles.Add(b.VetexIndex);
        m_Triangles.Add(c.VetexIndex);
    }

    private void OnDrawGizmos() {
        if (m_SquareGrid == null || m_SquareGrid.Squares == null) return;

        for (int x = 0; x < m_SquareGrid.Squares.GetLength(0); x++) {
            for (int y = 0; y < m_SquareGrid.Squares.GetLength(1); y++) {
                Gizmos.color = m_SquareGrid.Squares[x, y].TopLeft.Active ? Color.white : Color.red;
                Gizmos.DrawCube(m_SquareGrid.Squares[x, y].TopLeft.Position, Vector3.one * 0.4f);

                Gizmos.color = m_SquareGrid.Squares[x, y].TopRight.Active ? Color.white : Color.red;
                Gizmos.DrawCube(m_SquareGrid.Squares[x, y].TopRight.Position, Vector3.one * 0.4f);

                Gizmos.color = m_SquareGrid.Squares[x, y].BottomRight.Active ? Color.white : Color.red;
                Gizmos.DrawCube(m_SquareGrid.Squares[x, y].BottomRight.Position, Vector3.one * 0.4f);

                Gizmos.color = m_SquareGrid.Squares[x, y].BottomLeft.Active ? Color.white : Color.red;
                Gizmos.DrawCube(m_SquareGrid.Squares[x, y].BottomLeft.Position, Vector3.one * 0.4f);


                Gizmos.color = Color.grey;
                Gizmos.DrawSphere(m_SquareGrid.Squares[x, y].CentreTop.Position, 0.15f);
                Gizmos.DrawSphere(m_SquareGrid.Squares[x, y].CentreRight.Position, 0.15f);
                Gizmos.DrawSphere(m_SquareGrid.Squares[x, y].CentreBottom.Position, 0.15f);
                Gizmos.DrawSphere(m_SquareGrid.Squares[x, y].CentreLeft.Position, 0.15f);
            }
        }
    }

    [System.Serializable]
    public class SquareGrid {
        public Square[,] Squares;

        public SquareGrid(int[,] map, float squareSize) {

            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++) {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, -mapHeight / 2 + y * squareSize + squareSize / 2, 0);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    [System.Serializable]
    public class Square {
        public ControlNode TopRight, BottomRight, BottomLeft, TopLeft;
        public Node CentreTop, CentreRight, CentreBottom, CentreLeft;
        public int Configuration;

        public Square(ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft, ControlNode topLeft) {
            TopRight = topRight;   
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
            TopLeft = topLeft;

            CentreTop = TopLeft.Right;  
            CentreRight = BottomLeft.Above;
            CentreBottom = BottomLeft.Right;
            CentreLeft = BottomLeft.Above;

            if (topLeft.Active)
                Configuration += 8;
            if (topRight.Active)
                Configuration += 4;
            if (bottomRight.Active)
                Configuration += 2;
            if (bottomLeft.Active)
                Configuration += 1;
        }
    }

    [System.Serializable]
    public class Node {
        public Vector3 Position;
        public int VetexIndex = -1;

        public Node(Vector3 _pos) {
            Position = _pos;
        }
    }

    [System.Serializable]
    public class ControlNode : Node {
        public bool Active;
        public Node Above, Right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            Active = _active;
            Above = new Node(Position + Vector3.up * squareSize / 2f);
            Right = new Node(Position + Vector3.right * squareSize / 2f);
        }
    }
}

