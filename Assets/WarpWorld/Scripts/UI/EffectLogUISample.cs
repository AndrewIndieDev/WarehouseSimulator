using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WarpWorld.CrowdControl;
using WarpWorld.CrowdControl.Overlay;
using System.Collections;

public class EffectLogUISample : EffectUINode {
#pragma warning disable 0649
    [SerializeField] private RectTransform mainContent;
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
    [SerializeField] private GameObject specialStatusContainer;
    [SerializeField] private TMP_Text specialStatus;
    [SerializeField] private TMP_Text coinsSpent;
#pragma warning restore 0649
    protected override void SetVisibility(DisplayFlags displayFlags) {
        effectName.gameObject.SetActive((displayFlags & DisplayFlags.EffectName) != 0);
        effectIconElement.gameObject.SetActive((displayFlags & DisplayFlags.EffectIcon) != 0);
        userName.gameObject.SetActive((displayFlags & DisplayFlags.UserName) != 0);
        userIconElement.gameObject.SetActive((displayFlags & DisplayFlags.UserIcon) != 0);
        textSizeFitter.gameObject.SetActive(effectName.gameObject.activeSelf || userName.gameObject.activeSelf);
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

        if (effectInstance.user != null && effectInstance.user.roles != null && effectInstance.user.roles.Length > 0) {
            specialStatusContainer.SetActive(true);
            specialStatus.text = effectInstance.user.roles[0].ToUpper();
        }
        else {
            specialStatusContainer.SetActive(false);
        }

        coinsSpent.text = effectInstance.effect.Price.ToString();
    }
}
