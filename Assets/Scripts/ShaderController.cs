using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    public GameObject GameMat01;
    float mTIntensity = 0;
    bool keyPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.K))TriggerMaterial01();
        ResetMaterial01();
        
    }

    void TriggerMaterial01()
    {
        mTIntensity = Mathf.Clamp(mTIntensity + 0.4f, 0, 2);
        GameMat01.GetComponent<MeshRenderer>().material.SetFloat("_TouchIntensity", mTIntensity);

    }

    void ResetMaterial01()
    {   
        if(mTIntensity > 0) 
        {
            mTIntensity = Mathf.Clamp(mTIntensity - 0.1f, 0, 2);
            GameMat01.GetComponent<MeshRenderer>().material.SetFloat("_TouchIntensity", mTIntensity);  
        } 
    }
}
