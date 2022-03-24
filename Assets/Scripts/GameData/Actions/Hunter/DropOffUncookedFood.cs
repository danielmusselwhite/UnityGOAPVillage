using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffUncookedFood : GOAPAction {

    #region Setting up variables and preconditions and effects of the action
    //droppedOffUncookedFood set to false as agent hasn't dropped it off yet
    private bool droppedOffUncookedFood = false;
    //target fridge for where the agent will drop the uncooked food off to
    private FridgeComponent targetFridge;

    public DropOffUncookedFood()
    {
        //can't drop off Uncooked food if we haven't got any
        addPrecondition("hasUncookedFood", true);
        //we now have no uncooked food as we have dropped it off
        addEffect("hasUncookedFood", false);
        //we have collected uncooked food
        addEffect("collectUncookedFood", true);
    }
    #endregion

    #region reset
    public override void reset()
    {
        droppedOffUncookedFood = false;
        targetFridge = null;
    }
    #endregion

    #region isDone
    public override bool isDone()
    {
        return droppedOffUncookedFood;
    }
    #endregion

    #region requiresInRange
    public override bool requiresInRange()
    {
        return true;
    }
    #endregion

    #region checkProceduralPrecondition - find closest fridge
    public override bool checkProceduralPrecondition(GameObject agent)
    {

        //Array of all the fridges in the scene
        FridgeComponent[] fridges = (FridgeComponent[]) UnityEngine.GameObject.FindObjectsOfType(typeof(FridgeComponent));
        //the closest fridge is null as it hasn't been found yet
        FridgeComponent closest = null;
        //distance to the closest fridge also has not been found yet
        float closestDist = 0;

        //for every fridge in fridges
        foreach(FridgeComponent fridge in fridges)
        {
            //if closest doesn't exist..
            if(closest==null)
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
                if(dist<closestDist)
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
        targetFridge.NumUncookedFood += backpack.numUncookedFood;
        droppedOffUncookedFood = true;
        backpack.numUncookedFood = 0;

        return true;
    }
    #endregion
}
