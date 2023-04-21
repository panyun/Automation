using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Video
{
    /// <summary>
    /// Defines the video format that should be used for recording.
    /// </summary>
    public enum VideoFormat
    {
        /// <summary>
        /// Small file size, high cpu usage.
        /// </summary>
        x264,
        /// <summary>
        /// Medium file size, low cpu usage.
        /// </summary>
        xvid
    }
    /// <summary>
    /// Settings class for the <see cref="VideoRecorderComponent"/>.
    /// </summary>
    public class VideoRecorderSettings
    {
        /// <summary>
        /// The path to ffmpeg.exe.
        /// </summary>
        public string ffmpegPath { get; set; }

        /// <summary>
        /// The framerate used for capturing and playback.
        /// Defaults to 5.
        /// </summary>
        public uint FrameRate { get; set; } = 5;

        /// <summary>
        /// The path to the target video file.
        /// </summary>
        public string TargetVideoPath { get; set; }

        /// <summary>
        /// Flag to indicate if compressed images should be captured.<para />
        /// Helps if the recording machine is limited on memory.<para />
        /// Defaults to true.
        /// </summary>
        public bool UseCompressedImages { get; set; } = true;

        /// <summary>
        /// Defines the video format that is used to record the video.<para />
        /// This has an influence on the file size and the cpu load during recording.<para />
        /// Defaults to <see cref="Capturing.VideoFormat.x264"/>.
        /// </summary>
        public VideoFormat VideoFormat { get; set; } = VideoFormat.x264;

        /// <summary>
        /// An integer defining the quality of the video. The value is dependent on the <see cref="VideoFormat"/>.<para />
        /// <see cref="Capturing.VideoFormat.x264"/>: From 0 (lossless) to 51 (worst). Sane values are from 18 to 28.<para />
        /// <see cref="Capturing.VideoFormat.xvid"/>: From 1 (lossless) to 31 (worst). Sane values are around 5.
        /// </summary>
        public int VideoQuality { get; set; }

        /// <summary>
        /// Run the encoding with low processor priority.
        /// </summary>
        public bool EncodeWithLowPriority { get; set; }
    }
}
