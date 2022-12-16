//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using TMPro;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif

public class MicrosoftSpeechServiceController : MonoBehaviour
{
    private static MicrosoftSpeechServiceController _instance;

    public static MicrosoftSpeechServiceController Instace
    {
        get 
        {
            if (_instance == null) 
            {
                _instance = FindObjectOfType<MicrosoftSpeechServiceController>();
            }
            return _instance;
        }
    }

    // Hook up the two properties below with a Text and Button object in your UI.
    public TMP_Text outputText;
    public Button startRecoButton;

    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;

    private bool micPermissionGranted = false;

    public event Action<string> Recognized;

#if PLATFORM_ANDROID || PLATFORM_IOS
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    private TaskCompletionSource<int> stopTranslation;

    private long currentIdentify;

    private string currentSpeech;

    private Coroutine timerCoroutine;

    public async void StartRecognize()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechTranslationConfig.FromSubscription("133a2522a67741de87a430aa8d1b466f", "japanwest");

        var fromLanguage = "zh-TW";
        var toLanguages = new List<string> { "zh-Hant" };
        config.SpeechRecognitionLanguage = fromLanguage;
        toLanguages.ForEach(config.AddTargetLanguage);

        stopTranslation = new TaskCompletionSource<int>();

        using (var audioConfig = AudioConfig.FromDefaultMicrophoneInput())
        // Make sure to dispose the recognizer after use!
        using (var recognizer = new TranslationRecognizer(config, audioConfig))
        {
//            recognizer.Recognized += (s, e) =>
//            {
//                var lidResult = e.Result.Properties.GetProperty(PropertyId.SpeechServiceConnection_AutoDetectSourceLanguageResult);
//
//                Debug.Log($"RECOGNIZING in '{lidResult}': Text={e.Result.Text}");
//                Recognized?.Invoke(e.Result.Text);
//                //foreach (var element in e.Result.Translations)
//                //{
//                //    Debug.Log($"    TRANSLATING into '{element.Key}': {element.Value}");
//                //}
//            };
//
//            recognizer.Canceled += (s, e) =>
//            {
//                Debug.Log($"CANCELED: Reason={e.Reason}");
//                if (e.Reason == CancellationReason.Error)
//                {
//                    Debug.Log($"CANCELED: ErrorCode={e.ErrorCode}");
//                    Debug.Log($"CANCELED: ErrorDetails={e.ErrorDetails}");
//                    Debug.Log($"CANCELED: Did you update the subscription info?");
//                }
//
//                stopTranslation.TrySetResult(0);
//            };
//
//            recognizer.SessionStopped += (s, e) => {
//                Debug.Log("\nSession stopped event.");
//                Debug.Log($"\nStop translation.");
//                stopTranslation.TrySetResult(0);
//            };
//
//            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
//
//            Task.WaitAny(new[] { stopTranslation.Task });
//
//            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
//
//             stopTranslation = null;

            lock (threadLocker)
            {
                waitingForReco = true;
            }

            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result.
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query.
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false); // 單次

            // Checks result.
            string newMessage = string.Empty;
            if (result.Reason == ResultReason.TranslatedSpeech)
            {
                newMessage = result.Text + "\r\n";

//                foreach (var v in result.Translations)
//                {
//                    newMessage += $"Translated into {v.Key}: {v.Value}";
//                }
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                newMessage = "NOMATCH: Speech could not be recognized.";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            }

