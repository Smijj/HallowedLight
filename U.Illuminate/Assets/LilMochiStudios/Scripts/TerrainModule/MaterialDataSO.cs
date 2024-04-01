using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    [CreateAssetMenu(fileName = "New MaterialData", menuName = "ScriptableObjects/Create MaterialData")]
    public class MaterialDataSO : ScriptableObject
    {
        [Header("Material Properties")]
        public Material Material;
        [Tooltip("The higher the strength, the harder this material will be to mine.")]
        public float Hardness = 1;

        [Header("Spawn Settings")]
        [Range(0.1f, 5f), Tooltip("A Smaller number means the material will take up more of the chunk.")]
        public float SizeRange = 1.5f;
        [Tooltip("The shape of the ore. 0 = Solid, 1 = Circle, 2 = Random Shape")]
        public int SpawnShapeType = 0;

        [Header("Mining Settings")]
        [Tooltip("The size of the brush.")]
        public int BrushRadius = 5;
        [Tooltip("The strength of the brush.")]
        public float BrushStrength = 0.8f;
        [Tooltip("The higher this number, the less the brush effects the edges of the brush radius.")]
        public float BrushFalloff = 6f;

        [Header("Drops Settings")]
        public bool CanDropItem = false;
        [Tooltip("An Ore will drop for every 0.1 (10%) of the total ore chunk mined.")]
        public float DropRate = 0.1f;
        public List<Sprite> DropSprites = new List<Sprite>();

        public Sprite GetDropSprite() {
            if (DropSprites.Count == 0) return null;
            return DropSprites[Random.Range(0, DropSprites.Count)];
        }
    }
}
