using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Reload : MonoBehaviour {

    private bool dead = false;

    /*
    void Update()
    {
        if (dead)
        {
            EditorGUILayout.HelpBox("Try Again?", MessageType.Info , true);
        }
    }*/

	void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            //dead = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
