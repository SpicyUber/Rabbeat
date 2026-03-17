using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public float DelayInSeconds;
    public string SceneName;

    private bool _started = false;
    private float _t = 0;

    private void Update()
    {
        if (!_started) return;

        _t += Time.deltaTime;

        if(_t>=DelayInSeconds)
        SceneManager.LoadScene(SceneName);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        _started = true;
    }
}
