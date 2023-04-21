#if(NET48 || NET6_0)
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Video
{
    public class AudioRecorderComponent : Entity
    {
        public enum RecordType
        {
            loudspeaker = 0, // 扬声器
            microphone = 1 //麦克风
        }
        //录制的类型
        public RecordType recordType = RecordType.microphone;
        //生成音频文件的对象
        public WaveFileWriter writer = null;
        //录制麦克风的声音
        public WaveInEvent waveIn = null; //new WaveInEvent();
                                          //录制扬声器的声音
        public WasapiLoopbackCapture capture = null; //new WasapiLoopbackCapture();
    }
    public static class AudioRecorderComponentSystem
    {
        public static bool Start(this AudioRecorderComponent self, string fileName)
        {
            if (self.recordType == AudioRecorderComponent.RecordType.microphone)
            {
                self.waveIn = new WaveInEvent();
                self.writer = new WaveFileWriter(fileName, self.waveIn.WaveFormat);
                //开始录音，写数据
                self.waveIn.DataAvailable += (s, a) =>
                {
                    self.writer.Write(a.Buffer, 0, a.BytesRecorded);
                };
                //结束录音
                self.waveIn.RecordingStopped += (s, a) =>
                {
                    self.writer.Dispose();
                    self.writer = null;
                    self.waveIn.Dispose();

                };
                self.waveIn.StartRecording();
            }
            return true;
        }
        //结束录制
        public static void Stop(this AudioRecorderComponent self)
        {
            self.waveIn?.StopRecording();
            self.capture?.StopRecording();
        }
    }
}
#endif


