using System;

namespace SharpCdda.AudioSource
{
    public interface IAudioSource : IDisposable
    {
        /// <summary>
        /// Sample rate (Hz)
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        /// PCM Quantization bits.
        /// </summary>
        int BitsPerSample { get; }

        /// <summary>
        /// The number of channels.
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// Is IEEE float data.
        /// </summary>
        bool IsFloat { get; }

        /// <summary>
        /// Size of PCM data (Bytes)
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Stream reading position.
        /// </summary>
        int Position { set; get; }

        /// <summary>
        /// Read PCM 44.1Khz, 16bits, Stereo data to buffer.
        /// </summary>
        /// <param name="buffer">PCM data buffer.</param>
        /// <param name="offset">Buffer start offset.</param>
        /// <param name="count">Bytes to read.</param>
        /// <returns>Bytes read.</returns>
        int Read(byte[] buffer, int offset, int count);
    }
}
