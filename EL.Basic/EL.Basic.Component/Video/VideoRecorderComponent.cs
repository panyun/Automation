
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Pipes;
using System.Net;
using System.IO.Compression;
using EL.Capturing;

namespace EL.Video
{
    public class VideoRecorderComponentAwake : AwakeSystem<VideoRecorderComponent, VideoRecorderSettings, Func<VideoRecorderComponent, Task<CaptureImage>>>
    {
        public override void Awake(VideoRecorderComponent self, VideoRecorderSettings settings, Func<VideoRecorderComponent, Task<CaptureImage>> captureMethod)
        {
            VideoRecorderComponent.Instance = self;
            self._settings = settings;
            self._captureMethod = captureMethod;

        }
    }

    /// <summary>
    /// A video recorder which records the captured images into a video file.
    /// </summary>
    public class VideoRecorderComponent : Entity
    {
        public static VideoRecorderComponent Instance { get; set; }
        public VideoRecorderSettings _settings;
        public Func<VideoRecorderComponent, Task<CaptureImage>> _captureMethod;
        public BlockingCollection<ImageData> _frames;
        public Task _recordTask;
        public bool _shouldRecord;
        public Task _writeTask;
        public DateTime _recordStartTime;
        /// <summary>
        /// The path of the video file.
        /// </summary>
        public string TargetVideoPath => _settings.TargetVideoPath;
        /// <summary>
        /// The time since the recording started.
        /// </summary>
        public TimeSpan RecordTimeSpan => DateTime.UtcNow - _recordStartTime;
    }
    public class ImageData : IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsRepeatFrame { get; private set; }
        public byte[] Data { get; set; }

        public static readonly ImageData RepeatImage = new ImageData { IsRepeatFrame = true };

