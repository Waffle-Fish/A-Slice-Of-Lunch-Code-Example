using UnityEngine;
using UnityEngine.InputSystem;

public class CustomMouse_Level : MonoBehaviour
{
    [SerializeField] Sprite openGrab;
    [SerializeField] Sprite closeGrab;
    [SerializeField] Sprite fingerPoint;
    [SerializeField] Sprite knife;
    [SerializeField] Vector3 offset = Vector3.zero;
    SpriteRenderer spriteRenderer;
    public enum HandState { Point, Grab, Knife }

    HandState currentHand;
    private PlayerInputActions.PlayerActions playerInputActions;
    PlayerActions currentPlayerAction = PlayerActions.move;
    GameObject rotateObj;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rotateObj = transform.GetChild(2).gameObject;
    }

    private void OnEnable()
    {
        PlayerSwitchAction.OnPlayerActionChange += UpdateCursor;
    }

    void OnDisable()
    {
        PlayerSwitchAction.OnPlayerActionChange -= UpdateCursor;
    }

    private void Start()
    {
        Cursor.visible = false;
        spriteRenderer.sprite = fingerPoint;
        rotateObj.SetActive(false);
        playerInputActions = PlayerInputManager.Instance.PlayerActions;
    }

    private void Update()
    {
        if (currentPlayerAction == PlayerActions.move)
        {
            spriteRenderer.sprite = playerInputActions.LeftClick.IsInProgress() ? closeGrab : openGrab;
        }
        Vector2 mousePos = playerInputActions.MousePosition.ReadValue<Vector2>();
        Collider2D overlapCol = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(mousePos) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset));

        transform.position = Camera.main.ScreenToWorldPoint(mousePos) + ((spriteRenderer.sprite == knife) ? Vector3.zero : offset);
    }

    private void UpdateCursor(PlayerActions action)
    {
        currentPlayerAction = action;
        rotateObj.SetActive(false);
        switch (currentPlayerAction)
        {
            case PlayerActions.move:
                spriteRenderer.sprite = openGrab;
                break;
            case PlayerActions.slice:
                spriteRenderer.sprite = knife;
                break;
            case PlayerActions.rotate:
                spriteRenderer.sprite = fingerPoint;
                rotateObj.SetActive(true);
                break;
        }
    }
}
