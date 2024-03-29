using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    public class TerrainManager : MonoBehaviour {
        
        [Header("World Settings")]
        [SerializeField] private int m_WorldSizeX = 50;
        [SerializeField] private List<TerrainLayerData> m_TerrainLayerData;
        
        [Header("Chunk Settings")]
        [SerializeField] private float m_IsoValue = 1f;
        [SerializeField] private float m_UVScale = 0.1f;
        [SerializeField] private int m_ChunkGridSize = 33;
        [SerializeField] private float m_ChunkGridScale = 0.25f;


        [Header("Elements")]
        [SerializeField] private Chunk m_TerrainChunkPrefab;
        [SerializeField] private Chunk m_OreChunkPrefab;
        [SerializeField] private Transform m_TerrainParent;
        [SerializeField] private Transform m_OreParent;

        private void Start() {
            Generate();
            //GenerateOreChunk();
        }

        private void Generate() {
            float chunkSize = m_ChunkGridScale * (m_ChunkGridSize - 1);

            int worldGridPositionY = 0;

            for (int w = 0; w < m_TerrainLayerData.Count; w++) {        // For each terain layer
                for (int y = 0; y < m_TerrainLayerData[w].Depth; y++) { // For each row in this layer
                    for (int x = 0; x < m_WorldSizeX; x++) {            // For each column/item in this row
                        Vector2 spawnPosition = Vector2.zero;

                        // Set position based on grid
                        spawnPosition.x = x * chunkSize;
                        spawnPosition.y = y * chunkSize;

                        // Centre terrain on orgin of the world
                        spawnPosition.x -= (((float)m_WorldSizeX / 2) * chunkSize) - chunkSize / 2;
                        spawnPosition.y -= (worldGridPositionY * chunkSize) + ((m_TerrainLayerData[w].Depth * chunkSize) - chunkSize / 2);

                        // Instantiate and Initialize the TerrainChunk
                        Chunk terrainChunk = Instantiate(m_TerrainChunkPrefab, spawnPosition, Quaternion.identity, m_TerrainParent);
                        terrainChunk.Initialize(m_ChunkGridSize, m_ChunkGridScale, m_IsoValue, m_UVScale, m_TerrainLayerData[w].MaterialData);

                        GenerateOre(m_TerrainLayerData[w], spawnPosition, terrainChunk);
                    }
                }
                worldGridPositionY += m_TerrainLayerData[w].Depth;
            }
        }

        private void GenerateOre(TerrainLayerData layerData, Vector2 spawnPosition, Chunk terrainChunk) {
            if (layerData.OreSpawnTable.Length.Equals(0)) return;

            foreach (var ore in layerData.OreSpawnTable) {  // Could upgrade this for loop to loop through the ore spawn table randomly so it doesnt favour the first couple, but its fine for now
                if (Random.Range(0f, 1f) < ore.SpawnChance) {
                    // Instantiate and Initialize the OreChunk
                    Chunk OreChunk = Instantiate(m_OreChunkPrefab, spawnPosition, Quaternion.identity, m_OreParent);
                    OreChunk.Initialize(m_ChunkGridSize, m_ChunkGridScale, m_IsoValue, m_UVScale, ore.MaterialData);

                    // Remove terrain from behind OreChunk
                    terrainChunk.RemoveTerrain(OreChunk.GetGridData());
                    
                    // Stop it from generating a different kind of ore in this chunk on top of the one thats now been generated
                    break;
                }
            }
            
        }

        [System.Serializable]
        public class TerrainLayerData {
            public int Depth = 30;
            public MaterialDataSO MaterialData;
            public OreSpawnData[] OreSpawnTable;
        }

        [System.Serializable]
        public class OreSpawnData {
            public float SpawnChance = 0.1f;
            public MaterialDataSO MaterialData;
            
        }
    }
}
