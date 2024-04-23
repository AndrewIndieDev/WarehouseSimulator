using WarpWorld.CrowdControl;
using System;

namespace CrowdControlSampleGame {
    public class CCTakeCoins : CCEffectParameters {
        protected override EffectResult OnTriggerEffect(CCEffectInstanceParameters effectInstance) {
            if (Player.Instance.TakeCoins(effectInstance.GetQuantity("Total Coins"))) 
                return EffectResult.Success;

            return EffectResult.Retry;
        }

        public override bool CanBeRan() {
            return !Player.Instance.Dead;
        }
    }
}
