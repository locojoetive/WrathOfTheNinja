using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

//Cameras Position MUST be (0, 0, 0)!!!

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector2 currentVelocity = Vector2.zero;

    //Floating Camera
    public float maxSpeed = 20, velocity = 0.0F;
    private float interpolation = 0.0f;
    public float zPosition = -17F;

    //For loading stage bounds
    private string sceneName;
    Vector2 topRightEdgeVector;
    public List<Bounds> bound;
    public List<int> currentBounds = new List<int>(), currentPlayerBounds = new List<int>();
    
    // Use this for initialization
    void Start () {
        bound = new List<Bounds>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        topRightEdgeVector = Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) - transform.position;
        sceneName = SceneManager.GetActiveScene().name;
        ReadFile();
        currentBounds.Add(0);
    }
    
    void FixedUpdate()
    {
        HoldPositionInBounds();
    }

    void HoldPositionInBounds()
    {

        Vector3 position = player.position;
        interpolation = maxSpeed * Time.deltaTime;
        SetCurrentBounds();

        float left = Mathf.Infinity;
        float right = Mathf.NegativeInfinity;

        float bottom = Mathf.Infinity;
        float up = Mathf.NegativeInfinity;

        if (PlayerInCameraView())
        {
            for (int i = 0; i < currentBounds.Count; i++)
            {
                if (left > bound[currentBounds[i]].min.x)
                    left = bound[currentBounds[i]].min.x;

                if (right < bound[currentBounds[i]].max.x)
                    right = bound[currentBounds[i]].max.x;

                if (bottom > bound[currentBounds[i]].min.y)
                    bottom = bound[currentBounds[i]].min.y;

                if (up < bound[currentBounds[i]].max.y)
                    up = bound[currentBounds[i]].max.y;

            }
        }
        else
        {
            for (int i = 0; i < currentPlayerBounds.Count; i++)
            {
                if (left > bound[currentPlayerBounds[i]].min.x)
                    left = bound[currentPlayerBounds[i]].min.x;

                if (right < bound[currentPlayerBounds[i]].max.x)
                    right = bound[currentPlayerBounds[i]].max.x;

                if (bottom > bound[currentPlayerBounds[i]].min.y)
                    bottom = bound[currentPlayerBounds[i]].min.y;

                if (up < bound[currentPlayerBounds[i]].max.y)
                    up = bound[currentPlayerBounds[i]].max.y;

            }
        }
        if (left != Mathf.NegativeInfinity && right != Mathf.Infinity &&
            bottom != Mathf.NegativeInfinity && up != Mathf.Infinity)
        {
            if (position.x < left) position.x = left;// left;
            else if (position.x > right) position.x = right;//right;

            if (position.y < bottom) position.y = bottom; //bottom;
            else if (position.y > up) position.y = up; //up;   


            position.x = Mathf.Lerp(transform.position.x, position.x, interpolation);
            position.y = Mathf.Lerp(transform.position.y, position.y, interpolation);
            position.z = transform.position.z;
            if (position.x != Mathf.Infinity && position.x != Mathf.NegativeInfinity &&
                position.y != Mathf.Infinity && position.y != Mathf.NegativeInfinity &&
                position.z != Mathf.Infinity && position.z != Mathf.NegativeInfinity)
            transform.position = position;
        }
    }

    void SetCurrentBounds()
    {
        currentBounds.Clear();
        currentPlayerBounds.Clear();
        for (int i = 0; i < bound.Count; i++)
        {
            if (transform.position.x >= bound[i].min.x && transform.position.x <= bound[i].max.x
                && transform.position.y >= bound[i].min.y && transform.position.y <= bound[i].max.y)
            {
                currentBounds.Add(i);
            }

            if (player.position.x >= bound[i].min.x - topRightEdgeVector.x && player.position.x <= bound[i].max.x + topRightEdgeVector.x
                && player.position.y >= bound[i].min.y - topRightEdgeVector.y && player.position.y <= bound[i].max.y + topRightEdgeVector.y)
            {
                currentPlayerBounds.Add(i);
            }
        }
    }
    
    bool PlayerInCameraView()
    {
        foreach(int i in currentBounds)
        {
            if (currentPlayerBounds.Contains(i))
                return true;
        }
        return false;
    }

    void ReadFile()
    {
        string path = "Assets/_Backgrounds/_BoundingBoxData/" + sceneName + ".txt";
        
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string[] lines = reader.ReadToEnd().Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {

            float[] temp = new float[4];
            temp[0] = float.Parse(lines[i].Split(',')[0]) + topRightEdgeVector.x;//left
            temp[1] = float.Parse(lines[i].Split(',')[1]) + topRightEdgeVector.y;//bottom;
            temp[2] = float.Parse(lines[i].Split(',')[2]) - topRightEdgeVector.x;//right
            temp[3] = float.Parse(lines[i].Split(',')[3]) - topRightEdgeVector.y;//top

            Vector3 center = new Vector3((temp[0] + temp[2]) / 2F, (temp[1] + temp[3]) / 2F, 0.0f);
            Vector3 size = new Vector3(temp[2] - temp[0], temp[3] - temp[1], 0.0f);

            Bounds b = new Bounds(center, size);
            bound.Add(b);
        }
    }
}
