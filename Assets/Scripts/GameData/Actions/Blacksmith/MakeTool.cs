using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PickUpTool GOAPAction used by labourers to pick up a tool when they need one
public class MakeTool : GOAPAction
{

    #region Setting up variables
    private bool hasMadeTool = false;                   //Agent hasn't made the tool yet
    private AnvilComponent targetAnvil;                 //Where the agent will make the tool

    private float startTime = 0;            //Timer hasn't started
    public float smithingDuration = 3;       //Seconds to make the tool
    #endregion

    #region Preconditions and Effects
    //Needs to be named after the class
    public MakeTool()
    {
        addPrecondition("hasOre", true);        //this action will only be called if the agent has iron ore
        addPrecondition("hasWorkableWood", true);   //this action will only be called if the agent has workable wood
        addEffect("hasTools", true);                //when this action has finished the agent will now have made tools
    }
    #endregion

    #region Reset
    //override forces the defined abstract reset, from GOAPAction to do the following
    public override void reset()
    {
        hasMadeTool = false;        //sets hasMadeTool back to false
        targetAnvil = null;         //deletes the current targetCookingPot
        startTime = 0;              //reset timer
    }
    #endregion

    #region isDone?
    //overrides the defined isDone function in GOAPAction 
    public override bool isDone()
    {
        //returns hasMadeTool to the GOAPAgent, True if the agent has Made tool, False if the agent hasn't.
        return hasMadeTool;
    }
    #endregion

    #region requiresInRange?
    //overrides the predefined requiresInRange function in GOAPAction
    public override bool requiresInRange()
    {
        return true; //Yes the agent needs to be in range of the anvil to make the tools
    }
    #endregion

    #region checkProceduralPrecondition
    //overrides predefined checkProceduralPrecondition function in GOAPAction which returns a boolean value
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //we need to find the nearest anvil

        //anvils is an array of all of the objects in the scene which have the component AnvilComponent
        AnvilComponent[] anvils = (AnvilComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(AnvilComponent));
        //Set the value for the closest anvil to null as the closest anvil has not been found yet
        AnvilComponent closest = null;
        //Set the closestDist to 0 as the closest is unknown therefore the distance to it does not exist
        float closestDist = 0;

        //iterate the code once for every anvil in anvils
        foreach (AnvilComponent anvil in anvils)
        {
            //If the closest anvil doesn't have a value yet..
            if (closest == null)
            {
                //..then this is the first anvil in the for loop, choose it for now
                closest = anvil;
                //the distance to the closest anvil is equal to the magnitude (as the crow flies) between the anvils transform position and the agents transform position.
                closestDist = (anvil.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //Else the closest anvil does have a value..
            else
            {
                //..Is this anvil closer than the last one?

                //the distance to this anvil is equal to this anvils transform position - the transform position of the agent in magnitude / as the crow flies.
                float dist = (anvil.gameObject.transform.position - agent.transform.position).magnitude;

                //if the distance to this anvil is less than the closest to the last anvil..
                if (dist < closestDist)
                {
                    //.. then we have just found a closer anvil! Use this one instead.
                    closest = anvil;
                    closestDist = dist;
                }
            }

        }

        //if closest is null / doesn't exist..
        if (closest == null)
        {
            //.. then we haven't found any anvils

            //return false as no anvils were found.
            return false;
        }

        //the targetAnvil is the closest anvil
        targetAnvil = closest;
        //the target is the gameObject of the targetAnvil
        target = targetAnvil.gameObject;

        //return true to the GOAPAgent if closest is not null, return false if no anvils were found.
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

        if (Time.time - startTime > smithingDuration)
        {

            //loads the backpack of the agent up ready to be used
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));

            int Test = backpack.numOre - backpack.numFirewood;

            if(Test==0)
            {
                //then we have the same amount of firewood as ore

                //set firewood and ore to 0 and increase the number of tools by the
                backpack.numOfTools += backpack.numFirewood;
                backpack.numFirewood = 0;
                backpack.numOre = 0;
            }
            if(Test>0)
            {
                //then we have more firewood than ore

                //so decrease firewood by ore and increase tools by ore
                backpack.numOfTools += backpack.numOre;
                backpack.numFirewood -= backpack.numOre;
                backpack.numOre = 0;
            }
            if(Test<0)
            {
                //then we have more ore than firewood

                //so decrease ore by firewood and increase tools by firewood
                backpack.numOfTools += backpack.numFirewood;
                backpack.numOre -= backpack.numFirewood;
                backpack.numFirewood = 0;
            }


                //Set hasMadeTools to true as the agent now has made tools
                hasMadeTool = true;


        }
            //return true as the action succeeded
            return true;
     
    }
    #endregion
}
