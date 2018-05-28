using System;
using UnityEngine;
using Eppy;

namespace Forest.Network
{
    public class Edge
    {
        private Node node1;
        private Node node2;
        private string edgeType;
        private Tuple<string,string> nodeIDs; // fast access
        public string ID;

        public Edge(String _id, string _type,Node _node1, Node _node2)
        {
            Debug.Log("New edge " + _id);
            edgeType = _type;
            ID = _id;
            Node1 = _node1;
            Node2 = _node2;
        }

        public Edge( Node _node1, Node _node2)
        {
            //Debug.Log("New edge " + _id);
            edgeType = "default";
            ID = "-1";
            Node1 = _node1;
            Node2 = _node2;
        }

        public Node Node1
        {
            get
            {
                return node1;
            }

            set
            {
                node1 = value;
               // nodeIDs = new Tuple<string,string>(node1.ID,node1.ID);
            }
        }

        public Node Node2
        {
            get
            {
                return node2;
            }

            set
            {
                node2 = value;
               // nodeIDs = new Tuple<string, string>(node2.ID, node2.ID);
            }
        }
        public string EdgeType
        {
            get
            {
                return edgeType;
            }
            set
            {
                edgeType = value;
            }
        }

        public Edge()
        {
        }


        // fast access 
        public Tuple<string,string> getNodeIDs()
        {
            return nodeIDs;
        }
    }
}
