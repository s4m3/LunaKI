using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
 
 
public static class Persist
{
    public static void serializeObjectToFile( object obj, string filename )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
 
        // Create a StreamWriter to write the XML to disk
        StreamWriter sw = new StreamWriter( path );
        XmlSerializer serializer = new XmlSerializer( obj.GetType() );
        serializer.Serialize( sw, obj );
        sw.Close();
    }
	
	public static void serializeWaypointsToFile(List<Waypoint> waypoints, string filename)
	{
		string path = Persist.pathForDocumentsFile(filename);
		
		StreamWriter sw = new StreamWriter(path);
		foreach(Waypoint wp in waypoints)
		{
			sw.WriteLine(wp.id.ToString() + "_" + wp.originalPoint.ToString());
		}
		sw.Close();
	}
	
	public static void serializeNodesToFile(List<Node> nodes, string filename)
	{
		Debug.Log ("Nodes Count Serializing: " + nodes.Count);
		string path = Persist.pathForDocumentsFile(filename);
		
		StreamWriter sw = new StreamWriter(path);
		foreach(Node node in nodes)
		{
			//Debug.Log("node id: " + node.id);
			//Debug.Log("node successor count: " + node.Successors.Count);
			string successorList = "";
			foreach(Node successor in node.Successors)
			{
				successorList += successor.id + "*";
			}
			string successorListFinal = successorList.Length > 0 ? successorList.Substring(0, successorList.Length-1) : "";
			string posString = node.position.x.ToString() + ";" + node.position.y.ToString() + ";" + node.position.z.ToString(); 
			sw.WriteLine(node.id.ToString() + "_" + posString + "_" + successorListFinal);
		}
		sw.Close();
	}
	
	public static List<string> deserializeNodesFromFile(string filename)
	{
		string path = Persist.pathForDocumentsFile( filename );
		Debug.Log ("Path: " + path);
        StreamReader sr = new StreamReader( path );
		string line;
		List<string> stringList = new List<string>();
		while((line = sr.ReadLine()) != null)
		{
			stringList.Add(line);
		}
		//Debug.Log(stringList.Count);
		return stringList;
	}
   
   
    public static object deserializeObjectFromFile( string filename, Type type )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
        object obj;
 
        StreamReader sr = new StreamReader( path );
        XmlSerializer serializer = new XmlSerializer( type );
        obj = serializer.Deserialize( sr );
        sr.Close();
 
        return obj;
    }
   
   
    public static void writeStringToFile( string str, string filename )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
       
        StreamWriter sw = new StreamWriter( path );
        sw.Write( str );
        sw.Close();
    }
   
   
    public static string readStringFromFile( string filename )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
       
        StreamReader sr = new StreamReader( path );
        string str = sr.ReadToEnd();
        sr.Close();
       
        return str;
    }
   
   
    public static string pathForDocumentsFile( string filename )
    {
        // Application.dataPath returns
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data
#if !UNITY_IPHONE
        string path = Path.Combine( Application.dataPath, "Documents" );
        Debug.Log( path );
#else
        string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
#endif
       
        // Strip application name
        path = path.Substring( 0, path.LastIndexOf( '/' ) );
       
        return Path.Combine( Path.Combine( path, "Documents" ), filename );
    }
 
 
}
