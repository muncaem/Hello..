using UnityEngine;
using System.Collections;
using System;

public class STTManager : MonoBehaviour
{
    //private VoskRecognizer recognizer;

    //void Start()
    //{
    //    //Vosk.Vosk.SetLogLevel(0);
    //    var model = new Model("Assets/StreamingAssets/vosk-model-small-ko-0.22");
    //    recognizer = new VoskRecognizer(model, 16000.0f);
    //    StartMicrophone();
    //}

    //void StartMicrophone()
    //{
    //    AudioClip mic = Microphone.Start(null, true, 10, 16000);
    //    StartCoroutine(Recognize(mic));
    //}

    //IEnumerator Recognize(AudioClip clip)
    //{
    //    int length = clip.samples * clip.channels;
    //    float[] samples = new float[length];
    //    clip.GetData(samples, 0);

    //    // float[] �� byte[] ��ȯ
    //    byte[] byteData = new byte[length * sizeof(float)];
    //    Buffer.BlockCopy(samples, 0, byteData, 0, byteData.Length);

    //    if (recognizer.AcceptWaveform(byteData, byteData.Length))
    //    {
    //        var result = recognizer.Result();
    //        Debug.Log("�ν� ���: " + result);
    //    }
    //    else
    //    {
    //        var partial = recognizer.PartialResult();
    //        Debug.Log("�κ� ���: " + partial);
    //    }

    //    yield return new WaitForSeconds(2f);
    //}

}
