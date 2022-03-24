using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRubble : GOAPAction
{
    #region Setting up variables
    private bool pickedup = false;        //Hasn't pickedup the rubble yet
    private RubbleComponent targetRubble;     //Where the agent will get the ore from

    private float startTime = 0;            //Timer hasn't started
    public float gatheringDuration = 2;     //Seconds to gather the ore
    #endregion

    #region Preconditions and effects
    //needs to be named same as the class so GOAP can read it
    public PickupRubble()
    {
        addPrecondition("hasOre", false);   //If agent has already gathered ore it doesn't want more
        addEffect("hasOre", true);         //Upon mining the rock the agent will now have ore!
    }
    #endregion

    #region reset
    //When the action has been completed and variables need to be reset

    //overrides the abstract procedure reset from the labourer class
    public override void reset()
    {
        pickedup = false;      //Change pickedup back to false
        targetRubble= null;  //Change targetRubble back to null
        startTime = 0;      //Reset timer back to 0
    }
    #endregion

    #region isDone
    //overrides the abstract function isDone from the labourer class
    public override bool isDone()
    {
        //return pickedup, true if action succeeded, false if it failed.
        return pickedup;
    }
    #endregion

    #region requiresInRange
    //overrides the abstract function requiresInRange from the labourer class
    public override bool requiresInRange()
    {
        return true;    //Yes the agent needs to be near the rubble
    }
    #endregion

    #region checkProceduralPreconditions
    //overrides the abstract function checkProceduralPrecondition from the labourer class
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest rubble the miner can gather ore from

        //creates an array of all of the objects in the seen with the rubbleComponent attatched
        RubbleComponent[] rubbles = FindObjectsOfType(typeof(RubbleComponent)) as RubbleComponent[];
        //Predefines the closest rubble as null as the closest has not been found yet
        RubbleComponent closest = null;
        //as the closest rubble is not know yet the distance to it does not exist at this current time
        float closestDist = 0;

        //for every rubble in the rubbles array
        foreach (RubbleComponent rubble in rubbles)
        {
            //..if the closest rubble is null..
            if (closest == null)
            {
                //..this is the first rubble so choose it for now
                closest = rubble;
                //Distance to the closest rubble as the crow flies
                closestDist = (rubble.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet rubble isn't null
            else
            {
                //.. is this rubble closer than the last one?
                float dist = (rubble.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new rubble is small than the distance to the 'closest' rubble..
                if (dist < closestDist)
                {
                    //we have just found a closer rubble, use this one instead
                    closest = rubble;
                    closestDist = dist;
                }
            }
        }
        //the agent wants to go towards the closest rubble
        targetRubble = closest;
        target = targetRubble.gameObject;

        //returns true if the agent now has a target returns false if the agent doesn't.
        return closest != null;
    }
    #endregion

    #region perform
    //overrides the abstract perform procedure in the labourer class
    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > gatheringDuration)
        {
            //Has finished gathering the rubble

            //reference to the agents backpackComponent
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //increase the agents number of ore
            backpack.numOre += 3;
            pickedup = true;

        }
        //return true
        return true;
    }
    #endregion
}
