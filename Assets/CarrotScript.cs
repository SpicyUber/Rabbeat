using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CarrotScript : MonoBehaviour
{
    private bool _isCollected = false;
    private bool _isCollecting = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bounce(playerScript p)
    {
        if (_isCollected || _isCollecting) return;
        _isCollecting = true;
       
        StartCoroutine(BounceRoutine(p));

    }
    IEnumerator BounceRoutine(playerScript p)
    {
        int radius = 5;
        float time = 0f;
        float degrees = 0;
        Vector3 startPos = transform.position;
        float distance = (new Vector3(p.transform.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * degrees), p.transform.position.y, p.transform.position.z + radius * Mathf.Sin(Mathf.Deg2Rad * degrees)) - transform.position).magnitude/35f;
        yield return new WaitForSeconds(distance);
        while(time<= 0.3f) { transform.position= startPos + (new Vector3(p.transform.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * degrees), p.transform.position.y, p.transform.position.z + radius * Mathf.Sin(Mathf.Deg2Rad * degrees)) - startPos) * time / 0.3f; yield return null; time += Time.deltaTime; }

        time = 0f;
        
        
        while (time < 0.6f) { degrees = 360 * time / 0.6f; transform.position = new Vector3(p.transform.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * degrees), p.transform.position.y,p.transform.position.z + radius * Mathf.Sin(Mathf.Deg2Rad * degrees)); yield return null; time += Time.deltaTime; }
        while ((p.transform.position - transform.position).magnitude > 0.2f) { transform.position += (p.transform.position - transform.position).normalized *25f * Time.deltaTime; yield return null;  }
        CustomGameManager cgm = FindFirstObjectByType<CustomGameManager>();
        if(cgm.currentPlayerHealth<cgm.playerHealth)
        cgm.currentPlayerHealth+= 1;
        p.audioManager.source.PlayOneShot(p.audioManager.munch);
        gameObject.SetActive(false);
    }
}
