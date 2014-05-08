﻿using UnityEngine;
using System.Collections;



public class ARCameraController : MonoBehaviour{
	//GameScript
	private GameScript game;

	// Modes
	// Modes will determine which code to run to increase efficiency of calculations
	// Low level modes are nested in high level modes
	// All modes are ENUM and their values are listed below
	// High level modes: Pregame, Play & Explore
	private const int PREGAME_MODE = 0;
	private const int PLAY_MODE = 1;
	private const int EXPLORE_MODE = 2;
	private const int END_GAME_MODE = 3;

	// GUI Low level modes: Init, Start, Instructions
	private const int INIT_MODE = 0;
	private const int START_MODE = 1;
	private const int INSTR_MODE = 2;

	// UPDATE Low level modes: Selection, Movement, Scaling Mode
	private const int SEL_MODE = 3;
	private const int MOVE_MODE = 4;
	private const int SCALE_MODE = 5;
	private const int BASE_EX_MODE = 6;


	//Variables containing current modes for GAME
	public int highMode;
	public int lowMode;
	
	//GameObjects
	public GameObject ImageTarget;
	public GameObject Board;

	// Player Mode
	public bool p1;
	public bool p2;
	public GUIStyle turn;
	public GUIStyle redP2;
	public GUIStyle redP1;

	//Screen dimensions
	private int height;
	private int width;

	// Debug
	public GUIText sysinfo;

	public Transform debugSphere;
	public Color defaultColor = Color.white;

	// Focus	
	public int focusDistance;
	private Ray focusRay;
	private GameObject focusedObject;
	private Vector3 focusPoint;
	
	// Selection
	private GameObject selectedObject;

	//bools handling menu
	private bool targetFound;


	// Use this for initialization
	void Start () {
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		height = Screen.width;
		width = Screen.height;
		focusedObject = null;
		selectedObject = null;
		p1 = false;
		p2 = false;
		targetFound = false;
		highMode = PREGAME_MODE;
		lowMode = INIT_MODE;
		turn = new GUIStyle ();
		turn.normal.textColor = Color.black;
		game = Board.GetComponent<GameScript> ();
	}


	// Update is called once per frame
	void Update () {
		if (!targetFound) {
			targetFound = ImageTarget.GetComponent<ImageTargetTracking> ().targetFound;
			if(targetFound){
				lowMode = START_MODE;
			}
		}

		// Point Ray through center of camera viewport 
		focusRay = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		focusPoint = transform.position + focusRay.direction * focusDistance;

		// Debug
		// Draw ray during pause mode with sphere at the end
		Debug.DrawRay (transform.position, focusRay.direction * focusDistance);
		debugSphere = GameObject.FindGameObjectWithTag("debug").transform;
		debugSphere.position = focusPoint;
		if (highMode == PREGAME_MODE){
			// DON'T DO ANYTHING!

		} else if (highMode == PLAY_MODE) {
			// PLAY MODE

			if(lowMode == SEL_MODE){
				// Setup variable to hold raycast information
				RaycastHit hit;

				if (Physics.Raycast (focusRay, out hit, focusDistance)) {
					Debug.Log("Hit tag: " + hit.transform.tag);
					if(hit.transform.tag == "piece"){
						// Highlight Piece that's focused on
						Debug.Log("hit piece");
						hit.transform.GetComponent<GamePieceScript>().focus();
						focusedObject = hit.transform.gameObject;
					} else {
						Debug.Log("Searching");
						if(focusedObject != null){
							focusedObject.GetComponent<GamePieceScript>().resetColor();
							focusedObject = null;
						}
					}
				}else if(focusedObject!=null){

						focusedObject.GetComponent<GamePieceScript>().resetColor();
						focusedObject = null;
				}

			} else if(lowMode == MOVE_MODE){
				selectedObject.transform.position = focusPoint;
			}


		} else if(highMode == EXPLORE_MODE){
			// EXPLORE MODE
			if(lowMode==BASE_EX_MODE){
			
			}else if(lowMode == SCALE_MODE){

			}
		}

		//menu = true;
		
	}
	
