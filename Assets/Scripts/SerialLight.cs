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
    public string phrase_csv;//csvの名前（個々のデータ）
    public string phrase2phrase;//csvの名前（対応)
    public TextAsset csvFile;//csvファイル
    public TextAsset csvFile2;
    public List<string[]> csvDatas = new List<string[]>();//CSVの中身をいれるリスト
    public List<string[]> csvDatas2 = new List<string[]>();//CSVの中身をいれるリスト
    public int height = 0;//CSVの行数の初期化

    //Serialで送られてきた文字列をintにしたものを格納
    public int message_int = 0;
    // //さっき踏んだ位置(0~6)を格納しておく
    // public int prev_pos = 0;
    // //さっき踏んだフレーズID(phrase_csv)を格納しておく
    // public int prev_id = 0;
    // //今Activeな本の位置（次に踏まれうる本）を格納しておく
    // public List<int> active_pos = new List<int> { 1, 2, 3 };
    // //今Activeな本のID（phrase_csv）を格納しておく
    // public List<int> active_id = new List<int>{}

    //7つの本に表示される本のID(phrase_csv)
    public List<int> idForEach = new List<int> { 0, 2, 0, 0, 0, 0, 0 };

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
        phrase2phrase = "phrase2phrase_csv";
        //phrase_csv
        csvFile = Resources.Load("CSV/" + phrase_csv) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
            height++;
        }
        // Debug.Log("csvDatas.Count is" + csvDatas.Count);

        //phrase2phrase
        csvFile2 = Resources.Load("CSV/" + phrase2phrase) as TextAsset;
        StringReader reader2 = new StringReader(csvFile2.text);
        while (reader2.Peek() > -1)
        {
            string line = reader2.ReadLine();
            csvDatas2.Add(line.Split(','));
            height++;
        }

        //音処理
        AS = gameObject.AddComponent<AudioSource>();
        //csvからsoundsに格納
        for (int i = 1; i < csvDatas.Count; i++)
        {
            // Debug.Log("sound name is" + csvDatas[i][6]);            
            AudioClip audio = Resources.Load("Sound/" + csvDatas[i][6].Remove(csvDatas[i][6].Length - 4), typeof(AudioClip)) as AudioClip;
            sounds.Add(audio);
            // Debug.Log(sounds[i-1]);
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
            //文字のある場所が踏まれていたら
            if (idForEach[message_int] != 0 && message_int < 10)
            {
                Debug.Log(message);
                AS.PlayOneShot(sounds[idForEach[message_int] - 1]);

                //次に出す文字のリストを抽出
                List<List<int>> next_phrases = new List<List<int>>();//{{次表示するフレーズのID(phrase_csv),それを出すために参照したphrase2phrase_csvのID},{,},,,}
                for (int i = 1; i < csvDatas2.Count; i++)
                {
                    //phrase2phraseの2行目phrase_idに今踏まれたものがあったら
                    if (int.Parse(csvDatas2[i][1]) == idForEach[message_int])
                    {
                        List<int> id_phrase_link = new List<int> { int.Parse(csvDatas2[i][6]), int.Parse(csvDatas2[i][0]) };
                        next_phrases.Add(id_phrase_link);
                    }
                }
                Debug.Log(ShowListContentsInTheDebugLog(next_phrases));
                //ランダム抽出の準備
                // System.Random r = new System.Random((int)DateTime.Now.Ticks);

                //本1冊ずつ順番に
                for (int i = 0; i < idForEach.Count; i++)
                {
                    //背景のアニメーション
                    if (idForEach[i] != 0)//さっき表示されてた
                    {
                        if (place_dict[message_int].Contains(i))//次もだす
                        {
                            //idForEachの更新
                            //idForEach[i] = next_phrases[r.Next(1, next_phrases.Count)][0];
                            if (next_phrases.Count > 0)
                            {
                                int j = Random.Range(0, next_phrases.Count);
                                idForEach[i] = next_phrases[j][0];
                                next_phrases.RemoveAt(j);

                                transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 3);
                                Debug.Log(i + ":change");

                                Debug.Log("rand:" + j);
                            }
                            else
                            {
                                transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().sprite = Square;
                                transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 4);
                                Debug.Log(i + ":dissappear");
                                idForEach[i] = 0;
                            }

                            // Debug.Log("rand : " + Random.Range(0,next_phrases.Count));
                        }
                        else//次はださない、つまり消す
                        {
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().sprite = Square;
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 4);
                            Debug.Log(i + ":dissappear");
                            idForEach[i] = 0;
                        }

                    }
                    else if (place_dict[message_int].Contains(i))//さっきなくて新しく出す
                    {
                        if (next_phrases.Count > 0)
                        {
                            int j = Random.Range(0, next_phrases.Count);
                            idForEach[i] = next_phrases[j][0];
                            next_phrases.RemoveAt(j);

                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Image>().sprite = Square;
                            transform.GetChild(0).GetChild(i).gameObject.GetComponent<Animator>().SetInteger("State", 2);
                            Debug.Log(i + ":appear");

                            Debug.Log("rand:" + j);
                        }

                        // idForEach[i] = next_phrases[r.Next(1, next_phrases.Count)][0];
                    }

                    //文字の処理
                    //idForEachが０じゃなければ、つまり次に出すものがあれば
                    //ランダムにnext_phraseの1列目のidから
                    if (idForEach[i] != 0)
                    {
                        transform.GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = csvDatas[idForEach[i]][1];
                    }
                }
                Debug.Log(ShowListContentsInTheDebugLog(idForEach));
            }
            //			text.text = csvDatas[int.Parse(message)%5+1][int.Parse(message)%7]; // シリアルの値をテキストに表示
            else
            {//error message
                if (idForEach[message_int] == 0)
                {
                    Debug.Log("same or vacant");
                }
                // else if (active_pos.Contains(message_int))
                // {
                //     Debug.Log("You can't step on here.");
                // }
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