using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SerialLight : MonoBehaviour
{

    public SerialHandler serialHandler;//シリアルハンドラを指定
                                       //public Text text;
    public Sprite Square;//背景のしかく

    //csv処理
    public string phrase_csv;//csvの名前
    public TextAsset csvFile;//csvファイル
    public List<string[]> csvDatas = new List<string[]>();//CSVの中身をいれるリスト
    public int height = 0;//CSVの行数の初期化

    //Serialで送られてきた文字列をintにしたものを格納
    public int message_int = 0;
    //さっき踏んだ位置(0~6)を格納しておく
    public int prev_pos = 0;
    //今Activeな本の位置（次に踏まれうる本）を格納しておく
    public List<int> active_pos = new List<int> { 1, 2, 3 };
    //次に本がActiveになる場所のリスト[0が踏まれたら、1が踏まれたら、、、]
    public List<List<int>> place_dict = new List<List<int>> {
        new List<int>{1,2,3},
        new List<int>{0,3,4},
        new List<int>{0,3,5},
        new List<int>{0,1,2,4,5,6},
        new List<int>{1,3,6},
        new List<int>{2,3,6},
        new List<int>{3,4,5},
        };

    //音声再生ファイルを格納する変数
    public AudioSource AS;
    public List<AudioClip> sounds = new List<AudioClip>();
    
    // Use this for initialization
    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
        //csv読み込み
        phrase_csv = "phrase_csv";
        csvFile = Resources.Load("CSV/" + phrase_csv) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
            height++;
        }
        Debug.Log("csvDatas.Count is" + csvDatas.Count);
        AS = gameObject.AddComponent<AudioSource>();
        //csvからsoundsに格納
        for(int i = 1;i < csvDatas.Count;i++){
            Debug.Log("sound name is" + csvDatas[i][6]);
            
            AudioClip audio = Resources.Load("Sound/" + csvDatas[i][6].Remove(csvDatas[i][6].Length-4), typeof(AudioClip))as AudioClip;
            sounds.Add(audio);
            Debug.Log(sounds[i-1]);
        }
                
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
	 * シリアルを受け取った時の処理
	 */
    void OnDataReceived(string message)
    {
        try
        {
            //シリアルで送られてきた番号
            message_int = int.Parse(message);
            //active_posの中にmessage_intがあったら
            if (message_int != prev_pos && active_pos.Contains(message_int) && message_int < 10)
            {
                Debug.Log(message);
                AS.PlayOneShot(sounds[message_int%5]);
                for (int i = 0; i < transform.GetChild(0).childCount; i++)
                //foreach(int i in place_dict[message_int])
                {
                    if (active_pos.Contains(i))//さっき表示されてた
                    {
                        if (place_dict[message_int].Contains(i))//次もだす
                        {
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 3);
                            Debug.Log(i + ":change");
                        }
                        else//次はださない、つまり消す
                        {
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().sprite = Square;
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 4);
                            Debug.Log(i + ":dissappear");
                        }

                    }
                    else if(place_dict[message_int].Contains(i))//さっきなくて新しく出す
                    {
                        transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().sprite = Square;
                        transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 2);
                        Debug.Log(i + ":appear");
                    }
                    //文字の処理
                    transform.GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = csvDatas[message_int % 5 + 1][message_int % 7];
                }
                Debug.Log("message_int is" + message_int + "and active_pos was" + ShowListContentsInTheDebugLog(active_pos)+ "and next_place is" + ShowListContentsInTheDebugLog(place_dict[message_int]));
                prev_pos = message_int;
                active_pos = place_dict[message_int];
            }
            //			text.text = csvDatas[int.Parse(message)%5+1][int.Parse(message)%7]; // シリアルの値をテキストに表示
            else
            {//error message
                if (message_int == prev_pos)
                {
                    Debug.Log("same");
                }
                else if (active_pos.Contains(message_int))
                {
                    Debug.Log("You can't step on here.");
                }
                else
                {
                    Debug.Log("None.");

                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    //リストのデバッグ
    public string ShowListContentsInTheDebugLog<T>(List<T> list)
    {
        string log = "";

        foreach (var content in list)
        {
            log += content.ToString() + ", ";
        }

        return log;
    }
}