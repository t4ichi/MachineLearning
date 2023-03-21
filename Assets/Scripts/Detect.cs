using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace MachineLearning
{
    public class Detect : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] Button calcButton;
        [SerializeField] Button clearButton;

        [Space(10)]
        [SerializeField] TextMeshProUGUI resultText;

        [Header("手書き部分")]
        [SerializeField]  RawImage m_image = null;
        [SerializeField] RectTransform rt;
        private Texture2D m_texture = null;

        private NeuralNetWork nn;
        private double[] pixcel;

        //手書き文字の太さ
        private int thickness = 20;

        private Vector2 m_prePos;
        private Vector2 m_TouchPos;

        private float m_clickTime, m_preClickTime;

        private void Start()
        {
            //データ関連の初期化
            pixcel = new double[28 * 28];
            nn = new NeuralNetWork();
            nn.LoadWeight();

            //手書き文字関連の初期化
            var rect = rt.rect;
            m_texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
            TextureInit();
            m_image.texture = m_texture;

            //UI関連の初期化
            calcButton.onClick.AddListener(CalcButton);
            clearButton.onClick.AddListener(ResetButton);
            resultText.text = "";
        }

        /// <summary>
        /// 認識を実行するボタン
        /// </summary>
        private void CalcButton()
        {
            nn.CalcForward(pixcel);
            resultText.text = nn.GetMaxOutPut().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetButton()
        {
            pixcel = new double[28 * 28];
            TextureInit();
            resultText.text = "";
        }

        /// <summary>
        /// Textureをすべて白にする
        /// </summary>
        private void TextureInit()
        {
            for (int w = 0; w < m_texture.width; w++)
            {
                for (int h = 0; h < m_texture.height; h++)
                {
                    m_texture.SetPixel(w, h, Color.white);
                }
            }
            m_texture.Apply();
        }

        /// <summary>
        /// テクスチャーに点を描画してpixcelにデータを入れる
        /// </summary>
        private void TexturePlot(int x,int y)
        {
            for (int h = 0; h < thickness; ++h)
            {
                int ny = (int)(y + h);
                if (ny < 0 || ny > m_texture.height) continue;
                for (int w = 0; w < thickness; ++w)
                {
                    int nx = (int)(x + w);
                    if (nx >= 0 && nx <= m_texture.width)
                    {
                        m_texture.SetPixel(nx, ny, Color.black);
                        int px = Mathf.Abs(nx * 28 / m_texture.width - 0);
                        int py = Mathf.Abs(ny * 28 / m_texture.height - 27);
                        pixcel[py * 28 + px] = 1.0;
                    }
                }
            }

            m_texture.Apply();
        }

        /// <summary>
        /// 線を描画
        /// </summary>
        public void OnDrag(BaseEventData arg)
        {
            PointerEventData _event = arg as PointerEventData;

            m_TouchPos = _event.position;
            m_clickTime = _event.clickTime;

            float disTime = m_clickTime - m_preClickTime;

            var dir = m_prePos - m_TouchPos;
            if (disTime > 0.01) dir = new Vector2(0, 0);

            var dist = (int)dir.magnitude;

            dir = dir.normalized;

            for (int d = 0; d < dist; ++d)
            {
                var p_pos = m_TouchPos + dir * d;
                p_pos.y -= thickness / 2.0f;
                p_pos.x -= thickness / 2.0f;
                TexturePlot((int)p_pos.x, (int)p_pos.y);
            }
            m_texture.Apply();
            m_prePos = m_TouchPos;
            m_preClickTime = m_clickTime;
        }

        /// <summary>
        /// //点を描画
        /// </summary>
        public void OnTap(BaseEventData arg) 
        {
            PointerEventData _event = arg as PointerEventData;
            m_TouchPos = _event.position;

            var p_pos = m_TouchPos;
            p_pos.y -= thickness / 2.0f;
            p_pos.x -= thickness / 2.0f;
            TexturePlot((int)p_pos.x,(int)p_pos.y);
        }

        /// <summary>
        /// pixcelの中身を確認
        /// </summary>
        private void Log()
        {
            string s = "";
            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    if (pixcel[i * 28 + j] == 1.0)
                    {
                        s += "#";
                    }
                    else
                    {
                        s += ".";
                    }
                }
                s += "\n";
            }
            Debug.Log(s);
        }
    }
}