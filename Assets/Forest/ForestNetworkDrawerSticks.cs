using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Forest.Network;
using Vectrosity;
public class ForestNetworkDrawerSticks : MonoBehaviour
{

    NetworkData net;
    VectorLine line;
    List<Vector3> linePoints;
    public GameObject NodeObject;
    private GameObject[] nodeObjects;
    public GameObject[] EdgeObject;
    private List<GameObject> edgeObjects;

    // Use this for initialization
    void Start()
    {
        //net = NetworkData.ReadFromXml("Assets/Forest/FruchtermanReingoldLayout.xml");  //borst.xml
        net = NetworkData.ReadFromXml("Assets/Forest/borst.xml");  //borst.xml
        linePoints = new List<Vector3>();// { new Vector3(20, 30), new Vector2(100, 50) };



        nodeObjects = new GameObject[net.getNodeCount()];

     
        // Nodes
        int i = 0;
        var na = net.getNodesArray();
        foreach (Node n in na)
        {
            nodeObjects[i] = Instantiate(NodeObject);
            nodeObjects[i].transform.parent = transform;
            nodeObjects[i].transform.localPosition = n.Pos;
            TextMesh tm = nodeObjects[i].gameObject.transform.GetChild(0).GetComponent<TextMesh>();
            tm.text = n.Text;
            i++;
        }
        
        // LINES 
        edgeObjects = new List<GameObject>();
        var ea = net.getEdgesArray();
        foreach (var e in ea)
        {
            GameObject go = Instantiate(EdgeObject[Random.Range(0,EdgeObject.Length)]);
            Vector3 edge = e.Node2.Pos - e.Node1.Pos;
            float len = edge.magnitude* 0.75f;
            go.transform.localScale = new Vector3(len, len, len);
            go.transform.parent = transform;
         
            go.transform.localPosition = e.Node1.Pos+edge*0.5f;
            go.transform.localRotation = Quaternion.LookRotation(edge);
            edgeObjects.Add(go);
            linePoints.Add(e.Node1.Pos);
            linePoints.Add(e.Node2.Pos);
        }
        line = new VectorLine("NetworkLines", linePoints, 1f, LineType.Discrete);
        line.drawTransform = transform;
        //        line.points3 = linePoints;


    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(.1f, .13f, .05f));
        line.Draw3D();
    }
}
