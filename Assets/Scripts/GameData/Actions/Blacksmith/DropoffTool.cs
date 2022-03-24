using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropoffTool : GOAPAction
{

    #region Setting up variables
    private bool droppedOffTool = false;             //Agent hasn't dropped tool  yet
    private ToolChestComponent targetToolChest;     //Where the agent will drop the tools off to
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public DropoffTool()
    {
        addPrecondition("hasTools", true);    //this action will only be called if the agent has tools
        addEffect("collectTools", true);      //when this action has finished he will have reached his goal of collecting tools!
        addEffect("hasTools", false);
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        droppedOffTool = false;  //sets droppedOffTool back to false
        targetToolChest = null;    //deletes the current targetToolChest
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns droppedOffTool to the GOAPAgent, True if the agent has dropped off the tool false if the agent hasn't and the action failed
        return droppedOffTool;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the toolChest to drop tools off into it
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest tool chest

        //toolChests is an array of all of the objects in the scene which have the ToolChestComponent attatched
        ToolChestComponent[] toolChests = (ToolChestComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(ToolChestComponent));
        //Set the value for the closest toolChest to null as the closest toolChest has not been found yet
        ToolChestComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every toolChest in toolChests
        foreach (ToolChestComponent toolChest in toolChests)
        {
            //If the closest toolChest doesn't have a value yet
            if (closest == null)
            {
                //..then this is the first chest in the for loop, choose it for now
                closest = toolChest;
                //the distance to the closest chest is equal to the magnitude (as the crow flies) between the chests transform position and the agents transform position.
                closestDist = (toolChest.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //Else the closest toolChest does have a value..
            else
            {
                //..Is this chest closer than the last one?

                //the distance to this chest is equal to this chests transform position - the transform position of the agent in magnitude / as the crow flies.
                float dist = (toolChest.gameObject.transform.position - agent.transform.position).magnitude;

                //if the distance to this chest is less than the closest to the last chest..
                if (dist < closestDist)
                {
                    //.. then we have just found a closer chest! Use this one instead.
                    closest = toolChest;
                    closestDist = dist;
                }
            }

        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any chests

            //return false as no chests were found.
            return false;
        }

        //the targetToolChest is the closest chest
        targetToolChest = closest;
        //the target is the gameObject of the targetToolChest
        target = targetToolChest.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no toolChest were found.
        return closest != null;

    }
    #endregion

    #region perform action
    public override bool perform(GameObject agent)
    {
        //find the agents backpack
        BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
        targetToolChest.NumTools += backpack.numOfTools;
        droppedOffTool = true;
        backpack.numOfTools = 0;

        return true;
    }
    #endregion
}
