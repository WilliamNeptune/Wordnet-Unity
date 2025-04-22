using UnityEngine;
using System.Collections.Generic;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instance)
        {
            _instance = FindObjectOfType<UnityMainThreadDispatcher>();
            if (!_instance)
            {
                GameObject go = new GameObject("UnityMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
        }
        return _instance;
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (_executionQueue.Count > 0)
        {
            Action action;
            lock (_executionQueue)
            {
                action = _executionQueue.Dequeue();
            }
            action.Invoke();
        }
    }
}