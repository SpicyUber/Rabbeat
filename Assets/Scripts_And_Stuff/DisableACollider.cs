using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableACollider : MonoBehaviour
{
    private GameObject _player;
    [Header("DISABLE IF PLAYER IS ")]
    public float Height;
    [Header("HIGHER THAN THE PLATFORM")]
    public Collider Collider;
    // Start is called before the first frame update
    void Start()
    {
      _player=  GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    { if (_player == null || Collider == null) return;
        Collider.enabled = !((_player.transform.position.y - transform.position.y)>Height);
    }
}
