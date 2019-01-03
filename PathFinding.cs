using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * New algorithm would be some like that:
 *  - Get new destination point
 *  - Start A* from current position to dest point
 *  - While we can raycast it, it goes on
 *  - if not, then make NavMeshAgent dest the last reachable point by raycasting
 *  - continue A* flooding 
 *  - and so on while not reach the final dest point
 * 
 * pretty obvious, isn't it?
 **/

public class PathFinding
{
    class Node
    {
        public float g;
        public float h;
        public float f;
        public Tile tile;
        public Node came_from;

        public Node(Tile _t) {
            g = 0;
            h = 0;
            f = g + h;
            tile = _t;
            came_from = null;
        }
    }


    Node start;
    Node final;



    public PathFinding(Tile _start, Tile _final)
    {
        start.tile = _start;
        start.g = 0;
        start.h = h_func(start, final);
        start.f = start.h + start.g; 
        final.tile = _final;


    }


    private List<Node> MakeWay(Node start, Node final)
    {
        List<Node> Queue = new List<Node>();
        HashSet<Node> CloseSet = new HashSet<Node>();

        float t_g_score = 0;

        Queue.Add(start);

        while (Queue.Count > 0)
        {
            Queue.Sort(Compare);
            Node x = Queue[0];
            if(x.tile == final.tile)
            {
                return reconstructPathMap(start, final);
            }
            Queue.Remove(x);
            CloseSet.Add(x);

            foreach (Tile i in x.tile.neighbors)
            {
                Node y = new Node(i);
                if(i.GetState() == Tile.States.BUSY || CloseSet.Contains(y))
                {
                    continue;
                }
                t_g_score = x.g + Vector3.Distance(x.tile.go.transform.position, i.go.transform.position);

                if (!Queue.Contains(y))
                {
                    Queue.Add(y);
                }

                y.came_from = x;
                y.g = t_g_score;
                y.h = h_func(y, final);
                y.f = y.g + y.h;

            }
        }
        return new List<Node>();
    }

    public List<Tile> MakeWaypoints()
    {
        Tile startTile = start.tile;
        Tile finalTile = final.tile;

        List<Node> path = MakeWay(start, final);
        List<Tile> waypoints = new List<Tile>();

        foreach(Node i in path)
        {
            waypoints.Add(i.tile);
        }


        //RaycastHit[] hit;
        //Ray ray;

        //ray = new Ray(startTile.transform.position, Vector3.left)

        //if (Physics.Raycast(ray, out hit))
        //{
        //    temp = hit.transform.gameObject.GetComponent<Tile>();
        //    neighbors.Add(temp);
        //}

        //waypoints.Add(startTile);
        //path.Remove(start);
        //while(path.Count > 0)
        //{
        //    Tile to = path[0].tile;
        //    Tile from = startTile;
        //    ray = new Ray(from.go.transform.position, to.go.transform.position);
        //    hit = Physics.RaycastAll(ray);

        //    float maxDist = 0;
        //    Tile mostFarTile;
        //    foreach(RaycastHit i in hit)
        //    {
        //        float dist = Vector3.Distance(i.transform.position, from.go.transform.position);
        //        if (dist > maxDist)
        //        {
        //            maxDist = dist;
        //            mostFarTile = i.transform.gameObject.GetComponent<Tile>();
        //        }


        //    }

        //}

        return waypoints;
    }

    List<Node> reconstructPathMap(Node start, Node final)
    {
        List<Node> PathMap = new List<Node>();
        Node curr = final;
        while (curr != null){
            PathMap.Add(curr);
            curr = curr.came_from;
        }
        PathMap.Reverse();
        return PathMap;
    }

    float h_func(Node a, Node v)
    {
        return Mathf.Abs(Vector3.Distance(a.tile.go.transform.position,
                                          v.tile.go.transform.position));
    }

    int Compare(Node x, Node y)
    {
        return (int)x.f - (int)y.f;
    }


}
