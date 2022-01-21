using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFonction : MonoBehaviour
{
    // Start is called before the first frame update
    public void Onclick()
    {
        GameManager.instance.StartGame();
        this.gameObject.SetActive(false);
    }
}
