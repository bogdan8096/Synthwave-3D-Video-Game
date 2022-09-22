using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        animator.SetTrigger("isSpawned");
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            GameManager.Instance.UpdateCoinScore();
            animator.SetTrigger("isCollected");
            FindObjectOfType<AudioManager>().PlaySound("Currency");
        }
    }
}

