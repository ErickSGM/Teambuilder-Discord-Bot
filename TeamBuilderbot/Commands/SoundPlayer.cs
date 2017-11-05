using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord.Audio;

namespace TeamBuilderbot.Commands
{
    public class SoundPlayer
    {
        public async Task PlayStartSound(IAudioClient audioClient)
        {
            var countdownSound = new Random().Next(1, 7).ToString().PadLeft(3, '0');
            await SendCountDownAsync(audioClient, countdownSound);
            await SendCountDownAsync(audioClient, "start");
        }

        private Process CreateStream(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i Sounds/{path}.mp3 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            return Process.Start(ffmpeg);
        }

        private async Task SendCountDownAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            var ffmpeg = CreateStream(path);
            var output = ffmpeg.StandardOutput.BaseStream;
            var discord = client.CreatePCMStream(AudioApplication.Mixed);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
        }
    }
}