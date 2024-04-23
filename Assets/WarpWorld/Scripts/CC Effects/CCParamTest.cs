using WarpWorld.CrowdControl;
 
namespace CrowdControlSampleGame {
    public class CCParamTest : CCEffectParameters {
        protected override EffectResult OnTriggerEffect(CCEffectInstanceParameters effectInstance) {
            return EffectResult.Success;
        }

        public override bool CanBeRan()
        {
            return !Player.Instance.Dead;
        }
    }
}