        public void Dispose()
        {
            Data = null;
        }
    }

    public static class VideoRecorderComponentSystem
    {
        private static async Task RecordLoop(this VideoRecorderComponent self)
        {
            var frameInterval = TimeSpan.FromSeconds(1.0 / self._settings.FrameRate);
            var sw = Stopwatch.StartNew();
            var frameCount = 0;
            var totalMissedFrames = 0;
            while (self._shouldRecord)
            {
                var timestamp = DateTime.UtcNow;

                if (frameCount > 0)
                {
                    var requiredFrames = (int)Math.Floor(sw.Elapsed.TotalSeconds * self._settings.FrameRate);
                    var diff = requiredFrames - frameCount;
                    if (diff >= 5)
                    {
                        Log.Warn($"Adding many ({diff}) missing frame(s) to \"{Path.GetFileName(self.TargetVideoPath)}\".");
                    }
                    for (var i = 0; i < diff; ++i)
                    {
                        self._frames.Add(ImageData.RepeatImage);
                        ++frameCount;
                        ++totalMissedFrames;
                    }
                }

                using (var imgTast = self._captureMethod(self))
                {
                    var img = imgTast.Result;
                    var image = new ImageData
                    {
                        Data = self.BitmapToByteArray(img.Bitmap),
                        Width = img.Bitmap.Width,
                        Height = img.Bitmap.Height
                    };
                    self._frames.Add(image);
                    ++frameCount;
                }

                var timeTillNextFrame = timestamp + frameInterval - DateTime.UtcNow;
                if (timeTillNextFrame > TimeSpan.Zero)
                {
                    await Task.Delay(timeTillNextFrame);
                }
            }
            if (totalMissedFrames > 0)
            {
                Log.Warn($"Totally added {totalMissedFrames} missing frame(s) to \"{Path.GetFileName(self.TargetVideoPath)}\".");
            }
        }

        private static void WriteLoop(this VideoRecorderComponent self)
        {
            var videoPipeName = $"flaui-capture-{Guid.NewGuid()}";
            var ffmpegIn = new NamedPipeServerStream(videoPipeName, PipeDirection.Out, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 10000, 10000);
            const string pipePrefix = @"\\.\pipe\";
            Process ffmpegProcess = null;

            var isFirstFrame = true;
            ImageData lastImage = null;
            while (!self._frames.IsCompleted)
            {
                self._frames.TryTake(out var img, -1);
                if (img == null)
                {
                    // Happens when the queue is marked as completed
                    continue;
                }
                if (isFirstFrame)
                {
                    isFirstFrame = false;
                    Directory.CreateDirectory(new FileInfo(self.TargetVideoPath).Directory.FullName);
                    var videoInFormat = self._settings.UseCompressedImages ? "" : "-f rawvideo"; // Used when sending raw bitmaps to the pipe
                    var videoInArgs = $"-framerate {self._settings.FrameRate} {videoInFormat} -pix_fmt rgb32 -video_size {img.Width}x{img.Height} -i {pipePrefix}{videoPipeName}";
                    var videouOutCodec = self._settings.VideoFormat == VideoFormat.x264
                        ? $"-c:v libx264 -crf {self._settings.VideoQuality} -pix_fmt yuv420p -preset ultrafast"
                        : $"-c:v libxvid -qscale:v {self._settings.VideoQuality}";
                    var videoOutArgs = $"{videouOutCodec} -r {self._settings.FrameRate} -vf \"scale={img.Width.Even()}:{img.Height.Even()}\"";
                    ffmpegProcess = self.StartFFMpeg(self._settings.ffmpegPath, $"-y -hide_banner -loglevel warning {videoInArgs} {videoOutArgs} \"{self.TargetVideoPath}\"");
                    //-f dshow - i audio =\"virtual-audio-capturer\"
                    ffmpegIn.WaitForConnection();
                }
                if (img.IsRepeatFrame)
                {
                    // Repeat the last frame
                    if (lastImage != null)
                        ffmpegIn.WriteAsync(lastImage.Data, 0, lastImage.Data.Length);
                }
                else
                {
                    // Write the received frame and save it as last image
                    ffmpegIn.WriteAsync(img.Data, 0, img.Data.Length);
                    if (lastImage != null)
                    {
                        lastImage.Dispose();
                        lastImage = null;
                        GC.Collect();
                    }
                    lastImage = img;
                }
            }

            ffmpegIn.Flush();
            ffmpegIn.Close();
            ffmpegIn.Dispose();
            ffmpegProcess?.WaitForExit();
            ffmpegProcess?.Dispose();
        }

        /// <summary>
        /// Starts recording.
        /// </summary>
        public static void Start(this VideoRecorderComponent self)
        {
            self._frames = new BlockingCollection<ImageData>();
            self._shouldRecord = true;
            self._recordStartTime = DateTime.UtcNow;
            self._recordTask = Task.Factory.StartNew(async () => await self.RecordLoop(), TaskCreationOptions.LongRunning);
            self._writeTask = Task.Factory.StartNew(self.WriteLoop, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stops recording and finishes the video file.
        /// </summary>
        public static void Stop(this VideoRecorderComponent self)
        {
            if (self._recordTask != null)
            {
                self._shouldRecord = false;
                self._recordTask.Wait();
                self._recordTask = null;
            }
            self._frames.CompleteAdding();
            if (self._writeTask != null)
            {
                try
                {
                    self._writeTask.Wait();
                    self._writeTask = null;
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message, ex);
                }
            }
        }

        private static Process StartFFMpeg(this VideoRecorderComponent self, string exePath, string arguments)
        {

            var process = new Process
            {
                StartInfo =
                    {
                        FileName = exePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,

                    },
                EnableRaisingEvents = true
            };

            //process.StartInfo.Arguments=""
            process.OutputDataReceived += OnProcessDataReceived;
            process.ErrorDataReceived += OnProcessDataReceived;
            process.Start();
            if (self._settings.EncodeWithLowPriority)
            {
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            process.BeginErrorReadLine();
            return process;
        }

        private static void OnProcessDataReceived(object s, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
            {
                Log.Info(e.Data);
            }
        }

        private static byte[] BitmapToByteArray(this VideoRecorderComponent self, Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, self._settings.UseCompressedImages ? ImageFormat.Png : ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public static async Task<string> DownloadFFMpeg(this VideoRecorderComponent self, string targetFolder)
        {
            var bits = Environment.Is64BitOperatingSystem ? 64 : 32;
            if (bits == 32)
            {
                throw new NotSupportedException("The current FFMPEG builds to not support 32-bit.");
            }
            var uri = new Uri($"https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip");
            var archivePath = Path.Combine(Path.GetTempPath(), "ffmpeg.zip");
            var destPath = Path.Combine(targetFolder, "ffmpeg.exe");
            if (!File.Exists(destPath))
            {
                // Download
                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(uri, archivePath);
                }
                // Extract
                Directory.CreateDirectory(targetFolder);
                await Task.Run(() =>
                {
                    using (var archive = ZipFile.OpenRead(archivePath))
                    {
                        var exeEntry = archive.Entries.First(x => x.Name == "ffmpeg.exe");
                        exeEntry.ExtractToFile(destPath, true);
                    }
                });
                File.Delete(archivePath);
            }
            return destPath;
        }
        public static bool CombineAudio(this VideoRecorderComponent self, string audioFile, string videoFile, string outFile)
        {
            //1、视频文件中没有音频。
            //ffmpeg -i video.mp4 -i audio.wav -c:v copy -c:a aac -strict experimental output.mp4
            //string mergeCommandStr = $"-i {physicalPath}video2.mp4 -i {physicalPath}music1.mp3 -c:v copy -c:a aac -strict experimental {physicalPath}output.mp4  -y";

            //video.mp4,audio.wav分别是要合并的视频和音频，output.mp4是合并后输出的音视频文件。
            //2、下面的命令是用audio音频替换video中的音频 ffmpeg -i video.mp4 -i audio.wav -c:v copy -c:a aac -strict experimental -map 0:v:0 -map 1:a: 0 output.mp4
            //string mergeCommandStr = $"-i {physicalPath}video3.mp4 -i {physicalPath}AudioMerge.mp3 -c:v copy -c:a aac -strict experimental -map 0:v:0 -map 1:a:0 {physicalPath}AudioAndVideoMerge.mp4  -y";

            //3、c++音频视频合并(视频文件中没有音频的情况下)
            //"ffmpeg -i /tmp/mergeMp3/392118469203595327/392118469203595327.aac  -i /tmp/mergeMp3/392118469203595327/bg.mp4 -c copy -bsf:a aac_adtstoasc /tmp/mergeMp3/392118469203595327/392118469203595327.mp4 -y"
            //string mergeCommandStr3 = $"-i {physicalPath}video5.mp4  -i {physicalPath}AudioMerge.mp3 -c copy -bsf:a aac_adtstoasc {physicalPath}AudioAndVideoMerge1.mp4 -y";
            try
            {
                var arguments = $@"-i ""{videoFile}"" -i ""{audioFile}"" -c:v copy -c:a aac -strict experimental ""{outFile}""";
                var ffmpegProcess = self.StartFFMpeg(self._settings.ffmpegPath, arguments);
                ffmpegProcess?.WaitForExit();

                if (File.Exists(outFile))
                {
                    try
                    {
                        File.Delete(videoFile);
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        File.Delete(audioFile);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 执行
        /// C# Process进程调用 https://docs.microsoft.com/zh-cn/dotnet/api/system.diagnostics.process?view=net-5.0
        /// </summary>
        /// <param name="commandStr">执行命令</param>
        private static void CommandManager(this VideoRecorderComponent self,string ffmpegPath, string commandStr)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = self._settings.ffmpegPath;//要执行的程序名称(属性，获取或设置要启动的应用程序或文档。FileName 属性不需要表示可执行文件。 它可以是其扩展名已经与系统上安装的应用程序关联的任何文件类型。)
                    process.StartInfo.Arguments = " " + commandStr;//启动该进程时传递的命令行参数
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = false;//可能接受来自调用程序的输入信息  
                    process.StartInfo.RedirectStandardOutput = false;//由调用程序获取输出信息   
                    process.StartInfo.RedirectStandardError = false;//重定向标准错误输出
                    process.StartInfo.CreateNoWindow = false;//不显示程序窗口
                    process.Start();//启动程序
                    process.WaitForExit();//等待程序执行完退出进程(避免进程占用文件或者是合成文件还未生成)*
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
