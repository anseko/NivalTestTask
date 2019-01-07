using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{

    private Tile final = null;

    //а*
    private List<Tile> MakeWay(Tile start, Tile final)
    {
        List<Tile> Queue = new List<Tile>();
        HashSet<Tile> CloseSet = new HashSet<Tile>();

        float t_g_score = 0;

        Queue.Add(start);

        while (Queue.Count > 0)
        {
            Queue.Sort(Compare);
            Tile x = Queue[0];
            if(x == final)
            {
                return reconstructTilePathMap(start, final);
            }
            Queue.Remove(x);
            CloseSet.Add(x);

            foreach (Tile i in x.neighbors)
            {
                if(i.GetState() == Tile.States.BUSY || CloseSet.Contains(i))
                {
                    Debug.Log("i is already visited");
                    continue;
                }
                t_g_score = Vector3.Distance(x.go.transform.position, i.go.transform.position);

                if (!Queue.Contains(i))
                {
                    Queue.Add(i);
                }

                i.cameFrom = x;

            }
        }
        return new List<Tile>();
    }

    List<Tile> reconstructTilePathMap(Tile start, Tile final)
    {
        List<Tile> PathMap = new List<Tile>();
        Tile curr = final;
        while (curr != null)
        {
            PathMap.Add(curr);
            Tile temp = curr;
            curr = curr.cameFrom;
            temp.cameFrom = null;
        }
        PathMap.Reverse();
        return PathMap;
    }

    public List<Tile> MakeWaypoints(Tile startTile, Tile finalTile)
    {
        final = finalTile;
        List<Tile> waypoints = MakeWay(startTile, finalTile);

        Debug.Log("HERE path length:" + waypoints.Count);

        return waypoints;
    }


    int Compare(Tile x, Tile y)
    {

        return (int)(Vector3.Distance(x.go.transform.position, final.go.transform.position) -
                Vector3.Distance(y.go.transform.position, final.go.transform.position));
    }

}
