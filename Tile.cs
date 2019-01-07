using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TileStates;
using System;

public class Tile : MonoBehaviour
{
    public enum States
    {
        IDLE,
        AVALIABLE,
        BUSY
    }

    public static List<Tile> availableTiles;
    public static IdleTileState idle;
    public static AvailableTileState available;
    public static BusyTileState busy;

    public Material cubeMaterial;
    public GameObject cube;
    public States prevState;
    public Tile cameFrom { get; set; } //только для поиска пути

    public GameObject go
    {
        get;
        set;
    }

    public List<Tile> neighbors
    {
        get;
        set;
    }

    private TileState _state;

    void Start()
    {
        idle = new IdleTileState();
        available = new AvailableTileState();
        busy = new BusyTileState();

        availableTiles = new List<Tile>();

        _state = idle;
        prevState = States.IDLE;
        go = GetComponent<Transform>().gameObject;

        cube = Instantiate(cube);
        cube.transform.localPosition = go.transform.position + Vector3.up*0.55f;
        cube.SetActive(false);

        neighbors = new List<Tile>();
        GetNeighbors();
        cameFrom = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.transform == go.transform || hit.transform == cube.transform)
                {
                    _state.OnPointerClick(0, this);

                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == go.transform)
                {
                    _state.OnPointerClick(1, this);
                }
            }
        }
        _state.Update(this);
    }

    public void SetState(States state)
    {
        _state.OnExit(this);

        switch (state)
        {
            case States.AVALIABLE: _state = available; break;
            case States.BUSY: _state = busy; break;
            case States.IDLE: _state = idle; break;
        }

        _state.OnEnter(this);
    }

    public States GetState()
    {
        if( _state is IdleTileState)
        {
            return States.IDLE;
        }

        if( _state is AvailableTileState)
        {
            return States.AVALIABLE;
        }

        if(_state is BusyTileState)
        {
            return States.BUSY;
        }

        throw new System.MissingMethodException("Tile state is undefined");
    }

    private void GetNeighbors()
    {
        Tile temp;
        RaycastHit hit;
        Ray ray = new Ray(go.transform.position, Vector3.left);
        if (Physics.Raycast(ray, out hit))
        {
            temp = hit.transform.gameObject.GetComponent<Tile>();
                neighbors.Add(temp);
        }

        ray.direction = Vector3.right;
        if (Physics.Raycast(ray, out hit))
        {
            temp = hit.transform.gameObject.GetComponent<Tile>();
            neighbors.Add(temp);
        }

        ray.direction = Vector3.back;
        if (Physics.Raycast(ray, out hit))
        {
            temp = hit.transform.gameObject.GetComponent<Tile>();
            neighbors.Add(temp);
        }

        ray.direction = Vector3.forward;
        if (Physics.Raycast(ray, out hit))
        {
            temp = hit.transform.gameObject.GetComponent<Tile>();
            neighbors.Add(temp);
        }
    }

}
