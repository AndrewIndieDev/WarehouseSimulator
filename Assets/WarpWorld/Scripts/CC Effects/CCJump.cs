using WarpWorld.CrowdControl;

namespace CrowdControlSampleGame
{
    public class CCJump : CCEffect
    {
        protected override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (Player.Instance.CCJump())
                return EffectResult.Success;

            return EffectResult.Retry;
        }

        public override bool CanBeRan()
        {
            return !Player.Instance.Dead;
        }
    }
}