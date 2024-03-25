using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2Int m_GridSize;
    [SerializeField] private int m_ChunkGridSize;
    [SerializeField] private float m_ChunkGridScale;

    [Header("Elements")]
    [SerializeField] private TerrainGenerator m_TerrainGeneratorPrefab;

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
                spawnPosition.y -= (((float)m_GridSize.y / 2) * chunkSize) - chunkSize / 2;

                TerrainGenerator terrainGenerator = Instantiate(m_TerrainGeneratorPrefab, spawnPosition, Quaternion.identity, transform);

                terrainGenerator.Initialize(m_ChunkGridSize, m_ChunkGridScale);
            }
        }
    }
}
