using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLooper : MonoBehaviour
{
    public static ColorLooper Instance;
    public float speed = 0.1f;
    public Color startColor;
    public Color endColor;
    public float startTime;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PickObjects.Instance.plantClicked)
        {
            float t = (Time.time - startTime) * speed;
            GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, t);
        }
        else
        {
            float t = (Time.time - startTime) * speed;
            GetComponent<Renderer>().material.color = Color.Lerp(endColor, startColor, t);
        }

    }
}
