using System;
using System.Collections.Generic;

namespace MachineLearning
{
    public class Node
    {
        public List<Edge> inputs = new List<Edge>();
        public List<Edge> outputs = new List<Edge>();
        public double inValue;
        public double value;
        public double error;
        static Random random = new Random();

        /// <summary>
        /// 活性化関数
        /// </summary>
        public double Activation(double val)
        {
            return 1.0 / (1.0 + Math.Exp(-val));
        }

        /// <summary>
        /// 活性化関数を微分した関数
        /// </summary>
        public double DActivevations(double val)
        {
            return (1.0 - val) * val;
        }

        /// <summary>
        /// 隣のノードと接続する関数
        /// </summary>
        public Edge Connect(Node right)
        {
            Edge edge = new Edge();
            edge.left = this;
            edge.right = right;
            right.inputs.Add(edge);
            this.outputs.Add(edge);
            return edge;
        }

        /// <summary>
        /// 出力値を計算する関数
        /// </summary>
        public void CalcForward()
        {
            if (inputs.Count == 0) return;

            inValue = 0.0;
            foreach (Edge edge in inputs)
            {
                inValue += edge.left.value * edge.weight;
            }
            //出力値を算出
            value = Activation(inValue);
        }

        /// <summary>
        /// 正規分布乱数を生成する関数
        /// </summary>
        public static double GetRandom()
        {
            double r1 = random.NextDouble();
            double r2 = random.NextDouble();
            return (Math.Sqrt(-2.0 * Math.Log(r1)) * Math.Cos(2.0 * Math.PI * r2)) * 0.1;
        }

        /// <summary>
        /// 重みを乱数で初期化する関数
        /// </summary>
        public void InitWeight()
        {
            foreach (Edge edge in inputs)
            {
                edge.weight = GetRandom();
            }
        }

        /// <summary>
        /// (手順1) 誤差計算(出力そう)の関数
        /// </summary>
        public void CalcError(double t)
        {
            error = t - value;
        }

        /// <summary>
        /// (手順2)誤差計算(隠れ層)の関数
        /// </summary>
        public void CalcError()
        {
            error = 0.0;
            foreach (Edge edge in outputs)
            {
                error += edge.weight * edge.right.error;
            }
        }

        /// <summary>
        /// (手順3)重み更新の関数
        /// </summary>
        public void UpdateWeight(double alpha)
        {
            foreach (Edge edge in inputs)
            {
                //調整値の算出
                edge.weight += alpha * error * DActivevations(value) * edge.left.value;
            }
        }
    }
}

