using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TechTweaking.Bluetooth;

public class GameControllerBluetooth : MonoBehaviour
{
	public static GameControllerBluetooth Instance { get; set; }
	private BluetoothDevice device;

	private bool j_LeftJoystickInput	= false;
	private bool j_RightJoystickInput	= false;
	private bool j_UpJoystickInput		= false;
	private bool j_DownJoystickInput	= false;
	private bool j_Button0Input			= false;
	private bool j_Button1Input			= false;
	private bool j_Button2Input			= false;
	private bool j_Button3Input			= false;

	// variables used to get buttons states from the received bytes
	private const int bitButtonStateFirstByte	= 1;
	private const int bitButtonStateSecondByte	= 2;
	private const int bitMaskFirstByte	= 1 << bitButtonStateFirstByte;
	private const int bitMaskSecondByte = 1 << bitButtonStateSecondByte;

	private int b_LastStateButton0 = 0;
	private int b_LastStateButton1 = 0;
	private int b_LastStateButton2 = 0;
	private int b_LastStateButton3 = 0;
	private int j_LastJoystickValueX = 0;
	private int j_LastJoystickValueY = 0;

	public bool GameControllerLeftActive		{ get { return j_LeftJoystickInput || j_Button2Input;  } }
	public bool GameControllerRightActive		{ get { return j_RightJoystickInput || j_Button3Input; } }
	public bool GameControllerUpActive			{ get { return j_UpJoystickInput || j_Button0Input; } }
	public bool GameControllerDownActive		{ get { return j_DownJoystickInput || j_Button1Input; } }

	void Awake()
	{
		DontDestroyOnLoad(this);

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}


		BluetoothAdapter.enableBluetooth();//Force Enabling Bluetooth

		device = new BluetoothDevice();

		/*
		 * We need to identefy the device either by its MAC Adress or Name (NOT BOTH! it will use only one of them to identefy your device).
		 */
		device.Name = "HC-05";
		//device.MacAddress = "XX:XX:XX:XX:XX:XX";


