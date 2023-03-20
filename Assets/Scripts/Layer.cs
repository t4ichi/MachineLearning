using System;
using System.Collections.Generic;

namespace MachineLearning
{
    public class Layer
    {
        public List<Node> nodes = new List<Node>();

        /// <summary>
        /// コンストラスタ
        /// </summary>
        public Layer(int numNodes)
        {
            for (int i = 0; i < numNodes; i++)
            {
                Node node = new Node();
                nodes.Add(node);
            }
        }

        /// <summary>
        /// ノード全結合のための関数
        /// </summary>
        public void ConnectDensely(Layer rightLayer)
        {
            foreach (Node node in nodes)
            {
                foreach (Node nextNode in rightLayer.nodes)
                {
                    node.Connect(nextNode);
                }
            }
        }

        public void InitWeight()
        {
            foreach (Node node in nodes)
            {
                node.InitWeight();
            }
        }

        /// <summary>
        /// ノードにデータを入力するための関数
        /// </summary>
        public void SetInputData(double[] input)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].value = input[i];
            }
        }

        public void CalcForward()
        {
            foreach (Node node in nodes)
            {
                node.CalcForward();
            }
        }

        public void UpdateWeight(double alpa)
        {
            foreach (Node node in nodes)
            {
                node.UpdateWeight(alpa);
            }
        }
    }
}

