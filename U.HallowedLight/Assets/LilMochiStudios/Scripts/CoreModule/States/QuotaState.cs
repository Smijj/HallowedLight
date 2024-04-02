using LilMochiStudios.TerrainModule;
using static LilMochiStudios.CoreModule.QuotaManager;
using System.Collections.Generic;

namespace LilMochiStudios.CoreModule.States {

    public static class QuotaState {
        // Commands
        public static System.Action<MaterialDataSO> OnAddToQuota;
        public static System.Action<MaterialDataSO> OnRemoveFromQuota;

        // Queries
        public static System.Func<MaterialDataSO, QuotaItem> OnGetQuotaItem;

        // Events
        public static System.Action<List<QuotaItem>> OnQuotaChanged;
        public static System.Action OnQuotaReached;
    }
}
