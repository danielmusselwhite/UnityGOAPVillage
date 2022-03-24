using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class CookUncookedFood : GOAPAction
{

    #region Setting up variables
    private bool hasCookedFood = false;                             //Agent hasn't cooked the uncooked food yet
    private CookingPotComponent targetCookingPot;                   //Where the agent will cook the food

    private float startTime = 0;        //Timer hasn't started
    public float cookingDuration = 3;    //Seconds to cook the food
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public CookUncookedFood()
    {
        addPrecondition("hasUncookedFood", true);   //this action will only be called if the agent has uncooked food, to be cooked
        addEffect("hasCookedFood", true);           //when this action has finished the agent will now have cooked food
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasCookedFood = false;      //sets hasCookedFood back to false
        targetCookingPot = null;    //deletes the current targetCookingPot
        startTime = 0;              //reset timer
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasCookedFood to the GOAPAgent, True if the agent has CookedFood, False if the agent doesn't.
        return hasCookedFood;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the cooking pot to cook the food
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest cookingPot

        //CookingPots is an array of all of the objects in the scene which have the component CookingPotComponent
        CookingPotComponent[] cookingPots = (CookingPotComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(CookingPotComponent));
        //Set the value for the closest cookingPot to null tas the closest cooking pot has not been found yet
        CookingPotComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every cookingPot in cookingPots
        foreach (CookingPotComponent cookingPot in cookingPots)
        {
            //If the closest cookingPot doesn't have a value yet..
            if (closest == null)
            {
                //..then this is the first cookingPot in the for loop, choose it for now
                closest = cookingPot;
                //the distance to the closest cookingPot is equal to the magnitude (as the crow flies) between the cookingPots transform position and the agents transform position.
                closestDist = (cookingPot.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //Else the closest cookingPot does have a value..
            else
            {
                //..Is this cookingPot closer than the last one?

                //the distance to this cookingPot is equal to this cookingPots transform position - the transform position of the agent in magnitude / as the crow flies.
                float dist = (cookingPot.gameObject.transform.position - agent.transform.position).magnitude;

                //if the distance to this cookingPot is less than the closest to the last cookingPot..
                if (dist < closestDist)
                {
                    //.. then we have just found a closer cookingPot! Use this one instead.
                    closest = cookingPot;
                    closestDist = dist;
                }
            }
     
        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any cookingPots

            //return false as no cookingPots were found.
            return false;
        }

        //the targegtCookingPot is the closest cookingPot
        targetCookingPot = closest;
        //the target is the gameObject of the targetCookingPot
        target = targetCookingPot.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no cookingPots were found.
        return closest != null;

    }
    #endregion

    #region perform
    //overrides the perform function in the GOAPAction script. Returns a boolean value.
    public override bool perform(GameObject agent)
    {
        //if the timer is 0
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > cookingDuration)
        {
            //predefines the variable which be used as the middle man when cooking food
            int FoodCooked = 0;
            //loads the backpack of the agent up ready to be used
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            //The amount of food to be cooked will be equal to the amount of uncooked food the agent has
            FoodCooked = backpack.numUncookedFood;
            //the backpacks numUncookedFood is decreased by the amount of food which will be cooked (uncooked food will be 0)
            backpack.numUncookedFood -= FoodCooked;
            //backpacks cooked food is increased by the amount of uncooked food there was in it.
            backpack.numCookedFood += FoodCooked;


            //Set hasUCookedFood to true as the agent now has CookedFood
            hasCookedFood = true;

        }
        //return true as the action succeeded
        return true;
    }
    #endregion
}
