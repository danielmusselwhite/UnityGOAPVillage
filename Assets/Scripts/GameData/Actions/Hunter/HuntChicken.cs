using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntChicken : GOAPAction
{
    #region Setting up variables
    private bool hunted = false;        //Hasn't hunted the chicken yet
    private ChickenComponent targetChicken;     //Where the agent will get the uncooked meat from

    private float startTime = 0;        //Timer hasn't started
    public float huntingDuration = 3;    //Seconds to hunt the chicken
    #endregion

    #region Hunt Chicken Action
    //needs to be named same as the class so GOAP can read it
    public HuntChicken()
    {
        addPrecondition("hasUncookedFood", false);  //If agent has already gathered uncooked meat it doesn't want more
        addEffect("hasUncookedFood", true);         //Upon hunting the chicken it will have the effect of giving the agent UncookedMeat
    }

    //When the action has been completed and variables need to be reset
    public override void reset()
    {
        hunted = false;         //Change hunted to false, telling the hunter he has not hunted the chicken yet
        targetChicken = null;   //Change targetChicken to null, he wants to hunt a different chicken now
        startTime = 0;          //Reset timer back to 0
    }

    public override bool isDone()
    {
        return hunted;
    }

    public override bool requiresInRange()
    {
        return true;    //Yes the hunter needs to be near the chicken
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest chicken the hunter can kill
        ChickenComponent[] chickens = FindObjectsOfType(typeof(ChickenComponent)) as ChickenComponent[];    //Creates a list of all of the 'chickens' in the world
        ChickenComponent closest = null;                                                                    //The 'closest' chicken is defined but not assigned
        float closestDist = 0;                                                                              //The distance to the closest chicken

        foreach (ChickenComponent chicken in chickens)       //for every chicken in chickens..
        {
            //..if the closest chicken is null..
            if (closest == null)
            {
                //..this is the first chicken so choose it for now
                closest = chicken;
                //Distance to the closest chicken
                closestDist = (chicken.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet chicken isn't null
            else
            {
                //.. is this chicken closer than the last one?
                float dist = (chicken.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new chicken is small than the distance to the 'closest' chicken..
                if (dist < closestDist)
                {
                    //we have just found a closer chicken, use this one instead
                    closest = chicken;
                    closestDist = dist;
                }
            }
        }
        targetChicken = closest;    // The hunters target is the closest chicken
        target = targetChicken.gameObject;

        return closest != null;     //Return closest to the GOAPAction
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > huntingDuration)
        {
            //Has finished hunting
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            backpack.numUncookedFood += 3;      //increase hunters supply of uncooked meat by 1
            hunted = true;
        }
        return true;
    }
    #endregion
}
