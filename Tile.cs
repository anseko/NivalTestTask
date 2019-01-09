using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileStates;

public class Tile : MonoBehaviour
{
    public enum States
    {
        IDLE,
        AVALIABLE,
        BUSY
    }

    public static List<Tile> availableTiles;

    static IdleTileState idle;
    static AvailableTileState available;
    static BusyTileState busy;

    public Material cubeMaterial;
    public GameObject cube;
    public States prevState;
    public Tile cameFrom { get; set; } //только для поиска пути
    public Vector3 pos { get; set; } //только для поиска пути

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

    private void Awake()
    {
        idle = new IdleTileState();
        available = new AvailableTileState();
        busy = new BusyTileState();

        availableTiles = new List<Tile>();
        neighbors = new List<Tile>();

        _state = idle;
        prevState = States.IDLE;
    }

    void Start()
    {
       
        go = GetComponent<Transform>().gameObject;

        cube = Instantiate(cube);
        cube.transform.localPosition = go.transform.position + Vector3.up*0.55f;
        cube.SetActive(false);

        GetNeighbors();
        cameFrom = null;
        pos = go.transform.position;
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
