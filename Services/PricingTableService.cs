using System.Collections.Generic;
using System.Linq;

namespace RotoMonsterUI
{
    public class PricingTableService
    {
        public PricingTableResult Process(string tableId, Dictionary<string, string> params_)
        {
            var result = new PricingTableResult();
            var buyPrefix = "pricingbuy_" + tableId + "_";

            string eventTarget;
            params_.TryGetValue("__EVENTTARGET", out eventTarget);

            var key = params_.Keys.FirstOrDefault(k => k.StartsWith(buyPrefix));

            if (key == null && !string.IsNullOrEmpty(eventTarget)
                && eventTarget.StartsWith(buyPrefix))
                key = eventTarget;

            if (key == null) return result;

            result.PurchasedPlanId = key.Substring(buyPrefix.Length);
            result.AddOnSelected = params_.ContainsKey(
                "pricingaddon_" + tableId + "_" + result.PurchasedPlanId);

            return result;
        }
    }
}