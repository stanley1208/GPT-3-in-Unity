using OpenAI_API;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputTxt : MonoBehaviour
{
    public TMP_InputField Target;
    private string inputTxt = "";
    private string target_path = "Assets/Resources/choosePerson.txt";
    int count = -1;
    private string path = "";
    private string replace_father = "The following is a conversation between Me and Father. Father is a boss. Father is very clever and has his own company. 他使用中文回覆問題。" + "\r\n\r\n" + "Me:爸爸你做甚麼工作的?" + "\r\n" + "Father:我是一位公司的老闆，我們公司是做跟AI有關的產品。" + "\r\n" + "Me:爸爸你今天工作怎麼樣?" + "\r\n" + "Father:今天公司新來了幾位員工，他們都還在學習做事，所以今天教他們做事的員工還有身為老闆的我都很疲累..." + "\r\n" + "Me:你的公司在哪?" + "\r\n" + "Father:我的公司在美國矽谷!" + "\r\n" + "Me:";
    private string replace_brother = "The following is a conversation between Me and Brother. Brother is a competitive car racer and mechanic. Brother drives very well. Brother can fix every broken car he receive!. 他使用中文回覆問題。\r\n" + "\r\n\r\n" + "Me:哥哥你做甚麼工作的?" + "\r\n" + "Brother:我是一位賽車手，我開車技術很好。" + "\r\n" + "Me:哥哥你沒開車的時候在做甚麼?" + "\r\n" + "Brother:我沒開車的時候都在研究各種跑車與賽車，所以我也很會修壞掉的車子。" + "\r\n" + "Me:你能不能教我開車?" + "\r\n"+ "Brother:當然是沒問題啊，不過你要先有駕照!" + "\r\n" + "Me:";
    private string replace_mother = "The following is a conversation between Me and Mother. Mother is a famous singer. Mother sings very well and is about to hold a concert next week !. 他使用中文回覆問題。" + "\r\n\r\n" + "Me:媽媽你做甚麼工作的?" + "\r\n" + "Mother:我是一位知名的歌手，大家都說我唱歌非常的好聽。" + "\r\n" + "Me:媽媽你平常都唱甚麼歌?" + "\r\n" + "Mother:我平常都唱華語歌曲，但偶爾也會聽一些外國歌曲。" + "\r\n" + "Me:你會在哪邊舉辦演唱會?" + "\r\n" + "Mother:我下個禮拜準備要在小巨蛋開演唱會了!" + "\r\n" + "Me:";
    
    // 寫入
    private void WriteTxT(string p, string t)
    {
        try
        {
            StreamWriter sw = File.AppendText(p);
            sw.WriteLine(t);

            if (path == "Assets/Resources/father_prompt.txt")
            {
                sw.Write("Father:");
            }
            else if (path == "Assets/Resources/brother_prompt.txt")
            {
                sw.Write("Brother:");
            }
            else
            {
                sw.Write("Mother:");
            }
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

    //結束編輯並寫入 之後可能需要寫一個再次輸入時清空欄位
    public void endedit()
    {
        //讀取
        if (File.Exists(target_path) == false)
        {
            Debug.LogError("txt missing: " + target_path);
        }
        path = File.ReadAllText(target_path);
        Debug.Log(path);

        inputTxt = Target.GetComponent<TMP_InputField>().text;

        // 自動刷新的輸入
        if (File.Exists(path))
        {
            Debug.Log("檔案存在");
            WriteTxT(path, inputTxt);
            ++count;
        }
        else if(Directory.Exists(path))
        {
            Debug.Log("目錄存在");
            WriteTxT(path, inputTxt);
            ++count;
        }
        else
        {
            Debug.Log("不存在");
            Directory.CreateDirectory(path);
            WriteTxT(path, inputTxt);
            ++count;
        }
        
        if (count > 1)
        {
            if (path == "Assets/Resources/father_prompt.txt")
            {
                File.WriteAllText(path, replace_father);
            }
            else if (path == "Assets/Resources/brother_prompt.txt")
            {
                File.WriteAllText(path, replace_brother);
            }
            else
            {
                File.WriteAllText(path, replace_mother);
            }
            inputTxt = Target.GetComponent<TMP_InputField>().text;
            WriteTxT(path, inputTxt);
            count = 0;
        }
        Debug.Log(count);

    }
}

