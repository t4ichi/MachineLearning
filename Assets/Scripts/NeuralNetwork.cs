using System;
using System.Collections.Generic;
using System.IO;

namespace MachineLearning
{
    public class NeuralNetWork
    {
        public List<Layer> layers = new List<Layer>();

        /// <summary>
        /// 層を作成し、隣の層と全結合させる関数
        /// </summary>
        public void AddLayer(int numberOfNodes)
        {
            Layer newLayer = new Layer(numberOfNodes);
            if (layers.Count > 0)
            {
                layers[layers.Count - 1].ConnectDensely(newLayer);
            }
            layers.Add(newLayer);
        }

        /// <summary>
        /// 入力層にデータを入力後、出力層に向けて計算を行う関数
        /// </summary>
        /// <param name="inputdata"></param>
        public void CalcForward(double[] inputdata)
        {
            //入力層にデータを入力
            layers[0].SetInputData(inputdata);
    
        for (int i = 0; i < layers.Count; i++)
            {
                layers[i].CalcForward();
            }
        }

        /// <summary>
        /// 出力層から認識結果を取得する関数
        /// </summary>
        public int GetMaxOutPut()
        {
            int max = 0;
            double maxValue = 0.0;
            //出力層のノードの中から最大値を見つける
            for (int i = 0; i < layers[layers.Count - 1].nodes.Count; i++)
            {
                if (layers[layers.Count - 1].nodes[i].value > maxValue)
                {
                    max = i;
                    maxValue = layers[layers.Count - 1].nodes[i].value;
                }
            }
            return max;
        }

        /// <summary>
        /// ニューラルネットワーク全体の重みを調整して更新する
        /// </summary>
        public void InitWeight()
        {
            foreach (Layer layer in layers)
            {
                layer.InitWeight();
            }
        }

        /// <summary>
        /// ニューラルネットワーク全体の重みを調整して更新する
        /// </summary>
        /// <param name="alpa"></param>
        public void UpdateWeight(double alpa)
        {
            foreach (Layer layer in layers)
            {
                layer.UpdateWeight(alpa);
            }
        }

        public void CalcError(double[] trainData)
        {
            //出力層のノード数を取得
            int numOutput = layers[layers.Count - 1].nodes.Count;
            //出力層のノードの誤差を計算
            for (int i = 0; i < numOutput; i++)
            {
                layers[layers.Count - 1].nodes[i].CalcError(trainData[i]);
            }
            //隠れ層のノードの誤差を計算
            for (int i = layers.Count - 2; i >= 0; i--)
            {
                foreach (Node node in layers[i].nodes)
                {
                    node.CalcError();
                }
            }
        }

        /// <summary>
        /// ニューラルネットワークの構成と全体の重みをファイル「weight.dat」に保存する関数
        /// </summary>
        public void SaveWeight()
        {
            using (FileStream fsWeights = new FileStream(@"weight.dat", FileMode.OpenOrCreate))
            {
                using (BinaryWriter bwWeights = new BinaryWriter(fsWeights))
                {
                    bwWeights.Write(layers.Count);
                    foreach (Layer layer in layers)
                    {
                        bwWeights.Write(layer.nodes.Count);
                    }

                    foreach (Layer layer in layers)
                    {
                        foreach (Node node in layer.nodes)
                        {
                            foreach (Edge edge in node.inputs)
                            {
                                bwWeights.Write(edge.weight);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ファイルを読み込んで、ニューラルネットワークを作成し、全体の重みを設定する関数
        /// </summary>
        public void LoadWeight()
        {
            using (FileStream fsWeights = new FileStream(@"weight.dat", FileMode.Open))
            {
                using (BinaryReader brWeights = new BinaryReader(fsWeights))
                {
                    layers = new List<Layer>();
                    //層の数を読み込む
                    int numLayers = brWeights.ReadInt32();
                    for (int i = 0; i < numLayers; i++)
                    {
                        int numNodes = brWeights.ReadInt32();
                        AddLayer(numNodes);
                    }
                    foreach (Layer layer in layers)
                    {
                        foreach (Node node in layer.nodes)
                        {
                            foreach (Edge edge in node.inputs)
                            {
                                edge.weight = brWeights.ReadDouble();
                            }
                        }
                    }
                }
            }
        }
    }
}

