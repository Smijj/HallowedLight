using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Square
{
    private Vector2 m_Position;

    private Vector2 m_TopRight;
    private Vector2 m_BottomRight;
    private Vector2 m_BottomLeft;
    private Vector2 m_TopLeft;

    private Vector2 m_TopCentre;
    private Vector2 m_RightCentre;
    private Vector2 m_BottomCentre;
    private Vector2 m_LeftCentre;

    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;
    private List<Vector2> m_UVs;

    public Square(Vector2 position, float gridScale) {
        this.m_Position = position;

        m_TopRight = m_Position + gridScale * Vector2.one / 2;   // the /2 centres the whole square as all the proceeding verts are based in this ones position
        m_BottomRight = m_TopRight + Vector2.down * gridScale;
        m_BottomLeft = m_BottomRight + Vector2.left * gridScale;
        m_TopLeft = m_BottomLeft + Vector2.up * gridScale;
        
        m_TopCentre = m_TopLeft + (Vector2.right / 2) * gridScale;  // the /2 here moves the vert to the centre point between the corner verts 
        m_RightCentre = m_TopRight + (Vector2.down / 2) * gridScale;
        m_BottomCentre = m_BottomRight + (Vector2.left / 2) * gridScale;
        m_LeftCentre = m_BottomLeft + (Vector2.up / 2) * gridScale;

        
        this.m_Vertices = new List<Vector3>();
        this.m_Triangles = new List<int>();
        this.m_UVs = new List<Vector2>();
    }

    public void Triangulate(float isoValue, float[] values) {

        this.m_Vertices.Clear();
        this.m_Triangles.Clear();
        this.m_UVs.Clear();

        int configuration = GetConfiguration(isoValue, values);

        Interpolate(isoValue, values);
        Triangulate(configuration);
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

    private void Interpolate(float isoValue, float[] values) {

        float topLerp = Mathf.InverseLerp(values[3], values[0], isoValue);
        m_TopCentre = m_TopLeft + (m_TopRight - m_TopLeft) * topLerp;

        float rightLerp = Mathf.InverseLerp(values[0], values[1], isoValue);
        m_RightCentre = m_TopRight + (m_BottomRight - m_TopRight) * rightLerp;

        float bottomLerp = Mathf.InverseLerp(values[2], values[1], isoValue);
        m_BottomCentre = m_BottomLeft + (m_BottomRight - m_BottomLeft) * bottomLerp;

        float leftLerp = Mathf.InverseLerp(values[3], values[2], isoValue);
        m_LeftCentre = m_TopLeft + (m_BottomLeft - m_TopLeft) * leftLerp;
    }

    private void Triangulate(int configuration) {
        switch (configuration) {
            case 0:
                break;
            case 1:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_RightCentre, m_TopCentre });
                break;
            case 2:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                m_UVs.AddRange(new Vector2[] { m_RightCentre, m_BottomRight, m_BottomCentre });
                break;
            case 3:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_BottomRight, m_BottomCentre, m_TopCentre });
                break;
            case 4:
                m_Vertices.AddRange(new Vector3[] { m_BottomCentre, m_BottomLeft, m_LeftCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                m_UVs.AddRange(new Vector2[] { m_BottomCentre, m_BottomLeft, m_LeftCentre });
                break;
            case 5:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_LeftCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_LeftCentre, m_TopCentre });
                break;
            case 6:
                m_Vertices.AddRange(new Vector3[] { m_BottomRight, m_BottomLeft, m_LeftCentre, m_RightCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                m_UVs.AddRange(new Vector2[] { m_BottomRight, m_BottomLeft, m_LeftCentre, m_RightCentre });
                break;
            case 7:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomLeft, m_LeftCentre, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_BottomRight, m_BottomLeft, m_LeftCentre, m_TopCentre });
                break;
            case 8:
                m_Vertices.AddRange(new Vector3[] { m_LeftCentre, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2 });
                m_UVs.AddRange(new Vector2[] { m_LeftCentre, m_TopLeft, m_TopCentre });
                break;
            case 9:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_LeftCentre, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_RightCentre, m_LeftCentre, m_TopLeft });
                break;
            case 10:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 });
                m_UVs.AddRange(new Vector2[] { m_RightCentre, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft, m_TopCentre });
                break;
            case 11:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_BottomRight, m_BottomCentre, m_LeftCentre, m_TopLeft });
                break;
            case 12:
                m_Vertices.AddRange(new Vector3[] { m_BottomCentre, m_BottomLeft, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                m_UVs.AddRange(new Vector2[] { m_BottomCentre, m_BottomLeft, m_TopLeft, m_TopCentre });
                break;
            case 13:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_RightCentre, m_BottomCentre, m_BottomLeft, m_TopLeft });
                break;
            case 14:
                m_Vertices.AddRange(new Vector3[] { m_RightCentre, m_BottomRight, m_BottomLeft, m_TopLeft, m_TopCentre });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4 });
                m_UVs.AddRange(new Vector2[] { m_RightCentre, m_BottomRight, m_BottomLeft, m_TopLeft, m_TopCentre });
                break;
            case 15:
                m_Vertices.AddRange(new Vector3[] { m_TopRight, m_BottomRight, m_BottomLeft, m_TopLeft });
                m_Triangles.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                m_UVs.AddRange(new Vector2[] { m_TopRight, m_BottomRight, m_BottomLeft, m_TopLeft });
                break;
        }
    }

    private int GetConfiguration(float isoValue, float[] values) {
        int configuration = 0;
        if (values[0] > isoValue) {
            configuration += 1;

            // (1 << 0) = 0001
            // 0001 | 0001 = 0001 = 1
            // 0101 | 1010 = 1111 = 15
            //config = config | (1 << 0);
        }
        if (values[1] > isoValue) {
            configuration += 2;
        }
        if (values[2] > isoValue) {
            configuration += 4;
        }
        if (values[3] > isoValue) {
            configuration += 8;
        }

        return configuration;
    }
}
