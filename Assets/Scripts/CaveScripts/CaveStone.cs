using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveStone : MonoBehaviour
{
    public List<GameObject> caveStoneCells;
    private List<Cell> scriptCaveStoneCells;
    private Animator animator;
    private void Awake()
    {
        scriptCaveStoneCells = new List<Cell>();
        foreach (var cell in caveStoneCells)
        {
            scriptCaveStoneCells.Add(cell.GetComponent<Cell>());
        }
        animator = GetComponent<Animator>();
    }
    private bool haveCaveLvl = false, canAnimation = false;
    private AnimatorStateInfo currentStateInfo;
    private float animationLength;

    private void Update()
    {

        if (scriptCaveStoneCells.Where(e => e.nestedObject != null).Count() > 0)
        {
            haveCaveLvl = true;
            canAnimation = true;
        }
        else
        {
            haveCaveLvl = false;
        }
       
        if (haveCaveLvl && canAnimation)
        {
            StartAnimation("CaveStoneActivation");
        }
        else if (!haveCaveLvl && canAnimation)
        {

            StartAnimation("CaveStoneDeactivation");
        }

    }
    void StartAnimation(string animationName)
    {
        canAnimation = false;
        animator.Play(animationName);
        StartCoroutine(PlayAnimation());
    }
    private System.Collections.IEnumerator PlayAnimation()
    {
        // Ждем окончания реверсированной анимации
        yield return new WaitForSeconds(2);
        canAnimation = true;
    }

    public void StartAdventure()
    {
        if (haveCaveLvl)
        {
            SceneManager.LoadScene("Cave");
        }
    }

}