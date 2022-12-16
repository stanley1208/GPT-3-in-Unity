using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectInput : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject eventsys;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Input()
    {
        eventsys.GetComponent<WindowOpen>().enabled = false;
        Debug.Log("select");
    }

    public void Out()
    {
        eventsys.GetComponent<WindowOpen>().enabled = true;
        Debug.Log("deselect");
    }
}
