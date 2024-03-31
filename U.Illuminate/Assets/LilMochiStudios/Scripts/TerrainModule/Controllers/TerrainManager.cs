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
        [SerializeField] private TerrainChunk m_TerrainChunkPrefab;
        [SerializeField] private OreChunk m_OreChunkPrefab;
        [SerializeField] private MaterialDataSO m_BedrockMaterial;
        [SerializeField] private Transform m_TerrainParent;
        [SerializeField] private Transform m_OreParent;
        
        [SerializeField] private Transform m_DropShip;
        [SerializeField] private Transform m_Player;
        [SerializeField] private Transform m_MiningBot;

        private void Start() {
            Generate();
            //GenerateOreChunk();
        }

        private void Generate() {
            float chunkSize = m_ChunkGridScale * (m_ChunkGridSize - 1);

            int worldGridPositionY = 0;

            int spawnChunkPositionX = Random.Range(1, m_WorldSizeX-1);

            for (int w = 0; w < m_TerrainLayerData.Count; w++) {        // For each terain layer
                for (int y = 0; y < m_TerrainLayerData[w].Depth; y++) { // For each row in this layer
                    for (int x = 0; x < m_WorldSizeX; x++) {            // For each column/item in this row
                        bool worldBorderChunk = false;
                        if (x == 0 || x == m_WorldSizeX - 1 || y == 0 || y == m_TerrainLayerData[w].Depth - 1) {
                            worldBorderChunk = true;
                        }

                        Vector2 spawnPosition = Vector2.zero;

                        // Set position based on grid
                        spawnPosition.x = x * chunkSize;
                        spawnPosition.y = -y * chunkSize;

                        // Centre terrain on orgin of the world
                        spawnPosition.x -= (((float)m_WorldSizeX / 2) * chunkSize) - chunkSize / 2;
                        spawnPosition.y += (worldGridPositionY * chunkSize) + (((m_TerrainLayerData[w].Depth / 2) * chunkSize) + chunkSize / 2);

                        // Instantiate and Initialize the TerrainChunk
                        TerrainChunk terrainChunk = Instantiate(m_TerrainChunkPrefab, spawnPosition, Quaternion.identity, m_TerrainParent);

                        MaterialDataSO terrainMaterial = m_TerrainLayerData[w].MaterialData;
                        if (worldBorderChunk) {
                            terrainMaterial = m_BedrockMaterial;
                        }
                        terrainChunk.Initialize(m_ChunkGridSize, m_ChunkGridScale, m_IsoValue, m_UVScale, terrainMaterial);

                        if (y == 1 && x == spawnChunkPositionX) {
                            GenerateSpawn(terrainChunk);
                        } else {
                            if (!worldBorderChunk) {
                                // Dont generate an ore in the same chunk as the player spawns or in the world border
                                GenerateOre(m_TerrainLayerData[w].OreSpawnTable, spawnPosition, terrainChunk, y);
                            }
                        }
                    }
                }
                worldGridPositionY += m_TerrainLayerData[w].Depth;
            }
        }

        private void GenerateSpawn(TerrainChunk terrainChunk) {
            // Clear out spawn area
            float[,] spawnArea = new float[m_ChunkGridSize, m_ChunkGridSize];

            int randomness = 3;
            Vector2Int centerPoint = new Vector2Int(m_ChunkGridSize / 2, m_ChunkGridSize / 2);
            for (int y = 0; y < m_ChunkGridSize; y++) {
                for (int x = 0; x < m_ChunkGridSize; x++) {

                    //Calculate how much we should edit a particular grid point based on the distance from where the player clicked
                    Vector2Int editPoint = new Vector2Int(Random.Range(x - randomness, x + randomness), Random.Range(y - randomness, y + randomness));
                    float distance = Vector2.Distance(centerPoint, editPoint);
                    if (y >= centerPoint.y) {
                        float factor = Mathf.Exp(-distance * 0.8f / m_ChunkGridSize) * (m_ChunkGridSize * m_ChunkGridScale) / 6;
                        spawnArea[x, y] = factor;
                    }
                }
            }
            terrainChunk.RemoveTerrain(spawnArea);

            // Add dropship
            if (m_DropShip) m_DropShip.position = terrainChunk.transform.position;

            // Add player
            if (m_Player) m_Player.position = m_DropShip.position + Vector3.up;
            if (m_MiningBot) m_MiningBot.position = m_DropShip.position;

        }

        private void GenerateOre(OreSpawnData[] oreSpawnTable, Vector2 spawnPosition, Chunk terrainChunk, int DepthY) {
            if (oreSpawnTable.Length.Equals(0)) return;

            // Could upgrade this for loop to loop through the ore spawn table randomly so it doesnt favour the first couple, but its fine for now
            foreach (var ore in oreSpawnTable) {  
                if (DepthY < ore.MinimumDepth) continue;    // Dont spawn the ore if the depth isnt deep enough yet

                if (Random.Range(0f, 1f) < ore.SpawnChance) {
                    // Instantiate and Initialize the OreChunk
                    OreChunk OreChunk = Instantiate(m_OreChunkPrefab, spawnPosition, Quaternion.identity, m_OreParent);
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
            public int MinimumDepth = 0;
            public MaterialDataSO MaterialData;
            
        }
    }
}
