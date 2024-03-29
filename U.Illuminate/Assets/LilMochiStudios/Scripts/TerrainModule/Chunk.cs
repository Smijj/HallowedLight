using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class Chunk : MonoBehaviour {


        [Header("Elements")]
        [SerializeField] protected MeshFilter m_MeshFilter;
        [SerializeField] protected MeshRenderer m_MeshRenderer;
        protected Mesh m_Mesh;

        [Header("Debug")]
        [SerializeField] bool m_InitializeOnStart = false;
        [SerializeField] protected float m_UVScale;
        [SerializeField] protected int m_GridSize;
        [SerializeField] protected float m_GridScale;
        [SerializeField] protected float m_IsoValue;
        [SerializeField] protected MaterialDataSO m_MaterialData;

        protected SquareGrid m_SquareGrid;


        private void Reset() {
            if (!m_MeshFilter) m_MeshFilter = GetComponentInChildren<MeshFilter>();
            if (!m_MeshRenderer) m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        private void OnEnable() {
            //InputManager.OnTouching += OnMiningLaserContact;
            PlayerModule.States.MiningState.OnMiningLaserContact += OnMiningLaserContact;
        }
        private void OnDisable() {
            //InputManager.OnTouching -= OnMiningLaserContact;
            PlayerModule.States.MiningState.OnMiningLaserContact -= OnMiningLaserContact;
        }
        private void Start() {
            if (m_InitializeOnStart) Initialize(m_GridSize, m_GridScale, m_IsoValue, m_UVScale, m_MaterialData);
        }

        #region Events

        private void OnMiningLaserContact(Vector3 worldPosition) {
            worldPosition.z = 0f;

            // For spliting the terrain meshes into chunks, it converts the world space pos to a local pos to effect the correct mesh.
            worldPosition = transform.InverseTransformPoint(worldPosition);
            Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

            DestroyTerrain(gridPosition);
        }

        #endregion


        #region Public

        public void Initialize(int gridSize, float gridScale, float isoValue, float uvScale, MaterialDataSO materialData) {
            this.m_GridSize = gridSize;
            this.m_GridScale = gridScale;
            this.m_IsoValue = isoValue;
            this.m_UVScale = uvScale;
            this.m_MaterialData = materialData;

            m_Mesh = new Mesh();
            if (m_MeshRenderer && materialData) m_MeshRenderer.material = materialData.Material;

            InitializeGrid(materialData);
        }

        public virtual void InitializeGrid(MaterialDataSO materialData) {
            m_SquareGrid = new SquareGrid(m_GridSize, m_GridScale, m_IsoValue);
            GenerateMesh();
        }

        public void AddTerrain(float[,] gridData) {
            for (int y = 0; y < m_SquareGrid.GridData.GetLength(1); y++) {
                for (int x = 0; x < m_SquareGrid.GridData.GetLength(0); x++) {
                    m_SquareGrid.GridData[x, y] += gridData[x, y];
                }
            }
            GenerateMesh();
        }
        public void RemoveTerrain(float[,] gridData) {
            for (int y = 0; y < m_SquareGrid.GridData.GetLength(1); y++) {
                for (int x = 0; x < m_SquareGrid.GridData.GetLength(0); x++) {
                    m_SquareGrid.GridData[x, y] -= gridData[x, y];
                }
            }
            GenerateMesh();
        }

        public float[,] GetGridData() {
            return this.m_SquareGrid.GridData;
        }

        #endregion


        #region Private

        private void DestroyTerrain(Vector2Int gridPosition) {
            bool shouldGenerate = false;

            for (int y = gridPosition.y - m_MaterialData.BrushRadius / 2; y <= gridPosition.y + m_MaterialData.BrushRadius / 2; y++) {
                for (int x = gridPosition.x - m_MaterialData.BrushRadius / 2; x <= gridPosition.x + m_MaterialData.BrushRadius / 2; x++) {
                    Vector2Int currentGridPos = new Vector2Int(x, y);

                    // Check if grids pos is valid, if not skip this grid pos
                    if (!IsValidGridPosition(currentGridPos)) continue;

                    // Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                    float distance = Vector2.Distance(currentGridPos, gridPosition);

                    float strength = m_MaterialData.BrushStrength / m_MaterialData.Hardness;
                    //if (m_SquareGrid.GridData[currentGridPos.x, currentGridPos.y] < m_IsoValue) strength = m_MaterialData.BrushStrength;
                    float factor = Mathf.Exp(-distance * m_MaterialData.BrushFalloff / m_MaterialData.BrushRadius) * strength;

                    // Modify the grid point
                    m_SquareGrid.GridData[currentGridPos.x, currentGridPos.y] -= factor;

                    // This is to make sure that not all of the terrain meshes in different chunks update even if you clicked nowhere near them.
                    shouldGenerate = true;
                }
            }

            if (shouldGenerate)
                GenerateMesh();
        }


        protected void GenerateMesh() {
            m_Mesh = new Mesh();

            m_SquareGrid.Update();

            m_Mesh.vertices = m_SquareGrid.GetVerticesV3();
            m_Mesh.triangles = m_SquareGrid.GetTriangles();

            // Offset UVs so that textures applied to the terrain are correctly displayed. The order these operations happen is is very important!
            Vector2[] uvs = m_SquareGrid.GetUVs();
            for (int i = 0; i < uvs.Length; i++) {
                uvs[i] /= m_GridSize - 1;                             // This scales the uv to be the same size as the chunks mesh. Making the texture be the same size as the chunk
                uvs[i] += Vector2.one * this.m_GridScale / 2;       // This offsets the uv to make the texture lineup with the mesh
                uvs[i] /= this.m_GridScale;                         // This scales the uv to account for the GridScale
                uvs[i] /= m_UVScale;                                // Value that allows for changing the tiling of the UV's from the inspector
            }
            m_Mesh.uv = uvs;

            m_MeshFilter.mesh = m_Mesh;

            GenerateCollider();
        }

        private void GenerateCollider() {
            if (m_MeshFilter.TryGetComponent(out MeshCollider meshCollider))
                meshCollider.sharedMesh = m_Mesh;
            else
                m_MeshFilter.AddComponent<MeshCollider>().sharedMesh = m_Mesh;


            /// MEMORY LEAK maybe
            //if (m_MeshFilter.TryGetComponent(out EdgeCollider2D polyCollider))
            //    polyCollider.points  =m_SquareGrid.GetEdgeVertices();
            //else
            //    m_MeshFilter.AddComponent<EdgeCollider2D>().points = m_SquareGrid.GetEdgeVertices();

            //foreach (var item in m_SquareGrid.GetEdgeVertices()) {
            //    Debug.Log(item);
            //}
        }

        private bool IsValidGridPosition(Vector2Int gridPosition) {
            return gridPosition.x >= 0f && gridPosition.x < m_GridSize && gridPosition.y >= 0f && gridPosition.y < m_GridSize && m_SquareGrid.GridData[gridPosition.x, gridPosition.y] > 0;
        }

        private Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition) {
            Vector2Int gridPos = new Vector2Int();
            gridPos.x = Mathf.FloorToInt(worldPosition.x / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
            gridPos.y = Mathf.FloorToInt(worldPosition.y / m_GridScale + m_GridSize / 2 - m_GridScale / 2);
            return gridPos;
        }

        #endregion
    }
}
