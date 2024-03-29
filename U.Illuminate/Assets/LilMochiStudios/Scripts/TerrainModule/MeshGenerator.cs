using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private int m_MapSize = 10;
    [SerializeField] private int m_SquareSize = 1;
    
    [Header("Debug")]
    [SerializeField] private List<Vector3> m_Vertices;
    [SerializeField] private List<int> m_Triangles;
    private SquareGrid m_SquareGrid;

    private Dictionary<int, List<Triangle>> m_TriangleDict = new Dictionary<int, List<Triangle>>();
    private List<List<int>> m_Outlines = new List<List<int>>();
    private HashSet<int> m_CheckedVertices = new HashSet<int>();

    private MeshFilter m_MeshFilter;
    [SerializeField] private MeshFilter m_WallMeshFilter;



    private void Start() {
        if (!m_MeshFilter) m_MeshFilter = GetComponent<MeshFilter>();

        GenerateMesh(GenerateGridData(m_MapSize), m_SquareSize);
    }
    private int[,] GenerateGridData(int size) {
        int[,] map = new int[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                map[x, y] = 1;
                //map[x, y] = Random.Range(0,2);
            }
        }
        return map;
    }
    public int[,] GenerateCircleGridData(int size, float circleSize) {
        int[,] map = new int[size, size];
        
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                // Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                float distance = Vector2.Distance(new Vector2Int(size / 2, size / 2), new Vector2Int(x, y));
                float factor = Mathf.Exp(-distance * circleSize / size) * size / 6;

                map[x, y] = factor >= 1 ? 1 : 0;
            }
        }
        return map;
    }

    public void GenerateMesh(int[,] map, float squareSize) {

        m_Outlines.Clear();
        m_CheckedVertices.Clear();
        m_TriangleDict.Clear();

        m_SquareGrid = new SquareGrid(map, squareSize);

        m_Vertices = new List<Vector3>();
        m_Triangles = new List<int>();

        for (int x = 0; x < m_SquareGrid.Squares.GetLength(0); x++) {
            for (int y = 0; y < m_SquareGrid.Squares.GetLength(1); y++) {
                TriangulateSquare(m_SquareGrid.Squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        m_MeshFilter.mesh = mesh;

        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Triangles.ToArray();
        mesh.uv = GetVerticesV2();
        mesh.RecalculateNormals();

        Generate2DColliders();

        //CreateWallMesh();
    }

    private void CreateWallMesh() {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 1f;

        foreach (List<int> outline in m_Outlines) {
            for (int i = 0; i < outline.Count - 1; i++) {
                int startIndex = wallVertices.Count;
                wallVertices.Add(m_Vertices[outline[i]] + Vector3.forward * wallHeight / 2);   // Left vertex
                wallVertices.Add(m_Vertices[outline[i+1]] + Vector3.forward * wallHeight / 2);   // right
                wallVertices.Add(m_Vertices[outline[i]] - Vector3.forward * wallHeight / 2);   // bottom left
                wallVertices.Add(m_Vertices[outline[i+1]] - Vector3.forward * wallHeight / 2);   // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        m_WallMeshFilter.mesh = wallMesh;
    }

    private void Generate2DColliders() {

        // Clear Colliders
        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();
        for (int i = 0; i < currentColliders.Length; i++) {
            Destroy(currentColliders[i]);
        }

        CalculateMeshOutlines();

        foreach (List<int> outline in m_Outlines) {
            EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] edgePoints = new Vector2[outline.Count];

            for (int i = 0; i < outline.Count; i++) {
                edgePoints[i] = (Vector2)m_Vertices[outline[i]];
            }
            edgeCollider.points = edgePoints;
        }
    }

    private void TriangulateSquare(Square square) {
        switch (square.Configuration) {
            case 0:
            break;

            // 1 points
            case 1:
                MeshFromPoints(square.CentreLeft, square.CentreBottom, square.BottomLeft);
                break;
            case 2:
                MeshFromPoints(square.BottomRight, square.CentreBottom, square.CentreRight);
                break;
            case 4:
                MeshFromPoints(square.TopRight, square.CentreRight, square.CentreTop);
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

        for (int i = 0; i < points.Length - 2; i++)
            CreateTriangle(points[0], points[i + 1], points[i + 2]);
    }

    private void AssignVertices(Node[] points) {
        for (int i = 0; i < points.Length; i++) {
            if (points[i].VetexIndex == -1) {
                points[i].VetexIndex = m_Vertices.Count;
                m_Vertices.Add(points[i].Position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c) {
        m_Triangles.Add(a.VetexIndex);
        m_Triangles.Add(b.VetexIndex);
        m_Triangles.Add(c.VetexIndex);

        Triangle triangle = new Triangle(a.VetexIndex, b.VetexIndex, c.VetexIndex);
        AddTriangleToDictionary(triangle.VertexIndexA, triangle);
        AddTriangleToDictionary(triangle.VertexIndexB, triangle);
        AddTriangleToDictionary(triangle.VertexIndexC, triangle);

    }

    private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
        if (m_TriangleDict.ContainsKey(vertexIndexKey)) {
            m_TriangleDict[vertexIndexKey].Add(triangle);
        } else {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            m_TriangleDict.Add(vertexIndexKey, triangleList);
        }
    }

    private Vector2[] GetVerticesV2() {
        List<Vector2> verticesV2 = new List<Vector2>();
        for (int i = 0; i < this.m_Vertices.Count; i++) {
            verticesV2.Add((Vector2)this.m_Vertices[i]);
        }
        return verticesV2.ToArray();
    }

    private void CalculateMeshOutlines() {
        for (int vertexIndex = 0; vertexIndex < m_Vertices.Count; vertexIndex++) {
            if (m_CheckedVertices.Contains(vertexIndex)) continue;

            int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
            if (newOutlineVertex != -1) {
                m_CheckedVertices.Add(newOutlineVertex);

                List<int> newOutline = new List<int>();
                newOutline.Add(vertexIndex);
                m_Outlines.Add(newOutline);
                FollowOutline(newOutlineVertex, m_Outlines.Count - 1);
                m_Outlines[m_Outlines.Count - 1].Add(vertexIndex);
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIdex) {
        m_Outlines[outlineIdex].Add(vertexIndex);
        m_CheckedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
        if (nextVertexIndex != -1) {
            FollowOutline(nextVertexIndex, outlineIdex);
        }
    }

    private int GetConnectedOutlineVertex(int vertexIndex) {
        List<Triangle> trianglesContainingVertex = m_TriangleDict[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++) {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++) {
                int vertexB = triangle[j];
                if (vertexB == vertexIndex || m_CheckedVertices.Contains(vertexB)) continue;
                if (IsOutlineEdge(vertexIndex, vertexB)) {
                    return vertexB;
                }
            }
        }

        return -1;
    }

    private bool IsOutlineEdge(int vertexA, int vertexB) {
        List<Triangle> trianglesContainingVertexA = m_TriangleDict[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
            if (trianglesContainingVertexA[i].Contains(vertexB)) {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    struct Triangle {
        public int VertexIndexA;
        public int VertexIndexB;
        public int VertexIndexC;
        int[] Vertices;

        public Triangle (int a, int b, int c) {
            VertexIndexA = a; VertexIndexB = b; VertexIndexC = c;

            Vertices = new int[3];
            Vertices[0] = a;
            Vertices[1] = b;
            Vertices[2] = c;
        }

        public int this[int i] {
            get {
                return Vertices[i];
            }
        }

        public bool Contains(int vertexIndex) {
            return vertexIndex == VertexIndexA || vertexIndex == VertexIndexB || vertexIndex == VertexIndexC;
        }
    }

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

    public class Node {
        public Vector3 Position;
        public int VetexIndex = -1;

        public Node(Vector3 _pos) {
            Position = _pos;
        }
    }

    public class ControlNode : Node {
        public bool Active;
        //public float Value;
        public Node Above, Right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            Active = _active;
            Above = new Node(Position + Vector3.up * squareSize / 2f);
            Right = new Node(Position + Vector3.right * squareSize / 2f);
        }
    }
}

