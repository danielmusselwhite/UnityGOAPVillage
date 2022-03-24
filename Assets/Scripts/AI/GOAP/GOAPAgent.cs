using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//understands the state and uses the FSM and GOAPPlanner to operate
public sealed class GOAPAgent : MonoBehaviour {

    #region Predefining variables for the GOAPAgent
    //privates only belong to this instance

    
    //State machine controlling the agents states from FSM
    private FSM stateMachine;     //reference to the FSM script, finate state machine

    //Agents states from FSM and FSMState
    private FSM.FSMState idleState;             //finds something to do
    private FSM.FSMState moveToState;           //moves to a target
    private FSM.FSMState performActionState;    //performs an action

    //Agents available and current actions, from GOAPPlanner
    private HashSet<GOAPAction> availableActions;   //HashSet, graph/leaves of all the agents available actions
    private Queue<GOAPAction> currentActions;       //Queue of the actions the agent needs to do in order to reach its goal with the least cost

    //IGOAP dataProvider, provides world data and listens to feedback on planning to find a successful cheapest plan
    private IGOAP dataProvider; //this is the implementing class that provides our world data and listens to feedback on planning

    //GOAPPlanner which creates the plan for the agent to follow
    private GOAPPlanner planner; //planner which will give the agent its queue of current actions
    #endregion

    #region Intialization of variables within Start function
    //Intializes variables when script instance is enabled
    private void Start()
    {
        //initializing variables
        stateMachine = new FSM();                       //stateMachine variable is assigned to a new instance of the FSM script
        availableActions = new HashSet<GOAPAction>();   //availableActions is assigned as a blank new HashSet inherrited from GOAPActions
        currentActions = new Queue<GOAPAction>();       //currentActions is assigned as a blank new HashSet inherrited from GOAPACtions
        planner = new GOAPPlanner();                    //planner is assigned as a new GOAPPlanner class

        //runs scripts for the agent create their actions and states
        findDataProvider();                     //Finds data about the world
        createIdleState();                      //Creates the agents idle state
        createMoveToState();                    //Creates the agents move to state
        createPerformActionState();             //Creates the agents perform action state
        stateMachine.pushState(idleState);      //Pushes the stateMachines idle state to the front as this will be the agents first initial state
        loadActions();                          //Creates the agents actions
    }
    #endregion

    #region Update called every second
    private void Update()
    {
        /*calls the update function of the FSM for this instance.
         *the gameObject of the agents instance which is running this sript is sent int
         *is used to update the FSMState script with this game object and its current FSM state.*/
        stateMachine.Update(this.gameObject);
    }
    #endregion

    #region addAction - adds actions to the agents available actions
    //inherits GOAPAction script
    public void addAction(GOAPAction action)
    {
        //adds action from GOAPAction script to the agents availableActions
        availableActions.Add(action);
    }
    #endregion

    #region getAction - finds the agents available actions
    //inherits action type
    public GOAPAction getAction(Type action)
    {   
        //foreach GOAPAction (g) in the agents availableActions
        foreach(GOAPAction g in availableActions)
        {
            //if the actions type is the same as the GOAPAction g
            if (action.GetType().Equals(g))
                //return g
                return g;
        }
        return null;
    }
    #endregion

    #region removeAction - removes action from agents available actions
    //inherits GOAPAction class action
    public void removeAction(GOAPAction action)
    {
        //remove the inherited actions from the agents available actions
        availableActions.Remove(action);
    }
    #endregion

    #region hasActionPlan - checks if agent has an action plan
    private bool hasActionPlan()
    {
        //returns true if currentActions is more than 0 returns false if currentActions is 0 or less
        return currentActions.Count > 0;
    }
    #endregion

    //creating agents states

    #region createIdleState - creates agents IdleState
    //in the IdleState the agent is looking for a plan of actions for it to do
    private void createIdleState()
    {
        idleState = (fsm, gameObj) =>
        {

            //get the world state and the goal we want to plan for referencing IGOAP script
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();

            //Plan
            //Plan has the gameObject it will act on, the availableACtions for the agent, the worldState and the agents goal
            Queue<GOAPAction> plan = planner.plan(gameObject, availableActions, worldState, goal);

            //if plan is not null / if plan exists..
            if(plan!=null)
            {
                //.. then we have a plan!

                //the agents currentActions queue is equal to the plan
                currentActions = plan;
                //send the plan into the planFound script in the IGOAP / dataProvider
                dataProvider.planFound(goal, plan);

                //move to the performAction state
                //remove the agents first state (idle state)
                fsm.popState();
                //move to the performAction state
                fsm.pushState(performActionState);
            }

            //else if the plan doesn't exist/ is null..
            else
            {
                //.. we haven't got a plan!
                //let the dataProvider know the plan failed for its goal
                dataProvider.planFailed(goal);
                //remove first state
                fsm.popState();
                //return to idlestate to try again
                fsm.pushState(idleState);
            }
        };
    }
    #endregion

    #region createMovetoState
    //Creates the agents MoveToState which tells the agent to MoveTo its target
    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            //action is equal to the first action in the current actions queue
            GOAPAction action = currentActions.Peek();
            
            //if the action requires the agent to be in range of the target and the target doesn't exists..
            if(action.requiresInRange() && action.target == null)
            {
                //an error has occured. produce an error in the debug log
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none. Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()");
                fsm.popState(); //remove move state
                fsm.popState(); //remove perform state
                fsm.pushState(idleState);   //return to idle state
            }

