using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;

//Cameras Position MUST be (0, 0, 0)!!!

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector2 currentVelocity = Vector2.zero;
    public float smoothTime = 0.05f, maxSpeed = 20, velocity = 0.0F;
    public float zPosition = -17F;
    public float maxX = 0.0f, maxY = 0.0f;
    public Vector3 stageDimensions = Vector3.zero;


    Vector2 topRightEdgeVector;
    private List<Bounds> bound;
    private string sceneName;
    public int currentBound = 0;

    // Use this for initialization
    void Start () {
        bound = new List<Bounds>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        topRightEdgeVector = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        ReadFile();
    }

    float interpolation = 0.0f;

    void FixedUpdate()
    {
        interpolation = maxSpeed * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.position.x, interpolation);

        position = HoldPositionInBounds(position);
        transform.position = position;
    }

    Vector3 HoldPositionInBounds(Vector3 position)
    {
        currentBound = getCurrentCameraBound(position);

        float left = bound[currentBound].min.x + topRightEdgeVector.x;
        float right = bound[currentBound].max.x - topRightEdgeVector.x;

        float bottom = bound[currentBound].min.y + topRightEdgeVector.y;
        float up = bound[currentBound].max.y - topRightEdgeVector.y;


        if (position.x < left) position.x = Mathf.Lerp(this.transform.position.x, left, interpolation);// left;
        else if (position.x > right) position.x = Mathf.Lerp(this.transform.position.x, right, interpolation);//right;

        if (position.y < bottom) position.y = Mathf.Lerp(this.transform.position.y, bottom, interpolation); //bottom;
        else if (position.y > up) position.y = Mathf.Lerp(this.transform.position.y, up, interpolation); //up;
        

        return position;
    }

    int getCurrentCameraBound(Vector3 position)
    {
        for (int i = 0; i < bound.Count; i++)
        {
            if (player.position.x >= bound[i].min.x && player.position.x <= bound[i].max.x
              && player.position.y >= bound[i].min.y && player.position.y <= bound[i].max.y)
            //if (position.x - topRightEdgeVector.x >= bound[i].min.x && position.x + topRightEdgeVector.x <= bound[i].max.x
              //&& position.y - topRightEdgeVector.y >= bound[i].min.y && position.y + topRightEdgeVector.y <= bound[i].max.y)
            {
                Debug.Log("Player is in BoundingBox " + i);
                return i;
            }
        }

        return -1;
    }

    void ReadFile()
    {
        string path = "Assets/_Backgrounds/_BoundingBoxData/" + sceneName + ".txt";
        
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        string[] lines = reader.ReadToEnd().Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            Debug.Log(lines[i]);
            float[] temp = new float[4];
            temp[0] = float.Parse(lines[i].Split(',')[0]);
            temp[1] = float.Parse(lines[i].Split(',')[1]);
            temp[2] = float.Parse(lines[i].Split(',')[2]);
            temp[3] = float.Parse(lines[i].Split(',')[3]);

            Vector3 center = new Vector3((temp[0] + temp[2]) / 2F, (temp[1] + temp[3]) / 2F, 0.0f);
            Vector3 size = new Vector3(temp[2] - temp[0], temp[3] - temp[1], 0.0f);

            Bounds b = new Bounds(center, size);
            Debug.Log(b);
            bound.Add(b);
        }
    }
}
