using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffLogs : GOAPAction
{

    #region Setting up variables
    private bool droppedOffLogs = false;        //Agent hasn't dropped off logs yet
    private LogPileComponent targetLogPile;     //Where the agent will drop the logs off to
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public DropOffLogs()
    {
        addPrecondition("hasLogs", true);    //this action will only be called if the agent has logs
        addEffect("hasLogs", false);         //when this action is finished the agent won't have logs anymore
        addEffect("collectLogs", true);      //when this action has finished he will have reached his goal of collecting logs!
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        droppedOffLogs = false;     //sets droppedOffLogs back to false
        targetLogPile = null;    //deletes the current targetLogPile
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns droppedOffLogs to the GOAPAgent, True if the agent has dropped off the ore false if the agent hasn't and the action failed
        return droppedOffLogs;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the LogPile to drop more logs off into it
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest log pile

        //logPiles is an array of all of the objects in the scene which have the LogPileComponent attatched
        LogPileComponent[] logPiles = (LogPileComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(LogPileComponent));
        //Set the value for the closest logPile to null as the closest logPile has not been found yet
        LogPileComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every logPile in logPiles
        foreach (LogPileComponent logPile in logPiles)
        {
            //If the closest logPile doesn't have a value yet
            if (closest == null)
            {
                //..then this is the first logPile in the for loop, choose it for now
                closest = logPile;
                //the distance to the closest logPile is equal to the magnitude (as the crow flies) between the logPiles transform position and the agents transform position.
                closestDist = (logPile.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //Else the closest logPile does have a value..
            else
            {
                //..Is this logPile closer than the last one?

                //the distance to this logPile is equal to this logPiles transform position - the transform position of the agent in magnitude / as the crow flies.
                float dist = (logPile.gameObject.transform.position - agent.transform.position).magnitude;

                //if the distance to this logPile is less than the closest to the last logPile..
                if (dist < closestDist)
                {
                    //.. then we have just found a closer logPile! Use this one instead.
                    closest = logPile;
                    closestDist = dist;
                }
            }

        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any logPiles

            //return false as no logPiles were found.
            return false;
        }

        //the targetLogPile is the closest chest
        targetLogPile = closest;
        //the target is the gameObject of the targetLogPile
        target = targetLogPile.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no logPiles were found.
        return closest != null;

    }
    #endregion

    #region perform action
    public override bool perform(GameObject agent)
    {
        //find the agents backpack
        BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
        targetLogPile.numLogs += backpack.numLogs;
        droppedOffLogs = true;
        backpack.numLogs = 0;

        return true;
    }
    #endregion
}
