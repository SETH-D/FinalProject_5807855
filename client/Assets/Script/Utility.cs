using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

    public static void Delay(float delay, System.Action callback)
    {
        Co.Delay(callback, delay);
        //StartCoroutine (DelayProcess (delay, callback));
    }
}
