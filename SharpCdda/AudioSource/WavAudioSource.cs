using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpCdda.AudioSource
{
    public class WavAudioSource : IAudioSource
    {
        [StructLayout(LayoutKind.Sequential)]
        struct FmtChunk
        {
            public int formatChunkId;
            public int formatChunkSize;
            public ushort audioFormat;
            public ushort numChannels;
            public int sampleRate;
            public int byteRate;
            public ushort blockAlign;
            public ushort bitsPerSample;
        }
        
        // Private fields.
        private readonly BinaryReader streamReader;
        private readonly FmtChunk fmtChunk;
        private readonly uint dataChunkSize;

        // Constructor
        public WavAudioSource(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            this.streamReader = new BinaryReader(File.OpenRead(path));
            this.fmtChunk = ReadFmtChunk();

            if (MoveToChunk("data"))
            {
                this.dataChunkSize = this.streamReader.ReadUInt32();
            }
        }

        #region Implementation of IAudioSource interface.

        public int SampleRate
        {
            get
            {
                return this.fmtChunk.sampleRate;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return this.fmtChunk.bitsPerSample;
            }
        }

        public int Channels
        {
            get
            {
                return this.fmtChunk.numChannels;
            }
        }

        public bool IsFloat { private set; get; }

        public int Size
        {
            get
            {
                return (int)this.dataChunkSize;
            }
        }

        public int Position
        {
            set
            {
                this.streamReader.BaseStream.Position = value;
            }
            get
            {
                return (int)this.streamReader.BaseStream.Position;
            }
        }

        public int Read(IntPtr pBuffer, int count)
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            IntPtr pOffset = pBuffer;
            int bytesRead = 0;

            while (bytesRead < count)
            {
                // Read PCM data to buffer.
                int read = this.streamReader.Read(buffer, 0, Math.Min(bufferSize, count));

                // Copy PCM data from buffer to unmanaged memory.
                Marshal.Copy(buffer, 0, pOffset, read);
                bytesRead += read;
                pOffset = IntPtr.Add(pOffset, read);

                // If the buffer size and the number of bytes actually read into the buffer don't match,
                // it is treated as the end of the stream.
                if (read != bufferSize)
                {
                    break;
                }
            }

            int diff = count - bytesRead;
            for (int i = 0; i < diff; ++i)
            {
                Marshal.WriteByte(pOffset, 0);
                pOffset = IntPtr.Add(pOffset, 1);
            }

            return bytesRead;
        }

        public void Dispose()
        {
            this.streamReader.BaseStream.Dispose();
            this.streamReader.Dispose();
        }

        #endregion

        #region For analyze RIFF chunks.

        /// <summary>
        /// Checks whether the end of the stream has been read.
        /// </summary>
        /// <returns></returns>
        private bool IsEndOfStream()
        {
            return this.streamReader.BaseStream.Position >= this.streamReader.BaseStream.Length;
        }

        /// <summary>
        /// Reads the same number of bytes of data as the given binary data from the stream and checks whether it matches the given data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckMatchNextBytes(byte[] data)
        {
            for (int i = 0; i < data.Length && !IsEndOfStream(); ++i)
            {
                if (this.streamReader.ReadByte() != data[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Move to specified chunk.
        /// </summary>
        /// <param name="chunkName">Chunk name</param>
        /// <param name="findFromBegin">Whether to seek chunks from the beginning of the stream</param>
        /// <returns>Returns true if the chunk exists and was moved successfully, or false if the chunk does not exist or the move failed.</returns>
        private bool MoveToChunk(string chunkName, bool findFromBegin = true)
        {
            var data = Encoding.ASCII.GetBytes(chunkName);
            if (findFromBegin)
            {
                this.streamReader.BaseStream.Position = 0;
            }

            while (!IsEndOfStream())
            {
                if (CheckMatchNextBytes(data))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Move to fmt chunk ansd read it.
        /// </summary>
        private FmtChunk ReadFmtChunk()
        {
            FmtChunk fmt = new FmtChunk();

            if (MoveToChunk("fmt "))
            {
                this.streamReader.BaseStream.Position += 4;
                fmt.audioFormat = this.streamReader.ReadUInt16();

                if (fmt.audioFormat == 0x0001 || fmt.audioFormat == 0xFFFE)
                {
                    fmt.numChannels = this.streamReader.ReadUInt16();
                    fmt.sampleRate = this.streamReader.ReadInt32();
                    fmt.byteRate = this.streamReader.ReadInt32();
                    fmt.blockAlign = this.streamReader.ReadUInt16();
                    fmt.bitsPerSample = this.streamReader.ReadUInt16();

                    this.IsFloat = false;
                }
                else if (fmt.audioFormat == 0x0003)
                {
                    fmt.numChannels = this.streamReader.ReadUInt16();
                    fmt.sampleRate = this.streamReader.ReadInt32();
                    fmt.byteRate = this.streamReader.ReadInt32();
                    fmt.blockAlign = this.streamReader.ReadUInt16();
                    fmt.bitsPerSample = 32;

                    this.IsFloat = true;
                }
            }

            return fmt;
        }

        #endregion
    }
}
