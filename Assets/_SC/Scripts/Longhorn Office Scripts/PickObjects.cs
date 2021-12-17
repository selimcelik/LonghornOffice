using System.Collections;
using System.Collections.Generic;
using ManagerActorFramework;
using UnityEngine;

public class PickObjects : Actor<LevelManager>
{
    public bool startLevel = false;
    public bool levelEnd = false;

    public static PickObjects Instance;

    private Transform _selection;
    public bool mouseIsOnTheObject = false;

    public bool redPenClicked, bluePenClicked, glassClicked, plantClicked,waterDispenserClicked,whiteBoardClicked = false;

    public bool addForceToGlass = false;

    private int boardWillBeBlackFor2Times = 0;

    private bool plantOpen = false;
    private bool redPenDone, bluePenDone, glassDone,canClickDoor = false;

    [SerializeField] private List<GameObject> afterOneClickObjects = new List<GameObject>(); 
    // Start is called before the first frame update
    protected override void MB_Awake()
    {
        Instance = this;
    }
    protected override void MB_Start()
    {
        
    }

    // Update is called once per frame
    protected override void MB_Update()
    {
        if (!levelEnd && startLevel)
        {
            if (redPenDone && bluePenDone && glassDone)
            {
                canClickDoor = true;
                afterOneClickObjects[3].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            }

            if (_selection != null && !redPenClicked && !bluePenClicked && !glassClicked)
            {
                var selectionTransform = _selection.GetComponent<Transform>();
                selectionTransform.localScale = new Vector3(1, 1, 1);
                _selection = null;
                mouseIsOnTheObject = false;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var selection = hit.transform;
                if (!glassClicked && !bluePenClicked && !redPenClicked)
                {
                    if (hit.transform != null)
                    {
                        if (hit.transform.gameObject.tag == "RedPen" || hit.transform.gameObject.tag == "BluePen" || hit.transform.gameObject.tag == "Glass")
                        {
                            mouseIsOnTheObject = true;
                            hit.transform.localScale = new Vector3(2, 2, 2);
                        }

                    }
                    _selection = selection;
                }

            }

            if (Input.GetMouseButtonDown(0))
            {
                var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit1;
                if (Physics.Raycast(ray1, out hit1))
                {
                    if (hit1.transform != null)
                    {
                        if (hit1.transform.gameObject.tag == "RedPen")
                        {
                            redPenClicked = true;
                            StartCoroutine(boardWillBlink());
                        }

                        if (hit1.transform.gameObject.tag == "BluePen")
                        {
                            bluePenClicked = true;
                            StartCoroutine(boardWillBlink());
                        }

                        if (hit1.transform.gameObject.tag == "Glass")
                        {
                            glassClicked = true;
                            if (!plantOpen)
                            {
                                StartCoroutine(waterDispenserBlink());
                            }
                            if (addForceToGlass)
                            {
                                Rigidbody rb = SelectableObjects.Instance.selectableObjects[0].GetComponent<Rigidbody>();
                                rb.isKinematic = false;
                                //rb.AddForce(transform.forward*20,ForceMode.Impulse);
                                rb.AddExplosionForce(500, rb.transform.position, 50);
                                glassClicked = false;
                                glassDone = true;
                            }
                        }

                        if (hit1.transform.gameObject.tag == "Whiteboard" && redPenClicked)
                        {
                            PenFonc();
                        }
                        if (hit1.transform.gameObject.tag == "Whiteboard" && bluePenClicked)
                        {
                            PenFonc();
                        }

                        if (hit1.transform.gameObject.tag == "WaterDispenser" && glassClicked)
                        {
                            GlassFonc();
                        }

                        if (hit1.transform.gameObject.tag == "Plant" && glassClicked)
                        {
                            GlassFoncForPlant();
                        }

                        if (hit1.transform.gameObject.tag == "LevelEnd" && canClickDoor)
                        {
                            levelEnd = true;
                            Push(ManagerEvents.FinishLevel, true);
                        }
                    }
                }
            }
        }
 
    }

    private void GlassFoncForPlant()
    {
        plantClicked = true;
        ColorLooper.Instance.startTime = Time.time;
        SelectableObjects.Instance.selectableObjects[0].transform.position = afterOneClickObjects[2].transform.GetChild(1).transform.GetChild(0).transform.position;
        SelectableObjects.Instance.selectableObjects[0].transform.rotation = afterOneClickObjects[2].transform.GetChild(1).transform.GetChild(0).transform.rotation;
        afterOneClickObjects[1].transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        afterOneClickObjects[2].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(true);
        addForceToGlass = true;
        StartCoroutine(particleDisable());
    }

    private void GlassFonc()
    {
        SelectableObjects.Instance.selectableObjects[0].GetComponent<ColorLooper>().enabled = true;
        ColorLooper.Instance.startTime = Time.time;
        SelectableObjects.Instance.selectableObjects[0].transform.position = afterOneClickObjects[1].transform.GetChild(0).transform.GetChild(0).transform.position;
        afterOneClickObjects[1].transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
        plantOpen = true;
    }

    private void PenFonc()
    {
        if (redPenClicked)
        {
            SelectableObjects.Instance.selectableObjects[2].SetActive(false);
            afterOneClickObjects[0].transform.GetChild(0).gameObject.SetActive(true);
            redPenClicked = false;
            redPenDone = true;
        }
        if (bluePenClicked)
        {
            SelectableObjects.Instance.selectableObjects[1].SetActive(false);
            afterOneClickObjects[0].transform.GetChild(1).gameObject.SetActive(true);
            bluePenClicked = false;
            bluePenDone = true;
        }

        boardWillBeBlackFor2Times++;
        StartCoroutine(boardBlacker());
    }

    IEnumerator boardBlacker()
    {
        yield return new WaitForSeconds(1);
        afterOneClickObjects[0].GetComponent<MeshRenderer>().material.color = Color.black;
        yield return new WaitForSeconds(1);
        if (boardWillBeBlackFor2Times < 2)
        {
            afterOneClickObjects[0].GetComponent<MeshRenderer>().material = SelectableObjects.Instance.mat;

        }
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator boardWillBlink()
    {
        afterOneClickObjects[0].GetComponent<MeshRenderer>().material.color = Color.yellow;
        yield return new WaitForSeconds(1);
        afterOneClickObjects[0].GetComponent<MeshRenderer>().material = SelectableObjects.Instance.mat;
    }

    IEnumerator waterDispenserBlink()
    {
        afterOneClickObjects[1].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        afterOneClickObjects[1].transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        yield return new WaitForSeconds(1);
        afterOneClickObjects[1].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = SelectableObjects.Instance.mat;
        afterOneClickObjects[1].transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = SelectableObjects.Instance.mat;

    }
    IEnumerator particleDisable()
    {
        yield return new WaitForSeconds(1);
        afterOneClickObjects[2].transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
    }
}
