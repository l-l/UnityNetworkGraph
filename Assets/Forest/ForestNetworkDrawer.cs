using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Forest.Network;
using Vectrosity;
public class ForestNetworkDrawer : MonoBehaviour {

    NetworkData net;
    VectorLine line;
    List<Vector3> linePoints;
    public GameObject NodeObject;
    private GameObject[] nodeObjects;


    // Use this for initialization
    void Start () {
       net = NetworkData.ReadFromXml("Assets/Forest/borst.xml");

        linePoints = new List<Vector3>();// { new Vector3(20, 30), new Vector2(100, 50) };
        


        nodeObjects = new GameObject[net.getNodeCount()];
       
        for (int k=0;k<net.getNodeCount();k++)
        {
            nodeObjects[k] = Instantiate(NodeObject);
            nodeObjects[k].name = "node"+k;
            nodeObjects[k].transform.parent = transform;
         
        }


        // Nodes
        int i = 0;
        var na = net.getNodesArray();
        foreach (Node n in na)
        {
            nodeObjects[i].transform.localPosition = n.Pos;
            i++;
        }

        // LINES 
        var ea = net.getEdgesArray();
        foreach (var e in ea)
        {
            linePoints.Add(e.Node1.Pos);
            linePoints.Add(e.Node2.Pos);
        }
        line = new VectorLine("NetworkLines", linePoints, 1f, LineType.Discrete);
        line.drawTransform = transform;
//        line.points3 = linePoints;
        

    }
	
	// Update is called once per frame
	void Update () {

        line.Draw3D();
    }
}
