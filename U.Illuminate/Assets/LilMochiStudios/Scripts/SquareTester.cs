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
    [SerializeField] private float m_IsoValue;
    [SerializeField] private float m_TopLerp;
    [SerializeField] private bool m_EnableGizmos = false;
    [SerializeField] private float m_GizmosSize = 0.5f;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<int> m_Triangles = new List<int>();

    [Header("Configuration")]
    [SerializeField] private float m_TopRightValue;
    [SerializeField] private float m_BottomRightValue;
    [SerializeField] private float m_BottomLeftValue;
    [SerializeField] private float m_TopLeftValue;

    private void Start() {
        m_TopRight = m_Gridscale * Vector2.one / 2; 
        m_RightCentre = m_TopRight + (Vector2.down/2) * m_Gridscale;
        m_BottomRight = m_TopRight + Vector2.down * m_Gridscale; 
        m_BottomCentre = m_BottomRight + (Vector2.left / 2) * m_Gridscale;
        m_BottomLeft = m_BottomRight + Vector2.left * m_Gridscale;
        m_LeftCentre = m_BottomLeft + (Vector2.up / 2) * m_Gridscale;
        m_TopLeft = m_BottomLeft + Vector2.up * m_Gridscale;
        m_TopCentre = m_TopLeft + (Vector2.right / 2) * m_Gridscale;

        GenerateMesh();
    }

    private void Update() {
        GenerateMesh();
    }

    private void GenerateMesh() {
        Mesh mesh = new Mesh();

        m_Vertices.Clear();
        m_Triangles.Clear();

        Interpolate();
        Triangulate(GetConfiguration());

        mesh.vertices = m_Vertices.ToArray();
        mesh.triangles = m_Triangles.ToArray();

        m_MeshFilter.mesh = mesh;
    }

    private int GetConfiguration() {
        int config = 0;
        if (m_TopRightValue > m_IsoValue) {
            config += 1;

            // (1 << 0) = 0001
            // 0001 | 0001 = 0001 = 1
            // 0101 | 1010 = 1111 = 15
            //config = config | (1 << 0);
        }
        if (m_BottomRightValue > m_IsoValue) {
            config += 2;
        }
        if (m_BottomLeftValue > m_IsoValue) {
            config += 4;
        }
        if (m_TopLeftValue > m_IsoValue) {
            config += 8;
        }

        return config;
    }

    private void Interpolate() {

        // Top Centre
        //float topLerp = (m_IsoValue - m_TopLeftValue) / (m_TopRightValue - m_TopLeftValue);
        //topLerp = Mathf.Clamp01(topLerp);
        //m_TopCentre = m_TopLeft + (m_TopRight - m_TopLeft) * topLerp;

        float topLerp = Mathf.InverseLerp(m_TopLeftValue, m_TopRightValue, m_IsoValue);
        m_TopCentre = m_TopLeft + (m_TopRight - m_TopLeft) * topLerp;
        
        float rightLerp = Mathf.InverseLerp(m_TopRightValue, m_BottomRightValue, m_IsoValue);
        m_RightCentre = m_TopRight + (m_BottomRight - m_TopRight) * rightLerp;

        float bottomLerp = Mathf.InverseLerp(m_BottomLeftValue, m_BottomRightValue, m_IsoValue);
        m_BottomCentre = m_BottomLeft + (m_BottomRight - m_BottomLeft) * bottomLerp;

        float leftLerp = Mathf.InverseLerp(m_TopLeftValue, m_BottomLeftValue, m_IsoValue);
        m_LeftCentre = m_TopLeft + (m_BottomLeft - m_TopLeft) * leftLerp;
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
            case 6:
                m_Vertices.AddRange(new Vector3[] { m_BottomRight, m_BottomLeft, m_LeftCentre, m_RightCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;
            case 7:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomLeft, m_LeftCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;
            case 8:
                m_Vertices.AddRange(new Vector3[] { m_LeftCentre, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                break;
            case 9:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_LeftCentre, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;
            case 10:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 });
                break;
            case 11:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;
            case 12:
                m_Vertices.AddRange(new Vector3[] { m_BottomCentre, m_BottomLeft, m_TopLeft, m_TopCentre});
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;
            case 13:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;
            case 14:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomLeft, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                break;
            case 15:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomLeft, m_TopLeft});
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                break;
        }
    }

    private void OnDrawGizmos() {
        if (!m_EnableGizmos) return;
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(m_TopRight, m_GizmosSize);
        Gizmos.DrawSphere(m_BottomRight, m_GizmosSize);
        Gizmos.DrawSphere(m_BottomLeft, m_GizmosSize);
        Gizmos.DrawSphere(m_TopLeft, m_GizmosSize);

        Gizmos.DrawSphere(m_RightCentre, m_GizmosSize / 2f);
        Gizmos.DrawSphere(m_BottomCentre, m_GizmosSize / 2f);
        Gizmos.DrawSphere(m_LeftCentre, m_GizmosSize / 2f);
        Gizmos.DrawSphere(m_TopCentre, m_GizmosSize / 2f);
    }
}
