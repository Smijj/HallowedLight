using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LilMochiStudios.TerrainModule {
    [CreateAssetMenu(fileName = "New MaterialData", menuName = "ScriptableObjects/Create MaterialData")]
    public class MaterialDataSO : ScriptableObject
    {
        public float Hardness = 1;
        public Material Material;

    }
}
