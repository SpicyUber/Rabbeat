using System.Collections;
using UnityEngine;


public class ScanWave : MonoBehaviour
{
    private HeadphoneAttack _headphone;
    private bool _groundFound;
    // Start is called before the first frame update
    void Start()
    {

        

        
      
    
    
    }

    public void Shoot(int songKey, HeadphoneAttack headphone, float scanSpeed)
    {
        _headphone=headphone;
        GetComponent<AudioSource>().pitch= Mathf.Pow(1.059463f, songKey);
        GetComponent<AudioSource>().Play();
       
        StartCoroutine(Spread(scanSpeed));
    }


    IEnumerator Spread(float scanSpeed)
    {
         
        while (transform.localScale.magnitude<125*(1+PlayerPrefs.GetInt("AttackLvl"))) {
            transform.localScale = transform.localScale + Vector3.one * Time.deltaTime * scanSpeed * (1 + PlayerPrefs.GetInt("AttackLvl"));
            yield return null;
        }
       
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {


        
    }
    private void FindGround()
    {
        _groundFound = false;
        string str = "";
        //  RaycastHit[] hits = Physics.SphereCastAll(transform.parent.position, 0.4f, (-1)* transform.parent.up, maxDistance);
        RaycastHit[] hits = Physics.RaycastAll(transform.parent.position + Vector3.up * 2f, -transform.parent.up, 100);
        foreach (RaycastHit hit in hits)
        {
            str += "shadow from " + transform.parent.name + "  -> name: " + hit.collider.name + ", tag: " + hit.collider.tag + "distance" + hit.distance + ";\n ";





            if (hit.collider.CompareTag("Ground"))
            {

                transform.position = hit.point + hit.normal / 1000 + hit.normal * 0.1f;
                this.transform.up = hit.normal;



                _groundFound = true;
                break;
            };

        }

    }

        private void OnTriggerEnter(Collider other)
    {
        enemyScript e = other.gameObject.GetComponent<enemyScript>();
         if (e != null && _headphone!=null && EnemyIsUnique(e))
        {
            _headphone.Enemies.Enqueue(e);
            _headphone.AddMark(e);
        }
    }
    private void OnDestroy()
    {
        _headphone.ScanIsDone();
    }

    private bool EnemyIsUnique(enemyScript e)
    {
       
        foreach(enemyScript es in  _headphone.Enemies)
        {
            if (es == null) continue;

            if (es == e) return false;


        }

        return true;

    }
}
