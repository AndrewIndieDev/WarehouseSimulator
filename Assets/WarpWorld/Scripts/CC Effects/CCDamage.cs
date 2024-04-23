using WarpWorld.CrowdControl;

namespace CrowdControlSampleGame
{
    public class CCDamage : CCEffect
    {
        protected override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (Player.Instance.Damage())
            {
                return EffectResult.Success;
            }

            return EffectResult.Retry;
        }

        public override bool CanBeRan()
        {
            return !Player.Instance.Dead;
        }
    }
}