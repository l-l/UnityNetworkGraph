using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using System.Xml;
using Eppy;

namespace Forest.Network
{
    /// <summary>
    /// A simple undirected network implementation used for demonstration purposes
    /// </summary>
    public class NetworkData
    {

        List<Node> nodes;
        List<Edge> edges;
        Node root;
        /// <summary>
        /// The list of vertices
        /// </summary>
        List<string> nodeIDs = new List<string>();

        /// <summary>
        /// The list of edges
        /// </summary>
        List<Tuple<string, string>> edgeNodeIDs = new List<Tuple<string, string>>();

        /// <summary>
        /// Adds a vertex to the network
        /// </summary>
        /// <param name="v"></param>
       // [MethodImpl(MethodImplOptions.Synchronized)]
       public NetworkData()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
        }
        public void AddNode(Node n)
        {
          //  n.ID = (nodes.Count + 1).ToString();
            nodes.Add(n);
        }
        

        public void AddRootNode(Node n)
        {
        //    n.ID = (nodes.Count+1).ToString();
            n.isRoot = true;
            nodes.Add(n);
            root = n;
            
        }

        public Node getRoot()
        {
            return root;
        }

        public void AddEdge(Edge e)
        {
            if (e.Node1!=null)
            { 
         //   e.ID = (edges.Count + 1).ToString();
            edges.Add(e);
            
            edgeNodeIDs.Add(new Tuple<string,string>(e.Node1.ID, e.Node2.ID));
          //  Debug.Log("addingEdge " + e.Node1.ID + "/" + e.Node1.Text + " --- " + e.Node2.ID + "/" + e.Node2.Text + " total=" + edges.Count);
            }
            else
            {
                Debug.Log("note 1 is nul");
            }
        }
        
        public static NetworkData ReadFromXml(string file, char sep = ' ')
        {
            NetworkData net=new NetworkData();
            Debug.Log("loading");
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNodeList nodes = doc.SelectNodes("/Table/Nodes/Node");
            Debug.Log("loading done"+nodes.Count);
            foreach (XmlNode node in nodes)
            {
                Node n = new Node(node.SelectSingleNode("Node_id").InnerText,
                                node.SelectSingleNode("Node_text").InnerText,
                                 node.SelectSingleNode("Node_type").InnerText,
                                new Vector3((float)Convert.ToDouble(node.SelectSingleNode("Node_posX").InnerText),
                                            (float)Convert.ToDouble(node.SelectSingleNode("Node_posY").InnerText),
                                            (float)Convert.ToDouble(node.SelectSingleNode("Node_posZ").InnerText))
                                );
                   
                Debug.Log("lala "+n.Text);
                //        Debug.Log(node.Attributes.GetNamedItem("Node_id").Value);
                net.AddNode(n);

            }

            XmlNodeList edges = doc.SelectNodes("/Table/Edges/Edge");
            foreach (XmlNode edge in edges)
            { Node n1 = net.getNodeById(edge.SelectSingleNode("Edge_node1").InnerText);
                Node n2 = net.getNodeById(edge.SelectSingleNode("Edge_node2").InnerText);
                Edge e = new Edge(edge.SelectSingleNode("Edge_id").InnerText,
                                edge.SelectSingleNode("Edge_type").InnerText,
                                n1,
                                n2
                                );

                
                //        Debug.Log(node.Attributes.GetNamedItem("Node_id").Value);
                net.AddEdge(e);

            }



            Debug.Log("LOADING AND PARSING COMPLETE:  edges=" + net.getEdgeCount()+ " / nodes="+net.getNodeCount());
            return net;
        }
        public Node getNodeById(string _id)
        {
          //  Debug.Log("nodebyid " + _id);
            foreach (Node node in nodes)
            {
                if ( String.Equals(node.ID,_id)) return node;
            }
            return null;
        }

        public Node getNodeByText(string _txt)
        {
            //  Debug.Log("nodebyid " + _id);
            foreach (Node node in nodes)
            {
                if (String.Equals(node.Text, _txt)) return node;
            }
            return null;
        }

        public int getNodeCount()
        {
            return nodes.Count;
        }

        public int getEdgeCount()
        {
            return edges.Count;
        }

        public List<Node> getNodesArray()
        {
            return nodes;
        }

        public List<Edge> getEdgesArray()
        {
            return edges;
        }
        public List<Tuple<string,string>> getEdgeTuples()
        {
            return edgeNodeIDs;
        }