	void OnGUI () {
		if (highMode == PREGAME_MODE){
			// PREGAME MODE
			if(lowMode == INIT_MODE){
			// INITIALIZE MODE
			// Prompt user to find image target
				GUI.Label((new Rect (((width/2)-50),0, 150, 50)),"Find the Image Target", turn);
			}else if(lowMode == START_MODE){
			// START MODE
			// Prompt user to start game
				if (GUI.Button (new Rect ((width/2), (height-100), 150, 100), "START")) {
					lowMode = INSTR_MODE;
				}
			}else if(lowMode == INSTR_MODE){
			//
				turn.normal.textColor = Color.magenta;
				GUI.Label((new Rect (((width/2)-50),0, 150, 50)),"INSTRUCTIONS", turn);
				GUI.Label((new Rect (((width/5)-50),50, 550, 300)),"Black moves first. Players then alternate moves.Moves are allowed only on\nthe dark squares, so pieces always move diagonally. Single pieces are\nalways limited to forward moves (toward the opponent). A piece making a non-capturing\nmove (not involving a jump) may move only one square. A piece making a capturing \nmove (a jump) leaps over one of the opponent's pieces, landing in a straight \ndiagonal line on the other side. Only one piece may be captured in a single jump; \nhowever, multiple jumps are allowed on a single turn.When a piece is captured, it is \nremoved from the board.If a player is able to make a capture, there is no option -- the \njump must be made. If more than one capture is available, the player is free \nto choose whichever he or she prefers.When a piece reaches the furthest row from the player \nwho controls that piece, it is crowned and becomes a king. One of the pieces which \nhad been captured is placed on top of the king so that it is twice as high as a single \npiece. Kings are limited to moving diagonally, but may move both forward and backward. \n(Remember that single pieces, i.e. non-kings, are always limited to forward moves.)\nKings may combine jumps in several directions -- forward and backward -- on the same turn. \nSingle pieces may shift direction diagonally during a multiple capture turn, but must \nalways jump forward (toward the opponent). A player wins the game when the opponent cannot make a move. \nIn most cases, this is because all of the opponent's pieces have been captured, but it \ncould also be because all of his pieces are blocked in.", turn);
				if (GUI.Button (new Rect ((width/2), (height-100), 150, 100), "Continue")) {
					highMode = PLAY_MODE;
					lowMode = SEL_MODE;
					p1 = true;
					game.makeBoard ();
				}
			}

		} else if (highMode == PLAY_MODE) {
			// PLAY MODE
			
			if(lowMode == SEL_MODE){
				// SELECTION MODE
				///////////////
				// Selection //
				///////////////
				/// These two buttons should be on top of each other;
				/// Pick Up
				/// Place
				if (GUI.Button (new Rect ((width-250), (height-100), 150, 100), "Select Piece")) {
					if(focusedObject!=null){
						selectedObject = focusedObject;
						lowMode = MOVE_MODE;
						selectedObject.GetComponent<GamePieceScript>().select();
					}
				}
				/////////////////
				// Translation //
				/////////////////
				if (GUI.RepeatButton (new Rect ((width-100), (height-100), 100, 100), "Explore")) {
					highMode = EXPLORE_MODE;
				}
				
				/////////////////
				//   NewGame   //
				/////////////////
				if (GUI.RepeatButton (new Rect ((width-350), (height-100), 100, 100), "Resign")) {
					//translateMode = true;
				}


			}else if(lowMode == MOVE_MODE){
				// MOVEMENT MODE
				selectedObject.transform.position = focusPoint;
				if (GUI.Button (new Rect ((width-250),(height-100), 150, 100), "Place Piece")) {
					lowMode = SEL_MODE;
					selectedObject.GetComponent<GamePieceScript>().resetColor();
					selectedObject = null;
				}
				
				/////////////////
				//   NewGame   //
				/////////////////
				if (GUI.RepeatButton (new Rect ((width-350), (height-100), 100, 100), "Resign")) {
					//translateMode = true;
				}
			}
			
			
		} else if(highMode == EXPLORE_MODE){
			// EXPLORE MODE
			// Translation via Joystick

			/////////////////
			// Translation //
			/////////////////
			if (GUI.RepeatButton (new Rect ((width-100), (height-100), 100, 100), "SCALE")) {
				lowMode = SCALE_MODE;
			}else{
				lowMode = BASE_EX_MODE;
			}
			
			/////////////////
			//   NewGame   //
			/////////////////
			if (GUI.RepeatButton (new Rect ((width-350), (height-100), 100, 100), "Resign")) {
				//translateMode = true;
			}
			if(lowMode == SCALE_MODE){
				// SCALE MODE
				GUI.Label((new Rect (((width/2)-50),0, 150, 50)),"YOUR TURN PLAYER 1", turn);	
			}
		}
		//player message
		if (p1) {

			turn.normal.textColor = Color.magenta;
			GUI.Label((new Rect (((width/2)-50),0, 150, 50)),"YOUR TURN PLAYER 1", turn);
			p2 = false;
		}
		if (p2) {
			turn.normal.textColor = Color.magenta;
			GUI.Label((new Rect (((width/2)-50),0, 150, 50)),"YOUR TURN PLAYER 2", turn);
			p1 = false;
		}
		int number = 8;
		redP2.normal.textColor = Color.red;
		redP1.normal.textColor = Color.white;
		GUI.Label((new Rect ((width-100), 0, 100, 100)),"Player 1", redP1);
		GUI.Label((new Rect ((width-100), 50, 100, 100)),"Count: "+ number, redP1);
		GUI.Label((new Rect ((width-100), 100, 100, 100)),"Score: "+ number, redP1);
		GUI.Label((new Rect ((width-50), 0, 100, 100)),"Player 2", redP2);
		GUI.Label((new Rect ((width-50), 50, 100, 100)),"Count: "+ number, redP2);
		GUI.Label((new Rect ((width-50), 100, 100, 100)),"Score: "+ number, redP2);
	}
	
	void focusOnObject (Transform obj){
		if (focusedObject != null) {
			focusedObject.GetComponent<GamePieceScript>().resetColor();
		}
		Debug.Log("I'm looking at " + obj.name);
		GamePieceScript piece = obj.GetComponent<GamePieceScript> ();
		piece.focus ();
		focusedObject = obj.gameObject;
		sysinfo.text = "Focused on object: " + focusedObject.transform.name;
	}
	
	void defocusObject (){
		Debug.Log ("DEFOCUS: " + focusedObject.transform.name);
		focusedObject.GetComponent<GamePieceScript> ().resetColor ();
		focusedObject = null; 
		sysinfo.text = "Approach a game piece to select it.";
	}
	
	void selectObject (Transform obj){
		// Change object color to red
		Debug.Log ("Selecting Object: " + obj.transform.name);
		obj.gameObject.GetComponent<GamePieceScript> ().select ();
		selectedObject = obj.gameObject;
		focusedObject = null;

		sysinfo.text = "Selected Object: " + selectedObject.transform.name;
	}
	
	void deselectObject (){
		selectedObject.GetComponent<GamePieceScript> ().resetColor ();
		selectedObject = null;
		sysinfo.text = "Approach a game piece to select it.";
	}
}
