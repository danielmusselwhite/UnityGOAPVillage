using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class MakeUseableWood : GOAPAction
{

    #region Setting up variables
    private bool hasWorkableWood = false;                           //Agent hasn't made workable wood yet
    private LogPileComponent targetLogPile;                           //Where the agent will get the logs from
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public MakeUseableWood()
    {
        addPrecondition("hasWorkableWood", false);  //this action will only be called if the agent hasn't got any workable wood
        addEffect("hasWorkableWood", true);         //when this action has finished the agent will now have workable wood
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasWorkableWood = false;    //sets hasWorkableWood back to false
        targetLogPile = null;        //deletes the current targetLogPile
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasWorkableWood to the GOAPAgent, True if the agent has has workable wood, False if the agent doesn't.
        return hasWorkableWood;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the logpile to pick up the logs
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest logpile with logs in it

        //logs is an array of all of the objects in the scene which have the logpile component attatched
        LogPileComponent[] logs = (LogPileComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(LogPileComponent));
        //Set the value for the closest log to null as the closest log has not been found yet
        LogPileComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every log in logs
        foreach (LogPileComponent log in logs)
        {
            //if the log has more than 0 logs in it
            if (log.numLogs > 0)
            {
                //If the closest logpile doesn't have a value yet..
                if (closest == null)
                {
                    //..then this is the first logpile in the for loop, choose it for now
                    closest = log;
                    //the distance to the closest log is equal to the magnitude (as the crow flies) between the logs transform position and the agents transform position.
                    closestDist = (log.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest log does have a value..
                else
                {
                    //..Is this log closer than the last one?

                    //the distance to this log is equal to this log transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (log.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this log is less than the closest to the last log..
                    if (dist < closestDist)
                    {
                        //.. then we have just found a closer logpile! Use this one instead.
                        closest = log;
                        closestDist = dist;
                    }
                }
            }
        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any logpiles with logs in, in the scene

            //return false as no logs were found.
            return false;
        }

        //the targetLogPile is the closest logPile
        targetLogPile = closest;
        //the target is the gameObject of the targetLogPile
        target = targetLogPile.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no logpiles were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {
        //if the tagetLogPile has more than 0 logs
        if (targetLogPile.numLogs > 0)
        {
            //amount of logs taken from the log pile
            int LogsTaken;

            //if the targetLogPile has less than 3 logs..
            if (targetLogPile.numLogs < 3)
            {
                //..take away all of that logs.

                //..logsTaken is equal to all of the logs in the logpile
                LogsTaken = targetLogPile.numLogs;
            }
            //else the targetLogPile has more than or equal to 3 logs
            else
            {
                //..minus 3 from the Fridge

                LogsTaken = 3;
            }
            //.. minus foodTaken from the logs in the target logpile
            targetLogPile.numLogs -= LogsTaken;


            //Set hasWorkableWood to true as the agent now has workable wood
            hasWorkableWood = true;

            //add the uncooked food to the agents backpack

            //find the agents backpack component
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            backpack.numFirewood += LogsTaken*3;

            //return true as the action succeeded
            return true;
        }
        //else the fridge has 0 uncooked food
        else
        {
            //another agent got to it before this agent did!

            //return false as the action failed
            return false;
        }
    }
    #endregion
}
