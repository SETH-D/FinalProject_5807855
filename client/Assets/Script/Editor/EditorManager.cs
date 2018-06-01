using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorManager : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Utilis/Clear Player Pref")]
#endif
    private static void ClearPref()
    {
        PlayerPrefs.DeleteAll();
    }
}
