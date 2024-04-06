using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum EIgnoreAxis
{
    X,
    Y,
    Z,
    W,
    XY,
    XZ,
    XW,
    YZ,
    YW,
    ZW,
    XYZ,
    XYW,
    XZW,
    YZW
}

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomElement<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static Vector3 RandomUnitVectorInCone(this Vector3 targetDirection, float angle)
    {
        return RandomUnitVectorInCone(Quaternion.LookRotation(targetDirection), angle);
    }

    public static Vector3 RandomUnitVectorInCone(this Quaternion targetDirection, float angle)
    {
        var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
        return targetDirection * V;
    }

    public static Texture2D TextureFromGradient(Gradient g, int resolution)
    {
        Texture2D t = new Texture2D(resolution, 1);
        t.filterMode = FilterMode.Point;
        Color[] colors = new Color[resolution];
        for (int i = 0; i < resolution; ++i)
        {
            colors[i] = g.Evaluate((float)i / resolution);
        }
        t.SetPixels(colors);
        t.Apply();
        return t;
    }

    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

    public static float BetterSmoothstep(float a, float b, float x)
    {
        float t = Mathf.Clamp01((x - a) / (b - a));
        return t * t * (3.0f - (2.0f * t));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static bool AddUnique<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

    public static void ExecuteNextFrame(this MonoBehaviour behaviour, System.Action action)
    {
        behaviour.StartCoroutine(NextFrame(action));
    }

    private static IEnumerator NextFrame(System.Action action)
    {
        yield return 0;
        action();
    }

    public static bool IsValidIndex(this Array array, int index)
    {
        return array.Length > index && index >= 0;
    }

    public static bool IsValidIndex<T>(this List<T> array, int index)
    {
        return array.Count > index && index >= 0;
    }

    public static Vector2 IgnoreAxis(this Vector2 vector, EIgnoreAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EIgnoreAxis.X:
                return new Vector2(ignoredAxisValue, vector.y);
            case EIgnoreAxis.Y:
                return new Vector2(vector.x, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static Vector3 IgnoreAxis(this Vector3 vector, EIgnoreAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EIgnoreAxis.X:
                return new Vector3(ignoredAxisValue, vector.y, vector.z);
            case EIgnoreAxis.Y:
                return new Vector3(vector.x, ignoredAxisValue, vector.z);
            case EIgnoreAxis.Z:
                return new Vector3(vector.x, vector.y, ignoredAxisValue);
            case EIgnoreAxis.XY:
                return new Vector3(ignoredAxisValue, ignoredAxisValue, vector.z);
            case EIgnoreAxis.XZ:
                return new Vector3(ignoredAxisValue, vector.y, ignoredAxisValue);
            case EIgnoreAxis.YZ:
                return new Vector3(vector.x, ignoredAxisValue, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static Vector4 IgnoreAxis(this Vector4 vector, EIgnoreAxis ignoredAxis, float ignoredAxisValue = 0)
    {
        switch (ignoredAxis)
        {
            case EIgnoreAxis.X:
                return new Vector4(ignoredAxisValue, vector.y, vector.z, vector.w);
            case EIgnoreAxis.Y:
                return new Vector4(vector.x, ignoredAxisValue, vector.z, vector.w);
            case EIgnoreAxis.Z:
                return new Vector4(vector.x, vector.y, ignoredAxisValue, vector.w);
            case EIgnoreAxis.W:
                return new Vector4(vector.x, vector.y, vector.z, ignoredAxisValue);
            case EIgnoreAxis.XY:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, vector.w);
            case EIgnoreAxis.XZ:
                return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, vector.w);
            case EIgnoreAxis.XW:
                return new Vector4(ignoredAxisValue, vector.y, vector.z, ignoredAxisValue);
            case EIgnoreAxis.YZ:
                return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, vector.w);
            case EIgnoreAxis.YW:
                return new Vector4(vector.x, ignoredAxisValue, vector.z, ignoredAxisValue);
            case EIgnoreAxis.ZW:
                return new Vector4(vector.x, vector.y, ignoredAxisValue, ignoredAxisValue);
            case EIgnoreAxis.XYZ:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, ignoredAxisValue, vector.w);
            case EIgnoreAxis.XYW:
                return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, ignoredAxisValue);
            case EIgnoreAxis.XZW:
                return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, ignoredAxisValue);
            case EIgnoreAxis.YZW:
                return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, ignoredAxisValue);
        }
        Debug.Log("Unsupported ignored axis given");
        return vector;
    }

    public static void SaveAsPNG(this Texture2D texture, string fullPath)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }

    public static void AttachTo(this SkinnedMeshRenderer smr, SkinnedMeshRenderer parent)
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in parent.bones)
            boneMap[bone.gameObject.name] = bone;


        SkinnedMeshRenderer myRenderer = smr;

        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {
                Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
                continue;
            }
        }
        myRenderer.bones = newBones;

        smr.rootBone = parent.rootBone;
        smr.transform.parent = parent.transform.parent;
    }

    public static bool HasOnlyHexInString(this string text)
    {
        return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
    }

    public static string Nicify(this string s)
    {
        string result = "";
        if (s.Length > 1)
            s = char.ToUpper(s[0]) + s.Substring(1);
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsUpper(s[i]) == true && i != 0)
            {
                result += " ";
            }

            result += s[i];
        }
        return result;
    }
}
