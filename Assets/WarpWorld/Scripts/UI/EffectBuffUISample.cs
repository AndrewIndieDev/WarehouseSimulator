using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using WarpWorld.CrowdControl;
using WarpWorld.CrowdControl.Overlay;
using System.Collections;

public class EffectBuffUISample : EffectBuffUI {
#pragma warning disable 0649
    [SerializeField] protected RectTransform mainContent;
    [SerializeField] private CanvasGroup canvasGroup;
    [Space]
    [SerializeField] private TMP_Text effectName;
    [SerializeField] private Image effectIcon;
    [Space]
    [SerializeField] private TMP_Text userName;
    [SerializeField] private Image userIcon;
    [Space]
    [SerializeField] private RectTransform effectIconElement;
    [SerializeField] private RectTransform userIconElement;
    [SerializeField] private EffectTextSizeFitter textSizeFitter;
    [Space]
    [SerializeField] private TMP_Text timeLabel;
    [SerializeField] private RectTransform textPanel;
    [SerializeField] private RectTransform timePanel;
    [SerializeField] private RectTransform timeBar;
    [SerializeField] private RectTransform timeFillBar;
#pragma warning restore 0649

    private float totalWidth = 0;
    private bool setPos = false;
    private CCEffectInstanceTimed effectInstanceTimed;
    private CCEffectTimed effect;

    protected void LateUpdate() {
        if (effectInstanceTimed == null || effect == null || effect.Paused)
            return;

        if (!setPos) {
            totalWidth = effectIconElement.rect.width + userIconElement.rect.width + textPanel.rect.width + timePanel.rect.width;
            mainContent.transform.localPosition = new Vector3(-5.0f - totalWidth, mainContent.transform.localPosition.y, mainContent.transform.localPosition.z);
            timeBar.transform.localPosition = new Vector3(-5.0f - totalWidth, mainContent.transform.localPosition.y, mainContent.transform.localPosition.z);
            timeBar.sizeDelta = new Vector2(totalWidth, 5);
            setPos = true;
        }
    }

    protected override void SetVisibility(DisplayFlags displayFlags) {
        effectName.gameObject.SetActive((displayFlags & DisplayFlags.EffectName) != 0);
        effectIconElement.gameObject.SetActive((displayFlags & DisplayFlags.EffectIcon) != 0);
        userName.gameObject.SetActive((displayFlags & DisplayFlags.UserName) != 0);
        userIconElement.gameObject.SetActive((displayFlags & DisplayFlags.UserIcon) != 0);
        textSizeFitter.gameObject.SetActive(effectName.gameObject.activeSelf || userName.gameObject.activeSelf);
        gameObject.SetActive((displayFlags & DisplayFlags.Buff) != 0);
    }

    private IEnumerator AssignSprite(CCEffectInstance effectInstance) {
        if (effectInstance.user.profileIcon != null) {
            userIcon.sprite = effectInstance.user.profileIcon;
            yield break;
        }

        canvasGroup.alpha = 0;

        while (effectInstance.user.profileIcon == null) {
            yield return new WaitForSeconds(0.1f);
        }

        userIcon.sprite = effectInstance.user.profileIcon;
        canvasGroup.alpha = 1;
    }

    protected override void Setup(CCEffectInstance effectInstance) {
        effectIcon.sprite = effectInstance.effect.Icon;
        effectIcon.color = effectInstance.effect.IconColor;

        if (effectName.gameObject.activeSelf)
            effectName.text = effectInstance.effect.Name;

        if (effectInstance.user == null) {
            effectInstance.SetUser(CrowdControl.anonymousUser);
        }
        else {
            StartCoroutine(AssignSprite(effectInstance));
            userName.text = effectInstance.user.name;
        }

        if (textSizeFitter.gameObject.activeSelf)
            textSizeFitter.UpdateLayout();

        effectInstanceTimed = effectInstance as CCEffectInstanceTimed;
        effect = effectInstanceTimed.effect;
    }

    protected override void UpdateEffectTimer() {
        int seconds = Convert.ToInt32(effectInstanceTimed.unscaledTimeLeft) % 60;
        int minutes = Convert.ToInt32(effectInstanceTimed.unscaledTimeLeft) / 60;

        timeLabel.text = string.Format("{0}:{1}", minutes, seconds.ToString("D2"));

        float percentLeft = 1.0f - (effect.Duration - effectInstanceTimed.unscaledTimeLeft) / effect.Duration;

        timeFillBar.sizeDelta = new Vector2(totalWidth * percentLeft, 5.0f);
    }

    protected override bool Remove() {
        setPos = false;
        return true;
    }
}
