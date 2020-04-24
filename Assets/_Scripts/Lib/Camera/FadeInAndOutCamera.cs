using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInAndOutCamera : MonoBehaviour
{
    private Animator animator;
    private Image black;

    private float overlayColorAlpha;
    private bool colorSet;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        black = GetComponent<Image>();
    }

    private void Update()
    {
        if (UIManager.paused && !colorSet)
        {
            animator.SetBool("Overlay", true);
            colorSet = true;
        }
        else if (!UIManager.paused && colorSet)
        {
            animator.SetBool("Overlay", false);
            colorSet = false;
        }
    }

    private IEnumerator FadeOutContinue(string sceneName)
    {
        animator.SetBool("FadeOut", true);
        GameManager.paused = true;
        yield return new WaitUntil(() => black.color.a == 1);
        FindObjectOfType<Continue>().setText();
        yield return new WaitUntil(() => InputManager.jump);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private IEnumerator FadeOut(string sceneName)
    {
        animator.SetBool("FadeOut", true);
        GameManager.paused = true;
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void FadeToNextScene(string sceneName)
    {
        if (sceneName.Contains("stage") 
            && SceneManager.GetActiveScene().name.Contains("stage")
            && FindObjectOfType<NinjaStatesAnimationSound>().isDead()
        ) {
            StartCoroutine(FadeOutContinue(sceneName));
        } else
        {
            StartCoroutine(FadeOut(sceneName));
        }
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
        animator.SetBool("FadeOut", false);
    }
}
