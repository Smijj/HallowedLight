using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int m_GridSize;
    [SerializeField] private int m_WorldSizeX = 50;
    [SerializeField] private List<TerrainLayerData> m_TerrainLayerData;
    [SerializeField] private float m_IsoValue = 1f;
    [SerializeField] private int m_ChunkGridSize = 40;
    [SerializeField] private float m_ChunkGridScale = 0.1f;


    [Header("Elements")]
    [SerializeField] private TerrainChunk m_TerrainChunkPrefab;
    [SerializeField] private TerrainChunk m_OreChunkPrefab;
    [SerializeField] private Transform m_TerrainParent;
    [SerializeField] private Transform m_OreParent;

    private void Start() {
        Generate();
        //GenerateOreChunk();
    }

    private void Generate() {
        float chunkSize = m_ChunkGridScale * (m_ChunkGridSize - 1);

        int worldGridPositionY = 0;

        for (int w = 0; w < m_TerrainLayerData.Count; w++) {
            for (int y = 0; y < m_TerrainLayerData[w].Depth; y++) {
                for (int x = 0; x < m_WorldSizeX; x++) {
                    Vector2 spawnPosition = Vector2.zero;

                    // Set position based on grid
                    spawnPosition.x = x * chunkSize;
                    spawnPosition.y = y * chunkSize;

                    // Centre terrain on orgin of the world
                    spawnPosition.x -= (((float)m_WorldSizeX / 2) * chunkSize) - chunkSize / 2;
                    spawnPosition.y -= (worldGridPositionY * chunkSize) + ((m_TerrainLayerData[w].Depth * chunkSize) - chunkSize / 2);

                    // Instantiate and Initialize the TerrainChunk
                    TerrainChunk terrainChunk = Instantiate(m_TerrainChunkPrefab, spawnPosition, Quaternion.identity, m_TerrainParent);
                    terrainChunk.Initialize(m_ChunkGridSize, m_ChunkGridScale, m_IsoValue, m_TerrainLayerData[w].Material);


                    if (Random.Range(0f, 1f) < m_TerrainLayerData[w].OreSpawnChance) {
                        //GenerateOreChunk();
                        // Instantiate and Initialize the TerrainChunk
                        TerrainChunk OreChunk = Instantiate(m_OreChunkPrefab, spawnPosition, Quaternion.identity, m_OreParent);
                        OreChunk.Initialize(40, 0.05f, m_IsoValue, m_TerrainLayerData[w].OreMaterial);
                    }
                }
            }
            worldGridPositionY += m_TerrainLayerData[w].Depth;
        }
    }

    private void GenerateOreChunk() {
        TerrainChunk OreChunk = Instantiate(m_OreChunkPrefab, Vector3.zero, Quaternion.identity, transform);
        OreChunk.Initialize(40, 0.05f, m_IsoValue, null);
    }

    [System.Serializable]
    public class TerrainLayerData {
        public int Depth = 30;
        public Material Material;
        public float DiggingDifficulty = 1;
        public float OreSpawnChance = 0.1f;
        public Material OreMaterial;
    }
}
