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

    //    // float[] → byte[] 변환
    //    byte[] byteData = new byte[length * sizeof(float)];
    //    Buffer.BlockCopy(samples, 0, byteData, 0, byteData.Length);

    //    if (recognizer.AcceptWaveform(byteData, byteData.Length))
    //    {
    //        var result = recognizer.Result();
    //        Debug.Log("인식 결과: " + result);
    //    }
    //    else
    //    {
    //        var partial = recognizer.PartialResult();
    //        Debug.Log("부분 결과: " + partial);
    //    }

    //    yield return new WaitForSeconds(2f);
    //}

}
