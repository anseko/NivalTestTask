using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


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

    private void Awake()
    {
        pathFinding = new PathFinding();
        wayPoints = new List<Tile>();
        viewedTiles = new HashSet<Tile>();
        gotTile = false;

    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
       
        
        toggle = GameObject.Find("Toggle").GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { ToggleValueChanging(); });

        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        go = GetComponent<Transform>().gameObject;
        go.GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b));
    }


    void Update()
    {
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
                    pathThread = new Thread(new ThreadStart(delegate { wayPoints = pathFinding.MakeWaypoints(currTile, destTile); }));
                    pathThread.Start();
                    pathThread.Join();
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
                    currTile = GetCurrentTile();
                    pathThread = new Thread(new ThreadStart(delegate { wayPoints = pathFinding.MakeWaypoints(currTile, destTile); }));
                    pathThread.Start();
                    pathThread.Join();

                    if (wayPoints.Count > 0)
                    {
                        if (go.GetComponent<Renderer>().material.color == Color.black)
                        {

                            float r = Random.Range(0f, 1f);
                            float g = Random.Range(0f, 1f);
                            float b = Random.Range(0f, 1f);
                            go.GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b));
                        }
                        agent.isStopped = false;
                        gotTile = true;
                        Tile.availableTiles.Remove(destTile);
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
        //Debug.Log(temp);
        return temp;
    }

    private void OnDestroy()
    {
        
    }

    private void OnApplicationQuit()
    {
        
    }
}