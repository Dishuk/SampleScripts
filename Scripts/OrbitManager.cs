using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    public Action NewOrbitSpawned = default;

    [SerializeField] private int _distanceToCenterForNextGeneration;
    [SerializeField] private float _distanceToCenterForDestroy;
    [SerializeField] private int hostilesOrbitsLastIndex;
    [SerializeField] private GameObject[] _orbitsToSpawn;

    private Orbit _latestSpawnedOrbit;
    public List<Orbit> AllSpawnedOrbits { get; private set; }

    private bool isFirstOrbitsGenerated;
    private bool isLastOrbitIsHostile;


    private void Awake()
    {
        AllSpawnedOrbits = new List<Orbit>(0);
    }

    private void Start()
    {
        GenerateFirstEmptyOrbits();
    }

    private void FixedUpdate()
    {
        if (IsReadyForNewOrbit() && isFirstOrbitsGenerated == true)
        {
            GenerateOrbit(AllSpawnedOrbits.Count + 1, false);
        }

        if (IsReadyForDestroyFirstOrbit())
        {
            DestroyOrbit(0);
        }
    }



    private void GenerateFirstEmptyOrbits()
    {
        if (_latestSpawnedOrbit == null || IsReadyForNewOrbit())
        {
            for (int i = 0; i < _distanceToCenterForNextGeneration; i++)
            {
                GenerateOrbit(AllSpawnedOrbits.Count + 1, true);
            }
        }
        isFirstOrbitsGenerated = true;
    }

    private void GenerateOrbit(int posX, bool GenerateEmptyOrbits)
    {
        int index;
        if (GenerateEmptyOrbits == true)
        {
            index = 0;
        }
        else
        {
            if (isLastOrbitIsHostile == true)
            {
                index = UnityEngine.Random.Range(hostilesOrbitsLastIndex + 1, _orbitsToSpawn.Length);
            }
            else
            {
                index = UnityEngine.Random.Range(0, _orbitsToSpawn.Length);
            }
        }
        _latestSpawnedOrbit = Instantiate(_orbitsToSpawn[index], Vector3.zero, Quaternion.identity).GetComponent<Orbit>();
        _latestSpawnedOrbit.GenerateOrbitObjects (new Vector3(posX, 0, 0));
        AllSpawnedOrbits.Add(_latestSpawnedOrbit);
        if (_latestSpawnedOrbit.CompareTag("HostileOrbit"))
        {
            isLastOrbitIsHostile = true;
        }
        else {
            isLastOrbitIsHostile = false;
        }

        NewOrbitSpawned?.Invoke();
    }

    private void DestroyOrbit(int orbitIndex) {
        Destroy(AllSpawnedOrbits[orbitIndex].gameObject);
        AllSpawnedOrbits.RemoveAt(orbitIndex);
    }



    private bool IsReadyForNewOrbit()
    {
        float distance = (_latestSpawnedOrbit.mainOrbitGameObject.transform.position - _latestSpawnedOrbit.transform.position).magnitude;

        if (distance < _distanceToCenterForNextGeneration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsReadyForDestroyFirstOrbit()
    {
        float distance = (AllSpawnedOrbits[0].mainOrbitGameObject.transform.position - AllSpawnedOrbits[0].transform.position).magnitude;
        if (distance <= _distanceToCenterForDestroy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class SpecialOrbits 
{
    public GameObject orbit;
    public int scoreToSpawn;
}
