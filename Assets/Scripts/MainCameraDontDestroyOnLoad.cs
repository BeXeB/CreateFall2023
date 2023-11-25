using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraDontDestroyOnLoad : MonoBehaviour
{
    private static MainCameraDontDestroyOnLoad instance { get; set; }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
}
