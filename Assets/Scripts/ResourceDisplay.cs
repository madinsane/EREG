using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    public UnitManager unitManager;
    public Image resource;
    public float changeSpeed = 10f;
    private float value = 1;
    void Awake()
    {
        value = GetResourcePercent();
        resource.fillAmount = value;
    }

    float GetResourcePercent()
    {
        return 1;
    }

    void OnHit(int costPercent)
    {
        float diff = value - costPercent;
        value = Mathf.Lerp(value, diff, Time.deltaTime * changeSpeed);
        resource.fillAmount = value;
    }
}
