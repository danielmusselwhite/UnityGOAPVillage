using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Stack-based Finite State Machine.
 *Push and pop states to the FSM.
 * 
 *States should push other states onto the stack
 *And pop themselves off.*/

public class FSM
{
    //Private FSMState to each gameObject predefined/blank.
    private Stack<FSMState> stateStack = new Stack<FSMState>();

    //Invokes the FSMState script at run time
    public delegate void FSMState(FSM fsm, GameObject gameObject);

    #region Update
    //Update procedure happens every frame of the game, gameObject is passsed into it
    public void Update(GameObject gameObject)
    {
        //if the first/top object of the stack is not equal to null
        //if the stack has something in it..
        if(stateStack.Peek() != null)
        {
            //Invokes/executes the delegate FSMState interfacce with this instance of the FSM and the gameObject it is attatched to
            stateStack.Peek().Invoke(this, gameObject);
        }
    }
    #endregion

    #region pushState
    //Used by GOAPAgent script to push a certain state to the front of the stack.
    //Changes the Agents current active state to the one at the front of the stateStack
    public void pushState(FSMState state)
    {
        //brings the state that is being pushed by the agent to the front of the stateStack.
        //state at the front of the stack is the active state
        stateStack.Push(state);
    }
    #endregion

    #region popState
    /*Used by GOAPAgent script to remove a certain state from the stack
     *
     *Used if state is completed i.e. if the hunter has just finished killing a pig we don't want him in the same state.
     *We want him to do what is next in the stack.
     *
     *Also used if the action fails/ can't be completed we want the agent to do something else*/
    public void popState()
    {
        //Removes top value from the stack
        stateStack.Pop();
    }
    #endregion
}
