using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

public static class Helpers
{
    private static string _youtubeLinkPattern = @"^(https?:\/\/)?(www\.)?(youtube\.com\/(watch\?v=|shorts\/)|youtu\.be\/)[a-zA-Z0-9_-]{11}(&.*)?$";
    private static string _youtubePlaylistPattern = @"^(https?:\/\/)?(www\.)?youtube\.com\/playlist\?list=[a-zA-Z0-9_-]+$";
    private static string[] _youtubeAudioExtensions = new string[] { "mp4", "opus", "webm", "m4a", "flac", "mp3" };

    public static bool IsYouTubeURIValid(this string url)
    {
        return Regex.IsMatch(url, _youtubeLinkPattern) || Regex.IsMatch(url, _youtubePlaylistPattern);
    }

    public static void DownloadYouTubeVideo(string youtubeVideoURI, bool isAudioOnly, string outputFilePath)
    {
        Process cmd = new Process();

        cmd.StartInfo.FileName = "cmd.exe";
        cmd.StartInfo.RedirectStandardInput = true;
        cmd.StartInfo.RedirectStandardOutput = false;
        cmd.StartInfo.CreateNoWindow = true;
        cmd.StartInfo.UseShellExecute = false;
        cmd.StartInfo.WorkingDirectory = Path.GetFullPath("tools");

        cmd.Start();

        if (!isAudioOnly)
        {
            cmd.StandardInput.WriteLine($"yt-dlp.exe -f bestvideo+bestaudio --merge-output-format mkv -o \"{outputFilePath}\" --cookies cookies.txt -N 16 {youtubeVideoURI}");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
        else
        {
            string tempFile = outputFilePath.Substring(0, outputFilePath.Length - ".wav".Length);

            cmd.StandardInput.WriteLine($"yt-dlp.exe -f bestaudio -x --audio-quality 0 -o \"{tempFile}.%(ext)s\" --cookies cookies.txt -N 16 {youtubeVideoURI}");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            Process ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = "cmd.exe";
            ffmpeg.StartInfo.RedirectStandardInput = true;
            ffmpeg.StartInfo.RedirectStandardOutput = false;
            ffmpeg.StartInfo.CreateNoWindow = true;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.WorkingDirectory = Path.GetFullPath("tools");

            ffmpeg.Start();

            string targetExtension = "";

            foreach (string extension in _youtubeAudioExtensions)
            {
                if (File.Exists(tempFile + "." + extension))
                {
                    targetExtension = extension;
                    break;
                }
            }

            ffmpeg.StandardInput.WriteLine($"ffmpeg.exe -i \"{tempFile}.{targetExtension}\" -ar 96000 -ac 2 -c:a pcm_f32le -sample_fmt flt \"{outputFilePath}\"");
            ffmpeg.StandardInput.Flush();
            ffmpeg.StandardInput.Close();
            ffmpeg.WaitForExit();

            while (true)
            {
                try
                {
                    File.Delete(tempFile + "." + targetExtension);
                    break;
                }
                catch
                {

                }
            }
        }
    }
}