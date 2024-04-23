using UnityEngine;
using UnityEngine.EventSystems;
using WarpWorld.CrowdControl;

using System;

[RequireComponent(typeof(StandaloneInputModule))]
public class CrowdControlInputSetup : MonoBehaviour
{
    private StandaloneInputModule _standaloneInputModule;

    // Use this for initialization
    void Start()
    {
        _standaloneInputModule = gameObject.GetComponent<StandaloneInputModule>();

        bool isOK = true;
        isOK = CheckAxis(_standaloneInputModule.horizontalAxis, "Horizontal") && isOK;
        isOK = CheckAxis(_standaloneInputModule.verticalAxis, "Vertical") && isOK;
        isOK = CheckButton(_standaloneInputModule.submitButton, "Submit") && isOK;
        isOK = CheckButton(_standaloneInputModule.cancelButton, "Cancel") && isOK;

        _standaloneInputModule.enabled = isOK;
    }

#pragma warning disable 168, 219
    private bool CheckAxis(string axisName, string axisDirection)
    {
        try
        {
            float horizontalAxis = Input.GetAxis(axisName);
        }
        catch (Exception e)
        {
            CrowdControl.LogError("The Standalone Input Module's " + axisDirection + " inside of the Crowd Control prefab is assigned a key that doesn't exist in the Input Manager. Please re-assign this with a proper " + axisDirection + " axis and restart.");
            return false;
        }

        return true;
    }

    private bool CheckButton(string buttonKey, string buttonName)
    {
        try
        {
            bool isButton = Input.GetButton(buttonKey);

        }
        catch (Exception e)
        {
            CrowdControl.LogError("The Standalone Input Module's " + buttonName + " inside of the Crowd Control prefab is assigned a key that doesn't exist in the Input Manager. Please re-assign the " + buttonName + " key and restart.");
            return false;
        }

        return true;
    }
#pragma warning restore 168, 219
}
