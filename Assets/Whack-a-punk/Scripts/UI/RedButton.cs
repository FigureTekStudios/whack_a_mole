using UnityEngine;

public class RedButton : MonoBehaviour, IHittable
{
    [SerializeField]
    private GameObject moveDownPoint;

    [SerializeField] private float timeToMoveUp;
    [SerializeField] private float timeToMoveDown;
    [SerializeField] private float stayDownTime;
    
    private Vector3 startPosition;
    private Vector3 bottomPosition;

    private float _timer;
    
    private ButtonState state = ButtonState.idle;
    
    private void Start()
    {
        startPosition = transform.position;
        bottomPosition = moveDownPoint.transform.position;
    }

    private void Update()
    {
        if (state == ButtonState.idle) return;
        
        _timer += Time.deltaTime;

        switch (state)
        {
            case ButtonState.moveDown:
                MoveDown();
                break;
            case ButtonState.moveUp:
                MoveUp();
                break;
            case ButtonState.stayDown:
                if (_timer >= stayDownTime)
                {
                    state = ButtonState.moveUp;
                    _timer = 0;
                }
                break;
        }
    }
    
    private void MoveDown()
    {
        float timeInPercent = _timer / timeToMoveUp;
        timeInPercent = Mathf.Clamp(timeInPercent, 0, 1);
        
        transform.position = Vector3.Lerp(startPosition, bottomPosition, timeInPercent);
        
        if (_timer >= timeToMoveDown)
        {
            state = ButtonState.stayDown;
            _timer = 0;
        }
    }
    
    private void MoveUp()
    {
        float timeInPercent = _timer / timeToMoveUp;
        timeInPercent = Mathf.Clamp(timeInPercent, 0, 1);
        
        transform.position = Vector3.Lerp(bottomPosition, startPosition, timeInPercent);
        
        if (_timer >= timeToMoveUp)
        {
            transform.position = startPosition;
            state = ButtonState.idle;
            _timer = 0;
        }
    }

    public void Hit()
    {
        if (state != ButtonState.idle || GameManager.Instance == null) return;
        
        Debug.Log(" Game Ended: " + GameManager.Instance.GameEnded + " Game Started: " + GameManager.Instance.GameStarted);

        if (GameManager.Instance.GameEnded)
        {
            state = ButtonState.moveDown;
        
            GameManager.Instance.RestartGame();
        } else if (!GameManager.Instance.GameStarted)
        {
            state = ButtonState.moveDown;
        
            GameManager.Instance.StartGame();
        }
        else if (GameManager.Instance.GameStarted)
        {
            state = ButtonState.moveDown;
            GameManager.Instance.UsePowerUp();
        }
    }
}