using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WarpWorld.CrowdControl;
using WarpWorld.CrowdControl.Overlay;

public class EffectQueueUISample : EffectQueueUI {
#pragma warning disable 0649
    [SerializeField] private Image effectIcon;
    [SerializeField] private TMP_Text count;
#pragma warning restore 0649

    protected override void Setup(CCEffectInstance effectInstance) {
        this.effectInstance = effectInstance;
        effectIcon.sprite = effectInstance.effect.Icon;
        effectIcon.color = effectInstance.effect.IconColor;
        total = 1;
        SetTotal();
    }

    protected override void SetVisibility(DisplayFlags displayFlags) {
        gameObject.SetActive((displayFlags & DisplayFlags.Queue) != 0);
    }

    protected override void Add(CCEffectInstance effectInstance) {
        total++;
        SetTotal();
    }

    protected void SetTotal() {
        count.text = total.ToString();
    }

    protected override bool Remove() {
        total--;
        if (total == 0) return true;
        SetTotal();
        return false;
    }
}
