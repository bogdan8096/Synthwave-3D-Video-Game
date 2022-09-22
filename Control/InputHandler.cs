using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { set; get; }

    // variables used to check if there is a swiping input
    private bool m_IsSwiping = false;
    private Vector2 m_StartingTouch;
    // variables used to store what kind of swiping input has been made
    private bool m_SwipeLeft    = false;
    private bool m_SwipeRight   = false;
    private bool m_SwipeUp      = false;
    private bool m_SwipeDown    = false;

    // variables used for keyboard input detection
    private bool k_LeftKeyPressed   = false;
    private bool k_RightKeyPressed  = false;
    private bool k_UpKeyPressed     = false;
    private bool k_DownKeyPressed   = false;

    // defining public getters for each input action
    public bool LeftInputActive     { get { return k_LeftKeyPressed || m_SwipeLeft || GameControllerBluetooth.Instance.GameControllerLeftActive; } }
    public bool RightInputActive    { get { return k_RightKeyPressed || m_SwipeRight || GameControllerBluetooth.Instance.GameControllerRightActive; } }
    public bool UpInputActive       { get { return k_UpKeyPressed || m_SwipeUp || GameControllerBluetooth.Instance.GameControllerUpActive; } }
    public bool DownInputActive     { get { return k_DownKeyPressed || m_SwipeDown || GameControllerBluetooth.Instance.GameControllerDownActive; } }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInputs();
        HandleMobileInputs();
    }

    private void HandleKeyboardInputs()
    {
        // save in the following variables the state of each button
        k_LeftKeyPressed    = Input.GetKeyDown(KeyCode.LeftArrow);
        k_RightKeyPressed   = Input.GetKeyDown(KeyCode.RightArrow);
        k_UpKeyPressed      = Input.GetKeyDown(KeyCode.UpArrow);
        k_DownKeyPressed    = Input.GetKeyDown(KeyCode.DownArrow);
    }

    private void HandleMobileInputs()
    {
        // reset all swipe action variables
        m_SwipeLeft = m_SwipeRight = m_SwipeUp = m_SwipeDown = false;

        if (Input.touchCount == 1)
        {
            if (m_IsSwiping)
            {
                Vector2 diff = Input.GetTouch(0).position - m_StartingTouch;
                // Put difference in Screen ratio, but using only width, so the ratio is the same on both
                // axes (otherwise we would have to swipe more vertically...)
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);

                if (diff.magnitude > 0.01f) //we set the swip distance to trigger movement to 1% of the screen width
                {
                    if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                    {
                        if (diff.y < 0)
                        {
                            m_SwipeDown = true;
                        }
                        else
                        {
                            m_SwipeUp = true;
                        }
                    }
                    else
                    {
                        if (diff.x < 0)
                        {
                            m_SwipeLeft = true; ;
                        }
                        else
                        {
                            m_SwipeRight = true;
                        }
                    }

                    m_IsSwiping = false;
                }
            }

            // Input check is AFTER the swip test, that way if TouchPhase.Ended happen a single frame after the Began Phase
            // a swipe can still be registered (otherwise, m_IsSwiping will be set to false and the test wouldn't happen for that began-Ended pair)
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                m_StartingTouch = Input.GetTouch(0).position;
                m_IsSwiping = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                m_IsSwiping = false;
            }
        }
    }
}
