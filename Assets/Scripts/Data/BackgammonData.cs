using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NutData {
	public string color;
	public string line;
	public Vector2 position;
	public int number;
}

[Serializable]
public class MoveData {
	public int numberOfMove;
	public int numberOfHit;
	public int numberOfSingle;
	public int numberOfSingleOpponent;
	public int remainderMove;
	public List<SingleMoveData> moves;
}

[Serializable]
public class SingleMoveData {
	public string start;
	public string end;
	public int move;
}

[Serializable]
public class AnimationData {
	public GameObject go;
	public SingleMoveData move;
	public string color;
	public bool start;
	public string target;
	public float delay;
}

[Serializable]
public class TempMoveData {
	public int number;
	public string color;
	public SingleMoveData move;
}