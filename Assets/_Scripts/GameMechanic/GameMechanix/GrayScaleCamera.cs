using UnityEngine;
using UnityEngine.SceneManagement;

public class GrayScaleCamera : MonoBehaviour
{
    public Material grayscaleMaterial;
    private NinjaStatesAnimationSound player;
    public bool grayedOut = false;

    private void Start()
    {
        grayedOut = false;
        player = FindObjectOfType<NinjaStatesAnimationSound>();
        grayscaleMaterial.SetFloat("_BLACK_AND_WHITE", 0);
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!grayedOut && player != null && player.isDead())
        {
            grayscaleMaterial.SetFloat("_BLACK_AND_WHITE", 1);
            grayedOut = true;
        }
        Graphics.Blit(source, destination, grayscaleMaterial);
    }


    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<NinjaStatesAnimationSound>();
    }
}