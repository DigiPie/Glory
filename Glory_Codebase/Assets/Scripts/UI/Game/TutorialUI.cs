using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    // References
    public StateSystem stateSystem;
    public GameManager gameManager;
    public GameObject TutorialCanvas;
    public GameObject Intro1Scene;
    public GameObject Intro2Scene;
    public GameObject WalkScene;
    public GameObject JumpScene;
    public GameObject AttackScene;
    public GameObject DoneScene;

    public GameObject Dash1Scene;
    public GameObject Dash2Scene;
    public GameObject Dash3Scene;

    public GameObject NextWaveScene;
    public GameObject player1;

    void Awake()
    {

    }

    private void OnEnable()
    {
        if (stateSystem.IsIntro1())
        {
            Intro1Scene.SetActive(true);
        }
        else if (stateSystem.IsDash1())
        {
            Dash1Scene.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Previous"))
        {
            PrevState();
        }
        else if (Input.GetButtonDown("Submit"))
        {
            NextState();
        }
    }

    public void NextState()
    {
        if (stateSystem.IsIntro1())
        {
            Intro2Scene.SetActive(true);
            Intro1Scene.SetActive(false);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Intro2);
        }

        else if (stateSystem.IsIntro2())
        {
            WalkScene.SetActive(true);
            Intro2Scene.SetActive(false);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Walk);
        }

        else if (stateSystem.IsWalk())
        {
            JumpScene.SetActive(true);
            WalkScene.SetActive(false);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Jump);
        }

        else if (stateSystem.IsJump())
        {
            JumpScene.SetActive(false);
            AttackScene.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Attack);
            //player1.GetComponent<PlayerController>().AllowAttack(true);
        }

        else if (stateSystem.IsAttack())
        {
            AttackScene.SetActive(false);
            DoneScene.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Done);
        }

        // When entire tutorial is completed
        else if (stateSystem.IsDone())
        {
            DoneScene.SetActive(false);
            TutorialCanvas.SetActive(false);
            stateSystem.StartGameWave();
            stateSystem.SetTutorialState(StateSystem.TutorialState.Dash1);
        }

        else if (stateSystem.IsDash1())
        {
            Dash1Scene.SetActive(false);
            Dash2Scene.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Dash2);
        }
        
        else if (stateSystem.IsDash2())
        {
            Dash2Scene.SetActive(false);
            Dash3Scene.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.Dash3);
            gameManager.EnableSlide();
        }

        else if (stateSystem.IsDash3())
        {

            Dash3Scene.SetActive(false);
            TutorialCanvas.SetActive(false);
            stateSystem.StartGameWave();
            // Change State here;
        }
    }

    public void PrevState()
    {
        if (!stateSystem.IsIntro1())
        {
            if (stateSystem.IsIntro2())
            {
                Intro2Scene.SetActive(false);
                Intro1Scene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Intro1);
            }
            if (stateSystem.IsWalk())
            {
                WalkScene.SetActive(false);
                Intro2Scene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Intro2);
            }

            else if (stateSystem.IsJump())
            {
                JumpScene.SetActive(false);
                WalkScene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Walk);
            }

            else if (stateSystem.IsAttack())
            {
                AttackScene.SetActive(false);
                JumpScene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Jump);
            }

            else if (stateSystem.IsDone())
            {
                DoneScene.SetActive(false);
                AttackScene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Attack);
            }
        }

        // Dash Tutorial
        if (!stateSystem.IsDash1())
        {
            if(stateSystem.IsDash2())
            {
                Dash2Scene.SetActive(false);
                Dash1Scene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Dash1);
            } 

            else if (stateSystem.IsDash3())
            {
                Dash3Scene.SetActive(false);
                Dash2Scene.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.Dash2);
            }
        }
    }
}
