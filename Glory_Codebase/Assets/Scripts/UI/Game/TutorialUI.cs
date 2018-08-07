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

    public GameObject FirstSpell1;
    public GameObject FirstSpell2;
    public GameObject FirstSpell3;

    public GameObject SecondSpell1;
    public GameObject SecondSpell2;
    public GameObject SecondSpell3;

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
        else if (stateSystem.IsFirstSpell1())
        {
            FirstSpell1.SetActive(true);
        }
        else if (stateSystem.IsSecondSpell1())
        {
            SecondSpell1.SetActive(true);
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
            if (stateSystem.IsFirstSpell2() || stateSystem.IsSecondSpell2())
            {
                return;
            }
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
        }

        else if (stateSystem.IsFirstSpell1())
        {
            FirstSpell1.SetActive(false);
            FirstSpell2.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell2);
        }

        else if (stateSystem.IsFirstSpell2())
        {
            FirstSpell2.SetActive(false);
            FirstSpell3.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell3);
        }

        else if (stateSystem.IsFirstSpell3())
        {
            FirstSpell3.SetActive(false);
            TutorialCanvas.SetActive(false);
            stateSystem.StartGameWave();
        }

        else if (stateSystem.IsSecondSpell1())
        {
            SecondSpell1.SetActive(false);
            SecondSpell2.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.SecondSpell2);
        }

        else if (stateSystem.IsSecondSpell2())
        {
            SecondSpell2.SetActive(false);
            SecondSpell3.SetActive(true);
            stateSystem.SetTutorialState(StateSystem.TutorialState.SecondSpell3);
        }

        else if (stateSystem.IsSecondSpell3())
        {
            SecondSpell3.SetActive(false);
            TutorialCanvas.SetActive(false);
            stateSystem.StartGameWave();
            // Fill in state here if needed.
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
            if (stateSystem.IsDash2())
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

        if (!stateSystem.IsFirstSpell1())
        {
            if (stateSystem.IsFirstSpell2())
            {
                FirstSpell2.SetActive(false);
                FirstSpell1.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell1);
            }

            if (stateSystem.IsFirstSpell3())
            {
                FirstSpell3.SetActive(false);
                FirstSpell2.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.FirstSpell2);
            }
        }

        if (!stateSystem.IsSecondSpell1())
        {
            if (stateSystem.IsSecondSpell2())
            {
                SecondSpell2.SetActive(false);
                SecondSpell1.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.SecondSpell1);
            }

            if (stateSystem.IsSecondSpell3())
            {
                SecondSpell3.SetActive(false);
                SecondSpell2.SetActive(true);
                stateSystem.SetTutorialState(StateSystem.TutorialState.SecondSpell2);
            }
        }
    }

    public void SelectSpell1(bool isFireSpell)
    {
        NextState();
        gameManager.EnableSpell1(isFireSpell);
    }

    public void SelectSpell2(bool isEarthSpell)
    {
        NextState();
        gameManager.EnableSpell2(isEarthSpell);
    }
}
