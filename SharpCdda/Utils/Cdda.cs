using SharpCdda.AudioSource;

namespace SharpCdda.Utils
{
    internal static class Cdda
    {
        // Private constants.
        private const int AUDIO_SECTOR_SIZE = 2352;

        /// <summary>
        /// Calculates the size on disk of a specified number of bytes if it were written to disk.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int GetActualSizeOnDisc(int size)
        {
            var nBlocks = size / AUDIO_SECTOR_SIZE;
            if (size % AUDIO_SECTOR_SIZE != 0)
            {
                nBlocks++;
            }

            return nBlocks * AUDIO_SECTOR_SIZE;
        }

        /// <summary>
        /// Check if the format is CDDA compatible
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="bitsPerSample"></param>
        /// <param name="channels"></param>
        /// <param name="isFloat"></param>
        /// <returns></returns>
        public static bool CheckWaveFormatCompatibleWithCDDA(int sampleRate, int bitsPerSample, int channels, bool isFloat)
        {
            if (sampleRate != 44100 || bitsPerSample != 16 || channels != 2 || isFloat)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the format is CDDA compatible.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckAudioSourceCompatibleWithCDDA(IAudioSource source)
        {
            return CheckWaveFormatCompatibleWithCDDA(source.SampleRate, source.BitsPerSample, source.Channels, source.IsFloat);
        }
    }
}
