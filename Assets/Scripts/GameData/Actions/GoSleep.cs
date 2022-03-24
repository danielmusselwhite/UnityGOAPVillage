using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class GoSleep : GOAPAction
{

    #region Setting up variables
    private bool hasSlept = false;                          //Agent hasn't gone to sleep yet
    private BedComponent targetBed;             //Where the agent will go to sleep

    private float startTime = 0;        //Timer hasn't started
    public float sleepingDuration = 5;    //Seconds to go to sleep

    bool Sleeping = false;
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public GoSleep()
    {
        addPrecondition("isTired", true);  //this action will only be called if the agent hasn't got a tool
        addEffect("isTired", false);         //when this action has finished the agent will now have a tool
        addEffect("hasSlept", true);
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        Sleeping = false;
        startTime = 0;      //reset timer
        hasSlept = false;    //sets hasSlept back to false
        //if (targetBed!=null)
            
        targetBed = null;    //deletes the current targetBed
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasSlept to the GOAPAgent, True if the agent has slept, False if the agent hasn't.
        return hasSlept;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the bed in order to sleep.
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest bed

        //beds is an array of all of the objects in the scene which have then BedComponent attatched
        BedComponent[] beds = (BedComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(BedComponent));
        //Set the value for the closest bed to null as the closest bed has not been found yet
        BedComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every bed in beds array
        foreach (BedComponent bed in beds)
        {
            //if the bed is free/ there isn't an agent already in it..
            if (bed.isFree)
            {
                //If the closest bed doesn't have a value yet..
                if (closest == null)
                {
                    //..then this is the first bed in the for loop, choose it for now
                    closest = bed;
                    //the distance to the closest bed is equal to the magnitude (as the crow flies) between the bed transform position and the agents transform position.
                    closestDist = (bed.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest bed does have a value..
                else
                {
                    //..Is this cbed closer than the last one?

                    //the distance to this bed is equal to this beds transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (bed.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this bed is less than the closest to the last bed..
                    if (dist < closestDist)
                    {
                        //.. then we have just found a closer bed! Use this one instead.
                        closest = bed;
                        closestDist = dist;
                    }
                }
            }
        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any beds in the scene

            //return false as no beds were found.
            return false;
        }

        //the targetBed is the closest bed
        targetBed = closest;
        //the target is the gameObject of the targetBed
        target = targetBed.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no beds were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    { 
        Debug.Log(targetBed.isFree);
        //if the targetBed is free/ there isn't an agent already sleeping in it..
        if (targetBed.isFree||Sleeping==true)
        {
            Sleeping = true;
            Debug.Log("FREE BED");
            //the targetBed is no longer free
            targetBed.isFree = false;
            if (startTime == 0)
            {
                startTime = Time.time;
            }

            if (Time.time - startTime > sleepingDuration)
            {
                //Set hasSlept to true as the agent now has slept
                hasSlept = true;

                //find the agents sleepTimer component
                SleepTimer sleepTimer = (SleepTimer)agent.GetComponent(typeof(SleepTimer));

                sleepTimer.sleepTime = sleepTimer.maxSleepTime;

                targetBed.isFree = true;
                //return true as the action succeeded
            }
            return true;
        }

        //else the targetBed is no longer free
        else
        {
            Debug.Log("BED ISNT FREE");
            //another agent got there first!

            //return false as action failed.
            return false;
        }
    }
    #endregion
}
