using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    public static BGM bGM;
    private void Awake() {
        if (bGM != null && bGM != this) {
            Destroy(this.gameObject);
            return;
        }
        bGM = this;
        DontDestroyOnLoad(this);
    }
}
