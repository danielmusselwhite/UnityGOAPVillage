using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds resources for the Agent.
public class BackpackComponent : MonoBehaviour {

    public GameObject tool;
    public int numLogs;
    public int numFirewood;
    public int numOre;
    public string toolType = "ToolAxe"; //public so toolType can be changed outside of the script.. i.e. change it to "ToolSword" for hunter

    public int numUncookedFood;
    public int numCookedFood;

    public int numOfTools;
}
