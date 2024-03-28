using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    [CreateAssetMenu(fileName = "New MaterialData", menuName = "ScriptableObjects/Create MaterialData")]
    public class MaterialDataSO : ScriptableObject
    {
        public Material Material;
        public float Hardness = 1;
        [Range(0.1f, 5f), Tooltip("A Smaller number means the material will take up more of the chunk.")]
        public float SizeRange = 1.5f;
    }
}
