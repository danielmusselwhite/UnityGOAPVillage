using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepTimer : MonoBehaviour {

    public float maxSleepTime = 1f;
    public float sleepTime;
    public float tirednessPerFrame;

    public bool isTired;

    // Use this for initialization
    void Start ()
    {
        isTired = false;
        sleepTime = maxSleepTime;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (sleepTime > 0)
        {
            isTired = false;
            sleepTime -= tirednessPerFrame * Time.deltaTime;
        }
        if(sleepTime<=0)
        {
            //inside of labourer class isTired is a part of worldData so when it is true from here it is true in worldData so it is used by goap
            isTired = true;
        }
	}
}
