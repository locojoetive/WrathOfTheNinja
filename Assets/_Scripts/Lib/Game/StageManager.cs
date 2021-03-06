﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private static bool initializing = false;
    public static bool 
        hasTextSceneEnded,
        paused = false,
        onTitle,
        onControls,
        onIntro,
        onStage,
        onOutro,
        onCredits;

    internal static void changeCurrentCheckpoint(int newCheckpoint)
    {
        throw new NotImplementedException();
    }

    public static int 
        currentStage = 0,
        currentCheckpoint = 0,
        lastStage = 3;

    public static string activeScene;    
    private static FadeInAndOutCamera fade;

    private void Update()
    {
        ProtocolOnTitle();
        ProtocolOnControls();
        ProtocolOnIntro();
        ProtocolOnStage();
        ProtocolOnOutro();
        ProtocolOnCredits();
    }


    private void ProtocolOnTitle()
    {
        if (onTitle)
        {
            if (InputManager.confirm)
            {
                InputManager.confirm = false;
                fade.FadeToNextScene("wotn_controls");
            }
        }

    }

    private void ProtocolOnControls()
    {
        if (onControls)
        {
            if (InputManager.escape)
            {
                InputManager.escape = false;
                fade.FadeToNextScene("wotn_title");
            }
            else if (InputManager.confirm)
            {
                InputManager.confirm = false;
                currentStage = 1;
                fade.FadeToNextScene("wotn_intro");
            }
        }
    }


    private void ProtocolOnIntro()
    {
        if (!initializing && onIntro && hasTextSceneEnded)
        {
            initializing = true;
            currentStage = 1;
            fade.FadeToNextScene("wotn_stage_" + currentStage + "_0");
        }
    }

    private void ProtocolOnStage()
    {
        if (onStage)
        {
            if (InputManager.reset)
            {
                fade.FadeToNextScene("wotn_stage_" + currentStage + "_" + currentCheckpoint);
            }
        }
    }
    private void ProtocolOnOutro()
    {
        if (onOutro && hasTextSceneEnded && InputManager.confirm)
        {
            InputManager.confirm = false;
            hasTextSceneEnded = false;
            fade.FadeToNextScene("wotn_credits");
        }
    }

    private void ProtocolOnCredits()
    {
        if (onCredits)
        {
            if (!initializing && PlayCredits.creditsHaveEnded && (InputManager.escape || InputManager.confirm))
            {
                initializing = true;
                PlayCredits.creditsHaveEnded = false;
                InputManager.escape = false;
                InputManager.confirm = false;
                fade.FadeToNextScene("wotn_title");
            }
        }
    }

    public void IdentifyScene(Scene scene)
    {
        activeScene = scene.name;
        onTitle = activeScene.Contains("title");
        onControls = activeScene.Contains("controls");
        onIntro = activeScene.Contains("intro");
        onStage = activeScene.Contains("stage");
        onOutro = activeScene.Contains("outro");
        onCredits = activeScene.Contains("credits");
        if (onStage)
        {
            currentStage = ExtractStageNumber(activeScene);
        }
    }

    private int ExtractStageNumber(string activeScene)
    {
        string[] splitSceneName = activeScene.Split('_');
        if (splitSceneName.Length == 4)
            return Int32.Parse(splitSceneName[2]);
        return -1;
    }

    public static void InitializeNextStage()
    {
        if (!initializing)
        {
            initializing = true;
            currentStage++;
            currentCheckpoint = 0;
            if (currentStage > 0 && currentStage <= lastStage)
            {
                fade.FadeToNextScene("wotn_stage_" + currentStage + "_" + currentCheckpoint);
            }
            else
            {
                fade.FadeToNextScene("wotn_outro");
            }
        }
    }

    public static void ReloadScene()
    {
        if (!initializing)
        {
            initializing = true;
            fade.FadeToNextScene("wotn_stage_" + currentStage + "_" + currentCheckpoint);
        }
    }

    public static void ExitScene()
    {
        fade.FadeToNextScene("wotn_title");
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
        hasTextSceneEnded = false;
        initializing = false;
        IdentifyScene(scene);

        if (fade == null)
        {
            fade = FindObjectOfType<FadeInAndOutCamera>();
        }
    }
}
