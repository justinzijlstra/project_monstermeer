using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JK_random_animationStart : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
       Animator anim = GetComponent<Animator>();
       anim.Play("Mainline", 0, Random.Range(0.0f, 800.0f));
       //GetComponent<Animation>()["Mainline"].time = Random.Range(0.0f, GetComponent<Animation>()["Mainline"].length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
