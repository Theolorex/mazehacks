using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public static CamMove Instance { get; private set; }
    
    public Transform player;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private float _shakeAmount;
    private float _shakeEndTime;

    void Start()
    {
        Instance = this;
    }
    void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 currentCenter = player.position + offset;

        if (Time.time < _shakeEndTime)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-_shakeAmount, _shakeAmount),
                Random.Range(-_shakeAmount, _shakeAmount),
                0f
            );

            transform.position = currentCenter + shakeOffset;
        }
        else
        {
            transform.position = currentCenter;
        }
    }
    
    public void CameraShake(float shakeAmount)
    {
        _shakeAmount = shakeAmount;
        _shakeEndTime = Time.time + 0.1f;
    }
}
