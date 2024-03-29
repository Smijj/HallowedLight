using UnityEngine;

namespace LarrikinInteractive.Core {
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute() {
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Bootstrapper/Systems")));
            Debug.Log("[Bootstrapper] Initialized Essential Systems.");
        }
    }
}
