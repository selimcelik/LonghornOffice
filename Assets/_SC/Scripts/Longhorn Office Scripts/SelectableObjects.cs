using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObjects : MonoBehaviour
{

    public static SelectableObjects Instance;

    public List<GameObject> selectableObjects = new List<GameObject>();
    public Material mat;

    private bool mouseControl = true;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(blinkObjects());
    }

    // Update is called once per frame
    void Update()
    {
        if (!PickObjects.Instance.mouseIsOnTheObject && mouseControl)
        {
            StartCoroutine(blinkObjects());
            mouseControl = false;
        }
        if(PickObjects.Instance.mouseIsOnTheObject)
        {
            foreach (GameObject go in selectableObjects)
            {
                go.GetComponent<MeshRenderer>().material = mat;
            }
            StopAllCoroutines();
            mouseControl = true;
        }
    }

    IEnumerator blinkObjects()
    {
        foreach (GameObject go in selectableObjects)
        {
            go.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        yield return new WaitForSeconds(.5f);
        foreach (GameObject go in selectableObjects)
        {
            go.GetComponent<MeshRenderer>().material = mat;
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(blinkObjects());

    }
}
