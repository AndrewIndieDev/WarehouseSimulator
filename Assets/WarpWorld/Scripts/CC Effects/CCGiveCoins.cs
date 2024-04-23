using WarpWorld.CrowdControl;

namespace CrowdControlSampleGame {
    public class CCGiveCoins : CCEffectParameters {
        protected override EffectResult OnTriggerEffect(CCEffectInstanceParameters effectInstance) { 
            if (Player.Instance.GiveCoins(effectInstance.GetQuantity("Total Coins"))) 
                return EffectResult.Success;

            return EffectResult.Retry;
        }

        public override bool CanBeRan() {
            return !Player.Instance.Dead;
        }
    }
}
