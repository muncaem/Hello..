using System;
using UnityEngine;

/// <summary>
/// WAV 파일의 byte[] 데이터를 파싱해서 AudioClip으로 변환하는 클래스
/// (단일 채널, PCM 16bit 리틀 엔디안 형식 기준)
/// </summary>
public class WAV
{
    public float[] LeftChannel { get; private set; }  // 오디오 샘플(float형, -1.0 ~ 1.0)
    public int ChannelCount { get; private set; }     // 채널 수 (1 = mono, 2 = stereo)
    public int SampleCount { get; private set; }      // 전체 샘플 개수
    public int Frequency { get; private set; }        // 샘플링 주파수 (예: 16000, 44100)

    /// <summary>
    /// WAV 바이트 배열을 받아 파싱
    /// </summary>
    public WAV(byte[] wav)
    {
        // 22번째 바이트: 채널 수
        ChannelCount = wav[22];

        // 24~27번째 바이트: 샘플링 주파수 (예: 16000Hz)
        Frequency = BitConverter.ToInt32(wav, 24);

        // "data" 청크 찾기 (오디오 데이터 시작점)
        int pos = 12;
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = BitConverter.ToInt32(wav, pos);
            pos += 4 + chunkSize;
        }

        pos += 8; // "data" 이후 오디오 데이터 시작

        // 전체 샘플 수 계산 (2바이트당 1 샘플)
        SampleCount = (wav.Length - pos) / 2;
        LeftChannel = new float[SampleCount];

        // 샘플 데이터를 float로 변환
        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
            i++;
            pos += 2;
        }
    }

    /// <summary>
    /// 2바이트 (리틀 엔디안) → float 변환 (-1.0 ~ 1.0 범위로 정규화)
    /// </summary>
    private float BytesToFloat(byte firstByte, byte secondByte)
    {
        short s = (short)((secondByte << 8) | firstByte);  // 16비트 signed short로 변환
        return s / 32768.0f;
    }
}
