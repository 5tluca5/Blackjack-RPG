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
    }
}
