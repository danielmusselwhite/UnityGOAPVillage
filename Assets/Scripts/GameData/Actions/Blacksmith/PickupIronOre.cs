using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class PickupIronOre : GOAPAction
{

    #region Setting up variables
    private bool hasIronOre = false;                    //Agent hasn't picked it up yet
    private ToolChestComponent targetChest;               //Where the agent will pick it up from
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public PickupIronOre()
    {
        addPrecondition("hasOre", false);  //this action will only be called if the agent hasn't got any iron ore
        addEffect("hasOre", true);         //when this action has finished the agent will now have iron ore
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasIronOre = false;     //sets hasIronOre back to false
        targetChest = null;     //deletes the current targetChest
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasIronOre to the GOAPAgent, True if the agent has iron ore, False if the agent doesn't.
        return hasIronOre;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the chest to pick up the iron ore
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest chest with ironore in it

        //Chests is an array of all of the objects in the scene which have the ToolChestComponent attatched
        ToolChestComponent[] chests = (ToolChestComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(ToolChestComponent));
        //Set the value for the closest chest to null as the closest chest has not been found yet
        ToolChestComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every chest in chests
        foreach (ToolChestComponent chest in chests)
            //if the chest has more than 0 iron ore in it..
            if (chest.NumIronOre > 0)
            {
                //If the closest chest doesn't have a value yet..
                if (closest == null)
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

                    //the distance to this chest is equal to this chest transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (chest.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this chest is less than the closest to the last chest..
                    if (dist < closestDist)
                    {
                        //.. then we have just found a closer chest! Use this one instead.
                        closest = chest;
                        closestDist = dist;
                    }
                }
            }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any chests with iron ore in, in the scene

            //return false as no chests were found.
            return false;
        }

        //the targetChest is the closest chest
        targetChest = closest;
        //the target is the gameObject of the targetChest
        target = targetChest.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no chests were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {

        //if the target chest has more than 0 iron ore
        if (targetChest.NumIronOre>0)
        {
            //amount of iron taken from the chest.
            int IronTaken;

            if (targetChest.NumIronOre<2)
            {
                IronTaken = targetChest.NumIronOre;
            }
            else
            {
                IronTaken = 2;
            }


            targetChest.NumIronOre -= IronTaken;


            //Set hasIronOre to true as the agent now has ironOre
            hasIronOre = true;

            //add the ironOre to the agents backpack

            //find the agents backpack component
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            backpack.numOre += IronTaken;

            //return true as the action succeeded
            return true;
        }
        //else the chest has 0 iron ore
        else
        {
            //another agent got to it before this agent did!

            //return false as the action failed
            return false;
        }
    }
    #endregion
}
