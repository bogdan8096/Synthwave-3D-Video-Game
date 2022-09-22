using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController characterController;
    public SpawnManager spawnManager;

    private float k_TrackSpeedToJumpAnimSpeedRatio = 1.0f;
    private float animSpeed;
    private bool m_Running;
    private int m_CurrentLane;
    private Vector3 m_TargetPosition;
    private float m_LaneOffset = 3.0f;
    private float m_SpeedRatio;
    private float m_WorldDistance;

    private bool m_Sliding;
    private float m_SlideLength = 8.0f;
    private float m_SlideStart;

    private bool m_Jumping;
    private float m_JumpLength = 6.5f;
    private float m_JumpHeight = 2.0f;
    private float m_JumpStart;
    private bool m_JumpPressed = false;

    private float characterControllerHeight;
    private Vector3 characterControllerCenter;

    private Vector3 verticalTargetPosition;
    private float m_VerticalVelocity;
    private float m_Gravity;
    private float m_GravityScale = 0.75f;
    private float m_FastFallVelocity = -10.0f;

    private float m_Speed;
    private float m_MinSpeed = 10.0f;
    private float m_MaxSpeed = 20.0f;
    private float m_ScaledSpeed;

    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 2.0f;
    private float speedIncreaseAmount = 0.1f;

    private const float turnSpeed = 0.1f;

    private void Awake() 
    {
        AnimationHandler.Instance.SetAnimatorState(false);
        AnimationHandler.Instance.SetPauseAnimationSpeed(false);

        characterController = GetComponent<CharacterController>();

        m_Running = false; 
        m_CurrentLane = 1;
        m_TargetPosition = Vector3.zero;

        characterControllerHeight = characterController.height;
        characterControllerCenter = characterController.center;

        m_Speed = m_MinSpeed;
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        AnimationHandler.Instance.RebindAnimator();
    }

    // Update is called once per frame
    private void Update() 
    {
        if (!m_Running) return;

        m_SpeedRatio = (m_Speed - m_MinSpeed) / (m_MaxSpeed - m_MinSpeed);
        m_ScaledSpeed = m_Speed * Time.deltaTime;
        m_WorldDistance += m_ScaledSpeed;

        //Gather the inputs on which lane we should be
        if (InputHandler.Instance.LeftInputActive) 
        {
            ChangeLane(-1);
        }
        else if (InputHandler.Instance.RightInputActive) 
        {
            ChangeLane(1);
        }
        else if (InputHandler.Instance.UpInputActive) 
        {
            Jump();
        }
        else if (InputHandler.Instance.DownInputActive) 
        {
            if (!m_Sliding) Slide();
        }

        verticalTargetPosition.x = (m_TargetPosition - transform.position).x * m_Speed;
        verticalTargetPosition.z = m_Speed;

        if (m_Sliding) 
        {
            // Slide time isn't constant but the slide length is (even if slightly modified by speed, to slide slightly further when faster).
            // This is for gameplay reason, we don't want the character to drasticly slide farther when at max speed.
            float correctSlideLength = m_SlideLength * (1.0f + m_SpeedRatio);
            float ratio = (m_WorldDistance - m_SlideStart) / correctSlideLength;
            if (ratio >= 1.0f) 
            {
                // We slid to (or past) the required length, go back to running
                StopSliding();
            }
        }

        if (m_Jumping) 
        {
            // Same as with the sliding, we want a fixed jump LENGTH not fixed jump TIME. Also, just as with sliding,
            // we slightly modify length with speed to make it more playable.
            float correctJumpLength = m_JumpLength * (1.0f + m_SpeedRatio);
            float ratio = (m_WorldDistance - m_JumpStart) / correctJumpLength;
            if (ratio >= 1.0f) 
            {
                StopJumping();
            }
            else if(m_JumpPressed) 
            {
                verticalTargetPosition.y = m_VerticalVelocity; //Mathf.Sin(ratio * Mathf.PI) * m_JumpHeight;
                m_JumpPressed = false;
            }
            verticalTargetPosition.y += m_Gravity * m_GravityScale * Time.deltaTime;
        }

        characterController.Move(verticalTargetPosition * Time.deltaTime);

        
        //Rotate the player to the target position
        Vector3 direction = characterController.velocity;
        if (direction != Vector3.zero) 
        {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, turnSpeed);
        }

        if ((m_Speed < m_MaxSpeed) && (Time.time - speedIncreaseLastTick > speedIncreaseTime)) 
        {
            speedIncreaseLastTick = Time.time;
            m_Speed += speedIncreaseAmount;
        }
        GameManager.Instance.UpdateModifierScore(m_Speed - m_MinSpeed);
    }

    private void ChangeLane(int direction) 
    {
        if (!m_Running) 
            return;

        int targetLane = m_CurrentLane + direction;

        if (targetLane < 0 || targetLane > 2)
            // Ignore, we are on the borders.
            return;

        m_CurrentLane = targetLane;
        m_TargetPosition = new Vector3((m_CurrentLane - 1) * m_LaneOffset, 0, 0);

        AudioManager.Instance.PlaySound("Dodge");
        GameControllerBluetooth.Instance.ResetGameControllerInputs();
    }

    private void Jump() 
    {
       if (!m_Running) 
            return;

        if (!m_Jumping) 
        {
            if (m_Sliding) 
                StopSliding();

            AudioManager.Instance.PlaySound("Jump");

            float correctJumpLength = m_JumpLength * (1.0f + m_SpeedRatio);
            m_JumpStart = m_WorldDistance;
            animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (m_Speed / correctJumpLength);

            m_VerticalVelocity = (2 * m_JumpHeight * m_Speed) / (correctJumpLength / 2);
            m_Gravity = (-2 * m_JumpHeight * Mathf.Pow(m_Speed, 2)) / Mathf.Pow((correctJumpLength / 2), 2); ;

            AnimationHandler.Instance.SetAnimationSpeed(animSpeed);
            AnimationHandler.Instance.SetJumpAnimation();
            m_Jumping = true;
            m_JumpPressed = true;
        }
    }

    private void StopJumping() 
    {
        if (m_Jumping) 
        {
            m_Jumping = false;
            AnimationHandler.Instance.ResetJumpAnimation();
        }
    }

    private void Slide()
    {
        if (!m_Running) 
            return;

        if (!m_Sliding)
        {
            if (m_Jumping)
            {
                StopJumping();
                verticalTargetPosition.y = m_FastFallVelocity;
            }

            AudioManager.Instance.PlaySound("Slide");

            float correctSlideLength = m_SlideLength * (1.0f + m_SpeedRatio);
            m_SlideStart = m_WorldDistance;
            animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (m_Speed / correctSlideLength);

            AnimationHandler.Instance.SetAnimationSpeed(animSpeed);
            AnimationHandler.Instance.SetSlideAnimation();
            m_Sliding = true;

            float tempControllerHeight = characterController.height;
            tempControllerHeight /= 3;
            characterController.height = tempControllerHeight;

            Vector3 tempControllerCenter = characterController.center;
            tempControllerCenter.y /= 3;
            characterController.center = tempControllerCenter;
        }
    }

    private void StopSliding() 
    {
        if (m_Sliding)
        {
            AnimationHandler.Instance.ResetSlideAnimation();
            m_Sliding = false;
            characterController.height = characterControllerHeight;
            characterController.center = characterControllerCenter;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (transform.position.z >= 20 && hit.gameObject.tag == "Obstacle") 
        {
            StopRunning();
            GameManager.Instance.OnPlayerDeath();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "SpawnTrigger") spawnManager.SpawnTriggerEntered();
        if (other.tag == "JumpPowerUP") 
        {
            JumpPowerup.Instance.SetCollisionParameters();
            StartCoroutine(JumpPowerupActive()); 
        }
        if (other.tag == "CoinPowerUP")
        {
            CoinPowerup.Instance.SetCollisionParameters();
            StartCoroutine(CoinPowerupActive());
        }
        if (other.tag == "ScorePowerUP")
        {
            ScorePowerup.Instance.SetCollisionParameters();
            StartCoroutine(ScorePowerupActive());
        }
    }

    public void StartRunning()
    {
        m_Running = true;
    }
    public void StopRunning()
    {
        m_Running = false;
    }

    IEnumerator JumpPowerupActive()
    {
        float tempJumpLength = m_JumpLength;
        float tempJumpHeight = m_JumpHeight;

        m_JumpLength = 10.0f;
        m_JumpHeight = 4.0f;

        yield return new WaitForSeconds(10f);

        m_JumpLength = tempJumpLength;
        m_JumpHeight = tempJumpHeight;
    }

    IEnumerator CoinPowerupActive()
    {
        int coinMultiplier = 5;
        GameManager.Instance.CoinMultiplier = coinMultiplier;

        yield return new WaitForSeconds(20f);

        GameManager.Instance.CoinMultiplier = 1;
    }

    IEnumerator ScorePowerupActive()
    {
        int scoreMultiplier = 2;
        GameManager.Instance.ScoreMultiplier = scoreMultiplier;

        yield return new WaitForSeconds(20f);

        GameManager.Instance.CoinMultiplier = 1;
    }
}
