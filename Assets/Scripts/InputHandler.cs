using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

	private static InputHandler inputHandler;
    public BattlerBehaviour player1;
    public BattlerBehaviour player2;
    private Event e;

    // singleton pattern
	public static InputHandler instance {
		get {
			inputHandler = FindObjectOfType (typeof(InputHandler)) as InputHandler;
			inputHandler.Init();
			return inputHandler;
		}
	}

	private void Init() {

	}

	void OnGUI(){
		e = Event.current;

		if (e.type == EventType.KeyDown || e.type == EventType.KeyUp) {
			// We listen for keyboard events
			EventManager.TriggerEvent(string.Format("{0}-{1}", e.type, e.keyCode));
		}
	}

    void Awake()
    {
        InitializeSinglePlayerControls();
    }

    public void InitializeSinglePlayerControls()
    {
        // TODO maybe there's a better way to map these, but whatever
        // Right now, these are pretty hard-mapped to the keyboard. We could maybed
        // use an intermediary map to go from input event to these listeners...
        EventManager.StartListening("keyDown-RightArrow", player1.GoRight);
        EventManager.StartListening("keyUp-RightArrow", player1.StopRight);
        EventManager.StartListening("keyDown-LeftArrow", player1.GoLeft);
        EventManager.StartListening("keyUp-LeftArrow", player1.StopLeft);
        EventManager.StartListening("keyDown-UpArrow", player1.GoUp);
        EventManager.StartListening("keyDown-DownArrow", player1.GoDown);
        EventManager.StartListening("keyUp-UpArrow", player1.StopUp);
        EventManager.StartListening("keyUp-DownArrow", player1.StopDown);
        // EventManager.StartListening("KeyDown-Space", player1.Jump);
        // EventManager.StartListening("KeyUp-Space", player1.ResetJump);
        EventManager.StartListening("keyDown-A", player1.Attack);
        EventManager.StartListening("keyUp-A", player1.ResetAttack);
		EventManager.StartListening("keyDown-S", player1.Block);
        EventManager.StartListening("keyUp-S", player1.StopBlock);
    }

    public void InitializeTwoPlayerControls()
    {
        // TODO maybe there's a better way to map these, but whatever
        // Right now, these are pretty hard-mapped to the keyboard. We could maybed
        // use an intermediary map to go from input event to these listeners...
        EventManager.StartListening("KeyDown-RightArrow", player1.GoRight);
        EventManager.StartListening("KeyUp-RightArrow", player1.StopRight);
        EventManager.StartListening("KeyDown-LeftArrow", player1.GoLeft);
        EventManager.StartListening("KeyUp-LeftArrow", player1.StopLeft);
        EventManager.StartListening("KeyDown-UpArrow", player1.GoUp);
        EventManager.StartListening("KeyDown-DownArrow", player1.GoDown);
        EventManager.StartListening("KeyUp-UpArrow", player1.StopUp);
        EventManager.StartListening("KeyUp-DownArrow", player1.StopDown);
        // EventManager.StartListening("KeyDown-RightShift", player1.Jump);
        // EventManager.StartListening("KeyUp-RightShift", player1.ResetJump);
        EventManager.StartListening("KeyDown-RightControl", player1.Attack);
        EventManager.StartListening("KeyUp-RightControl", player1.ResetAttack);

        EventManager.StartListening("KeyDown-D", player2.GoRight);
        EventManager.StartListening("KeyUp-D", player2.StopRight);
        EventManager.StartListening("KeyDown-A", player2.GoLeft);
        EventManager.StartListening("KeyUp-A", player2.StopLeft);
        EventManager.StartListening("KeyDown-W", player2.GoUp);
        EventManager.StartListening("KeyDown-S", player2.GoDown);
        EventManager.StartListening("KeyUp-W", player2.StopUp);
        EventManager.StartListening("KeyUp-S", player2.StopDown);
        // EventManager.StartListening("KeyDown-LeftShift", player2.Jump);
        // EventManager.StartListening("KeyUp-LeftShift", player2.ResetJump);
        EventManager.StartListening("KeyDown-LeftControl", player2.Attack);
        EventManager.StartListening("KeyUp-LeftControl", player2.ResetAttack);
    }

    public void ClearControlEvents()
    {
        // Cancel Player Controls
        EventManager.StopListening("KeyDown-RightArrow", player1.GoRight);
        EventManager.StopListening("KeyUp-RightArrow", player1.StopRight);
        EventManager.StopListening("KeyDown-LeftArrow", player1.GoLeft);
        EventManager.StopListening("KeyUp-LeftArrow", player1.StopLeft);
        EventManager.StopListening("KeyDown-UpArrow", player1.GoUp);
        EventManager.StopListening("KeyDown-DownArrow", player1.GoDown);
        EventManager.StopListening("KeyUp-UpArrow", player1.StopUp);
        EventManager.StopListening("KeyUp-DownArrow", player1.StopDown);
        EventManager.StopListening("KeyDown-Space", player1.Jump);
        EventManager.StopListening("KeyUp-Space", player1.ResetJump);
        EventManager.StopListening("KeyDown-A", player1.Attack);
        EventManager.StopListening("KeyUp-A", player1.ResetAttack);
    }
}