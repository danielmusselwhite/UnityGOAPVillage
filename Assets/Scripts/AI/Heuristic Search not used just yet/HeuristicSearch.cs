//PseudoCode for the Heuristic Search A* algorithm

/*
function A*(start, goal)
{
        //the set of nodes already evaluated
    closedSet := {}


        //the set of currently discovered nodes that are not evaluated yet
        //initially, only the start node is know
    openSet := {start}

        //For each node, which node it can most efficiently be reached from.
        //If a node can be reached from many nodes, cameFrom will eventually contain
        //the most efficient previous step.
    cameFrom := the empty map

        //For each node, the cost of getting from the start node to that node.
    gScore := map with default value of infinity

        //The cost of going from start to start is zero.
    gScore[start] := 0

        //For each node, the total cost of getting from the start node to the goal
        //by passing by that node. That value is partly known partly heuristic
    fScore := map with default value of Infinity

        //For each node the total cost of getting from the start node to the goal
        //by passing by that node. That value is partly known, partly heuristic.
    fScore := map with default value of Infinity

        //For the fisrt node, that value is completely heuristic.
    fScore[start] := heuristic_cost_estimate(start,goal)



    while openSet is not empty
    {
        current:=the node in openSet having the lowest fScore[] value
        if current = goal
        {
            return reconstruct_path(cameFrom,current)
        }

        openSet.Remove(current)
        openSet.Add(current)

        foreach neighbour of current
        {
            if neighbour in closedSet
            {
                continue // ignore the neighbour which is already evaluated
            }

            if neighbour not in openSet
            {
                openSet.Add(neighbour)  //discover a new node
            }

            tentative_gScore := gScore[current] + dist_between(current,neighbour)
            if tentative_gScore >= gScore[neighbour]
            {
                continue //this is not a better path
            }

            //this path is the best until now. Record it!

            else
            {
                cameFrom[neighbour] := current
                gScore[neighbour] := tentative_gScore
                fScore[neighbour] := gScore[neighbour] + heuristic_cost_estimate(neighbour,goal)
            }
        }

        return failure

    }

    function reconstruct_path(cameFrom, current)
        total_path := [current]
        while current in cameFrom.Keys:
        {
            current:=cameFrom[current]
            total_path.append(current)
        }
        return total_path







}

 */