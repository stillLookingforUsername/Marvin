using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject _afterImagePrefab;   //store ref to the image prefab that we will use to create afterImage

    private Queue<GameObject> _availableObjects = new Queue<GameObject>();      //use to store all the objects what we r not currently using

    // Singleton to access this script from our other scripts
    public static PlayerAfterImagePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;    //Assigns the current object that this script is attached to, to this static variable Instance
        GrowPool();
    }

    // to create more gameObject for the pool
    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(_afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);   //this will make the gameObject that we create a child of the gameObject that this script is attached to
            AddToPool(instanceToAdd);
        }
    }

    //fun to add the gameObject create, to the queue
    //public so that we can call it frm our other scripts via the singleton 
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        _availableObjects.Enqueue(instance);
    }

    //fun to get an object from the pool
    //we will call this function frm other scripts instead of instantiate

    public GameObject GetFromPool()
    {
        if (_availableObjects.Count == 0)
        {
            GrowPool();
        }

        var instance = _availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }



}
