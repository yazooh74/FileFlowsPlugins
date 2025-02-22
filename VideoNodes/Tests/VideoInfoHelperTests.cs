﻿#if(DEBUG)

namespace VideoNodes.Tests
{
    using FileFlows.VideoNodes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class VideoInfoHelperTests
    {
        [TestMethod]
        public void VideoInfoTest_JudgeDreed()
        {
            var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger());
            var info = vi.Read(@"D:\videos\unprocessed\Injustice.mkv");
            Assert.IsNotNull(info);

        }

        [TestMethod]
        public void VideoInfoTest_SubtitleRemover()
        {
            const string file = @"D:\videos\unprocessed\Bourne.mkv";
            var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger());
            var vii = vi.Read(@"D:\videos\unprocessed\Masters of the Universe (1987) Bluray-1080p.mkv.skip");

            SubtitleRemover remover = new SubtitleRemover();
            remover.SubtitlesToRemove = new List<string>
            {
                "subrip", "srt"
            };
            var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
            args.GetToolPathActual = (string tool) => @"C:\utils\ffmpeg\ffmpeg.exe";
            args.TempPath = @"D:\videos\temp";

            new VideoFile().Execute(args);

            int output = remover.Execute(args);

            Assert.AreEqual(1, output);

        }

        [TestMethod]
        public void VideoInfoTest_DetectBlackBars()
        {
            //const string file = @"D:\videos\unprocessed\The Witcher - S02E05 - Turn Your Back.mkv";
            //const string file = @"D:\videos\unprocessed\Hawkeye (2021) - S01E05 - Ronin.mkv";
            const string file = @"\\ORACLE\tv\Dexter - New Blood\Season 1\Dexter - New Blood - S01E07 - Skin of Her Teeth.mkv";
            //var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger(), false, string.Empty);
            //vi.Read(@"D:\videos\unprocessed\Bourne.mkv");

            var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
            args.GetToolPathActual = (string tool) => @"C:\utils\ffmpeg\ffmpeg.exe";
            args.TempPath = @"D:\videos\temp";

            int result = new DetectBlackBars().Execute(args);

            Assert.IsTrue(result > 0);
        }


        [TestMethod]
        public void VideoInfoTest_NvidiaCard()
        {
            const string file = @"D:\videos\unprocessed\Bourne.mkv";
            const string ffmpeg = @"C:\utils\ffmpeg\ffmpeg.exe";
            var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
            //args.Process = new FileFlows.Plugin.ProcessHelper(args.Logger);

            var node = new VideoEncode();
            node.SetArgs(args);
            bool result = node.HasNvidiaCard(ffmpeg);

            Assert.IsTrue(result);
        }
        //[TestMethod]
        //public void VideoInfoTest_CanEncodeNvidia()
        //{
        //    const string file = @"D:\videos\unprocessed\Bourne.mkv";
        //    const string ffmpeg = @"C:\utils\ffmpeg\ffmpeg.exe";
        //    var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
        //    //args.Process = new FileFlows.Plugin.ProcessHelper(args.Logger);

        //    var node = new VideoEncode();
        //    node.SetArgs(args);
        //    bool result = node.CanProcessEncoder(ffmpeg, "hevc_nvenc -preset hq");

        //    Assert.IsTrue(result);
        //}
        //[TestMethod]
        //public void VideoInfoTest_CanEncodeIntel()
        //{
        //    const string file = @"D:\videos\unprocessed\Bourne.mkv";
        //    const string ffmpeg = @"C:\utils\ffmpeg\ffmpeg.exe";
        //    var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
        //    //args.Process = new FileFlows.Plugin.ProcessHelper(args.Logger);

        //    var node = new VideoEncode();
        //    node.SetArgs(args);
        //    bool result = node.CanProcessEncoder(ffmpeg, "h264_qsv");

        //    Assert.IsTrue(result);
        //}


        [TestMethod]
        public void VideoInfoTest_AudioTrackReorder()
        {
            var node = new AudioTrackReorder();
            var original = new List<AudioStream>
            {
                new AudioStream{ Codec = "aac", Language = "fre"},
                new AudioStream{ Codec = "dts", Language = "fre"},
                new AudioStream{ Codec = "aac", Language = "eng"},
                new AudioStream{ Codec = "aac", Language = "mao"},
                new AudioStream{ Codec = "dts", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "eng"},
                new AudioStream{ Codec = "ac3", Language = "fre"},
            };
            node.Languages = new List<string> { "eng" };
            node.OrderedTracks = new List<string> { "ac3", "aac" };
            var reordered = node.Reorder(original);

            Assert.AreEqual("ac3", reordered[0].Codec);
            Assert.AreEqual("eng", reordered[0].Language);

            Assert.AreEqual("aac", reordered[1].Codec);
            Assert.AreEqual("eng", reordered[1].Language);
            
            Assert.AreEqual("ac3", reordered[2].Codec);
            Assert.AreEqual("mao", reordered[2].Language);

            Assert.AreEqual("ac3", reordered[3].Codec);
            Assert.AreEqual("fre", reordered[3].Language);

            Assert.AreEqual("aac", reordered[4].Codec);
            Assert.AreEqual("fre", reordered[4].Language);

            Assert.AreEqual("aac", reordered[5].Codec);
            Assert.AreEqual("mao", reordered[5].Language);

            Assert.AreEqual("dts", reordered[6].Codec);
            Assert.AreEqual("fre", reordered[6].Language);

            Assert.AreEqual("dts", reordered[7].Codec);
            Assert.AreEqual("mao", reordered[7].Language);
        }



        [TestMethod]
        public void VideoInfoTest_AudioTrackReorder_Channels()
        {
            var node = new AudioTrackReorder();
            var original = new List<AudioStream>
            {
                new AudioStream{ Codec = "aac", Language = "fre", Channels = 5.1f},
                new AudioStream{ Codec = "dts", Language = "fre", Channels = 2},
                new AudioStream{ Codec = "aac", Language = "eng", Channels = 2.1f},
                new AudioStream{ Codec = "aac", Language = "mao", Channels = 8},
                new AudioStream{ Codec = "dts", Language = "mao" , Channels=7.1f} ,
                new AudioStream{ Codec = "ac3", Language = "mao", Channels = 6.2f},
                new AudioStream{ Codec = "ac3", Language = "eng", Channels = 5.1f},
                new AudioStream{ Codec = "ac3", Language = "fre", Channels = 8},
            };


            node.Channels = new List<string> { "8", "5.1", "7.1",  "6.2" };
            var reordered = node.Reorder(original);

            int count = 0;
            foreach (var chan in new[] { ("aac", "mao", 8f), ("ac3", "fre", 8), ("aac", "fre", 5.1f), ("ac3", "eng", 5.1f), ("dts", "mao", 7.1f),
                ("ac3", "mao", 6.2f), ("dts", "fre", 2), ("aac", "eng", 2.1f) })
            {
                Assert.AreEqual(chan.Item1, reordered[count].Codec);
                Assert.AreEqual(chan.Item2, reordered[count].Language);
                Assert.AreEqual(chan.Item3, reordered[count].Channels);
                ++count;
            }
        }

        [TestMethod]
        public void VideoInfoTest_AudioTrackReorder_NothingConfigured()
        {
            var node = new AudioTrackReorder();
            var original = new List<AudioStream>
            {
                new AudioStream{ Codec = "aac", Language = "fre"},
                new AudioStream{ Codec = "dts", Language = "fre"},
                new AudioStream{ Codec = "aac", Language = "eng"},
                new AudioStream{ Codec = "aac", Language = "mao"},
                new AudioStream{ Codec = "dts", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "eng"},
                new AudioStream{ Codec = "ac3", Language = "fre"},
            };
            node.Languages = null;
            node.OrderedTracks = new List<string>();
            var reordered = node.Reorder(original);

            for(int i = 0; i < original.Count; i++)
            {
                Assert.AreEqual(original[i].Codec, reordered[i].Codec);
                Assert.AreEqual(original[i].Language, reordered[i].Language);
            }
        }

        [TestMethod]
        public void VideoInfoTest_AudioTrackReorder_NoLanguage()
        {
            var node = new AudioTrackReorder();
            var original = new List<AudioStream>
            {
                new AudioStream{ Codec = "aac", Language = "fre"},
                new AudioStream{ Codec = "dts", Language = "fre"},
                new AudioStream{ Codec = "aac", Language = "eng"},
                new AudioStream{ Codec = "aac", Language = "mao"},
                new AudioStream{ Codec = "dts", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "eng"},
                new AudioStream{ Codec = "ac3", Language = "fre"},
            };
            node.OrderedTracks = new List<string> { "ac3", "aac" };
            var reordered = node.Reorder(original);

            Assert.AreEqual("ac3", reordered[0].Codec);
            Assert.AreEqual("mao", reordered[0].Language);

            Assert.AreEqual("ac3", reordered[1].Codec);
            Assert.AreEqual("eng", reordered[1].Language);

            Assert.AreEqual("ac3", reordered[2].Codec);
            Assert.AreEqual("fre", reordered[2].Language);

            Assert.AreEqual("aac", reordered[3].Codec);
            Assert.AreEqual("fre", reordered[3].Language);

            Assert.AreEqual("aac", reordered[4].Codec);
            Assert.AreEqual("eng", reordered[4].Language);

            Assert.AreEqual("aac", reordered[5].Codec);
            Assert.AreEqual("mao", reordered[5].Language);

            Assert.AreEqual("dts", reordered[6].Codec);
            Assert.AreEqual("fre", reordered[6].Language);

            Assert.AreEqual("dts", reordered[7].Codec);
            Assert.AreEqual("mao", reordered[7].Language);
        }



        [TestMethod]
        public void VideoInfoTest_AudioTrackReorder_NoCodec()
        {
            var node = new AudioTrackReorder();
            var original = new List<AudioStream>
            {
                new AudioStream{ Codec = "aac", Language = "fre"},
                new AudioStream{ Codec = "dts", Language = "fre"},
                new AudioStream{ Codec = "aac", Language = "eng"},
                new AudioStream{ Codec = "aac", Language = "mao"},
                new AudioStream{ Codec = "dts", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "mao"},
                new AudioStream{ Codec = "ac3", Language = "eng"},
                new AudioStream{ Codec = "ac3", Language = "fre"},
            };
            node.Languages = new List<string> { "eng" };
            var reordered = node.Reorder(original);

            Assert.AreEqual("aac", reordered[0].Codec);
            Assert.AreEqual("eng", reordered[0].Language);

            Assert.AreEqual("ac3", reordered[1].Codec);
            Assert.AreEqual("eng", reordered[1].Language);

            Assert.AreEqual("aac", reordered[2].Codec);
            Assert.AreEqual("fre", reordered[2].Language);

            Assert.AreEqual("dts", reordered[3].Codec);
            Assert.AreEqual("fre", reordered[3].Language);

            Assert.AreEqual("aac", reordered[4].Codec);
            Assert.AreEqual("mao", reordered[4].Language);

            Assert.AreEqual("dts", reordered[5].Codec);
            Assert.AreEqual("mao", reordered[5].Language);

            Assert.AreEqual("ac3", reordered[6].Codec);
            Assert.AreEqual("mao", reordered[6].Language);

            Assert.AreEqual("ac3", reordered[7].Codec);
            Assert.AreEqual("fre", reordered[7].Language);
        }


        [TestMethod]
        public void ComskipTest()
        {
            const string file = @"D:\videos\unprocessed\The IT Crowd - 2x04 - The Dinner Party - No English.mkv";
            const string ffmpeg = @"C:\utils\ffmpeg\ffmpeg.exe";
            var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);

            args.GetToolPathActual = (string tool) => @"C:\utils\ffmpeg\ffmpeg.exe";
            args.TempPath = @"D:\videos\temp";


            var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger());
            var vii = vi.Read(file);
            args.SetParameter("VideoInfo", vii);
            //args.Process = new FileFlows.Plugin.ProcessHelper(args.Logger);

            var node = new ComskipRemoveAds();
            int output = node.Execute(args);
            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Comskip_Chapters()
        {
            const string file = @"D:\videos\recordings\Rescue My Renovation (2001).ts";
            const string ffmpeg = @"C:\utils\ffmpeg\ffmpeg.exe";
            var logger = new TestLogger();
            var args = new FileFlows.Plugin.NodeParameters(file, logger, false, string.Empty);

            args.GetToolPathActual = (string tool) => @"C:\utils\ffmpeg\ffmpeg.exe";
            args.TempPath = @"D:\videos\temp";


            var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger());
            var vii = vi.Read(file);
            args.SetParameter("VideoInfo", vii);
            //args.Process = new FileFlows.Plugin.ProcessHelper(args.Logger);

            var node = new ComskipChapters();
            int output = node.Execute(args);
            Assert.AreEqual(1, output);
        }



        [TestMethod]
        public void VideoInfoTest_Subtitle_Extractor()
        {
            const string file = @"D:\videos\Injustice.mkv";
            var vi = new VideoInfoHelper(@"C:\utils\ffmpeg\ffmpeg.exe", new TestLogger());
            var vii = vi.Read(file);

            SubtitleExtractor node = new ();
            //node.OutputFile = file + ".sup";
            var args = new FileFlows.Plugin.NodeParameters(file, new TestLogger(), false, string.Empty);
            args.GetToolPathActual = (string tool) => @"C:\utils\ffmpeg\ffmpeg.exe";
            args.TempPath = @"D:\videos\temp";

            new VideoFile().Execute(args);

            int output = node.Execute(args);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void VideoInfoTest_AC1()
        {
            string ffmpegOutput = @"Input #0, mov,mp4,m4a,3gp,3g2,mj2, from '/media/Videos/#-Test Tdarr/Input3/input file.mp4':
  Metadata:
    major_brand     : mp42
    minor_version   : 512
    compatible_brands: mp42iso6
    creation_time   : 2022-01-07T06:30:47.000000Z
    title           : Episode title
    comment         : Episode description
  Duration: 00:42:23.75, start: 0.000000, bitrate: 3174 kb/s
    Stream #0:0(und): Video: h264 (High) (avc1 / 0x31637661), yuv420p, 1920x1080 [SAR 1:1 DAR 16:9], 2528 kb/s, 23.98 fps, 23.98 tbr, 24k tbn, 47.95 tbc (default)
    Metadata:
      creation_time   : 2022-01-07T06:30:47.000000Z
      handler_name    : VideoHandler
    Stream #0:1(deu): Audio: eac3 (ec-3 / 0x332D6365), 48000 Hz, 5.1(side), fltp, 640 kb/s (default)
    Metadata:
      creation_time   : 2022-01-07T06:30:47.000000Z
      handler_name    : SoundHandler
    Side data:
      audio service type: main";
            var vi = VideoInfoHelper.ParseOutput(null, ffmpegOutput);
            Assert.AreEqual(1920, vi.VideoStreams[0].Width);
            Assert.AreEqual(1080, vi.VideoStreams[0].Height);
        }


        [TestMethod]
        public void VideoInfoTest_Chapters()
        {
            string ffmpegOutput = @"[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 3 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 4 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
Input #0, matroska,webm, from 'D:\downloads\sabnzbd\complete\movies\Cast.Away.2000.BluRay.1080p.REMUX.AVC.DTS-HD.MA.5.1-LEGi0N\b0e4afee2ced4ae3a3592b82ae335608.mkv':
  Metadata:
    encoder         : libebml v1.4.2 + libmatroska v1.6.4
    creation_time   : 2022-02-02T22:32:47.000000Z
  Duration: 02:23:46.66, start: 0.000000, bitrate: 38174 kb/s
  Chapters:
    Chapter #0:0: start 0.000000, end 110.819000
      Metadata:
        title           : Chapter 01
    Chapter #0:1: start 110.819000, end 517.851000
      Metadata:
        title           : Chapter 02
    Chapter #0:2: start 517.851000, end 743.326000
      Metadata:
        title           : Chapter 03
    Chapter #0:3: start 743.326000, end 1061.269000
      Metadata:
        title           : Chapter 04
    Chapter #0:4: start 1061.269000, end 1243.534000
      Metadata:
        title           : Chapter 05
    Chapter #0:5: start 1243.534000, end 1360.234000
      Metadata:
        title           : Chapter 06
    Chapter #0:6: start 1360.234000, end 1545.461000
      Metadata:
        title           : Chapter 07
    Chapter #0:7: start 1545.461000, end 1871.620000
      Metadata:
        title           : Chapter 08
    Chapter #0:8: start 1871.620000, end 2155.320000
      Metadata:
        title           : Chapter 09
    Chapter #0:9: start 2155.320000, end 2375.623000
      Metadata:
        title           : Chapter 10
    Chapter #0:10: start 2375.623000, end 2543.207000
      Metadata:
        title           : Chapter 11
    Chapter #0:11: start 2543.207000, end 2794.208000
      Metadata:
        title           : Chapter 12
    Chapter #0:12: start 2794.208000, end 3109.314000
      Metadata:
        title           : Chapter 13
    Chapter #0:13: start 3109.314000, end 3389.052000
      Metadata:
        title           : Chapter 14
    Chapter #0:14: start 3389.052000, end 3694.357000
      Metadata:
        title           : Chapter 15
    Chapter #0:15: start 3694.357000, end 3873.119000
      Metadata:
        title           : Chapter 16
    Chapter #0:16: start 3873.119000, end 4391.846000
      Metadata:
        title           : Chapter 17
    Chapter #0:17: start 4391.846000, end 4657.736000
      Metadata:
        title           : Chapter 18
    Chapter #0:18: start 4657.736000, end 4749.745000
      Metadata:
        title           : Chapter 19
    Chapter #0:19: start 4749.745000, end 4842.045000
      Metadata:
        title           : Chapter 20
    Chapter #0:20: start 4842.045000, end 5197.901000
      Metadata:
        title           : Chapter 21
    Chapter #0:21: start 5197.901000, end 5640.176000
      Metadata:
        title           : Chapter 22
    Chapter #0:22: start 5640.176000, end 6037.365000
      Metadata:
        title           : Chapter 23
    Chapter #0:23: start 6037.365000, end 6321.398000
      Metadata:
        title           : Chapter 24
    Chapter #0:24: start 6321.398000, end 6458.368000
      Metadata:
        title           : Chapter 25
    Chapter #0:25: start 6458.368000, end 6810.470000
      Metadata:
        title           : Chapter 26
    Chapter #0:26: start 6810.470000, end 6959.953000
      Metadata:
        title           : Chapter 27
    Chapter #0:27: start 6959.953000, end 7499.575000
      Metadata:
        title           : Chapter 28
    Chapter #0:28: start 7499.575000, end 7707.575000
      Metadata:
        title           : Chapter 29
    Chapter #0:29: start 7707.575000, end 7941.725000
      Metadata:
        title           : Chapter 30
    Chapter #0:30: start 7941.725000, end 8214.414000
      Metadata:
        title           : Chapter 31
    Chapter #0:31: start 8214.414000, end 8626.656000
      Metadata:
        title           : Chapter 32
  Stream #0:0(eng): Video: h264 (High), yuv420p(progressive), 1920x1080 [SAR 1:1 DAR 16:9], 23.98 fps, 23.98 tbr, 1k tbn, 47.95 tbc (default)
    Metadata:
      title           : English
      BPS             : 33666894
      DURATION        : 02:23:46.618000000
      NUMBER_OF_FRAMES: 206832
      NUMBER_OF_BYTES : 36303929846
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:1(eng): Audio: dts (DTS-HD MA), 48000 Hz, 5.1(side), s32p (24 bit) (default)
    Metadata:
      title           : English
      BPS             : 4236399
      DURATION        : 02:23:46.624000000
      NUMBER_OF_FRAMES: 808746
      NUMBER_OF_BYTES : 4568228448
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:2(eng): Audio: ac3, 48000 Hz, stereo, fltp, 224 kb/s (comment)
    Metadata:
      title           : English commentary
      BPS             : 224000
      DURATION        : 02:23:46.656000000
      NUMBER_OF_FRAMES: 269583
      NUMBER_OF_BYTES : 241546368
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:3(eng): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : English (SDH)
      BPS             : 25275
      DURATION        : 02:14:32.439000000
      NUMBER_OF_FRAMES: 1740
      NUMBER_OF_BYTES : 25504616
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:4(spa): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : Spanish
      BPS             : 21585
      DURATION        : 02:12:54.884000000
      NUMBER_OF_FRAMES: 1412
      NUMBER_OF_BYTES : 21517695
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES";
            var vi = VideoInfoHelper.ParseOutput(null, ffmpegOutput);
            Assert.AreEqual(32, vi.Chapters?.Count ?? 0);
            Assert.AreEqual("Chapter 32", vi.Chapters[31].Title);
            Assert.AreEqual(TimeSpan.FromSeconds(8214.414000), vi.Chapters[31].Start);
            Assert.AreEqual(TimeSpan.FromSeconds(8626.656000), vi.Chapters[31].End);
        }



        [TestMethod]
        public void VideoInfoTest_Chapters_NoStart()
        {
            string ffmpegOutput = @"[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 3 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 4 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
Input #0, matroska,webm, from 'D:\downloads\sabnzbd\complete\movies\Cast.Away.2000.BluRay.1080p.REMUX.AVC.DTS-HD.MA.5.1-LEGi0N\b0e4afee2ced4ae3a3592b82ae335608.mkv':
  Metadata:
    encoder         : libebml v1.4.2 + libmatroska v1.6.4
    creation_time   : 2022-02-02T22:32:47.000000Z
  Duration: 02:23:46.66, start: 0.000000, bitrate: 38174 kb/s
  Chapters:
    Chapter #0:0: end 110.819000
      Metadata:
        title           : Chapter 01
    Chapter #0:1: start 110.819000, end 517.851000
      Metadata:
        title           : Chapter 02
    Chapter #0:2: start 517.851000, end 743.326000
      Metadata:
        title           : Chapter 03
    Chapter #0:3: start 743.326000, end 1061.269000
      Metadata:
        title           : Chapter 04
    Chapter #0:4: start 1061.269000, end 1243.534000
      Metadata:
        title           : Chapter 05
    Chapter #0:5: start 1243.534000, end 1360.234000
      Metadata:
        title           : Chapter 06
    Chapter #0:6: start 1360.234000, end 1545.461000
      Metadata:
        title           : Chapter 07
    Chapter #0:7: start 1545.461000, end 1871.620000
      Metadata:
        title           : Chapter 08
    Chapter #0:8: start 1871.620000, end 2155.320000
      Metadata:
        title           : Chapter 09
    Chapter #0:9: start 2155.320000, end 2375.623000
      Metadata:
        title           : Chapter 10
    Chapter #0:10: start 2375.623000, end 2543.207000
      Metadata:
        title           : Chapter 11
    Chapter #0:11: start 2543.207000, end 2794.208000
      Metadata:
        title           : Chapter 12
    Chapter #0:12: start 2794.208000, end 3109.314000
      Metadata:
        title           : Chapter 13
    Chapter #0:13: start 3109.314000, end 3389.052000
      Metadata:
        title           : Chapter 14
    Chapter #0:14: start 3389.052000, end 3694.357000
      Metadata:
        title           : Chapter 15
    Chapter #0:15: start 3694.357000, end 3873.119000
      Metadata:
        title           : Chapter 16
    Chapter #0:16: start 3873.119000, end 4391.846000
      Metadata:
        title           : Chapter 17
    Chapter #0:17: start 4391.846000, end 4657.736000
      Metadata:
        title           : Chapter 18
    Chapter #0:18: start 4657.736000, end 4749.745000
      Metadata:
        title           : Chapter 19
    Chapter #0:19: start 4749.745000, end 4842.045000
      Metadata:
        title           : Chapter 20
    Chapter #0:20: start 4842.045000, end 5197.901000
      Metadata:
        title           : Chapter 21
    Chapter #0:21: start 5197.901000, end 5640.176000
      Metadata:
        title           : Chapter 22
    Chapter #0:22: start 5640.176000, end 6037.365000
      Metadata:
        title           : Chapter 23
    Chapter #0:23: start 6037.365000, end 6321.398000
      Metadata:
        title           : Chapter 24
    Chapter #0:24: start 6321.398000, end 6458.368000
      Metadata:
        title           : Chapter 25
    Chapter #0:25: start 6458.368000, end 6810.470000
      Metadata:
        title           : Chapter 26
    Chapter #0:26: start 6810.470000, end 6959.953000
      Metadata:
        title           : Chapter 27
    Chapter #0:27: start 6959.953000, end 7499.575000
      Metadata:
        title           : Chapter 28
    Chapter #0:28: start 7499.575000, end 7707.575000
      Metadata:
        title           : Chapter 29
    Chapter #0:29: start 7707.575000, end 7941.725000
      Metadata:
        title           : Chapter 30
    Chapter #0:30: start 7941.725000, end 8214.414000
      Metadata:
        title           : Chapter 31
    Chapter #0:31: start 8214.414000, end 8626.656000
      Metadata:
        title           : Chapter 32
  Stream #0:0(eng): Video: h264 (High), yuv420p(progressive), 1920x1080 [SAR 1:1 DAR 16:9], 23.98 fps, 23.98 tbr, 1k tbn, 47.95 tbc (default)
    Metadata:
      title           : English
      BPS             : 33666894
      DURATION        : 02:23:46.618000000
      NUMBER_OF_FRAMES: 206832
      NUMBER_OF_BYTES : 36303929846
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:1(eng): Audio: dts (DTS-HD MA), 48000 Hz, 5.1(side), s32p (24 bit) (default)
    Metadata:
      title           : English
      BPS             : 4236399
      DURATION        : 02:23:46.624000000
      NUMBER_OF_FRAMES: 808746
      NUMBER_OF_BYTES : 4568228448
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:2(eng): Audio: ac3, 48000 Hz, stereo, fltp, 224 kb/s (comment)
    Metadata:
      title           : English commentary
      BPS             : 224000
      DURATION        : 02:23:46.656000000
      NUMBER_OF_FRAMES: 269583
      NUMBER_OF_BYTES : 241546368
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:3(eng): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : English (SDH)
      BPS             : 25275
      DURATION        : 02:14:32.439000000
      NUMBER_OF_FRAMES: 1740
      NUMBER_OF_BYTES : 25504616
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:4(spa): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : Spanish
      BPS             : 21585
      DURATION        : 02:12:54.884000000
      NUMBER_OF_FRAMES: 1412
      NUMBER_OF_BYTES : 21517695
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES";
            var vi = VideoInfoHelper.ParseOutput(null, ffmpegOutput);
            Assert.AreEqual(32, vi.Chapters?.Count ?? 0);
            Assert.AreEqual(TimeSpan.FromSeconds(0), vi.Chapters[0].Start);
            Assert.AreEqual(TimeSpan.FromSeconds(110.819000), vi.Chapters[0].End);
        }
        [TestMethod]
        public void VideoInfoTest_Chapters_Bad()
        {
            string ffmpegOutput = @"[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 3 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
[matroska,webm @ 00000263322abdc0] Could not find codec parameters for stream 4 (Subtitle: hdmv_pgs_subtitle (pgssub)): unspecified size
Consider increasing the value for the 'analyzeduration' (0) and 'probesize' (5000000) options
Input #0, matroska,webm, from 'D:\downloads\sabnzbd\complete\movies\Cast.Away.2000.BluRay.1080p.REMUX.AVC.DTS-HD.MA.5.1-LEGi0N\b0e4afee2ced4ae3a3592b82ae335608.mkv':
  Metadata:
    encoder         : libebml v1.4.2 + libmatroska v1.6.4
    creation_time   : 2022-02-02T22:32:47.000000Z
  Duration: 02:23:46.66, start: 0.000000, bitrate: 38174 kb/s
  Chapters:
    Chapter #0:0: end 110.819000
      Metadata:
        title           : Chapter 01
    Chapter #0:1: start 110.819000, end 517.851000
      Metadata:
        title           : Chapter 0200, end 5640.176000
      Metadata:
        title           : Chapter 2200, end 7499.575000
      Metadata:
        title           : Chapter 28
    Chapter #0:28: start 7499.575000, end 7707.575000
      Metadata:
        title           : Chapter 29
    Chapter #0:29: start 7707.575000, end 7941.725000
      Metadata:
        title           : Chapter 30
    Chapter #0:30: start 7941.725000, end 8214.414000
      Metadata:
        title           : Chapter 31
    Chapter #0:31: start 8214.414000, end 8626.656000
      Metadata:
        title           : Chapter 32
  Stream #0:0(eng): Video: h264 (High), yuv420p(progressive), 1920x1080 [SAR 1:1 DAR 16:9], 23.98 fps, 23.98 tbr, 1k tbn, 47.95 tbc (default)
    Metadata:
      title           : English
      BPS             : 33666894
      DURATION        : 02:23:46.618000000
      NUMBER_OF_FRAMES: 206832
      NUMBER_OF_BYTES : 36303929846
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:1(eng): Audio: dts (DTS-HD MA), 48000 Hz, 5.1(side), s32p (24 bit) (default)
    Metadata:
      title           : English
      BPS             : 4236399
      DURATION        : 02:23:46.624000000
      NUMBER_OF_FRAMES: 808746
      NUMBER_OF_BYTES : 4568228448
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:2(eng): Audio: ac3, 48000 Hz, stereo, fltp, 224 kb/s (comment)
    Metadata:
      title           : English commentary
      BPS             : 224000
      DURATION        : 02:23:46.656000000
      NUMBER_OF_FRAMES: 269583
      NUMBER_OF_BYTES : 241546368
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:3(eng): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : English (SDH)
      BPS             : 25275
      DURATION        : 02:14:32.439000000
      NUMBER_OF_FRAMES: 1740
      NUMBER_OF_BYTES : 25504616
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES
  Stream #0:4(spa): Subtitle: hdmv_pgs_subtitle
    Metadata:
      title           : Spanish
      BPS             : 21585
      DURATION        : 02:12:54.884000000
      NUMBER_OF_FRAMES: 1412
      NUMBER_OF_BYTES : 21517695
      _STATISTICS_WRITING_APP: mkvmerge v64.0.0 ('Willows') 64-bit
      _STATISTICS_WRITING_DATE_UTC: 2022-02-02 22:32:47
      _STATISTICS_TAGS: BPS DURATION NUMBER_OF_FRAMES NUMBER_OF_BYTES";
            var vi = VideoInfoHelper.ParseOutput(null, ffmpegOutput);
            Assert.AreEqual(6, vi.Chapters?.Count ?? 0);
            Assert.AreEqual("Chapter 29", vi.Chapters[2].Title);
        }
    }
}

#endif