using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } //singletonize this b 
    
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraShake(float shakeAmount)
    {
        //based off currentCenter, shakeAmount based off a radius around
    }
}
