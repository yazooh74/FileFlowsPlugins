﻿namespace FileFlows.VideoNodes.FfmpegBuilderNodes
{
    public class FfmpegBuilderVideo10Bit : FfmpegBuilderNode
    {
        public override int Outputs => 1;

        public override string HelpUrl => "https://docs.fileflows.com/plugins/video-nodes/ffmpeg-builder/video-10-bit";

        public override int Execute(NodeParameters args)
        {
            var videoInfo = GetVideoInfo(args);
            if (videoInfo == null || videoInfo.VideoStreams?.Any() != true)
                return -1;

            var stream = Model?.VideoStreams?.Where(x => x.Deleted == false)?.FirstOrDefault();
            if (stream != null)
                stream.OptionalEncodingParameters.AddRange(new[] { "-pix_fmt:v:{index}", "yuv420p10le" });

            return 1;
        }
    }
}
