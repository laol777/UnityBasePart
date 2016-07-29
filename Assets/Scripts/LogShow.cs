using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogShow : MonoBehaviour {

    [SerializeField]Text textLog;

	void Start()
    {
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += handleUnityLog;
        gameObject.GetComponent<Material>().color = Color.black;
    }

    private void handleUnityLog(string logString, string stackTrace, LogType type)
    {
        textLog.text = stackTrace;
    }

    void Update()
    {
        //textLog.text = Screen.dpi.ToString();
        //textLog.fontSize = 40;
       // textLog.fontSize = 1000 / Screen.dpi; 
    }


}
