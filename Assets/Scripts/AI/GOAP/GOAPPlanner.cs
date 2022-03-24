using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//plans what actions the GOAP agent can complete to fulfill a goal state

public class GOAPPlanner
{
    /*Plan out sequence of actions which can fulfill the goal.
     *Returns null if a plan could not be found,
     *Or returns a list of actions that must be performered in order to fulfill the goal.*/

    // plan function, returns Queue<GOAPAction> variable with the value of 'queue' in this function. Passes in variables in brackets.
    public Queue<GOAPAction> plan(GameObject agent, HashSet<GOAPAction>availableActions, HashSet<KeyValuePair<string,object>>worldState, HashSet<KeyValuePair<string,object>>goal)
    {
        #region Reset available actions
        //Reset the actions so plan will start fresh each time
        foreach(GOAPAction action in availableActions)
        {
            //Call doReset function from the GOAPAction script foreach action
            action.doReset();
        }
        #endregion

        #region Discover usableActions
        //Check what actions can run using their checkProceduralPrecondition (from GOAPAction abstract bool)

        //Predefine the usableActions HashSet before assigning variables to it.
        HashSet<GOAPAction> usableActions = new HashSet<GOAPAction>();

        //For each GOAPAction which is in available actions
        foreach(GOAPAction action in availableActions)
        {
            //If this actions procedural precondition for the agent is met
            if(action.checkProceduralPrecondition(agent))
            {
                //add this to the agents usable actions
                usableActions.Add(action);
            }
        }
        //All of the agents actions that can run are now stored in usableActions.
        #endregion

        #region Build tree
        //Build up the tree and record the leaf nodes that provide a solution to the goal.
        List<Node> leaves = new List<Node>();
        #endregion

        #region Build graph
        //start of graph, think abstractly as it being the first line / the titles of each below
        Node start = new Node(null, 0, worldState, null);
        //Build a graph of the actions that meet all of the criteria. 
        bool success = buildGraph(start, leaves, usableActions, goal);
        #endregion

        #region Has a successful path been found?
        //if success isn't true / we didn't find a successful path
        if(!success)
        {
            Debug.Log("NO PLAN");
            //return null as no viable plan has been found yet to be tried again in the next iteration
            return null;
        }
        #endregion

        #region Find cheapest leaf (/path)
        //Get the cheapest 'leaf' from the 'tree'

        //Predefine 'cheapest' Node
        Node cheapest = null;

        //foreach Node type (called leaf for use in this loop) in leaves List<Node> (list of Nodes, see build tree)
        foreach(Node leaf in leaves)
        {
            //if cheapest hasn't been defined yet..
            if (cheapest == null)
            {
                //.. then this is the first in the for loop and as thus is the first cheapest to be tested against
                //so cheapest is the current leaf at the moment
                cheapest = leaf;
            }
            //else if cheapest has a value..
            else
            {
                //..if this leaf has a lower running cost than the current cheapest values running cost..
                if (leaf.runningCost < cheapest.runningCost)
                {
                    //..then we have just found a cheaper leaf so the cheapest is now equal to this leaf
                    cheapest = leaf;
                }
            }
        }
        #endregion

        #region Get its node and work back through its parents
        //predefine new List of GOAPActions called result 
        List<GOAPAction> result = new List<GOAPAction>();
        //Node n is equal to the cheapest leaf
        Node n = cheapest;

        //while n is not equal to null / it exists
        while(n!=null)
        {
            //if n's action is not equal to null..
            if(n.action!=null)
            {
                //insert n's action into the front of result
                result.Insert(0, n.action);
            }
            /*n is now equal to its parent (or null if it doesn't have a parent)
             *returns to start of loop with new value of n*/
            n = n.parent;
        }
        #endregion

        #region Now have action list in correct order
        //Predefine a new Queue to be used by the GOAPAction
        Queue<GOAPAction> queue = new Queue<GOAPAction>();

        //foreach GOAPAction 'a' in result List of GOAPActions
        foreach(GOAPAction a in result)
        {
            //add GOAPAction to the back of the queue (queueing behind the first, first in first out)
            queue.Enqueue(a);
        }
        #endregion

        //we have a plan!
        //return it
        return queue;
    }

    #region buildGraph class for building the graph
    /*Returns true if at least one solution was found.
     *The possible paths are stored in the leaves list.
     * Each leaf has a 'runningCost' value.
     * Whichever 'tree' of leaves with the lowest cost will be the best action sequence.*/
    private bool buildGraph(Node parent, List<Node> leaves, HashSet<GOAPAction> usableActions, HashSet<KeyValuePair<string,object>>goal)
    {
        //defining foundOne as false as a possible path has not been found yet.
        bool foundOne = false;

        //G=Go through each action avaialble at this node and see if it can be used in this situation
        foreach(GOAPAction action in usableActions)
        {
            //If the parents state has the conditions for this actions preconditions then it can be used here.
            if(inState(action.Preconditions,parent.state))
            {
                //Apply the action's effects to the parent state
                HashSet<KeyValuePair<string, object>> currentState = populateState(parent.state, action.Effects);
                Debug.Log(GOAPAgent.prettyPrint(currentState));

                //new node in the tree with the parent/agent, the parent/agents runningCost + the actions cost, the agents currentState and the action itself
                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

                //if inState goal and currentState are the same then..
                if(inState(goal,currentState))
                {
                    //.. we found a solution!
                   
                    //add this node to the leaves
                    leaves.Add(node);
                    //change found one to true
                    foundOne = true;
                }
                //..else  inState goal and currentState aren't the same then..
                else
                {
                    //.. we haven't found a solution yet, so test all the remaining actions and branch out the tree

                    //find a larger subset of actions from the subset
                    HashSet<GOAPAction> subset = actionSubset(usableActions, action);
                    //found returns true if they can build a graph using the subset
                    bool found = buildGraph(node, leaves, subset, goal);
                    
                    //.. if a solution has been found this way then..
                    if(found)
                    {
                        //.. foundOne is true!
                        foundOne = true;
                    }
                }
            }
        }
        return foundOne;
    }
    #endregion

