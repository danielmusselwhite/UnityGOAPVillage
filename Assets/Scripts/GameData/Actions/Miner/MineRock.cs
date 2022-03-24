using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineRock : GOAPAction
{
    #region Setting up variables
    private bool mined = false;        //Hasn't mined the rock yet
    private RockComponent targetRock;     //Where the agent will get the ore from

    private float startTime = 0;        //Timer hasn't started
    public float miningDuration = 1;    //Seconds to mine the rock
    #endregion

    #region Preconditions and effects
    //needs to be named same as the class so GOAP can read it
    public MineRock()
    {
        addPrecondition("hasTool", true);   //Agent needs a tool to mine the rock
        addPrecondition("hasOre", false);   //If agent has already gathered ore it doesn't want more
        addEffect("hasOre", true);         //Upon mining the rock the agent will now have ore!
    }
    #endregion

    #region reset
    //When the action has been completed and variables need to be reset

    //overrides the abstract procedure reset from the labourer class
    public override void reset()
    {
        mined = false;      //Change mined back to false
        targetRock = null;  //Change targetRock back to null
        startTime = 0;      //Reset timer back to 0
    }
    #endregion

    #region isDone
    //overrides the abstract function isDone from the labourer class
    public override bool isDone()
    {
        //return mined, true if action succeeded, false if it failed.
        return mined;
    }
    #endregion

    #region requiresInRange
    //overrides the abstract function requiresInRange from the labourer class
    public override bool requiresInRange()
    {
        return true;    //Yes the hunter needs to be near the pig
    }
    #endregion

    #region checkProceduralPreconditions
    //overrides the abstract function checkProceduralPrecondition from the labourer class
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest rock the miner can mine

        //creates an array of all of the objects in the seen with the rockComponent
        RockComponent[] rocks = FindObjectsOfType(typeof(RockComponent)) as RockComponent[];
        //Predefines the closest rock as null as the closest has not been found yet
        RockComponent closest = null;
        //as the closest rock is not know yet the distance to it does not exist at this current time
        float closestDist = 0;

        //for every rock in the rocks array
        foreach (RockComponent rock in rocks)
        {
            //..if the closest rock is null..
            if (closest == null)
            {
                //..this is the first rock so choose it for now
                closest = rock;
                //Distance to the closest rock as the crow flies
                closestDist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet rock isn't null
            else
            {
                //.. is this rock closer than the last one?
                float dist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new rock is small than the distance to the 'closest' rock..
                if (dist < closestDist)
                {
                    //we have just found a closer rock, use this one instead
                    closest = rock;
                    closestDist = dist;
                }
            }
        }
        //the agent wants to go towards the closest rock
        targetRock = closest;
        target = targetRock.gameObject;

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

        if (Time.time - startTime > miningDuration)
        {
            //Has finished mining

            //reference to the agents backpackComponent
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //increase the agents number of ore
            backpack.numOre += 5;
            mined = true;

            //reference to the agents tool
            ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;

            //active the toolComponents use procedure, which will decrease its strength by the value inputted (0.34)
            tool.use(0.34f);
            //if tool destroyed returns true (if the tool has less than 0 strength)
            if (tool.destroyed())
            {
                //destroy that game object
                Destroy(backpack.tool);
                //set backpack tool back to null
                backpack.tool = null;
            }
        }
        //return true
        return true;
    }
    #endregion
}
