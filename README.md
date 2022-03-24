# GOAP

* This is an old project I did from 2018
	* Inspired by "https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793"
		* But with major revisions and customisation, most significant of which are:
			* Added multiple new agents
			* Changing the implementation so it works for multiple prioritised goals. This made it much more efficient as: if a sequence of actions to meet the goal of priority 1 couldn't be found, it would try to find a sequence of actions to meet the goal of priority 2.
			* <b>Changing the search algorithm from progressive (base state to goal state) to regressive (goal state to base state). Whilst searching progressively the search algorithm would consider all actions the agent could do from their base state, including actions which would never lead to them completing their current goal. By searching down from the goal state the search algorithm would only consider actions which could lead to the agent meeting its current goal, thus improving its efficiency.  </b>
			
## Install options

* Create a new Unity 2018.2.8f1 Personal project

## Description

* Uses the GOAP (Goal-Oriented Action Planning)framework for the AI agents
	* GOAP works like a Finite State Machine, but instead of hard coding the transitions you give each agent a set of actions (each having a cost, a required state and end state), a goal state and a base state (the state the agent starts in).
	* From this using A\* search algorithm it finds the sequence of actions from the agent's current state to its goal state at the lowest cost.
	* This is a good way to program AI NPCs as it allows them to be more responsive, if there is a change in the environment which now negates one path it will find a new path to its goal in real time.
		* These pre and post conditions may not necessarily be on the agent itself, they may be of the world
	
## Agent's explained

* In our system, each of the agents are given 3 simple goal states:
	1. To sleep when they have run out of energy
	2. To eat when they are hungry
	3. To do their main task (e.g. for the logger, gathering wood)
	
* Each agent is given 5 actions, we will use the concrete examples from the logger
	1. Go to the closest free bed (precondition = tired; postcondition = rested)
		* Satisfies goal of sleeping
	2. Go to the fridge (precondition = fridge has cooked food, and hungry; postcondition = full)
		* Satisfies goal of eating
	3. Pick up tool (precondition = chest has tool(s) in it; postcondition = has tool)
		* Intermediate action to achieve the main goal
	4. Pick up sticks (precondition = none; postcondition = gathered wood)
		* Satisfies logger goal of gathering wood
			* exclusive to logger, other agents have an equivalent action for doing their main task without a tool
	5. Chop tree (precondition = has tool; postcondition = gathered wood)
		* Satisfies logger goal of gathering wood
			* exclusive to logger, other agents have an equivalent action for using their tool to do their main task

* The sum of getting a tool + doing the tool action is lower than the cost of doing the action without the tool; so in real-time the agents will find the cheapest possible path
	* i.e. if they have a tool they will prefer to do their tool action, if they don't have a tool but there is one in the chest, they will go to the chest then do their tool action, and if the chest is empty and they don't have a tool they will do their no requirement action.


	
* The agents interact with eachother through the world. 
	* One simple stream of these interactions are:
		* Hunter kills a pig/chicken and puts the raw meat in the fridge
		* Chef takes the raw meat, takes it to cooker, then puts the cooked meat back in the fridge
		* Now, whenever an agent wants to fulfil their eating goal when they are hungry, they can go to the fridge while it has cooked meat in it to satisfy their hunger
		
		
	* Watch the demo_video.mp4 to see this working