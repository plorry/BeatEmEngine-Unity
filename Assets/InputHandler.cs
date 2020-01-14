using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

	private static InputHandler inputHandler;
    public BattlerBehaviour player1;
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
		EventManager.StartListening("KeyDown-Space", player1.Jump);
		EventManager.StartListening("KeyUp-Space", player1.ResetJump);
		EventManager.StartListening("KeyDown-A", player1.Attack);
		EventManager.StartListening("KeyUp-A", player1.ResetAttack);
    }
}