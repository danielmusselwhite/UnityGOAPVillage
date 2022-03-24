using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Chef labourer subclass
public class Chef : Labourer
{
    //overrides the labouorers abstract function createGoalState
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {

        //creates the goal variable and makes it blank
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //if HungerTimer isHungry is true and the target fridge has more than 0 food.. (targetFridge only exists if the fridge has more than 0 food so if it is null then there is no fridge with more than 0 food)
        if(GetComponent<HungerTimer>().isHungry && GetComponent<GoEat>().target!=null)
        {
            //agent wants to go eat
            goal.Add(new KeyValuePair<string, object>("hasAte", true));
        }

        //if SleepTimer isTired is true and the targetbed is free..
        if (GetComponent<SleepTimer>().isTired && GetComponent<GoSleep>().target!=null)
        {
            //agent wants to sleep
            goal.Add(new KeyValuePair<string, object>("hasSlept", true));
        }

        //else
        else
        {
            //agent wants to collect cooked food
            goal.Add(new KeyValuePair<string, object>("collectCookedFood", true));
        }


        return goal;
    }

}

