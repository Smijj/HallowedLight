using UnityEngine;

namespace LilMochiStudios.TerrainModule.States {

    public static class TerrainState {

        // Request
        public static System.Func<Vector3, float> OnGetGridIsoValueFromWorldPos;
        public static System.Func<float> OnGetIsoValue;
    }
}
