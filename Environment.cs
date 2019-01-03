﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Environment : MonoBehaviour
{
    const int MAX_UNITS = 5;
    const int MIN_UNITS = 1;
    const int MAX_SIZE = 10;
    const int MIN_SIZE = 5;


    public static List<Tile> gameField;
    public List<Unit> units;
    public GameObject TilePrefab;
    public GameObject UnitPrefab;
    public NavMeshSurface meshSurface;
    // Start is called before the first frame update
    void Start()
    {
        gameField = new List<Tile>();

        meshSurface.layerMask = LayerMask.GetMask("Default");
        int size = Random.Range(MIN_SIZE, MAX_SIZE);
        Debug.Log("gameField size:"+size);
        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                GameObject go = Instantiate(TilePrefab);
                Tile temp = go.GetComponent<Tile>();
                gameField.Add(temp);
                Vector3 pos = new Vector3(i * go.transform.localScale.x, 0, j * go.transform.localScale.z);
                go.transform.SetPositionAndRotation(pos, Quaternion.identity);
            }
        }
        meshSurface.BuildNavMesh();

        size = Random.Range(MIN_UNITS, MAX_UNITS);
        Debug.Log("Num of units:" + size);
        //создать юнитов
        for (int i=0;i< size; i++)
        {
            GameObject go = Instantiate(UnitPrefab);
            Unit temp = go.GetComponent<Unit>();
            units.Add(temp);
            Vector3 pos = Vector3.zero + Vector3.up;
            go.transform.SetPositionAndRotation(pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
       
    }
}
