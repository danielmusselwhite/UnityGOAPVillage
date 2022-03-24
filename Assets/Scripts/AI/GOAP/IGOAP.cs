using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Collects world data for the agent
 * that will be used for GOAP planning (by the GOAPPlanner).
 * 
 *The interface that our real labourer actor uses
 * Ties into events for GOAP and the FSM*/

/*Any agent that wants to use GOAP must implement this interface.
 * It provides information to the GOAPplanner so it can plan what actions to use.
 * 
 * It also provides an interface for the planner to give feedback to the Agent and report success/failure.*/
public interface IGOAP
{
    //below are references to procedures and functions in the labourer script

    #region getWorldState
    /*The starting state of the Agent and the world.
     Supply what states are needed for actions to run.*/
    HashSet<KeyValuePair<string, object>> getWorldState();
    #endregion

    #region createGoalState
    /*Gives the planner a new goal so it can figure out which actions are needed to fulfill it to create the plan.*/
    HashSet<KeyValuePair<string, object>> createGoalState();
    #endregion

    #region planFailed
    /*No sequence of actions could be found for the supplied goal.
     *Agent will need to try another goal.*/
     void planFailed(HashSet<KeyValuePair<string,object>>failedGoal);
    #endregion

    #region planFound
    /*A plan was fuound for the supplied goal
     These are the actions the agent will need to perform, in order*/
    void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions);
    #endregion

    #region actionsFinished
    /*All actions are complete and the goal was reached!*/
    void actionsFinished();
    #endregion

    #region planAborted
    /*One of the actions couldn't complete and caused the plan to abort.
     *The action is returned.*/
    void planAborted(GOAPAction aborter);
    #endregion

    #region moveAgent
    /*Called during update. 
     *Moves the agent towards the target in order for the next action to be able to perform.
     *Returns true if the Agent is at the target and the next action can perform.
     *Returns false if the Agent is not in the targets range yet.*/
    bool moveAgent(GOAPAction nextAction);
    #endregion
}