using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherBranches : GOAPAction
{
    #region Setting up variables
    private bool pickedup = false;              //Hasn't pickedup the branches yet
    private BranchesComponent targetBranches;     //Where the agent will get the logs from

    private float startTime = 0;             //Timer hasn't started
    public float gatheringDuration = 2;     //Seconds to gather the logs
    #endregion

    #region Preconditions and effects
    //needs to be named same as the class so GOAP can read it
    public GatherBranches()
    {
        addPrecondition("hasLogs", false);   //If agent has already gathered logs it doesn't want more
        addEffect("hasLogs", true);         //Upon gathering the branches the agent will now have logs
    }
    #endregion

    #region reset
    //When the action has been completed and variables need to be reset

    //overrides the abstract procedure reset from the labourer class
    public override void reset()
    {
        pickedup = false;      //Change pickedup back to false
        targetBranches = null;  //Change targetBranches back to null
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
        return true;    //Yes the agent needs to be near the branches
    }
    #endregion

    #region checkProceduralPreconditions
    //overrides the abstract function checkProceduralPrecondition from the labourer class
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest branches the agent can gather logs from

        //creates an array of all of the objects in the seen with the branchesComponent attatched
        BranchesComponent[] branches = FindObjectsOfType(typeof(BranchesComponent)) as BranchesComponent[];
        //Predefines the closest branches as null as the closest has not been found yet
        BranchesComponent closest = null;
        //as the closest branches is not know yet the distance to it does not exist at this current time
        float closestDist = 0;

        //for every branch in the branches array
        foreach (BranchesComponent branch in branches)
        {
            //..if the closest branch is null..
            if (closest == null)
            {
                //..this is the first branch so choose it for now
                closest = branch;
                //Distance to the closest branch as the crow flies
                closestDist = (branch.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet branch isn't null
            else
            {
                //.. is this branch closer than the last one?
                float dist = (branch.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new branch is small than the distance to the 'closest' branch..
                if (dist < closestDist)
                {
                    //we have just found a closer branch, use this one instead
                    closest = branch;
                    closestDist = dist;
                }
            }
        }
        //the agent wants to go towards the closest branches
        targetBranches = closest;
        target = targetBranches.gameObject;

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
            //Has finished gathering the logs

            //reference to the agents backpackComponent
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //increase the agents number of logs
            backpack.numLogs += 3;
            pickedup = true;

        }
        //return true
        return true;
    }
    #endregion
}
