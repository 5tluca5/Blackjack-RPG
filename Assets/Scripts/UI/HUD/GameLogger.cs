using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance { get; private set; }

    [SerializeField] int maxLogCount = 5;
    [SerializeField] GameObject logTextPrefab;

    Queue<string> logMessages = new Queue<string>();
    List<GameLogText> logTexts = new List<GameLogText>();

    bool isShowingLog = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(logMessages.Count > 0 && !isShowingLog)
        {
            ShowLog();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string message)
    {
        Debug.Log(message);

        logMessages.Enqueue(message);
    }

    void ShowLog()
    {
        if (isShowingLog) return;

        if (logMessages.Count == 0)
        {
            isShowingLog = false;
            return;
        }

        isShowingLog = true;

        GameLogText logText;

        if (transform.childCount < maxLogCount)
        {
            logText = Instantiate(logTextPrefab, transform).GetComponent<GameLogText>();
        }
        else
        {
            logText = transform.GetChild(0).GetComponent<GameLogText>();
            logText.Reset();
            logText.transform.SetAsLastSibling();
        }


        logText.PlayTypewriterEffect(logMessages.Dequeue()).Subscribe(_ =>
        {
            isShowingLog = false;
        });
    }
}
