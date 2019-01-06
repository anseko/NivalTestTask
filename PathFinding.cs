using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    Unit unit;

    public PathFinding(Unit _unit)
    {
        unit = _unit;
    }

    //а*
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
                    Debug.Log("y is already visited");
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


    //обход в ширину
    private List<Tile> MakeEzWay(Tile start, Tile final)
    {
        Queue<Tile> Queue = new Queue<Tile>();
        HashSet<Tile> CloseSet = new HashSet<Tile>();

        Queue.Enqueue(start);

        while (Queue.Count > 0)
        {
            Tile x = Queue.Dequeue();
            CloseSet.Add(x);
            if (x == final)
            {

                return reconstructTilePathMap(start, final);
            }


            foreach (Tile i in x.neighbors)
            {
                if (i.GetState() == Tile.States.BUSY || CloseSet.Contains(i))
                {
                    Debug.Log("y is already visited");
                    continue;
                }
                if (!Queue.Contains(i))
                {
                    Queue.Enqueue(i);
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

        //Node start;
        //Node final;
        //start = new Node(startTile);
        //final = new Node(finalTile);
        //start.g = 0;
        //start.h = h_func(start, final);
        //start.f = start.h + start.g;
        
        //List<Node> path = MakeEzWay(startTile, finalTile);

        List<Tile> waypoints = MakeEzWay(startTile, finalTile);

        Debug.Log("HERE path length:" + waypoints.Count);

        return waypoints;
//        yield return null;

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
