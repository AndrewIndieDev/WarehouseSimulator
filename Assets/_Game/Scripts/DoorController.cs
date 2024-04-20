using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Open"))
        {
            if (Application.isPlaying)
                (target as DoorController)?.Open();
            else
                (target as DoorController).doorTransform.localPosition = (target as DoorController).openPosition;
        }
        if (GUILayout.Button("Close"))
        {
            if (Application.isPlaying)
                (target as DoorController)?.Close();
            else
                (target as DoorController).doorTransform.localPosition = (target as DoorController).closePosition;
        }
    }
}

#endif

public class DoorController : MonoBehaviour
{
    private enum DoorType { Receiving, Dispatch }
    public static DoorController receiving;
    public static DoorController dispatch;
    [SerializeField] private DoorType doorType;

    private void Awake()
    {
        switch (doorType)
        {
            case DoorType.Receiving:
                receiving = this;
                break;
            case DoorType.Dispatch:
                dispatch = this;
                break;
        }
    }

    [SerializeField] private AudioSource audioSource;
    public Transform doorTransform;
    public Vector3 openPosition;
    public Vector3 closePosition;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private float openTime = 5f;

    [HideInInspector] public bool isInTransition = false;
    [HideInInspector] public bool isOpen = false;
    

    public void Open()
    {
        audioSource.clip = openSound;
        audioSource.Play();
        isInTransition = true;
        isOpen = true;
        StartCoroutine(OpenDoor());
    }
    
    public void Close()
    {
        audioSource.clip = closeSound;
        audioSource.Play();
        isInTransition = true;
        isOpen = false;
        StartCoroutine(CloseDoor());
    }

    public void ToggleOpen()
    {
        if (isInTransition) return;
        if (isOpen)
            Close();
        else
            Open();
    }
    
    IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(0.5f);
        float i = 0;
        while (i < openTime)
        {
            i += Time.deltaTime;
            doorTransform.localPosition = Vector3.Lerp(closePosition, openPosition, openCurve.Evaluate(i / openTime));
            yield return null;
        }
        isInTransition = false;
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(0.1f);
        float i = 0;
        while (i < openTime)
        {
            i += Time.deltaTime;
            doorTransform.localPosition = Vector3.Lerp(openPosition, closePosition, openCurve.Evaluate(i / openTime));
            yield return null;
        }
        isInTransition = false;
    }
}
