using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

using Forest.Network;

public class GraphCSCtrl : MonoBehaviour {

    public ComputeShader graphCS;
    public GameObject inst;
    int count=0;
    public GameObject[] gos;

    // FR config  > to calculate "k" and "maxDist" in init
    float w = 50;
    float h = 50;
    //---
    float maxDist; // calculated, based on area
    float curMaxDist; // current maxDist, used for cooling
    float k;// calculated fr constant
   

    // shader
    Node[] nodeData;
    //Node[] nodeDataOutput;
    ComputeBuffer _nodeBuffer;

    ComputeBuffer _edgesBuffer;
    Vector2[] edgesData;

    Vector4[] edges;
    int kernel_fruchtermanReingold;


    //drawing
    VectorLine line;
    List<Vector3> linePoints;

    // Use this for initialization
    void Start () {


        // shader init

        
        InitShader();


        gos = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                gos[i] = Instantiate<GameObject>(inst);
            gos[i].transform.parent = this.gameObject.transform;

            }


        //    RunShader();
        linePoints = new List<Vector3>();// { new Vector3(20, 30), new Vector2(100, 50) };
        
        // VECTROSITY LINE DRAWING
        
        line = new VectorLine("VectroLine", linePoints, 2f, LineType.Discrete);
        line.points3.Add(new Vector3(0, 0, 0));
        line.points3.Add(new Vector3(5, 2, 3));
        line.drawTransform = this.gameObject.transform;
        line.Draw3DAuto();
         
        for (int j = 0; j < edgesData.Length; j++)
        {
            linePoints.Add(new Vector3(0, 0, 0));
        }
       

    }
	
	// Update is called once per frame
	void Update () {
        RunShader();
        updateInstances(nodeData);

    }

    void RunShader()
    {
        curMaxDist *= 0.99f; // cooling of the algorithm
       // maxDist = 1f;
        if (curMaxDist>0.01)
            { 
            graphCS.SetFloat("maxDist", curMaxDist);
            kernel_fruchtermanReingold = graphCS.FindKernel("FruchtermanReingold");

            _nodeBuffer = new ComputeBuffer(nodeData.Length, sizeof(float) * 4 + sizeof(uint));  // how many bytes a single element in the buffer is, >>  5 float variables in the struct -> 1x float3 1xbool  +1x float 
            _nodeBuffer.SetData(nodeData);
            graphCS.SetBuffer(kernel_fruchtermanReingold, "nodeBuffer", _nodeBuffer);

       
        
            graphCS.Dispatch(kernel_fruchtermanReingold, nodeData.Length, 1, 1);

            _nodeBuffer.GetData(nodeData); //nodeDataOutput
                                           // Debug.Log(nodeData[0].pos);
                                           // Debug.Log("Debug="+nodeData[0]._debug);

            _nodeBuffer.Release();
                //  _edgesBuffer.Release();

            }
    }
    void InitShader()
    {

        // network data from text


        //createNetworkData();
        // DUMMY DATA -------------------------------------------------------
        createNetworkDummyData();
        //--------------------------------------------------------------------

        k = Mathf.Sqrt((w* h) / count);
    
        maxDist = Mathf.Sqrt(w*h) / 10f;
        curMaxDist = maxDist;
        // SHADER INIT

        // mandatory variable initialisation
        graphCS.SetInt("Iterations", 1);    // optional: use several iterations of the same algorithm per frame
        graphCS.SetFloat("k", k);           // constant determined by area
        graphCS.SetFloat("maxDist", maxDist);            // the maximum step length, originally "temperature" that can be used to "cool" the system as it approaches its finl state
        graphCS.SetVector("force", new Vector3(0,0, 0)); // an additional force, applied to all nodes
        //graphCS.SetInt("numEdges", edges.Length);
        //graphCS.SetVectorArray("edges", edges);

        _edgesBuffer = new ComputeBuffer(edgesData.Length, sizeof(float) * 2);  // how many bytes a single element in the buffer is, >>  5 float variables in the struct -> 1x float3 1xbool  +1x float
        _edgesBuffer.SetData(edgesData);

        graphCS.SetBuffer(kernel_fruchtermanReingold, "edgeBuffer", _edgesBuffer);
        
    }

   

    void close()
    {
        _edgesBuffer.Release();
        _nodeBuffer.Release();

    }

   
    void updateInstances(Vector3[] pos)
    {
        for (int i=0;i<count;i++)
        {
            gos[i].transform.localPosition = pos[i];
        }
    }

    void updateInstances(Node[] node)
    {
        for (int i = 0; i < count; i++)
        {
            gos[i].transform.localPosition = node[i].pos;
        }

        linePoints.Clear();
        foreach (var e in edgesData)
        {
            linePoints.Add(nodeData[(int)e.x].pos);
            linePoints.Add(nodeData[(int)e.y].pos);
        }
        line.points3 = linePoints;
    }

    void createNetworkData()
    {
        NetworkData net = NetworkData.CreateFromTextFile("Assets/Forest/testext.txt");

        var ea = net.getEdgesArray();
        var na = net.getNodesArray();
        count = na.Count;

        nodeData = new Node[count];
        for (int i = 0; i < nodeData.Length; i++)
        {
            nodeData[i] = new Node();
            nodeData[i].pos = Random.insideUnitSphere * 20f;
            nodeData[i].isStatic = false;// (Random.value < 0.1);

        }
        nodeData[0].pos = Vector3.zero;
        nodeData[0].isStatic = true;
       // edges = new Vector4[ea.Count];
        edgesData = new Vector2[ea.Count];

        for (int i = 0; i < ea.Count; i++)
        {// Debug.Log("node id =") ;
            var n1 = Mathf.Max(0, int.Parse(ea[i].Node1.ID) - 1);
            var n2 = Mathf.Max(0, int.Parse(ea[i].Node2.ID) - 1);

           // edges[i] = new Vector4(n1, n2, 0, 0);
            edgesData[i] = new Vector2(n1, n2);
        }
    }

    void createNetworkDummyData()
    {
        count = 40;
        nodeData = new Node[count];
        for (int i = 0; i < count; i++)
        {
            nodeData[i] = new Node();
            nodeData[i].pos = Random.insideUnitSphere * 10f;
        }
        edgesData = new Vector2[count + 10];
        for (int j = 0; j < (count + 10); j++)
        {
            var v = Random.Range(0, count);
            while (v == (j % count))
            {
                v = Random.Range(0, count);
            }
            edgesData[j] = new Vector2(j % count, v);
        }

    }


    // the cpu version of the shader struct
    struct Node
    {
        public Vector3 pos;
        public bool isStatic;
        public float _debug;
    }
}
