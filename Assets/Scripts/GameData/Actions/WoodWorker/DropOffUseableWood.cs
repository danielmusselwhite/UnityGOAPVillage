using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffUseableWood : GOAPAction
{

    #region Setting up variables
    private bool droppedOffWood = false;        //Agent hasn't dropped off wood yet
    private UseableWoodComponent targetWoodPile;     //Where the agent will drop the wood off to

    private float startTime = 0;        //Timer hasn't started
    public float makingDuration = 3;    //Seconds to turn logs into workableWood
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public DropOffUseableWood()
    {
        addPrecondition("hasWorkableWood", true);    //this action will only be called if the agent has wood
        addEffect("hasWorkableWood", false);         //when this action is finished the agent won't have wood anymore
        addEffect("collectWorkableWood", true);      //when this action has finished he will have reached his goal of collecting workablewood!
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        startTime = 0;
        droppedOffWood = false;     //sets droppedOffWood back to false
        targetWoodPile = null;    //deletes the current target wood pile
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns droppedOffWood to the GOAPAgent, True if the agent has dropped off the wood false if the agent hasn't and the action failed
        return droppedOffWood;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the woodpile to drop more wood off into it
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest wood pile

        //woodPiles is an array of all of the objects in the scene which have the UseableWoodPileComponent attatched
        UseableWoodComponent[] woodPiles = (UseableWoodComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(UseableWoodComponent));
        //Set the value for the closest woodPile to null as the closest woodPile has not been found yet
        UseableWoodComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every woodPile in woodPiles
        foreach (UseableWoodComponent woodPile in woodPiles)
        {
            //If the closest woodPile doesn't have a value yet
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
            //.. then we haven't found any woodPiles

            //return false as no woodPiles were found.
            return false;
        }

        //the targetwoodPile is the closest woodPile
        targetWoodPile = closest;
        //the target is the gameObject of the targetwoodPile
        target = targetWoodPile.gameObject;
        
        //return true to the GOAPAgent if closest is not null, return false if no woodPiles were found.
        return closest != null;

    }
    #endregion

    #region perform action
    public override bool perform(GameObject agent)
    {
        //if the timer is 0
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > makingDuration)
        {
            //find the agents backpack
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            targetWoodPile.numUsableWood += backpack.numFirewood;
            droppedOffWood = true;
            backpack.numFirewood = 0;
        }
        //return true as the action succeeded
        return true;
    }
    #endregion
}
