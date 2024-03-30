using LilMochiStudios.TerrainModule;
using UnityEngine;

namespace LilMochiStudios.PlayerModule.States {

    public static class DestructableState {
        public static System.Action<Vector3> OnDestructableContact;
        public static System.Action<MaterialDataSO> OnDestructableDropItem;
    }
}