		/*
		 * 10 equals the char '\n' which is a "new Line" in Ascci representation, 
		 * so the read() method will retun a packet that was ended by the byte 10. simply read() will read lines.
		 */
		device.setEndByte(255);

	}

    private void Start()
    {
		connect();
		StartReadingCoroutine();
	}

    public void connect()
	{
		device.connect();
	}

	public void disconnect()
	{
		device.close();
	}

    private void OnApplicationQuit()
    {
		disconnect();
	}

    public void StartReadingCoroutine()
    {
		/*
		 * The ManageConnection Coroutine will start when the device is ready for reading.
		 */
		device.ReadingCoroutine = ManageConnection;
	}

	public void StopReadingCoroutine()
	{
		/*
		 * The ManageConnection Coroutine will stop.
		 */
		StopCoroutine(ManageConnection(device));
	}

	IEnumerator ManageConnection(BluetoothDevice device)
	{
		while (device.IsConnected && device.IsReading)
		{

			//polll all available packets
			BtPackets packets = device.readAllPackets();

			if (packets != null)
			{

				/*
				 * parse packets, packets are ordered by indecies (0,1,2,3 ... N),
				 * where Nth packet is the latest packet and 0th is the oldest/first arrived packet.
				 * 
				 * Since this while loop is looping one time per frame, we only need the Nth(the latest potentiometer/joystick position in this frame).
				 * 
				 */
				int N = packets.Count - 1;
				//packets.Buffer contains all the needed packets plus a header of meta data (indecies and sizes) 
				//To get a packet we need the INDEX and SIZE of that packet.
				int indx = packets.get_packet_offset_index(N);
				int size = packets.get_packet_size(N);

				if (size == 4)
				{
					// packets.Buffer[indx] equals lowByte(x1) and packets.Buffer[indx+1] equals highByte(x2)
					int val1 = (packets.Buffer[indx + 1] << 8) | packets.Buffer[indx];
					int val2 = (packets.Buffer[indx + 3] << 8) | packets.Buffer[indx + 2];

					// get button states from received bits
					int b_StateButton0 = (val1 & bitMaskFirstByte) >> bitButtonStateFirstByte;
					int b_StateButton1 = (val1 & bitMaskSecondByte) >> bitButtonStateSecondByte;

					// Reset button state bits; Shift back 3 bits, because there was << 3 in Arduino
					val1 &= ~(1 << bitMaskFirstByte);
					val1 &= ~(1 << bitMaskSecondByte);
					val1 = val1 >> 3;

					// get button states from received bits
					int b_StateButton2 = (val2 & bitMaskFirstByte) >> bitButtonStateFirstByte;
					int b_StateButton3 = (val2 & bitMaskSecondByte) >> bitButtonStateSecondByte;

					// Reset button state bits; Shift back 3 bits, because there was << 3 in Arduino
					val2 &= ~(1 << bitMaskFirstByte);
					val2 &= ~(1 << bitMaskSecondByte);
					val2 = val2 >> 3;

					//#########Converting val1, val2 into something similar to Input.GetAxis (Which is from -1 to 1)#########
					//since any val is from 0 to 1023
					int j_JoystickValueX, j_JoystickValueY;

					if (val1 < 340) j_JoystickValueX = -1;
					else if (val1 < 640) j_JoystickValueX = 0;
					else j_JoystickValueX = 1;

					if (val2 < 340) j_JoystickValueY = -1;
					else if (val2 < 640) j_JoystickValueY = 0;
					else j_JoystickValueY = 1;

					bool setGameControllerInputs = SetNewGameControllerInputs(j_JoystickValueX, j_JoystickValueY, b_StateButton0, b_StateButton1, b_StateButton2, b_StateButton3);
					//float Axis1 = ((float)val1 / 1023f) * 2f - 1f;
					//float Axis2 = ((float)val2 / 1023f) * 2f - 1f;

					if(setGameControllerInputs)
                    {
						SetJoystickActiveInputs(j_JoystickValueX, j_JoystickValueY, b_StateButton0, b_StateButton1, b_StateButton2, b_StateButton3);
					}

					//x: -axis2; y = axis1; axis112: -1; 0; 1
					/*
					 * 
					 * Now Axis1 or Axis2  value will be in the range -1...1. Similar to Input.GetAxis
					 * Check out :
					 * 
					 * https://docs.unity3d.com/ScriptReference/Input.GetAxis.html
					 * 
					 * https://unity3d.com/learn/tutorials/topics/scripting/getaxis
					 */
					StoreLastInputs(j_JoystickValueX, j_JoystickValueY, b_StateButton0, b_StateButton1, b_StateButton2, b_StateButton3);
				}
			}
			yield return null;
		}
	}

	private void SetJoystickActiveInputs(int joystickx, int joysticky, int button0, int button1, int button2, int button3)
    {
		switch(joystickx)
        {
			case -1:
				j_RightJoystickInput = true;
				break;
			case  0:
				j_LeftJoystickInput	 = false;
				j_RightJoystickInput = false;
				break;
			case  1:
				j_LeftJoystickInput = true;
				break;
        }

		switch (joysticky)
		{
			case -1:
				j_DownJoystickInput = true;
				break;
			case 0:
				j_DownJoystickInput = false;
				j_UpJoystickInput   = false;
				break;
			case 1:
				j_UpJoystickInput = true;
				break;
		}

		if (button0 == 1) j_Button0Input = true;
		else j_Button0Input = false;

		if (button1 == 1) j_Button1Input = true;
		else j_Button1Input = false;

		if (button2 == 1) j_Button2Input = true;
		else j_Button2Input = false;

		if (button3 == 1) j_Button3Input = true;
		else j_Button3Input = false;
	}
	private void StoreLastInputs(int joystickx, int joysticky, int button0, int button1, int button2, int button3)
    {
		b_LastStateButton0 = button0;
		b_LastStateButton1 = button1;
		b_LastStateButton2 = button2;
		b_LastStateButton3 = button3;

		j_LastJoystickValueX = joystickx;
		j_LastJoystickValueY = joysticky;
	}
	private bool SetNewGameControllerInputs(int joystickx, int joysticky, int button0, int button1, int button2, int button3)
    {
		if (joystickx != j_LastJoystickValueX) return true;
		if (joysticky != j_LastJoystickValueY) return true;
		if (button0 != b_LastStateButton0) return true;
		if (button1 != b_LastStateButton1) return true;
		if (button2 != b_LastStateButton2) return true;
		if (button3 != b_LastStateButton3) return true;
		return false;
    }

	public void ResetGameControllerInputs()
    {
		j_LeftJoystickInput		= false;
		j_RightJoystickInput	= false;
		j_UpJoystickInput		= false;
		j_DownJoystickInput		= false;
		j_Button0Input = false;
		j_Button1Input = false;
		j_Button2Input = false;
		j_Button3Input = false;
	}
}
