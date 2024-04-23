using WarpWorld.CrowdControl;
using UnityEngine;

namespace CrowdControlSampleGame
{
    public class CCInvertControls : CCEffectTimed {
        protected override EffectResult OnStartEffect(CCEffectInstanceTimed effectInstance) {
            if (Player.Instance?.SetInvertControlState(true) == true)
                return EffectResult.Success;

            return EffectResult.Retry;
        }

        protected override bool OnStopEffect(CCEffectInstanceTimed effectInstance, bool force) {
            return Player.Instance?.SetInvertControlState(false) == true;
        }

        protected override void OnPauseEffect() {
            Debug.LogFormat("[CC DEMO EFFECT]: Pause Timer: {0}", Name);
        }

        protected override void OnResumeEffect() {
            Debug.LogFormat("[CC DEMO EFFECT]: Resume Timer: {0}", Name);
            Player.Instance.SetInvertControlState(true);
        }

        protected override void OnResetEffect() {
            Debug.LogFormat("[CC DEMO EFFECT]: Reset Timer: {0}", Name);
        }

        protected override bool RunningCondition() {
            return !Player.Instance.Dead;
        }
    }
}