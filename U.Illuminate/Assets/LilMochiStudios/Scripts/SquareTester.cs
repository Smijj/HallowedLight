using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTester : MonoBehaviour
{
    private Vector2 m_TopRight;
    private Vector2 m_BottomRight;
    private Vector2 m_BottomLeft;
    private Vector2 m_TopLeft;

    private Vector2 m_RightCentre;
    private Vector2 m_BottomCentre;
    private Vector2 m_LeftCentre;
    private Vector2 m_TopCentre;

    [Header("Elements")]
    [SerializeField] private MeshFilter m_MeshFilter;


    [Header("Settings")]
    [SerializeField] private float m_Gridscale = 1f;
    [SerializeField] private bool m_EnableGizmos = false;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();

    [Header("Configuration")]
    [SerializeField] private bool m_TopRightState;
    [SerializeField] private bool m_BottomRightState;
    [SerializeField] private bool m_BottomLeftState;
    [SerializeField] private bool m_TopLeftState;

    private void Start() {
        m_TopRight = m_Gridscale * Vector2.one / 2; 
        m_RightCentre = m_TopRight + (Vector2.down/2) * m_Gridscale;
        m_BottomRight = m_TopRight + Vector2.down * m_Gridscale; 
        m_BottomCentre = m_BottomRight + (Vector2.left / 2) * m_Gridscale;
        m_BottomLeft = m_BottomRight + Vector2.left * m_Gridscale;
        m_LeftCentre = m_BottomLeft + (Vector2.up / 2) * m_Gridscale;
        m_TopLeft = m_BottomLeft + Vector2.up * m_Gridscale;
        m_TopCentre = m_TopLeft + (Vector2.right / 2) * m_Gridscale;

        //Vector3[] vertices = new Vector3[8];
        //int[] triangles = new int[6];

        //vertices[0] = m_TopRight;
        //vertices[1] = m_RightCentre;
        //vertices[2] = m_BottomRight;
        //vertices[3] = m_BottomCentre;
        //vertices[4] = m_BottomLeft;
        //vertices[5] = m_LeftCentre;
        //vertices[6] = m_TopLeft;
        //vertices[7] = m_TopCentre;


        //triangles[0] = 7;
        //triangles[1] = 1;
        //triangles[2] = 3;
        //triangles[3] = 7;
        //triangles[4] = 3;
        //triangles[5] = 5;

        Mesh mesh = new Mesh();

        m_Vertices.Clear();
        m_Triangles.Clear();

        Triangulate(GetConfiguration());

        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Triangles.ToArray();

        m_MeshFilter.mesh = mesh;
    }

    private void Update() {
        //Debug.Log("Config: " + GetConfiguration());
    }

    private int GetConfiguration() {
        int config = 0;
        if (m_TopRightState) {
            config += 1;

            // (1 << 0) = 0001
            // 0001 | 0001 = 0001 = 1
            // 0101 | 1010 = 1111 = 15
            //config = config | (1 << 0);
        }
        if (m_BottomRightState) {
            config += 2;
        }
        if (m_BottomLeftState) {
            config += 4;
        }
        if (m_TopLeftState) {
            config += 8;
        }

        return config;
    }

    private void Triangulate(int config) {
        switch (config) {
            case 0:


                break;
            case 1:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                break;
            case 2:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                break;
            case 3:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3});
                break;
            case 4:
                m_Vertices.AddRange(new Vector3[] { m_BottomCentre, m_BottomLeft, m_LeftCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                break;
            case 5:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_LeftCentre, m_TopCentre});
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 });
                break;
        }
    }

    private void OnDrawGizmos() {
        if (!m_EnableGizmos) return;
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(m_TopRight, m_Gridscale / 4f);
        Gizmos.DrawSphere(m_BottomRight, m_Gridscale / 4f);
        Gizmos.DrawSphere(m_BottomLeft, m_Gridscale / 4f);
        Gizmos.DrawSphere(m_TopLeft, m_Gridscale / 4f);

        Gizmos.DrawSphere(m_RightCentre, m_Gridscale / 8f);
        Gizmos.DrawSphere(m_BottomCentre, m_Gridscale / 8f);
        Gizmos.DrawSphere(m_LeftCentre, m_Gridscale / 8f);
        Gizmos.DrawSphere(m_TopCentre, m_Gridscale / 8f);
    }
}
