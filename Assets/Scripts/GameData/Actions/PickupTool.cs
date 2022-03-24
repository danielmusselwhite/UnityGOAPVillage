using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class PickupTool : GOAPAction {

    #region Setting up variables
    private bool hasTool = false;                           //Agent hasn't got a tool yet
    private ToolChestComponent targetToolChest;             //Where the agent will get the tool from
    Quaternion toolRotation = Quaternion.Euler(0, 0, -45);  //tools angle rotation
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public PickupTool()
    {
        addPrecondition("hasTool", false);  //this action will only be called if the agent hasn't got a tool
        addEffect("hasTool", true);         //when this action has finished the agent will now have a tool
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasTool = false;    //sets hasTool back to false
        targetToolChest = null;    //deletes the current targetToolChest
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasTool to the GOAPAgent, True if the agent has a tool, False if the agent doesn't.
        return hasTool;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the tool chest to pick up the tool.
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest toolChest which has spare tools

        //toolChests is an array of all of the objects in the scene which have then ToolChestComponent attatched
        ToolChestComponent[] toolChests = (ToolChestComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(ToolChestComponent));
        //Set the value for the closest toolChest to null as the closest toolChest has not been found yet
        ToolChestComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every chest in toolChests
        foreach(ToolChestComponent chest in toolChests)
        {
            //if the chest has more than 0 tools..
            if(chest.NumTools>0)
            {
                //If the closest chest doesn't have a value yet..
                if(closest==null)
                {
                    //..then this is the first chest in the for loop, choose it for now
                    closest = chest;
                    //the distance to the closest chest is equal to the magnitude (as the crow flies) between the chests transform position and the agents transform position.
                    closestDist = (chest.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest chest does have a value..
                else
                {
                    //..Is this chest closer than the last one?
                    
                    //the distance to this chest is equal to this chests transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (chest.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this chest is less than the closest to the last chest..
                    if(dist<closestDist)
                    {
                        //.. then we have just found a closer chest! Use this one instead.
                        closest = chest;
                        closestDist = dist;
                    }
                }
            }
        }

        //if closest is null / doesn't exist..
        if (closest==null)
        {
            //.. then we haven't found any chests with any tools in, in the scene

            //return false as no chests were found.
            return false;
        }

        //the targetToolChest is the closest toolChest
        targetToolChest = closest;
        //the target is the gameObject of the targetToolChest
        target = targetToolChest.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no chests were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {
        //if the targetToolChest has more than 0 tools.
        if (targetToolChest.NumTools > 0)
        {
            //Take away a tool from the Tool Chest
            targetToolChest.NumTools -= 1;
            //Set hasTool to true as the agent now has a tool
            hasTool = true;

            //create the tool and add it to the agent

            //find the agents backpack component
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //the tool object which will be attatched to the agent object
            GameObject prefab = Resources.Load<GameObject>(backpack.toolType);
            //add the tool to the agent
            GameObject tool = Instantiate(prefab, transform.position, transform.rotation*toolRotation) as GameObject;
            //add the tool to the backpack
            backpack.tool = tool;
            //attatch the tool
            tool.transform.parent = transform;

            //return true as the action succeeded
            return true;
        }
        //else the toolChest has 0 tools
        else
        {
            //another agent got to it before this agent did!

            //return false as the action failed
            return false;
        }
    }
    #endregion
}
