using OpenAI_API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class OpenAI_old : MonoBehaviour
{
    // path 還沒改成從檔案位置開始找
    private string path = "D:/GPTCHATBOT/Assets/Resources/waiter_prompt.txt";
  
    public Text text;
    public string apikey = "";
    private string resultStr = "";

    //寫入
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
        var task = StartAsync();
        
    }

    async Task StartAsync()
    {
        //讀取
        if (File.Exists(path) == false)
        {
            Debug.LogError("txt missing: " + path);
        }
        var txt = File.ReadAllText(path);
        

        //api金鑰
        

        //訓練模組設定
        var api = new OpenAI_API.OpenAIAPI(apikey, engine: "text-davinci-003");
        string prompt = txt;
        var result = await api.Completions.CreateCompletionAsync(
            prompt,
            temperature: 0.9,
            max_tokens: 150,
            top_p: 1);

        //var result = await api.Search.GetBestMatchAsync("RaycastHit", "Unity3D", "Godot", "Unreal Engine", "GameMaker");
        //Console.WriteLine(result.ToString());

        //輸出
        resultStr = result.ToString();
        //寫入文件
        WriteTxT(path, resultStr.ToString() + "\nHuman:");
        
    }
}