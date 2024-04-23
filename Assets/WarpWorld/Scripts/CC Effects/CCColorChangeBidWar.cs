using UnityEngine;
using WarpWorld.CrowdControl;

namespace CrowdControlSampleGame
{
    public class CCColorChangeBidWar : CCEffectBidWar
    {
        protected override EffectResult OnTriggerEffect(CCEffectInstanceBidWar effectInstance)
        {
            if (Player.Instance.Dead)
            {
                return EffectResult.Retry;
            }

            Player.Instance?.ChangeColor(effectInstance.BidName);

            return EffectResult.Success;
        }

        EffectResult TriggerEffect(CCEffectInstanceBidWar effectInstance, EffectResult result)
        {
            if (!Player.Instance.Dead)
            {
                Player.Instance?.ChangeColor(effectInstance.BidName);
                Debug.LogFormat("[CC DEMO EFFECT]: New winner for {0}: {1}", Name, effectInstance.BidName);
            }

            return result;
        }
    }
}