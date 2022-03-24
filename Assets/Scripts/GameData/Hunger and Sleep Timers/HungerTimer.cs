using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerTimer : MonoBehaviour {

    public float maxEatTime = 1f;
    public float EatTime;
    public float hungerPerFrame;

    public bool isHungry;

    // Use this for initialization
    void Start ()
    {
        isHungry = false;
        EatTime = maxEatTime;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (EatTime > 0)
        {
            isHungry = false;
            EatTime -= hungerPerFrame * Time.deltaTime;
        }
        if(EatTime<=0)
        {
            //inside of labourer class isHungry is a part of worldData so when it is true from here it is true in worldData so it is used by goap
            isHungry = true;
        }
	}
}
