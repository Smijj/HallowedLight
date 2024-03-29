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

        [Header("Mining Settings")]
        [Tooltip("The size of the brush.")]
        public int BrushRadius = 5;
        [Tooltip("The strength of the brush.")]
        public float BrushStrength = 0.8f;
        [Tooltip("The higher this number, the less the brush effects the edges of the brush radius.")]
        public float BrushFalloff = 6f;
    }
}
