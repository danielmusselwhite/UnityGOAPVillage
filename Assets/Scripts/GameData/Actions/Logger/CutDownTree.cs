using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutDownTree : GOAPAction
{
    #region Setting up variables
    private bool cutDown = false;           //Hasn't cutdown the tree yet
    private TreeComponent targetTree;              //where the agent will get the logs from

    private float startTime = 0;        //Timer hasn't started
    public float cuttingDownDuration = 1;    //Seconds to cutdown the tree
    #endregion

    #region Preconditions and effects
    //needs to be named same as the class so GOAP can read it
    public CutDownTree()
    {
        addPrecondition("hasTool", true);   //Agent needs a tool to cut down the tree
        addPrecondition("hasLogs", false);  //If agent has already gathered logs it doesn't want more
        addEffect("hasLogs", true);         //Upon cutting down the tree, the agent will now have logs
    }
    #endregion

    #region reset
    //When the action has been completed and variables need to be reset

    //overrides the abstract procedure reset from the labourer class
    public override void reset()
    {
        cutDown = false;      //Change cutDown back to false
        targetTree = null;  //Change targetTree back to null
        startTime = 0;      //Reset timer back to 0
    }
    #endregion

    #region isDone
    //overrides the abstract function isDone from the labourer class
    public override bool isDone()
    {
        //return cutDown, true if action succeeded, false if it failed.
        return cutDown;
    }
    #endregion

    #region requiresInRange
    //overrides the abstract function requiresInRange from the labourer class
    public override bool requiresInRange()
    {
        return true;    //Yes the logger needs to be near the tree
    }
    #endregion

    #region checkProceduralPreconditions
    //overrides the abstract function checkProceduralPrecondition from the labourer class
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        //Find the nearest tree the logger can cutdown

        //creates an array of all of the objects in the seen with the tree component
        TreeComponent[] trees = FindObjectsOfType(typeof(TreeComponent)) as TreeComponent[];
        //Predefines the closest tree as null as the closest has not been found yet
        TreeComponent closest = null;
        //as the closest tree is not know yet the distance to it does not exist at this current time
        float closestDist = 0;

        //for every tree in the trees array
        foreach (TreeComponent tree in trees)
        {
            //..if the closest tree is null..
            if (closest == null)
            {
                //..this is the first tree so choose it for now
                closest = tree;
                //Distance to the closest tree as the crow flies
                closestDist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
            }
            //..else the closet tree isn't null
            else
            {
                //.. is this tree closer than the last one?
                float dist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
                //If the distance to this new tree is small than the distance to the 'closest' tree..
                if (dist < closestDist)
                {
                    //we have just found a closer tree, use this one instead
                    closest = tree;
                    closestDist = dist;
                }
            }
        }
        //the agent wants to go towards the closest tree
        targetTree = closest;
        target = targetTree.gameObject;

        //returns true if the agent now has a target returns false if the agent doesn't.
        return closest != null;
    }
    #endregion

    #region perform
    //overrides the abstract perform procedure in the labourer class
    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > cuttingDownDuration)
        {
            //Has finished cutting down the tree

            //reference to the agents backpackComponent
            BackpackComponent backpack = (BackpackComponent)agent.GetComponent(typeof(BackpackComponent));
            //increase the agents number of logs
            backpack.numLogs += 5;
            cutDown = true;

            //reference to the agents tool
            ToolComponent tool = backpack.tool.GetComponent(typeof(ToolComponent)) as ToolComponent;

            //active the toolComponents use procedure, which will decrease its strength by the value inputted (0.34)
            tool.use(0.34f);
            //if tool destroyed returns true (if the tool has less than 0 strength)
            if (tool.destroyed())
            {
                //destroy that game object
                Destroy(backpack.tool);
                //set backpack tool back to null
                backpack.tool = null;
            }
        }
        //return true
        return true;
    }
    #endregion
}
