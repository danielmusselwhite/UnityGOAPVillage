using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class PickupWorkableWood : GOAPAction
{

    #region Setting up variables
    private bool hasWorkableWood = false;               //Agent hasn't picked it up yet
    private UseableWoodComponent targetWood;             //Where the agent will pick it up from
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public PickupWorkableWood()
    {
        addPrecondition("hasWorkableWood", false);  //this action will only be called if the agent hasn't got any workable wood
        addEffect("hasWorkableWood", true);         //when this action has finished the agent will now have workable wood
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasWorkableWood = false;     //sets hasWorkableWood back to false
        targetWood = null;           //deletes the current wood pile
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasWorkableWood to the GOAPAgent, True if the agent has workable wood, False if the agent doesn't.
        return hasWorkableWood;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the wood pile to pick up the wood
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest woodpile with wood in it

        //woodPiles is an array of all of the objects in the scene which have the UseableWoodPileComponent attatched
        UseableWoodComponent[] woodPiles = (UseableWoodComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(UseableWoodComponent));
        //Set the value for the closest woodPile to null as the closest woodPile has not been found yet
        UseableWoodComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every chest in chests
        foreach (UseableWoodComponent woodPile in woodPiles)
            //if the woodPile has more than 0 wood in it..
            if (woodPile.numUsableWood > 0)
            {
                //If the closest woodPile doesn't have a value yet..
                if (closest == null)
                {
                    //..then this is the first woodPile in the for loop, choose it for now
                    closest = woodPile;
                    //the distance to the closest woodPile is equal to the magnitude (as the crow flies) between the woodPiles transform position and the agents transform position.
                    closestDist = (woodPile.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest woodPile does have a value..
                else
                {
                    //..Is this woodPile closer than the last one?

                    //the distance to this woodPile is equal to this woodPiles transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (woodPile.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this woodPile is less than the closest to the last woodPile..
                    if (dist < closestDist)
                    {
                        //.. then we have just found a closer woodPile! Use this one instead.
                        closest = woodPile;
                        closestDist = dist;
                    }
                }
            }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any woodPiles with wood in, in the scene

            //return false as no woodPiles were found.
            return false;
        }

        //the targetWood is the closest woodPile
        targetWood = closest;
        //the target is the gameObject of the targetWood
        target = targetWood.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no woodpiles were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {

        //if the target woodPile has more than 0 wood
        if (targetWood.numUsableWood > 0)
        {
            //amount of wood taken from the woodPile.
            int WoodTaken;

            if (targetWood.numUsableWood < 2)
            {
                WoodTaken = targetWood.numUsableWood;
            }
            else
            {
                WoodTaken = 2;
            }


            targetWood.numUsableWood -= WoodTaken;


            //Set hasWorkableWood to true as the agent now has wood
            hasWorkableWood = true;

            //add the wood to the agents backpack

            //find the agents backpack component
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            backpack.numFirewood+=WoodTaken;

            //return true as the action succeeded
            return true;
        }
        //else the wood pile has 0 wood
        else
        {
            //another agent got to it before this agent did!

            //return false as the action failed
            return false;
        }
    }
    #endregion
}
