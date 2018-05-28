using System;
using UnityEngine;
using System.Collections.Generic;
namespace Forest.Network
{ 
    public class Node
    {

            public string ID;
            public string Text;
            public string NodeType;
            public Vector3 Pos;
            public bool isFixed=false;
            public bool isRoot = false;
        public List<Node> parentNodes=new List<Node>();  
        public List<Node> childNodes=new List<Node>();



        public Node()
	    {
	    }
        public Node(string _id, string _text, string _type, Vector3 _pos)
        {
            ID = _id;
            Debug.Log("new Node id " + _id);
            Text = _text;
            NodeType = _type;
            Pos = _pos;
        }
        public Node( string _text)
        {
            ID = "-1";
     
            Text = _text;
            NodeType = "default";
            Pos = Vector3.zero;
        }
        public Node hasChildWithText(string _text)
        {
            foreach (var child in childNodes)
            {
                if (String.Equals(child.Text, _text))
                {
                    return child;
                }
            }
            return null;
        }

        public Node hasParentWithText(string _text)
        {
            foreach (var parent in parentNodes)
            {
                if (String.Equals(parent.Text, _text))
                {
                    return parent;
                }
            }
            return null;
        }

        public void addChild(Node _n)
        {
            childNodes.Add(_n);
        }
        public void addParent(Node _n)
        {
            parentNodes.Add(_n);
        }
   

    }
}
