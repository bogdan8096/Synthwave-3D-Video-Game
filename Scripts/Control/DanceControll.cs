using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceControll : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Dance()
    {
        anim.SetInteger("danceNumber", Random.Range(0, 12));
        anim.SetTrigger("changeDance");
    }
}
