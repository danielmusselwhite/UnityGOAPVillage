using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPAction : MonoBehaviour // Actions that the GOAP agent can perform
{
    #region Setting Up Variables
    //Setting up variables to be used in the action.
    private HashSet<KeyValuePair<string, object>> preconditions;    //Preconditions required for the agents action
    private HashSet<KeyValuePair<string, object>> effects;           //The effect this action will have

    private bool inRange = false;                                   //Used if the agent has to be within a certain range of the target

    /* The cost of performing the action
      Figure out a weight that suits the action chosen.
      Changing it will affect what actions are chosen during planning.
     
      Higher the cost the more the AI wants to do it.*/
    public float cost = 1f;

    //An action often has to perform on a target. This object is the target, can be null if the action doesn't require a target.
    public GameObject target;
    #endregion

    #region Initialising and setting up the GOAPs action upon calling
    //GOAPAction has been called; setting up variables.
    public GOAPAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    //GOAPAction has finished. Reset variables.
    public void doReset()
    {
        inRange = false;
        target = null;
        reset();
    }

    #endregion

    #region Agent Acting on Action

    // abstract procedures used by the agent

    //Reset any variables which need to be reset before planing happens again
    public abstract void reset();

    //Is the action done?
    public abstract bool isDone();

    //Procedurally check if this action can run. Not all actions need this but some might.
    public abstract bool checkProceduralPrecondition(GameObject agent);

    /*Run the action.
     * Returns True if the action performed successful
     * Returns false if something happened and it can no longer perform.
     * If this is the case the action queue should clear out and the goal cannot be reached.*/
    public abstract bool perform(GameObject agent);

    /*Does the action need to be within range of the target game object?
     * If it does then the moveTo state will need to run for this action and hence requriesInRange will be true
     * If not then the moveTo state will not need to run for this action and hencerequiresInRange will be false.*/
    public abstract bool requiresInRange();

    /*Is the agent in range of the target?
     *The MoveTo state will set this and it will be reset each time the action is performed*/
    public bool isInRange()
    {
        return inRange;
    }
    //Set the 'Inrange' bool to the 'inRange' bool of this object
    public void setInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    //Add the precondition key and value
    public void addPrecondition(string key,object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    //Remove the preconditions key and value
    public void removePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string,object>kvp in preconditions)
        {
            if(kvp.Key.Equals(key))
            {
                remove = kvp;
            }
        }
    }

    //Add the effects key and value
    public void addEffect(string key,object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    //Remove the effects key and value
    public void removeEffect(string key, object value)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach(KeyValuePair<string,object>kvp in effects)
        {
            if(kvp.Key.Equals (key))
            {
                remove = kvp;
            }
        }
    }

    //Gets and returns the Preconditions as a HashSet 
    public HashSet<KeyValuePair<string,object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    //Gets and returns the Effects as a HashSet
    public HashSet<KeyValuePair<string,object>>Effects
    {
        get
        {
            return effects;
        }
    }
    #endregion

    /*Stores the preconditions and effects of the agent
     * Knows whether the agent must be in range of the target
     * If the agent needs to be in range of the target it pushs the MoveTo state when its needed
     * Also knowing when it is done, determined by implementing the action class.
     */
}
