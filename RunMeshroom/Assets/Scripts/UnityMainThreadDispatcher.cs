using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance;
    private static bool _initialized = false;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_initialized)
        {
            _instance = FindAnyObjectByType<UnityMainThreadDispatcher>();
            if (_instance == null)
            {
                GameObject go = new GameObject("UnityMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
            }
            _initialized = true;
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _initialized = true;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않도록
    }

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// 메인 스레드에서 실행될 작업을 큐에 추가합니다.
    /// </summary>
    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}