using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntPig : GOAPAction
{
    #region Setting up variables
    private bool hunted = false;        //Hasn't hunted the pig yet
    private PigComponent targetPig;     //Where the agent will get the uncooked meat from

    private float startTime = 0;        //Timer hasn't started
    public float huntingDuraion = 2;    //Seconds to hunt the pig
    #endregion

    #region Hunt Pig Action
    //needs to be named same as the class so GOAP can read it
    public HuntPig()
    {
        addPrecondition("hasTool", true);           //Agent needs a tool to hunt the pig
        addPrecondition("hasUncookedFood", false);  //If agent has already gathered uncooked meat it doesn't want more
        addEffect("hasUncookedFood", true);         //Upon hunting the pig it will have the effect of giving the agent UncookedMeat
    }

    //When the action has been completed and variables need to be reset
    public override void reset()
    {
        hunted = false;     //Change hunted to false, telling the hunter he has not hunted the pig yet
        targetPig = null;   //Change targetPig to null, he wants to hunt a differnet pig now
        startTime = 0;      //Reset timer back to 0
    }

    public override bool isDone()
    {
        return hunted;
    }

    public override bool requiresInRange()
    {
        return true;    //Yes the hunter needs to be near the pig
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest pig the hunter can kill
        PigComponent[] pigs = FindObjectsOfType(typeof(PigComponent)) as PigComponent[];    //Creates a list of all of the 'pigs' in the world
        PigComponent closest = null;                                                         //The 'closest' pig is defined but not assigned
        float closestDist = 0;                                                              //The distance to the closest pig

        foreach(PigComponent pig in pigs)       //for every pig in pigs..
        {
            //..if the closest pig is null..
            if (closest == null)
            {
                //..this is the first pig so choose it for now
                closest = pig;
                //Distance to the closest pig
                closestDist = (pig.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet pig isn't null
            else
            {
                //.. is this pig closer than the last one?
                float dist = (pig.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new pig is small than the distance to the 'closest' pig..
                if (dist<closestDist)
                {
                    //we have just found a closer pig, use this one instead
                    closest = pig;
                    closestDist = dist;
                }
            }
        }
        targetPig = closest;    // The hunters target is the closest pig
        target = targetPig.gameObject;

        return closest != null;     //Return closest to the GOAPAction
    }

    public override bool perform (GameObject agent)
    {
        if(startTime==0)
        {
            startTime = Time.time;
        }

        if(Time.time-startTime>huntingDuraion)
        {
            //Has finished hunting
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            backpack.numUncookedFood += 5;      //increase hunters supply of uncooked meat by 2
            hunted = true;
            ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;
   
            tool.use(0.34f);
            if(tool.destroyed())
            {
                Destroy(backpack.tool);
                backpack.tool = null;
            }
        }
        return true;
    }
    #endregion
}
