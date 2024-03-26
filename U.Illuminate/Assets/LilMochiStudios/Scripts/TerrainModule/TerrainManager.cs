using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int m_GridSize;
    [SerializeField] private float m_IsoValue = 1f;
    [SerializeField] private int m_ChunkGridSize = 40;
    [SerializeField] private float m_ChunkGridScale = 0.1f;


    [Header("Elements")]
    [SerializeField] private TerrainChunk m_TerrainChunkPrefab;

    private void Start() {
        Generate();
    }

    private void Generate() {
        float chunkSize = m_ChunkGridScale * (m_ChunkGridSize - 1);

        for (int y = 0; y < m_GridSize.y; y++) {
            for (int x = 0; x < m_GridSize.x; x++) {
                Vector2 spawnPosition = Vector2.zero;

                spawnPosition.x = x * chunkSize;
                spawnPosition.y = y * chunkSize;

                // Centre terrain on orgin of the world
                spawnPosition.x -= (((float)m_GridSize.x / 2) * chunkSize) - chunkSize / 2;
                spawnPosition.y -= (((float)m_GridSize.y) * chunkSize) - chunkSize / 2;

                TerrainChunk terrainGenerator = Instantiate(m_TerrainChunkPrefab, spawnPosition, Quaternion.identity, transform);

                terrainGenerator.Initialize(m_ChunkGridSize, m_ChunkGridScale, m_IsoValue);
            }
        }
    }
}