        public static NetworkData CreateFromTextFile(string file)
        {
            Debug.Log("create from text");
            string[] lines = System.IO.File.ReadAllLines(file);
            NetworkData net = new NetworkData();

            Node travNode=new Node();

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                if (net.getRoot() != null) travNode = net.getRoot();


                for (int i=0;i<words.Length;i++)
                {
                    if (net.getRoot() == null)
                    {
                        net.AddRootNode(new Node(words[i]));
                        travNode = net.getRoot();
                    }

                    else
                    {
                        if (!String.Equals(travNode.Text, words[i]))
                        { 
                            Node ch =  travNode.hasChildWithText(words[i]);
                            if (ch == null)
                            {
                                Node nn = new Node(words[i]);
                                nn.addParent(travNode);
                                nn.ID=(net.getNodeCount() + 1).ToString();
                                net.AddNode(nn);
                                travNode.addChild(nn);
                                Debug.Log("creating new node _" + nn.Text + "_ with parent " + travNode.Text);
                            

                                Edge ne = new Edge(travNode, nn);
                                ne.ID = (net.getEdgeCount() + 1).ToString();
                                net.AddEdge(ne);
                                travNode = nn;
                            }
                            else
                            {
                                travNode = ch;
                            
                            }
                        }
                    }
                    
                   
                }
             //   Debug.Log(net.getNodeCount());
            }

            return net;
        }

        
        public void SaveToXml(string file)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            XmlNode xtable=doc.CreateElement("Table");
            doc.AppendChild(xtable);

            XmlNode xnodes = doc.CreateElement("Nodes");
            

            foreach (Node node in nodes)
            {
                XmlNode xnode = doc.CreateElement("Node");
                    XmlNode xnode_id= doc.CreateElement("Node_id");
                    xnode_id.AppendChild(doc.CreateTextNode(node.ID.ToString()));
                    xnode.AppendChild(xnode_id);

                    XmlNode xnode_text = doc.CreateElement("Node_text");
                    xnode_text.AppendChild(doc.CreateTextNode(node.Text.ToString()));
                    xnode.AppendChild(xnode_text);

                    XmlNode xnode_type = doc.CreateElement("Node_type");
                    xnode_type.AppendChild(doc.CreateTextNode(node.NodeType.ToString()));
                    xnode.AppendChild(xnode_type);

                    XmlNode xnode_posX = doc.CreateElement("Node_posX");
                    xnode_posX.AppendChild(doc.CreateTextNode(node.Pos.x.ToString()));
                    xnode.AppendChild(xnode_posX);

                    XmlNode xnode_posY = doc.CreateElement("Node_posY");
                    xnode_posY.AppendChild(doc.CreateTextNode(node.Pos.y.ToString()));
                    xnode.AppendChild(xnode_posY);

                    XmlNode xnode_posZ = doc.CreateElement("Node_posZ");
                    xnode_posZ.AppendChild(doc.CreateTextNode(node.Pos.z.ToString()));
                    xnode.AppendChild(xnode_posZ);

                xnodes.AppendChild(xnode);
            }

            xtable.AppendChild(xnodes);


            XmlNode xedges = doc.CreateElement("Edges");
            foreach (Edge edge in edges)
            {
                Debug.Log("saving edges");
                XmlNode xedge = doc.CreateElement("Edge");
                    XmlNode xedge_id = doc.CreateElement("Edge_id");
                    xedge_id.AppendChild(doc.CreateTextNode(edge.ID.ToString()));
                    xedge.AppendChild(xedge_id);

                    XmlNode xedge_type = doc.CreateElement("Edge_type");
                    xedge_type.AppendChild(doc.CreateTextNode(edge.EdgeType.ToString()));
                    xedge.AppendChild(xedge_type);


                    XmlNode xedge_node1 = doc.CreateElement("Edge_node1");
                    xedge_node1.AppendChild(doc.CreateTextNode(edge.Node1.ID.ToString()));
                    xedge.AppendChild(xedge_node1);

                    XmlNode xedge_node2 = doc.CreateElement("Edge_node2");
                    xedge_node2.AppendChild(doc.CreateTextNode(edge.Node2.ID.ToString()));
                    xedge.AppendChild(xedge_node2);


                xedges.AppendChild(xedge);
            }

            xtable.AppendChild(xedges);
            doc.Save(file);
        }

    }
}
