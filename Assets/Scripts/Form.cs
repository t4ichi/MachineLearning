using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using XCharts;
using XCharts.Runtime;

namespace MachineLearning
{
    public class Form :MonoBehaviour
	{
        [Header("Training")]
        [SerializeField] Slider trainSlider;
        [SerializeField] TextMeshProUGUI trainlabel;

        [Header("Testing")]
        [SerializeField] Slider testSlider;
        [SerializeField] TextMeshProUGUI testlabel;

        [Space(10)]
        [SerializeField] Button button1;

        [Space(10)]
        [SerializeField] TextMeshProUGUI scorelabel;
        [SerializeField] LineChart linechart;
		NeuralNetWork nn;
		const int num_input_nodes = 28 * 28;
		const int num_hidden_nodes = 100;
		const int num_output_nodes = 10;
		double alpa = 0.1;

        const int num_training_data = 50000;
        const int num_test_data = 10000;
        const int num_data = num_training_data + num_test_data;

        double[][] pixel;
        double[][] label;
        int[] labelIndex;

        void Start()
        {
            nn = new NeuralNetWork();
            nn.AddLayer(num_input_nodes);
            nn.AddLayer(num_hidden_nodes);
            nn.AddLayer(num_output_nodes);
            button1.onClick.AddListener(Button1_Click);
            GraphInit();
        }

        public void Button1_Click()
        {
            button1.enabled = false;
            Train(this.GetCancellationTokenOnDestroy()).Forget();
        }

        /// <summary>
        /// 機械学習の関数
        /// </summary>
        async UniTaskVoid Train(CancellationToken token)
        {
            LoadData();
            nn.InitWeight();
            await Training(token);
            double score = await Test(num_test_data, num_training_data, token);
            scorelabel.text = "Score : " +  score.ToString() + "%";
            //weight.dat とlabel.txtの作成（結果の保存）
            //TODO : 保存を確認するポップアップ
            nn.SaveWeight();
            SaveLabelName(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            button1.enabled = true;
        }

        private async UniTask Training(CancellationToken token)
        {
            for (int i = 0; i < num_training_data; i++)
            {
                trainlabel.text = (i + 1) + "/" + num_training_data;
                trainSlider.value = (float)(i + 1) / (float)num_training_data;
                nn.CalcForward(pixel[i]);
                nn.CalcError(label[i]);
                nn.UpdateWeight(alpa);
                //100個ごとに精度を検証して表示
                if (i % 100 == 0)
                {
                    await Test(100, i, token);
                }
                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// 精度を検証
        /// </summary>
        private async UniTask<double> Test(int dataLength, int graphX,CancellationToken token)
        {
            int ok = 0;
            int offset = num_training_data;
            int lastData = offset + dataLength;
            for(int i = 0;i < dataLength; i++)
            {
                nn.CalcForward(pixel[offset + i]);
                if(nn.GetMaxOutPut() == labelIndex[offset + i])
                {
                    ok++;
                }
                testlabel.text = (i + 1) + "/" + dataLength;
                testSlider.value = (float)(i + 1) / (float)dataLength;
                await UniTask.Yield(token);
            }
            double score = (double)ok / (double)dataLength * 100;
            Debug.Log("score : " + score + "%");
            GraphPlot(graphX, score);
            return score;
        }

        /// <summary>
        /// MNISTのファイルを読み込む
        /// </summary>
        public void LoadData()
        {
            pixel = new double[num_data][];
            label = new double[num_data][];
            labelIndex = new int[num_data];

            using (FileStream fsPixcel = new(@"train-images-idx3-ubyte", FileMode.Open))
            {
                Debug.Log("Find : train-images-idx3-ubyte");
                using (BinaryReader brPixcel = new BinaryReader(fsPixcel))
                {
                    brPixcel.ReadInt32();
                    brPixcel.ReadInt32();
                    brPixcel.ReadInt32();
                    brPixcel.ReadInt32();
                    for(int i = 0;i < num_data; i++)
                    {
                        pixel[i] = new double[num_input_nodes];
                        for(int j = 0;j < num_input_nodes; j++)
                        {
                            pixel[i][j] = (double)brPixcel.ReadByte() / 255.0 * 0.99 + 0.01;
                        }
                    }
                }
            }
            using (FileStream fsLabel = new(@"train-labels-idx1-ubyte", FileMode.Open))
            {
                Debug.Log("Find : train-labels-idx1-ubyte");
                using (BinaryReader brLabel = new BinaryReader(fsLabel))
                {
                    brLabel.ReadInt32();
                    brLabel.ReadInt32();
                    for(int i = 0;i < num_data; i++)
                    {
                        label[i] = new double[num_output_nodes];
                        for(int j = 0;j < num_output_nodes; j++)
                        {
                            label[i][j] = 0.01;
                        }
                        labelIndex[i] = brLabel.ReadByte();
                        label[i][labelIndex[i]] = 0.99;
                    }
                }
            }
        }

        private void SaveLabelName(string[] labelNames)
        {
            using(StreamWriter writer = new StreamWriter(@"label.txt"))
            {
                for(int i = 0;i < labelNames.Length; i++)
                {
                    if(i > 0)
                    {
                        writer.Write(",");
                    }
                    writer.Write(labelNames[i]);
                }
            }
        }
        /// <summary>
        /// グラフを初期化
        /// </summary>
        private void GraphInit()
        {
            linechart.Init();
            linechart.RemoveData();
            linechart.AddSerie<Line>("line");
        }

        /// <summary>
        /// グラフに点を描画
        /// </summary>
        private void GraphPlot(int x,double y)
        {
            linechart.AddXAxisData(x.ToString());
            linechart.AddData(0, y);
        }
    }
}

