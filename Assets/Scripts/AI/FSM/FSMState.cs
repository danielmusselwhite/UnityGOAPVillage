using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FSM's current state
public interface FSMState
{
    //Current state of the FSM for the gameObject has been changed (updated)
    void Update(FSM fsm, GameObject gameObject);
} 