using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        if (collision.gameObject.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.hasWon = true;
            GetComponentInChildren<Light>().enabled = false;
            GameManager.instance.playerEndLevel();
            GameManager.instance.MoveBall(player._playerColor, player.transform.position);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