            //if the IGOAP moveAgent action is true
            if(dataProvider.moveAgent(action))
            {
                //remove the first state from the list
                fsm.popState();
            }

            #region only allows agents with a movable component to move
            /*MovableComponent movable = (MovableComponent) gameObj.GetComponent(typeof(MovableComponent));
			if (movable == null) {
				Debug.Log("<color=red>Fatal error:</color> Trying to move an Agent that doesn't have a MovableComponent. Please give it one.");
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			float step = movable.moveSpeed * Time.deltaTime;
			gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, action.target.transform.position, step);

			if (gameObj.transform.position.Equals(action.target.transform.position) ) {
				// we are at the target location, we are done
				action.setInRange(true);
				fsm.popState();
			}*/
            #endregion
        };

        
    }
    #endregion

    #region createPerformActionState
    //creates the agents performActionState responsible for making the agent perform its actions
    private void createPerformActionState()
    {
        //perform the action
        performActionState = (fsm, gameObj) =>
            {
                //if hasActionPlan is false / the agent doesn't have an action plan..
                if(!hasActionPlan())
                {
                    //.. there are no actions to perform, agent has completed his plan.
                    Debug.Log("<color=red>Done actions</color>");       //write to debug log that the plan has been completed
                    //remove the last action from the FSM
                    fsm.popState();
                    //return the FSM state to idleState
                    fsm.pushState(idleState);
                    //tell the IGOAP that the plan has been completed
                    dataProvider.actionsFinished();
                    return;
                }

                //the action is equal to the action at the top of the currentActions queue
                GOAPAction action = currentActions.Peek();

                //if isDone from GOAPAction class is true..
                if(action.isDone())
                {
                    //.. then the action is done. Remove it so we can perform the next one
                    currentActions.Dequeue();   //..remove the top one from the queue
                }

                //if hasAction plan is true / the agent has an action plan..
                if(hasActionPlan())
                {
                    //.. perform the next action in the plan

                    //action is equal to the first item in the currentActions stack
                    action = currentActions.Peek();
                    //inRange is true if the action requires the agent to be in range of the target and the agent is in range of the target
                    bool inRange = action.requiresInRange() ? action.isInRange() : true;

                    //if in range is true..
                    if(inRange)
                    {
                        //.. then the agent is able to perform the action

                        //success is returned true or false after trying to perform the action using GOAPAction script perform function
                        bool success = action.perform(gameObj);

                        //if success is false..
                        if(!success)
                        {
                            //.. action failed, we need to plan again

                            //remove the action that just failed from the stack
                            fsm.popState();
                            //return stack to idleState
                            fsm.pushState(idleState);
                            //tell the IGOAP the plan failed and what action caused it to fail
                            dataProvider.planAborted(action);
                        }

                    }

                    //else the agent isn't in range..
                    else
                    {
                        //..we need to move there first

                        //push agent to the moveToState
                        fsm.pushState(moveToState);
                    }
                }

                //..else hasAction plan is false..
                else
                {
                    //.. there are no actions left in this plan! Move back to idle/plan state

                    //remove the top state in the stack
                    fsm.popState();
                    //push back to idleState
                    fsm.pushState(idleState);
                    //tell the IGOAP that the plan finishe
                    dataProvider.actionsFinished();
                }
            };
    }
    #endregion

    //Provide information to the GOAPAgent

    #region findDataprovider
        private void findDataProvider()
            {
                //for every component in the gameObjects components with the type of each component
                foreach(Component comp in gameObject.GetComponents(typeof(Component)))
                {
                    //only add assignable components to dataProvider so no errors are caused

                    //if the IGOAP type is assignable from the same type as the component 'comp'..
                    if (typeof(IGOAP).IsAssignableFrom(comp.GetType()))
                    {
                        //.. then add these components into the dataProvider
                        dataProvider = (IGOAP)comp;
                        return;
                    }
                }
            }
    #endregion

    #region loadActions
    private void loadActions()
    {
        //array of GOAPActions is equal to all of the components of the gameObject which are of the class GOAPAction. i.e. chop wood / kill pig 
        GOAPAction[] actions = gameObject.GetComponents<GOAPAction>();

        //for every GOAPAction class in actions GOAPAction[] array..
        foreach(GOAPAction a in actions)
        {
            //..add them to the available actions 
            availableActions.Add(a);
        }
    }
    #endregion

    #region prettyPrint actions
    public static string prettyPrint(HashSet<KeyValuePair<string, object>> state)
    {
        String s = "";
        foreach (KeyValuePair<string, object> kvp in state)
        {
            s += kvp.Key + ":" + kvp.Value.ToString();
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(Queue<GOAPAction> actions)
    {
        String s = "";
        foreach (GOAPAction a in actions)
        {
            s += a.GetType().Name;
            s += "-> ";
        }
        s += "GOAL";
        return s;
    }

    public static string prettyPrint(GOAPAction[] actions)
    {
        String s = "";
        foreach (GOAPAction a in actions)
        {
            s += a.GetType().Name;
            s += ", ";
        }
        return s;
    }

    public static string prettyPrint(GOAPAction action)
    {
        String s = "" + action.GetType().Name;
        return s;
    }
    #endregion
}
