using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class textureSwitcherScript : MonoBehaviour
{
    public rhythmSystemScript rs;
    public Texture2D[] materials = new Texture2D[16];
    public Texture2D[] startMaterials = new Texture2D[16];
    private int lastBeat=-1;
    private bool switchBool = true;
    //materials is an array of texture alternatives to switch to on the beats
    // material[0]<=> material 1
    // Start is called before the first frame update
    void Start()
    {
        if (rs == null) throw new UnityException("rhythm system is null");
       
       

    }

    // Update is called once per frame
    void Update()
    {
       /* for (int i = 0; i < materialNames.Length; i++)
        {
            if (materials[i] == null) { Debug.Log("material" + i + "is null"); }
        }*/
        if (lastBeat==-1||(rs.beatMap[rs.beatIndex].isActive&&lastBeat!=rs.beatIndex)) { 
       for(int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {   if (switchBool)
                        this.transform.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = materials[i];
                    else this.transform.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = startMaterials[i];
            }

        }
       switchBool= !switchBool;
        lastBeat = rs.beatIndex;
        }

    }
}
