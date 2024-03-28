using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class SquareTester : MonoBehaviour {
        //private Vector2 m_TopRight;
        //private Vector2 m_BottomRight;
        //private Vector2 m_BottomLeft;
        //private Vector2 m_TopLeft;

        //private Vector2 m_RightCentre;
        //private Vector2 m_BottomCentre;
        //private Vector2 m_LeftCentre;
        //private Vector2 m_TopCentre;

        [Header("Elements")]
        [SerializeField] private MeshFilter m_MeshFilter;

        [Header("Settings")]
        [SerializeField] private float m_Gridscale = 1f;
        [SerializeField] private float m_IsoValue;

        [SerializeField] private bool m_EnableGizmos = false;
        [SerializeField] private float m_GizmosSize = 0.5f;

        private List<Vector3> m_Vertices = new List<Vector3>();
        private List<int> m_Triangles = new List<int>();

        [Header("Configuration")]
        [SerializeField] private float m_TopRightValue;
        [SerializeField] private float m_BottomRightValue;
        [SerializeField] private float m_BottomLeftValue;
        [SerializeField] private float m_TopLeftValue;

        private void Update() {
            GenerateMesh();
        }

        private void GenerateMesh() {
            Mesh mesh = new Mesh();

            m_Vertices.Clear();
            m_Triangles.Clear();

            Square square = new Square(Vector3.zero, m_Gridscale);
            square.Triangulate(m_IsoValue, new float[] { m_TopRightValue, m_BottomRightValue, m_BottomLeftValue, m_TopLeftValue });

            mesh.vertices = square.GetVertices();
            mesh.triangles = square.GetTriangles();

            m_MeshFilter.mesh = mesh;
        }


        //private void OnDrawGizmos() {
        //    if (!m_EnableGizmos) return;
        //    Gizmos.color = Color.red;

        //    Gizmos.DrawSphere(m_TopRight, m_GizmosSize);
        //    Gizmos.DrawSphere(m_BottomRight, m_GizmosSize);
        //    Gizmos.DrawSphere(m_BottomLeft, m_GizmosSize);
        //    Gizmos.DrawSphere(m_TopLeft, m_GizmosSize);

        //    Gizmos.DrawSphere(m_RightCentre, m_GizmosSize / 2f);
        //    Gizmos.DrawSphere(m_BottomCentre, m_GizmosSize / 2f);
        //    Gizmos.DrawSphere(m_LeftCentre, m_GizmosSize / 2f);
        //    Gizmos.DrawSphere(m_TopCentre, m_GizmosSize / 2f);
        //}
    }
}
