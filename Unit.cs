using System.Collections;
using System.Collections.Generic;
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

    private PathFinding pathFinding;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

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

    // Update is called once per frame
    void Update()
    {
        if (!toggle.isOn)
        {
            GetNewDestination();
        }
        else
        {
            if(agent.remainingDistance < stoppingDistance)
            {
                agent.isStopped = true;
            }
        }
    }

    void GetNewDestination()
    {

        if (!agent.pathPending && agent.remainingDistance < stoppingDistance || destTile.GetState() == Tile.States.BUSY)
        {
            int tileNum = Random.Range(0, Environment.gameField.Count);
            destTile = Environment.gameField[tileNum];
            if (destTile.GetState() != Tile.States.BUSY)
            {
                agent.SetDestination(destTile.go.transform.position);
            }
        }
    }

    void GoAndStay()
    {
        int count = Tile.availableTiles.Count;
        Debug.Log(count);
        if (count > 0)
        {
            int tileNum = Random.Range(0, count);
            destTile = Tile.availableTiles[tileNum];
            agent.SetDestination(destTile.go.transform.position);
            Debug.Log(agent.pathStatus);
            if (agent.pathStatus != NavMeshPathStatus.PathPartial) //если путь построен
            {
                Tile.availableTiles.Remove(destTile);
                gotTile = true;
            }
            else
            {
                go.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                agent.isStopped = true;
            }
        }
        else
        {
            go.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            agent.isStopped = true;
        }

    }


    void BackToNormal()
    {
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
                GoAndStay();
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
        return temp;
    }

}