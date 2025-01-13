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
        /// Read PCM data and store it in unmanaged memory.
        /// </summary>
        /// <param name="pBuffer">The pointer of unmanaged memory</param>
        /// <param name="bytesToRead">Bytes to read.</param>
        /// <returns>Number of bytes actually read</returns>
        int Read(IntPtr pBuffer, int bytesToRead);
    }
}
