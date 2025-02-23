using IMAPI2;
using SharpCdda.AudioSource;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpCdda.DiscWriter
{
    public class AudioCDWriterBase : IAudioCDWriter
    {
        // Protected fields.
        protected readonly List<IAudioSource> tracks;
        protected DiscDrive drive;
        protected int progressPercent;

        // Private fields.
        private readonly List<IntPtr> allocatedHGlobalPointers;
        private bool isDisposed;

        // Events.
        public event EventHandler WriteCompleted;
        public event EventHandler ProgressChanged;

        // Constructor
        public AudioCDWriterBase(DiscDrive drive)
        {
            Engine.ThrowExceptionIfNotStarted();

            this.tracks = new List<IAudioSource>();
            this.allocatedHGlobalPointers = new List<IntPtr>();
            this.drive = drive;
        }

        // Destructor
        ~AudioCDWriterBase()
        {
            Dispose();
        }

        #region Properties

        /// <summary>
        /// Disc drive
        /// </summary>
        public DiscDrive Drive
        {
            get
            {
                return this.drive;
            }
        }

        /// <summary>
        /// Gets the progress in percentage.
        /// </summary>
        public int Progress
        {
            get
            {
                return this.progressPercent;
            }
        }

        /// <summary>
        /// Sets or gets whether, if a used CD-RW is inserted, the media will be automatically erased before writing tracks to it.
        /// </summary>
        public bool AutoEraseRewritableMedia { set; get; } = false;

        #endregion

        protected void InvokeProgressChangedEvent(object sender, EventArgs args)
        {
            this.ProgressChanged?.Invoke(sender, args);
        }

        protected void InvokeWriteCompletedEvent(object sender, EventArgs args)
        {
            this.WriteCompleted?.Invoke(sender, args);
        }

        #region Unmanaged Memory

        /// <summary>
        /// Allocate HGlobal memory.
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        protected IntPtr Allocate(int cb)
        {
            var result = Marshal.AllocHGlobal(cb);
            this.allocatedHGlobalPointers.Add(result);

            return result;
        }

        /// <summary>
        /// Release all allocated HGlobal memory.
        /// </summary>
        protected void ReleaseAllocatedHGlobals()
        {
            // Release HGlobal resources.
            foreach (IntPtr ptr in this.allocatedHGlobalPointers)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// Read PCM audio data to unmanaged memory.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected IntPtr ReadPCMAudioData(IAudioSource source)
        {
            var ptr = Allocate(source.Size);
            var offset = ptr;
            var readBuffer = new byte[source.SampleRate * source.Channels * (source.BitsPerSample / 8)];
            var totalBytesRead = 0;
            var isEndOfData = false;

            while (!isEndOfData && totalBytesRead != source.Size)
            {
                int read = source.Read(readBuffer, 0, readBuffer.Length);
                isEndOfData = read != readBuffer.Length;

                Marshal.Copy(readBuffer, 0, offset, read);
                offset = IntPtr.Add(offset, read);
                totalBytesRead += read;
            }

            return ptr;
        }

        #endregion

        #region Disc eraser

        /// <summary>
        /// Erase old data.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void EraseOldData(MsftDiscFormat2RawCD writer)
        {
            bool isRewritableMediaAvailable = writer.CurrentPhysicalMediaType == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW;
            bool isBlankMediaAvailable = writer.MediaPhysicallyBlank;

            if (isRewritableMediaAvailable)
            {
                if (!isBlankMediaAvailable)
                {
                    if (this.AutoEraseRewritableMedia)
                    {
                        var eraser = new DiscEraser(this.drive);
                        eraser.EraseMedia();
                    }
                    else
                    {
                        throw new Exception("The disc has been used. Please format the disc before writing tracks to it.");
                    }
                }
            }
            else
            {
                if (!isBlankMediaAvailable)
                {
                    throw new Exception("The disc has been used. This disc is non-rewritable and cannot be overwritten.");
                }
            }
        }

        /// <summary>
        /// Erase old data.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void EraseOldData(MsftDiscFormat2TrackAtOnce writer)
        {
            bool isRewritableMediaAvailable = writer.CurrentPhysicalMediaType == IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW;
            bool isBlankMediaAvailable = writer.MediaPhysicallyBlank;

            if (isRewritableMediaAvailable)
            {
                if (!isBlankMediaAvailable)
                {
                    if (this.AutoEraseRewritableMedia)
                    {
                        var eraser = new DiscEraser(this.drive);
                        eraser.EraseMedia();
                    }
                    else
                    {
                        throw new Exception("The disc has been used. Please format the disc before writing tracks to it.");
                    }
                }
            }
            else
            {
                if (!isBlankMediaAvailable)
                {
                    throw new Exception("The disc has been used. This disc is non-rewritable and cannot be overwritten.");
                }
            }
        }

        #endregion

        /// <summary>
        /// Cast unsigned 64-bits integer to _ULARGE_INTEGER.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected _ULARGE_INTEGER ConvertToULargeInteger(ulong value)
        {
            return new _ULARGE_INTEGER() { QuadPart = value };
        }

        /// <summary>
        /// Add audio track.
        /// </summary>
        /// <param name="source"></param>
        public virtual void AddAudioTrack(IAudioSource source)
        {
            this.tracks.Add(source);
        }

        /// <summary>
        /// Delete all tracks that are to be burned.
        /// </summary>
        public virtual void ClearAudioTracks()
        {
            this.tracks.Clear();
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Excludes the specified tracks from being burned.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RemoveAudioTrack(IAudioSource source)
        {
            this.tracks.Remove(source);
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Exclude the track at the specified index from being written.
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveAudioTrackAt(int index)
        {
            this.tracks.RemoveAt(index);
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Exclude all the tracks you want to burn.
        /// </summary>
        public virtual void Clear()
        {
            this.tracks.Clear();
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            ReleaseAllocatedHGlobals();

            this.drive = null;
            this.isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Burns tracks to disc in the background.
        /// When writing is finished, the WriteCompleted event is raised.
        /// </summary>
        public virtual void Write()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write all the tracks, the thread will be busy until the writing is finished.
        /// </summary>
        public virtual void WriteSynchronously()
        {
            throw new NotImplementedException();
        }
    }
}
