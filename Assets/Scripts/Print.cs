using UnityEngine;
using System.Collections;

public class Print : MonoBehaviour {
	public static bool ShowLog = true;
	public static bool ShowWarning = true;
	public static bool ShowError = true;
	public static uint LogLevel = 10;

	private Print(){}

	public static void Log( object message){
		Log (10, message);
	}

	public static void Log( uint logLevel, object message){
		if( ShowLog && logLevel <= LogLevel)
			Debug.Log("(LL) " + message);
	}

	public static void Warning (object message){
		Warning(10,message);
	}

	public static void Warning( uint logLevel, object message){
		if( ShowWarning && logLevel <= LogLevel)
			Debug.LogWarning("(WW) " + message);
	}

	public static void Error (object message){
		Error( 10, message);
	}

	public static void Error (uint logLevel, object message)
	{
		if (ShowError && logLevel <= LogLevel)
			Debug.LogError ("(EE) " + message);
	}

	public static void Assert (bool assert)
	{
		if (!assert) {
			Debug.LogError ("(EE) Assertion failed!");
			Debug.Break ();
		}
	}
	
}