using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public enum ClickState
    {
        Up = 0, Down, Hold, 
    };

    public GameObject[] clickPlate;
    public GameObject locationPlate;

    public ClickState clickState;

    GameplayManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameplayManager>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ClickHandler();
	}

    void ClickHandler()
    {
        transform.position = GetCurrentTouchPosition().GetValueOrDefault();

        if (Input.GetMouseButtonDown(0))
        {
            if (gameManager.isHinting)
                return;

            if (gameManager.isFinish)
                return;

            if (locationPlate == null)
                return;

            clickState = ClickState.Down;

            if (clickPlate[0] == null)
            {

                clickPlate[0] = locationPlate;
                StartCoroutine(clickPlate[0].GetComponent<PlateProperties>().OpeningAnimation(0));
                gameManager.ClickCount();

                return;
            }

            clickPlate[1] = locationPlate;

            if (clickPlate[0] == clickPlate[1])
            {
                clickPlate[1] = null;
                return;
            }

            if (clickPlate[0] != clickPlate[1])
            {
                if (locationPlate.GetComponent<PlateProperties>().inActive)
                    return;

                if (clickPlate[1].GetComponent<PlateProperties>().plateState == PlateState.Opening || clickPlate[1].GetComponent<PlateProperties>().plateState == PlateState.Closing || clickPlate[1].GetComponent<PlateProperties>().plateState == PlateState.Open)
                    return;

                if (clickPlate[0].GetComponent<PlateProperties>().symbol == clickPlate[1].GetComponent<PlateProperties>().symbol)
                {
                    StartCoroutine(clickPlate[1].GetComponent<PlateProperties>().OpeningAnimation(0));

                    clickPlate[0].GetComponent<PlateProperties>().inActive = true;
                    clickPlate[1].GetComponent<PlateProperties>().inActive = true;
                    gameManager.UpdatePair(-1);
                    gameManager.ClickCount();
                    gameManager.UpdateScore(100);

                    clickPlate[0] = clickPlate[1] = null;
                }
                else
                {

                    StartCoroutine(clickPlate[1].GetComponent<PlateProperties>().OpeningAnimation(0));
                    StartCoroutine(clickPlate[0].GetComponent<PlateProperties>().CloseAnimation(clickPlate[1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));
                    StartCoroutine(clickPlate[1].GetComponent<PlateProperties>().CloseAnimation(0));
                    gameManager.ClickCount();

                    clickPlate[0] = clickPlate[1] = null;
                }
            }

        }

        /*if (Input.GetMouseButton(0))
            clickState = ClickState.Hold;*/

        if (Input.GetMouseButtonUp(0))
        {
            if (gameManager.isFinish)
                return;

            clickState = ClickState.Up;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<PlateProperties>())
        {
            locationPlate = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlateProperties>())
        {
            locationPlate = null;
        }
    }

    Vector3? GetCurrentTouchPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;

        if (plane.Raycast(ray, out rayDistance))
        {
            return (ray.GetPoint(rayDistance));
        }

        return null;
    }
}
