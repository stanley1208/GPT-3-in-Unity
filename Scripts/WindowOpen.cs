using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowOpen : MonoBehaviour
{
    public GameObject InputField;
    public GameObject Player;
    public TMP_Text Human_Input;
    // public Text AI_Reply;
    
    private bool Talk_WindowON = false;
    public Text Talk_to_name;
    private string target_path = "Assets/Resources/choosePerson.txt";
    public GameObject QuitButton;
    private bool QuitButtonON = false;


    void Awake()
    {
        Cursor.visible = false;
        QuitButton.SetActive(QuitButtonON);
        InputField.SetActive(Talk_WindowON);                          // 對話框
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.E) && !QuitButtonON)      // E鍵觸發
        {
            InputField.GetComponent<TMP_InputField>().text = "";              // 清空輸入文字
            OpenAI.resultStr = "";                                               // 清空輸出文字
            if (!Talk_WindowON)  // 未開啟視窗則
            {
                Cursor.visible = true;
                // 抓取對應角色
                if (Player.transform.position.x < 448)
                {
                    File.WriteAllText(target_path, "Assets/Resources/mother_prompt.txt");
                    Talk_to_name.text = "To Mother";
                }
                else if (Player.transform.position.x > 500)
                {
                    File.WriteAllText(target_path, "Assets/Resources/brother_prompt.txt");
                    Talk_to_name.text = "To Brother";

                }
                else
                {
                    File.WriteAllText(target_path, "Assets/Resources/father_prompt.txt");
                    Talk_to_name.text = "To Father";
                }
                Talk_WindowON = true;                                         // 開
                InputField.SetActive(Talk_WindowON);                          // 對話框
                Player.GetComponent<InputManager>().enabled = !Talk_WindowON; // 輸入控制
            }
            else
            {
                Cursor.visible = false;
                Talk_WindowON = false;                                        // 關
                InputField.SetActive(Talk_WindowON);                          // 對話框(預設關)
                Player.GetComponent<InputManager>().enabled = !Talk_WindowON; // 輸入控制(預設開)
            }
        }
        if ( Input.GetKeyDown(KeyCode.Escape) )                               // Esc觸發
        {
            if (Talk_WindowON)
            {
                InputField.GetComponent<TMP_InputField>().text = "";              // 清空輸入文字
                OpenAI.resultStr = "";                                               // 清空輸出文字
                Cursor.visible = false;
                Talk_WindowON = false;                                            // 關閉視窗
                InputField.SetActive(Talk_WindowON);                              // 關閉對話框
                Player.GetComponent<InputManager>().enabled = !Talk_WindowON;     // 恢復輸入控制
            }
            else if(QuitButtonON)
            {
                Cursor.visible = false;
                Player.GetComponent<InputManager>().enabled = true; // 輸入控制
                QuitButtonON = false;
                QuitButton.SetActive(QuitButtonON);
            }
            else
            {
                Cursor.visible = false;
                Player.GetComponent<InputManager>().enabled = false; // 輸入控制
                QuitButtonON = true;
                QuitButton.SetActive(QuitButtonON);
            }
        }
    }

    void LateUpdate()
    {
        if ( Input.GetKeyDown(KeyCode.E) && !QuitButtonON)      // E鍵觸發
        {
            InputField.GetComponent<TMP_InputField>().text = "";              // 清空輸入文字
            OpenAI.resultStr = "";                                               // 清空輸出文字
        }
    }
}
