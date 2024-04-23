using WarpWorld.CrowdControl;

namespace CrowdControlSampleGame
{
    public class CCRecover : CCEffect
    {
        protected override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (Player.Instance.Recover())
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