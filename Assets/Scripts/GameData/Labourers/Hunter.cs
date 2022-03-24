using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hunter : Labourer
{
    //The hunters only goal will be to kill pigs and give increase the villages supply of uncooked food.
    //The KillPig option will fulfill this goal.

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //if HungerTimer isHungry is true and the target fridge has more than 0 food.. (targetFridge only exists if the fridge has more than 0 food so if it is null then there is no fridge with more than 0 food)
        if(GetComponent<HungerTimer>().isHungry && GetComponent<GoEat>().target!=null)
        {
            //agent wants to go eat
            goal.Add(new KeyValuePair<string, object>("hasAte", true));
        }

        //if SleepTimer isTired is true..
        if (GetComponent<SleepTimer>().isTired && GetComponent<GoSleep>().target != null )
        {
            //agent wants to sleep
            goal.Add(new KeyValuePair<string, object>("hasSlept", true));
        }

        //else
        else
        {
            //agent wants to collect uncooked food.
            goal.Add(new KeyValuePair<string, object>("collectUncookedFood", true));
        }


        return goal;
    }

}

