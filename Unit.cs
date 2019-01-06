using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Threading;

/**
 * --обработать случай, когда вершина недостижима 2 - done
 * --переделать GoAndStay 1 - done
 * --произвести оптимизацию (заменить клетки на котнрольные точки)  5
 * --общий рефакторинг 3
 * --реализовать анимированное передвижение 4
 * 
 **/

public class Unit : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject go
    {
        get;
        set;
    }

    private const float stoppingDistance = 0.5f;

    private Tile destTile;
    private Toggle toggle;
    private bool gotTile;
    private Tile currTile;

    private List<Tile> wayPoints;
    private HashSet<Tile> viewedTiles; 
    private PathFinding pathFinding;
    private Thread pathThread;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        pathFinding = new PathFinding(this);
        wayPoints = new List<Tile>();
        pathThread = new Thread(new ThreadStart(
        delegate
        {
           
        }));

        viewedTiles = new HashSet<Tile>();

        gotTile = false;
        toggle = GameObject.Find("Toggle").GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate { ToggleValueChanging(); });

        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        go = GetComponent<Transform>().gameObject;
        Debug.Log(r + " " + g + " " + b);
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b));
    }


    void Update()
    {
        //if (!pathThread.IsAlive)
        //{
        //    pathThread.Start();
        //}
        if (!toggle.isOn)
        {
            GetNewDestination();
        }
        else
        {
            GoAndStay();
        }
    }


    private void GetNewDestination()
    {

        if (!agent.pathPending && agent.remainingDistance < stoppingDistance || destTile.GetState() == Tile.States.BUSY)
        {
            if (wayPoints.Count == 0)
            {
                int tileNum = Random.Range(0, Environment.gameField.Count);
                destTile = Environment.gameField[tileNum];
                if (destTile.GetState() != Tile.States.BUSY)
                {
                    currTile = GetCurrentTile();

                    wayPoints = pathFinding.MakeWaypoints(currTile, destTile);
                }
            }
            else
            {
                    agent.SetDestination(wayPoints[0].go.transform.position);
                    wayPoints.Remove(wayPoints[0]);
            }
        }
    }


    private void GoAndStay()
    {
        if (!gotTile)
        {
            int count = Tile.availableTiles.Count;
            if (count > 0)
            {
                int tileNum = Random.Range(0, count);
                destTile = Tile.availableTiles[tileNum];
                if (!viewedTiles.Contains(destTile))
                {
                    viewedTiles.Add(destTile);
                    wayPoints = pathFinding.MakeWaypoints(GetCurrentTile(), destTile);

                    if (wayPoints.Count > 0)
                    {
                        Tile.availableTiles.Remove(destTile);
                        gotTile = true;
                        agent.isStopped = false;
                        agent.SetDestination(wayPoints[0].go.transform.position);
                        wayPoints.Remove(wayPoints[0]);
                    }
                    else
                    {
                        go.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                        agent.isStopped = true;
                    }
                }
            }
            else
            {
                go.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                agent.isStopped = true;
            }
        }
        else
        {
            if (agent.remainingDistance < stoppingDistance)
            {
                if (wayPoints.Count > 0)
                {
                    agent.SetDestination(wayPoints[0].go.transform.position);
                    wayPoints.Remove(wayPoints[0]);
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
    }


    private void BackToNormal()
    {
        viewedTiles.Clear();
        agent.isStopped = false;
        if (gotTile)
        {
            gotTile = false;
            if (!Tile.availableTiles.Contains(destTile) && destTile.GetState() == Tile.States.AVALIABLE)
            {
                Tile.availableTiles.Add(destTile);
            }
        }
        else
        {

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            go.GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b));
        }
    }


    private void ToggleValueChanging()
    {
            if (toggle.isOn)
            {
               // Debug.Log("Toggle is on");
               //GoAndStay();
            }
            else
            {
              // Debug.Log("Toggle is off");
                BackToNormal();
            }
    }


    private Tile GetCurrentTile()
    {
        Tile temp = null;
        Ray ray = new Ray(go.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
        temp = hit.transform.gameObject.GetComponent<Tile>();
        }
        Debug.Log(temp);
        return temp;
    }

}