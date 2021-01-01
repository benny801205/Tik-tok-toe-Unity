using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UserpanelLoader))]
public class ShakeDetector : MonoBehaviour
{

    public float ShaekDtectionThreshold;

    private UserpanelLoader panellaoder;
    private float sqrShaekDtectionThreshold;




    // Start is called before the first frame update
    void Start()
    {
        sqrShaekDtectionThreshold = Mathf.Pow(ShaekDtectionThreshold, 2);
        panellaoder = GetComponent<UserpanelLoader>();


    }

    // Update is called once per frame
    void Update()
    {


        //if(Input.GetKeyDown("space"))
        if(Input.acceleration.sqrMagnitude >= sqrShaekDtectionThreshold)
        {
            Debug.Log("detect and load to game page");
            panellaoder.LoadGameScene();
            
        }


    }
}
