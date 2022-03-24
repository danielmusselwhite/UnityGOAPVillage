using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class PickupUncookedFood : GOAPAction
{

    #region Setting up variables
    private bool hasUncookedFood = false;                           //Agent hasn't picked up UncookedFood yet
    private FridgeComponent targetFridge;                           //Where the agent will get the UncookedFood from
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public PickupUncookedFood()
    {
        addPrecondition("hasUncookedFood", false);  //this action will only be called if the agent hasn't got any uncooked food
        addEffect("hasUncookedFood", true);         //when this action has finished the agent will now have uncooked food
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasUncookedFood = false;    //sets hasUncookedFood back to false
        targetFridge = null;        //deletes the current targetFridge
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasUncookedFood to the GOAPAgent, True if the agent has UncookedFood, False if the agent doesn't.
        return hasUncookedFood;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the fridge to pick up the uncooked food
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest fridge with food in it

        //Fridges is an array of all of the objects in the scene which have the FridgeComponent attatched
        FridgeComponent[] fridges = (FridgeComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(FridgeComponent));
        //Set the value for the closest fridge to null tas the closest fridge has not been found yet
        FridgeComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every fridge in fridges
        foreach (FridgeComponent fridge in fridges)
        {
            //if the fridge has more than 0 uncooked food..
            if (fridge.NumUncookedFood > 0)
            {
                //If the closest fridge doesn't have a value yet..
                if (closest == null)
                {
                    //..then this is the first fridge in the for loop, choose it for now
                    closest = fridge;
                    //the distance to the closest fridge is equal to the magnitude (as the crow flies) between the fridges transform position and the agents transform position.
                    closestDist = (fridge.gameObject.transform.position - agent.transform.position).magnitude;
                }
                //Else the closest fridge does have a value..
                else
                {
                    //..Is this fridge closer than the last one?

                    //the distance to this fridge is equal to this fridge transform position - the transform position of the agent in magnitude / as the crow flies.
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
            //.. then we haven't found any fridges with uncooked food in, in the scene

            //return false as no fridges were found.
            return false;
        }

        //the targetFridge is the closest fridge
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
        //if the targetFridge has more than 0 uncooked food
        if (targetFridge.NumUncookedFood > 0)
        {
            //amount of food taken from the fridge.
            int FoodTaken;

            //if the targetFridge has less than 3 food..
            if(targetFridge.NumUncookedFood <3)
            {
                //..take away all of that food.

                //..FoodTaken is equal to all of the uncooked food in the fridge
                FoodTaken = targetFridge.NumUncookedFood;
            }
            //else the targetFridge has more than or equal to 3 uncooked food in the fridge..
            else
            {
                //..minus five from the Fridge

                //FoodTaken = 3
                FoodTaken = 3;
            }
            //.. minus foodTaken from the uncooked food in the fridges value
            targetFridge.NumUncookedFood -= FoodTaken;


            //Set hasUncookedFood to true as the agent now has uncookedFood
            hasUncookedFood = true;

            //add the uncooked food to the agents backpack

            //find the agents backpack component
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            backpack.numUncookedFood += FoodTaken;

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
