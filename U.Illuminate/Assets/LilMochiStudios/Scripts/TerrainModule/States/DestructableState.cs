using UnityEngine;

namespace LilMochiStudios.TerrainModule.States {

    public static class DestructableState {
        public static System.Action<Vector3> OnDestructableContact;
        public static System.Action<MaterialDataSO, Vector2> OnDestructableDropItem;
    }
}
