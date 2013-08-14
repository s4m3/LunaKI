using UnityEngine;
using System.Collections;
/// REALLY IMPORTANT NOTE.
/// When using the mesh morpher you should absolutely make sure that you turn
/// off generate normals automatically in the importer, or set the normal angle to 180 degrees.
/// When importing a mesh Unity automatically splits vertices based on normal creases.
/// However the mesh morpher requires that you use the same amount of vertices for each mesh and that
/// those vertices are laid out in the exact same way. Thus it wont work if unity autosplits vertices based on normals.
[RequireComponent(typeof(MeshFilter))]
public class AGMorph : MonoBehaviour {

    public Mesh[] m_Meshes;
	private bool morphRunning = false;
    public float  morphTime = 1.0F; /// The time it takes for one loop to complete
    private float runningTime = 0;

    private int    m_SrcMesh = -1;
    private int    m_DstMesh = -1;
    private float  m_Weight = -1;
    private Mesh   m_Mesh;
	
	private AGActor.LightState currentLightState;

    /// Set the current morph in    
    public void SetComplexMorph (int srcIndex, int dstIndex, float t)
    {
        if (m_SrcMesh == srcIndex && m_DstMesh == dstIndex && Mathf.Approximately(m_Weight, t))
            return;
        
        Vector3[] v0 = m_Meshes[srcIndex].vertices;
        Vector3[] v1 = m_Meshes[dstIndex].vertices;
        Vector3[] vdst = new Vector3[m_Mesh.vertexCount];
        for (int i=0;i<vdst.Length;i++)
            vdst[i] = Vector3.Lerp(v0[i], v1[i], t);

        m_Mesh.vertices = vdst;
        m_Mesh.RecalculateBounds();
    }

    /// t is between 0 and m_Meshes.Length - 1.
    /// 0 means the first mesh, m_Meshes.Length - 1 means the last mesh.
    /// 0.5 means half of the first mesh and half of the second mesh.
	public void SetMorph (float t)
    {
        int floor = (int)t;
        floor = Mathf.Clamp (floor, 0, m_Meshes.Length - 2);
        float fraction = t - floor;
        fraction = Mathf.Clamp(t - floor, 0.0F, 1.0F);
        SetComplexMorph (floor, floor + 1, fraction);
    }
    
    void Awake ()
    {
        MeshFilter filter  = GetComponent(typeof(MeshFilter)) as MeshFilter;
        
        // Make sure all meshes are assigned!
        for (int i=0;i<m_Meshes.Length;i++)
        {
        	if (m_Meshes[i] == null)
        	{	
        		Debug.Log("MeshMorpher mesh  " + i + " has not been setup correctly");
        		morphRunning = false;
        		return;
        	}
        }
		
		//  At least two meshes
		if (m_Meshes.Length < 2)
		{
			Debug.Log ("The mesh morpher needs at least 2 source meshes");
        	morphRunning = false;
        	return;
		}

        filter.sharedMesh = m_Meshes[0];
        m_Mesh = filter.mesh;
		int vertexCount = m_Mesh.vertexCount;
        for (int i=0;i<m_Meshes.Length;i++)
        {
        	if (m_Meshes[i].vertexCount != vertexCount)
        	{	
        		//Debug.Log("Mesh " + i + " doesn't have the same number of vertices as the first mesh");
        		morphRunning = false;
        		return;
        	}
        }
    }
	//ab hier @author:Simon (nur fuer mich(Simon) und David Strippgen wichtig)
    /*void Update ()
    {
        if (morphRunning)
        {
			float time;
			if(currentLightState == AGActor.LightState.Real)
			{
	            float deltaTime = Time.deltaTime * (m_Meshes.Length - 1) / morphTime;
	            runningTime += deltaTime;
	            time = Mathf.Clamp(runningTime, 0, m_Meshes.Length - 1);
	            SetMorph (time);
				if(time == 1)
				{
					morphRunning = false;
					currentLightState = AGActor.LightState.Dream;
				}
			} else if(currentLightState == AGActor.LightState.Dream)
			{
				float deltaTime = Time.deltaTime * (m_Meshes.Length - 1) / morphTime;
	            runningTime -= deltaTime;
	            time = Mathf.Clamp(runningTime, 0, m_Meshes.Length - 1);
	            SetMorph (time);
				if(time == 0) 
				{
					morphRunning = false;
					currentLightState = AGActor.LightState.Real;
				}
			}
        }
    }
	
	public void morphTo(AGActor.LightState lightState)
	{
		if(currentLightState == lightState) return;
		
		if(currentLightState == AGActor.LightState.Dream) runningTime = 1;
		else runningTime = 0;
		
		morphRunning = true;
	}
	*/
 }