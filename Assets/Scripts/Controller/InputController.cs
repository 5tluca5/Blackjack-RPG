using UniRx;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] GameController gameController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.F4))
        {
            gameController.SetGamePhase(GamConstant.GamePhase.ShuffleDeck);
        }
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            gameController.ReceivedInput(KeyCode.Mouse0);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            gameController.ReceivedInput(KeyCode.Mouse1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            gameController.ReceivedInput(KeyCode.E);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            gameController.ReceivedInput(KeyCode.Space);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gameController.ReceivedInput(KeyCode.D);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gameController.ReceivedInput(KeyCode.S);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            gameController.ReceivedInput(KeyCode.Q);
        }
    }
}
