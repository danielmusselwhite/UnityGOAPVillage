using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A tool used for mining ore, chopping wood and hunting pigs.
 *Tool have strength that gets used up each time they are used.
 *When their strength is depleted they should be destroyed by their agent.*/
public class ToolComponent : MonoBehaviour {

    //predefining the tools strength
    public float strength; // 0 to 1 or 0% to 100%
	//Initializing
	void Start ()
    {
        strength = 1; // full strength
	}
	
    //Use up the tool by causing damage, damage should be a percent from 0 to 1 where 1 is 100%
    public void use(float damage)
    {
        Debug.Log("Damaged tool");
        strength -= damage;
    }
    //function which returns true if strength is less than or equal to 0
    public bool destroyed()
    {
        return strength <= 0f;
    }
}
