using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoEat : GOAPAction
{

    #region Setting up variables
    private bool hasAte = false;                          //Agent hasn't eaten yet
    private FridgeComponent targetFridge;             //Where the agent will go to eat

    private float startTime = 0;        //Timer hasn't started
    public float eatingDuration = 5;    //Seconds to go finish eating
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public GoEat()
    {
        addPrecondition("isHungry", true);  //this action will only be called if the agent hasn't eaten
        addEffect("isHungry", false);         //when this action has finished the agent will now have eaten
        addEffect("hasAte", true);
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        startTime = 0;      //reset timer
        hasAte = false;    //sets hasAte back to false
        targetFridge= null;    //deletes the current targetFridge
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasAte to the GOAPAgent, True if the agent has slept, False if the agent hasn't.
        return hasAte;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the fridge in order to eat
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest fridge

        //fridges is an array of all of the objects in the scene which have then FridgeComponent attatched
        FridgeComponent[] fridges = (FridgeComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(FridgeComponent));
        //Set the value for the closest fridge to null as the closest fridge has not been found yet
        FridgeComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every fridge in fridges array
        foreach (FridgeComponent fridge in fridges)
        {
            //if the fridge has more than 0 cooked food in it
            if (fridge.NumCookedFood > 0)
            {
                //If the closest fridge doesn't have a value yet..
                if (closest == null)
                {
                    //..then this is the first fridge in the for loop with more than 0 food, choose it for now
                    closest = fridge;
                    //the distance to the closest fridge is equal to the magnitude (as the crow flies) between the bed transform position and the agents transform position.
                    closestDist = (fridge.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest fridge does have a value..
                else
                {
                    //..Is this fridge closer than the last one?

                    //the distance to this fridge is equal to this fridges transform position - the transform position of the agent in magnitude / as the crow flies.
                    float dist = (fridge.gameObject.transform.position - agent.transform.position).magnitude;

                    //if the distance to this fridge is less than the closest to the last fridge..
                    if (dist < closestDist)
                    {
                        //.. then we have just found a closer fridge! Use this one instead.
                        closest = fridge;
                        closestDist = dist;
                    }
                }
            }
        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any fridges in the scene with more than 0 cooked food

            //return false as no fridges were found.
            return false;
        }

        //the targetFridge is the closest bed
        targetFridge = closest;
        //the target is the gameObject of the targetFridge
        target = targetFridge.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no fridges were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {
        //if the targetFridge still has more than 0 cooked food in it
        if (targetFridge.NumCookedFood > 0)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
            }

            if (Time.time - startTime > eatingDuration)
            {
                targetFridge.NumCookedFood -= 1;
                //Set hasSlept to true as the agent now has slept
                hasAte = true;

                //find the agents hungerTimer component
                HungerTimer hungerTimer = (HungerTimer)agent.GetComponent(typeof(HungerTimer));

                hungerTimer.EatTime = hungerTimer.maxEatTime;

                //return true as the action succeeded
            }
            return true;
        }
        //else the targetFridge has 0 cooked food in it
        else
        {
            //another agent got there first!

            //return false as action failed.
            return false;
        }
    }
    #endregion
}