    #region actionSubset class for creating the part of the table listing actions that are attatched to the agent
    //         subset - a part of a larger group of related things. In this instance its a part of the table, the list of all actions the agent can do.
    //Create a subset of actions excluding the removeMe one. Creates a new set.
    //Passes variables in, in brackets.
    private HashSet<GOAPAction>actionSubset(HashSet<GOAPAction>actions,GOAPAction removeMe)
    {
        //Predefines new HashSet called subset
        HashSet<GOAPAction> subset = new HashSet<GOAPAction>();

        //Foreach action in actions 
        foreach(GOAPAction action in actions)
        {
            //If the action is not removeMe
            if(!action.Equals(removeMe))
            {
                //add it to the list of possible actions.
                subset.Add(action);
            }
        }
        return subset;
    }
    #endregion

    #region inState class for checking if all items are in state
    /*Check that all items in the 'test' are in 'state'.
     *If just one of them does not match or is not there then this returns false.
     *
     *test refers to preconditions, state refers to current state.
     *test and state HashSets are passed into the function when it is called*/
    private bool inState(HashSet<KeyValuePair<string,object>> test, HashSet<KeyValuePair<string,object>>state)
    {
        //Preset allMatch variable to z
        bool allMatch = true;

        //for every KeyValuePair in the test HashSet  (preconditions)..
        foreach(KeyValuePair<string,object> t in test)
        {
            //..Preset match variable to false for every 't'
            bool match = false;

            //for every KeyValuePair in the 'state' HashSet (current state)..
            foreach(KeyValuePair<string,object>s in state)
            {
                //..If 's' is equal to 't' (if the current state is equal to the precondition)..
                if(s.Equals(t))
                {
                    //.. then it matches, so match becomes true
                    match = true;
                    break;
                }
            }
            //But if one of the preconditions doesn't match the state..
            if(!match)
            {
                //.. then not all of them match
                allMatch = false;
            }
        }
        //return the allMatch value
        return allMatch;
    }
    #endregion

    #region populateState for applying the stateChange to the current state
    //Apply the stateChange to the currentState
    //HashSet<KeyValuePair<string,object>> function called populate state with the HashSets, currentState and stateChange passed in
    private HashSet<KeyValuePair<string,object>> populateState(HashSet<KeyValuePair<string,object>> currentState, HashSet<KeyValuePair<string,object>> stateChange)
    {
        //Predefines new HashSet called state before a value has been assigned to it
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();


        /*copy the KVPs over as new objects
         *foreach KeyValuePair (s) in currentState HashSet..*/
        foreach (KeyValuePair<string,object> s in currentState)
        {
            //Adds the key and value of each 's' into the state variable we had predefined.
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }


        /*If the key exists in the current state we want to change its value.
         *Foreach KeyValuePair in the HashSet stateChange.*/
        foreach(KeyValuePair<string,object>change in stateChange)
        {
            //Presets 'exists' bool to false as we haven't found the key in the current state yet
            bool exists = false;

            //foreach KeyValuePair (s) in the state HashSet
            foreach (KeyValuePair<string,object> s in state)
            {
                //if 's' is equal to 'change'..
                if(s.Equals(change))
                {
                    //.. then this key in stateChange is also in currentState, so exists is true
                    exists = true;
                    //break the for loop for s in state to return to the for loop for change in statechange
                    break;
                }
            }

            /*Update the state for the key if it exists in current state already
             *if exists is true/ this 'change' value is also in the currentState HashSet..*/
            if(exists)
            {
                //..remove the Key and its value from 'state' where the key is equal to 'change'
                state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });
                //define new KeyValuePair called updated with the values of change.Key and change.Value
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                //add this updated state to state HashSet.
                state.Add(updated);
            }

            //else if it doesn't exist in the current state then we want to add it.
             else
            {
                //Add to the state HashSet a new key value pair with the values of the state change.key and change.value
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }
    #endregion

    #region Node class for building tree

    //Used for building up the graph and holding the running costs of actions

    private class Node
    {
        //Predefining variables
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public GOAPAction action;

        //If parent, runningCost, state and action all exist..
        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string,object>> state, GOAPAction action)
        {
            //assign them to 'this' instance versions of those variables.
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }

    }

    #endregion
}