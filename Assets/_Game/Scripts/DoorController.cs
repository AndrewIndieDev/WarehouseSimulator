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
        if (!Application.isPlaying) return;
        if (GUILayout.Button("Open"))
        {
            (target as DoorController)?.Open();
        }
        if (GUILayout.Button("Close"))
        {
            (target as DoorController)?.Close();
        }
    }
}

#endif

public class DoorController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closePosition;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private float openTime = 5f;

    public void Open()
    {
        audioSource.clip = openSound;
        audioSource.Play();
        StartCoroutine(OpenDoor());
    }
    
    public void Close()
    {
        audioSource.clip = closeSound;
        audioSource.Play();
        StartCoroutine(CloseDoor());
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
    }
}
