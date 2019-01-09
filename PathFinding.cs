using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{

    private Tile final = null;

    private List<Tile> MakeWay(Tile start, Tile final)
    {
        List<Tile> Queue = new List<Tile>();
        HashSet<Tile> CloseSet = new HashSet<Tile>();

        Queue.Add(start);

        while (Queue.Count > 0)
        {
            Queue.Sort(Compare);
            Tile x = Queue[0];
            if(x == final)
            {
                return ReconstructTilePathMap(start, final);
            }
            Queue.Remove(x);
            CloseSet.Add(x);

            foreach (Tile i in x.neighbors)
            {
                if(i.GetState() == Tile.States.BUSY || CloseSet.Contains(i))
                {
                    //Debug.Log("i is already visited");
                    continue;
                }

                if (!Queue.Contains(i))
                {
                    Queue.Add(i);
                }

                i.cameFrom = x;

            }
        }
        return new List<Tile>();
    }


    private List<Tile> ReconstructTilePathMap(Tile start, Tile final)
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
        //Debug.Log("HERE path length:" + waypoints.Count);

        return waypoints;
    }


    private int Compare(Tile x, Tile y)
    {
        return (int)(Vector3.Distance(x.pos, final.pos) -
                    Vector3.Distance(y.pos, final.pos));
    }

}
