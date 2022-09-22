using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePowerup : MonoBehaviour
{
    public static ScorePowerup Instance { set; get; }
    private Animator animator;
    public AudioSource source;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        source.enabled = false;
    }

    public void SetCollisionParameters()
    {
        animator.SetTrigger("isCollected");
        source.enabled = true;
        Invoke("objectDisable", 0.5f);
    }

    private void objectDisable()
    {
        gameObject.SetActive(false);
    }
}
