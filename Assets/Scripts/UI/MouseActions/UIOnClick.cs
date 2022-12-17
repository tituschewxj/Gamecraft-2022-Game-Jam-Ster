using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    PlayerManager playerManager; // sound for click;
    // Start is called before the first frame update
    void Start() {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    public void OnPointerDown (PointerEventData eventData) 
    {   
        playerManager.SetMouseDown(true);
    }
    public void OnPointerUp (PointerEventData eventData) {
        playerManager.SetMouseDown(false);
    }
}
