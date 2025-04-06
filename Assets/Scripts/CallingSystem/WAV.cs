using System;
using UnityEngine;

/// <summary>
/// WAV ������ byte[] �����͸� �Ľ��ؼ� AudioClip���� ��ȯ�ϴ� Ŭ����
/// (���� ä��, PCM 16bit ��Ʋ ����� ���� ����)
/// </summary>
public class WAV
{
    public float[] LeftChannel { get; private set; }  // ����� ����(float��, -1.0 ~ 1.0)
    public int ChannelCount { get; private set; }     // ä�� �� (1 = mono, 2 = stereo)
    public int SampleCount { get; private set; }      // ��ü ���� ����
    public int Frequency { get; private set; }        // ���ø� ���ļ� (��: 16000, 44100)

    /// <summary>
    /// WAV ����Ʈ �迭�� �޾� �Ľ�
    /// </summary>
    public WAV(byte[] wav)
    {
        // 22��° ����Ʈ: ä�� ��
        ChannelCount = wav[22];

        // 24~27��° ����Ʈ: ���ø� ���ļ� (��: 16000Hz)
        Frequency = BitConverter.ToInt32(wav, 24);

        // "data" ûũ ã�� (����� ������ ������)
        int pos = 12;
        while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
        {
            pos += 4;
            int chunkSize = BitConverter.ToInt32(wav, pos);
            pos += 4 + chunkSize;
        }

        pos += 8; // "data" ���� ����� ������ ����

        // ��ü ���� �� ��� (2����Ʈ�� 1 ����)
        SampleCount = (wav.Length - pos) / 2;
        LeftChannel = new float[SampleCount];

        // ���� �����͸� float�� ��ȯ
        int i = 0;
        while (pos < wav.Length)
        {
            LeftChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
            i++;
            pos += 2;
        }
    }

    /// <summary>
    /// 2����Ʈ (��Ʋ �����) �� float ��ȯ (-1.0 ~ 1.0 ������ ����ȭ)
    /// </summary>
    private float BytesToFloat(byte firstByte, byte secondByte)
    {
        short s = (short)((secondByte << 8) | firstByte);  // 16��Ʈ signed short�� ��ȯ
        return s / 32768.0f;
    }
}
