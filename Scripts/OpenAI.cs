using OpenAI_API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class OpenAI : MonoBehaviour
{
    // path 還沒改成從檔案位置開始找
    private string target_path = "Assets/Resources/choosePerson.txt";
    private string path = "";
    public Text text;
    // api金鑰
    public string apikey = "";
    //寫入
    public static string resultStr = "";
    private void WriteTxT(string p, string t)
    {
        try
        {
            StreamWriter sw = File.AppendText(p);
            sw.WriteLine(t);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Write Error" + e.Message);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        //輸出到螢幕
        text.text = resultStr;
    }
    //執行GPT-3
    public void runGPT()
    {
        //讀取path
        if (File.Exists(target_path) == false)
        {
            Debug.LogError("txt missing: " + target_path);
        }
        path = File.ReadAllText(target_path);

        var task = StartAsync();
    }

    async Task StartAsync()
    {
        //讀取文件
        if (File.Exists(path) == false)
        {
            Debug.LogError("txt missing: " + path);
        }
        var txt = File.ReadAllText(path);
        Debug.Log("read txt");

        //api金鑰
        

        //訓練模組設定
        var api = new OpenAI_API.OpenAIAPI(apikey, engine: "text-davinci-003");
        string prompt = txt;
        var result = await api.Completions.CreateCompletionAsync(
            prompt,
            temperature: 0.9,
            max_tokens: 150,
            top_p: 1.0,
            frequencyPenalty: 0.5,
            presencePenalty: 0.0
            );

        //var result = await api.Search.GetBestMatchAsync("RaycastHit", "Unity3D", "Godot", "Unreal Engine", "GameMaker");
        //Console.WriteLine(result.ToString());
        resultStr = result.ToString();



        //寫入文件
        WriteTxT(path, resultStr + "\nMe:");
    }
}