using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A general labourer class.
 *Should subclass this for specific Labourer classes 
 *and implement the createGoalState() method that will populate the goal for the GOAP planner*/
public abstract class Labourer : MonoBehaviour, IGOAP {
    
    //Predefining variables
    #region predefining variables
    //reference to the BackpackComponent
    public BackpackComponent backpack;
    //reference to SleepTimer component
    public SleepTimer sleepTimer;
    //reference to HungerTimer component
    public HungerTimer hungerTimer;
    //Setting agents movespeed
    public float moveSpeed = 1;

    //ToolRotation on back
    Quaternion toolRotation = Quaternion.Euler(0,0,-45);
    #endregion

    #region Initializing variables - used for equiping tool if relevant
    //Initializing variables
    void Start ()
    {
        //if the agents backpack doesn't exist..
		if(backpack==null)
        {
            //..add a backpackComponent to the agent
            backpack = gameObject.AddComponent<BackpackComponent>() as BackpackComponent;
        }
        //if the agent can have a tool..
        if (backpack.toolType != "null")
        {
            //if backpack.tool is null / the agent doesn't have a tool in its backpack
            if (backpack.tool == null)
            {
                //the tool prefab is equal to the GameObject resource with the same name as the toolType in the agents backpack
                GameObject prefab = Resources.Load<GameObject>(backpack.toolType);
                //Creates the tools real world object
                GameObject tool = Instantiate(prefab, transform.position, transform.rotation * toolRotation) as GameObject;
                //gives the backpacks tool the GameObject instance of the tool we just created
                backpack.tool = tool;
                //attatches the tool onto the agent
                tool.transform.parent = transform;
            }
        }
	}
    #endregion

    //Below are the procedures which are used by the IGOAP script
    #region getWorldState - gets information about the world around the agent.
    //getWorldState function which returns a HashSet of KVP's for the worldData - all of the agents preconditions are kept in here
    public HashSet<KeyValuePair<string,object>>getWorldState()
    {
        //Predefines a blank new HashSet KVP called worldData
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        //Adds KVPs to the worldData for the agent with their backpack information

        //the amount of uncooked food the agent has if it has more than 0
        worldData.Add(new KeyValuePair<string, object>("hasUncookedFood", (backpack.numOre > 0)));
        
        //the amount of cooked food the agent has if it has more than 0
        worldData.Add(new KeyValuePair<string, object>("hasCookedFood", (backpack.numCookedFood > 0)));
        
        //the amount of ore the agent has if it has more than 0
        worldData.Add(new KeyValuePair<string, object>("hasOre", (backpack.numOre > 0)));
        
        //the amount of logs the agent has if it has more than 0
        worldData.Add(new KeyValuePair<string, object>("hasLogs", (backpack.numLogs > 0)));
        
        //the amount of workableWood the agent has if it has more than 0
        worldData.Add(new KeyValuePair<string, object>("hasWorkableWood", (backpack.numFirewood > 0)));
        
        //if the agent has a GameObject called tool (tool isn't null) add this to worldData
        worldData.Add(new KeyValuePair<string, object>("hasTool", (backpack.tool!=null)));

        worldData.Add(new KeyValuePair<string, object>("isTired", (sleepTimer.isTired)));

        worldData.Add(new KeyValuePair<string, object>("isHungry", (hungerTimer.isHungry)));

        worldData.Add(new KeyValuePair<string, object>("hasTools", (backpack.numOfTools > 0)));
        return worldData;
    }
    #endregion

    //Below are defined but implemented in the subclasses i.e. hunter (why code is blank)
    #region createGoalState
    //creates the goal state for the IGOAP
    public abstract HashSet<KeyValuePair<string, object>> createGoalState();
    #endregion

    #region planFailed
    public void planFailed(HashSet<KeyValuePair<string,object>> failedGoal)
    {
        /*Not handling this here since we are making sure our goals will always succeed.
         *But normally you want to make sure the world state has changed before running
         *The same goal again or else it will just fail.*/
    }
    #endregion

    #region planFound
    //plan has been found! Pass in the goal and the queue of actions required to reach that goal.
    public void planFound(HashSet<KeyValuePair<string,object>> goal, Queue<GOAPAction> actions)
    {
        Debug.Log("<color=green>Plan found</color> " + GOAPAgent.prettyPrint(actions));
    }
    #endregion

    #region actionsFinished
    //everything in the queue is done, goals have been completed for this action to reach the goal!
    public void actionsFinished()
    {
        Debug.Log("<color=blue>Actions completed</color>");
    }
    #endregion

    #region planAborted
    public void planAborted(GOAPAction aborter)
    {
        /*An action bailed out of the plan. State has been reset to make a new plan again.
         *Take note of what happened and make sure if you run the same goal that it can succeed*/
        Debug.Log("<color=red>Plan Aborted</color> " + GOAPAgent.prettyPrint(aborter));
    }
    #endregion

    #region moveAgent
    public bool moveAgent(GOAPAction nextAction)
    {
        Debug.Log("<color=green>MoveAgent</color> ");
        //move towards the nextActions target

        //value difference for each 'step' (movement per time frame)
        float step = moveSpeed * Time.deltaTime;
        //Moves the agent towards the target by the step value
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position,step);

        //if the agents position is equal to the targets position
        if(gameObject.transform.position.Equals(nextAction.target.transform.position))
        {
            //then the agent is in range of the target so set nextAction setInRange value to true
            nextAction.setInRange(true);
            //return true
            return true;
        }
        else
        {
            //else return false and script is called upon again
            return false;
        }
    }
    #endregion
}
