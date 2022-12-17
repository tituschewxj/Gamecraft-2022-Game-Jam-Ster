using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    bool showTutorialText;
    UIController uIController;
    public float tweenTime = 0.1f;
    [SerializeField]
    private Sprite defaultCursor, clickableCursor, attackCursor, craftableCursor, unableToAttackCursor;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (FindObjectOfType<TutorialScene>()) {
            showTutorialText = true; // returns true if tutorial scene
            uIController = FindObjectOfType<UIController>();
        } else {
            showTutorialText = false;
        }
        
    }
    void Update() {
        Focus();
    }
    public void SetDefaultCursor() {
        spriteRenderer.sprite = defaultCursor;
        SetBounce(false);
    }
    public void SetClickableCursor() {
        spriteRenderer.sprite = clickableCursor;
        SetBounce(true);
    }
    public void SetAttackCursor() {
        spriteRenderer.sprite = attackCursor;
        SetBounce(true);
        if (showTutorialText) uIController.ChangeBottomText("Click to attack monster");
    }
    public void SetCraftableCursor() {
        spriteRenderer.sprite = craftableCursor;
        SetBounce(true);
        if (showTutorialText) uIController.ChangeBottomText("Combine Items in inventory with spells");
    }
    public void SetUnableToAttackCursor() {
        spriteRenderer.sprite = unableToAttackCursor;
        SetBounce(true);
        if (showTutorialText) uIController.ChangeBottomText("Insufficient items in inventory");
    }
    public void SetBounce(bool isBounceTrue) {
        if (isBounceTrue) {
            LeanTween.scale (gameObject, Vector3.one * 0.9f, tweenTime * 5).setLoopPingPong().setEase(LeanTweenType.easeInOutSine);
        } else {
            LeanTween.cancel(gameObject);
            LeanTween.scale (gameObject, Vector3.one, tweenTime);
        }
    }
    public void Focus() {
        if (Input.GetMouseButtonDown(0)) Cursor.visible = false;
    }

}