            lock (threadLocker)
            {
                message = newMessage;
                waitingForReco = false;
                Debug.Log(message);
            }
        }
    }

    public async void StartRecognize(PushAudioInputStream audioInputStream)
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechTranslationConfig.FromSubscription("133a2522a67741de87a430aa8d1b466f", "japanwest");

        var fromLanguage = "zh-TW";
        var toLanguages = new List<string> { "zh-Hant" };
        config.SetProperty(PropertyId.Speech_LogFilename, Path.Combine(Application.persistentDataPath, "VoiceLog"));
        config.SpeechRecognitionLanguage = fromLanguage;
        toLanguages.ForEach(config.AddTargetLanguage);

        stopTranslation = new TaskCompletionSource<int>();

        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
        // Make sure to dispose the recognizer after use!
        using (var recognizer = new TranslationRecognizer(config, audioConfig))
        {
            var phraseList = PhraseListGrammar.FromRecognizer(recognizer);
            phraseList.AddPhrase("意識清楚");
            //phraseList.AddPhrase("瞳孔兩邊等大");
            //phraseList.AddPhrase("呼吸30");
            //phraseList.AddPhrase("脈搏140");
            //phraseList.AddPhrase("收縮壓200");
            //phraseList.AddPhrase("體溫45");
            //phraseList.AddPhrase("膚色");
            //phraseList.AddPhrase("血氧100");
            //phraseList.AddPhrase("昏迷指數3");

            recognizer.Recognized += (s, e) =>
            {
                //if (timerCoroutine != null)
                //    StopCoroutine(timerCoroutine);

                //var lidResult = e.Result.Properties.GetProperty(PropertyId.SpeechServiceConnection_AutoDetectSourceLanguageResult);

                ////Debug.Log($"RECOGNIZING in '{lidResult}': Text={e.Result.Text}");
                //if (currentIdentify != 0 && currentIdentify != e.Result.OffsetInTicks) 
                //{
                //    Recognized?.Invoke(currentSpeech);
                //}
                //currentIdentify = e.Result.OffsetInTicks;
                //currentSpeech = e.Result.Text;

                //timerCoroutine = StartCoroutine(RecognizeSpeechTimer(1, () => 
                //{
                //    currentIdentify = 0;
                //    Recognized?.Invoke(currentSpeech);
                //}));

                if (e.Result.Reason == ResultReason.TranslatedSpeech)
                {
                    Recognized?.Invoke(e.Result.Text);
                }

                //foreach (var element in e.Result.Translations)
                //{
                //    Debug.Log($"    TRANSLATING into '{element.Key}': {element.Value}");
                //}
            };

            recognizer.Canceled += (s, e) =>
            {
                Debug.Log($"CANCELED: Reason={e.Reason}");
                if (e.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={e.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you update the subscription info?");
                }

                stopTranslation.TrySetResult(0);
            };

            recognizer.SessionStopped += (s, e) => {
                Debug.Log("\nSession stopped event.");
                Debug.Log($"\nStop translation.");
                stopTranslation.TrySetResult(0);
            };

            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            Task.WaitAny(new[] { stopTranslation.Task });

            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

            stopTranslation = null;
        }
    }

    public static async Task SpeechRecognitionWithCompressedInputPushStreamAudio()
    {
        // <recognitionWithCompressedInputPushStreamAudio>
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");

        var stopRecognition = new TaskCompletionSource<int>();

        using (var pushStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetCompressedFormat(AudioStreamContainerFormat.MP3)))
        {
            using (var audioInput = AudioConfig.FromStreamInput(pushStream))
            {
                // Creates a speech recognizer using audio stream input.
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                    };

                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\nSession started event.");
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\nSession stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    //using (BinaryAudioStreamReader reader = Helper.CreateBinaryFileReader(@"whatstheweatherlike.mp3"))
                    //{
                    //    byte[] buffer = new byte[1000];
                    //    while (true)
                    //    {
                    //        var readSamples = reader.Read(buffer, (uint)buffer.Length);
                    //        if (readSamples == 0)
                    //        {
                    //            break;
                    //        }
                    //        pushStream.Write(buffer, readSamples);
                    //    }
                    //}
                    pushStream.Close();

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }
        }
        // </recognitionWithCompressedInputPushStreamAudio>
    }

    public void StopRecognize() 
    {
        if (stopTranslation != null)
            stopTranslation.TrySetResult(0);
    }

    void Start()
    {
                if (outputText == null)
                {
                    UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
                }
                else if (startRecoButton == null)
                {
                    message = "startRecoButton property is null! Assign a UI Button to it.";
                    UnityEngine.Debug.LogError(message);
                }
                else
                {
                    // Continue with normal initialization, Text and Button objects are present.
        #if PLATFORM_ANDROID
                    // Request to use the microphone, cf.
                    // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
                    message = "Waiting for mic permission";
                    if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                    {
                        Permission.RequestUserPermission(Permission.Microphone);
                    }
        #elif PLATFORM_IOS
                    if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
                    {
                        Application.RequestUserAuthorization(UserAuthorization.Microphone);
                    }
        #else
                    micPermissionGranted = true;
                    message = "Click button to recognize speech";
        #endif
//                    startRecoButton.onClick.AddListener(ButtonClick);
//                    ButtonClick();
                }

        // Continue with normal initialization, Text and Button objects are present.
//#if PLATFORM_ANDROID
//        // Request to use the microphone, cf.
//        // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
//        message = "Waiting for mic permission";
//        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
//        {
//            Permission.RequestUserPermission(Permission.Microphone);
//        }
//#elif PLATFORM_IOS
//        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
//        {
//            Application.RequestUserAuthorization(UserAuthorization.Microphone);
//        }
//#else
//        micPermissionGranted = true;
//        message = "Click button to recognize speech";
//#endif

//        StartRecognize();
    }

    void Update()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#elif PLATFORM_IOS
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#endif

        lock (threadLocker)
        {
            if (startRecoButton != null)
            {
                startRecoButton.interactable = !waitingForReco && micPermissionGranted; //沒有在等待Rec且有mic才可以按按鈕
            }
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }

    IEnumerator RecognizeSpeechTimer(float seconds, Action callback) 
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}
// </code>
