using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateProperties : MonoBehaviour
{

    public int symbol;
    public int position;
    public bool inActive;

    public PlateState plateState;
    public SpriteRenderer spriteSymbol;

    GameplayManager gameManager;

    Animator anim;


    void Awake()
    {
        gameManager = FindObjectOfType<GameplayManager>();
        spriteSymbol = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameplayManager.enterState == GameplayManager.EnterState.New)
            inActive = false;
    }

    public IEnumerator OpeningAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetBool("IsOpening", true);
        plateState = PlateState.Opening;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetBool("IsOpening", false);
        plateState = PlateState.Open;

    }

    public IEnumerator CloseAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetBool("IsClosing", true);
        plateState = PlateState.Closing;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetBool("IsClosing", false);
        plateState = PlateState.Close;
    }

}
