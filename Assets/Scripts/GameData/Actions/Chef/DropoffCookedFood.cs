using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropoffCookedFood : GOAPAction
{

    #region Setting up variables and preconditions and effects of the action
    //droppedOffCCokedFood set to false as agent hasn't dropped it off yet
    private bool droppedOffCookedFood = false;
    //target fridge for where the agent will drop the cooked food off to
    private FridgeComponent targetFridge;

    public DropoffCookedFood()
    {
        //can't drop off cooked food if we haven't got any
        addPrecondition("hasCookedFood", true);
        //we now have no cooked food as we have dropped it off
        addEffect("hasCookedFood", false);
        //we have collected cooked food
        addEffect("collectCookedFood", true);
    }
    #endregion

    #region reset
    //overrides the abstract reset procedure in the labourers class
    public override void reset()
    {
        droppedOffCookedFood = false;
        targetFridge = null;
    }
    #endregion

    #region isDone
    //overrides the abstract isDone function in the labourers class
    public override bool isDone()
    {
        //returns droppedOffCookedFood true if the agent has false if they haven't
        return droppedOffCookedFood;
    }
    #endregion

    #region requiresInRange
    //overrides the abstract requiresInRange function in the labourers class
    public override bool requiresInRange()
    {
        //yes the agent needs to be in range of the fridge to drop food off into it
        return true;
    }
    #endregion

    #region checkProceduralPrecondition - find closest fridge
    //overrides the checkProceduralPrecondition abstract function of the labourer class
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //find the nearest fridge

        //Array of all the fridges in the scene
        FridgeComponent[] fridges = (FridgeComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(FridgeComponent));
        //the closest fridge is null as it hasn't been found yet
        FridgeComponent closest = null;
        //distance to the closest fridge also has not been found yet
        float closestDist = 0;

        //for every fridge in fridges
        foreach (FridgeComponent fridge in fridges)
        {
            //if closest doesn't exist..
            if (closest == null)
            {
                //.. then this is the first fridge, make it the closest
                closest = fridge;
                //distance to the closest fridge as the crow flies
                closestDist = (fridge.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //else closest does exist
            else
            {
                //is this new fridge closer than the last one>
                float dist = (fridge.gameObject.transform.position - agent.transform.position).magnitude;
                //if the distance to this fridge is smaller than the distance to the current closest fridge..
                if (dist < closestDist)
                {
                    //.. then we have just found a closer fridge! use this one instead.
                    closest = fridge;
                    closestDist = dist;
                }
            }
        }
        //if closest doesn't exist
        if (closest == null)
        {
            //return false there is no fridge
            return false;
        }

        //targetFridge is the closest one
        targetFridge = closest;
        // target is the gameObject tied to the targetFridge
        target = targetFridge.gameObject;

        // return true if closest is not equal to null
        return closest != null;
    }
    #endregion

    #region perform action
    public override bool perform(GameObject agent)
    {
        //find the agents backpack
        BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
        //increases the Fridges stock of food by the agents number of CookedFood
        targetFridge.NumCookedFood += backpack.numCookedFood;
        //set droppedOffCookedFood to true as the agent dropped cooked food off
        droppedOffCookedFood = true;
        //set the amount of Cooked food in the backpack to 0
        backpack.numCookedFood = 0;

        return true;
    }
    #endregion
}
