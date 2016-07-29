using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestJSON : MonoBehaviour {

    public bool checkMe;
  
    public int[,] level;
  
    List<string> test = new List<string>();

    string json;
    
    int i = 1;

	void Update () {
        if (checkMe)
        {
            level = new int[i, i];
            for (int j = 0; j < i; j++)
            {
                for (int k = 0; k < i; k++)
                {
                    level[j, k] = i;
                }

            }
            ++i;
            checkMe = false;
            test.Add(JsonUtility.ToJson(level));
        }
    }
}
