using IMAPI2;
using SharpCdda.AudioSource;
using SharpCdda.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpCdda.DiscWriter
{
    public class DiscAtOnceWriter : IAudioCDWriter
    {
        // Private fields.
        private readonly DiscDrive drive;
        private readonly List<IAudioSource> tracks;
        private readonly List<IntPtr> allocatedHGlobalPointers;
        private int progressPercent;

        // Events
        public event EventHandler WriteCompleted;
        public event EventHandler ProgressChanged;

        // Constructor
        public DiscAtOnceWriter(DiscDrive drive)
        {
            Engine.ThrowExceptionIfNotStarted();

            this.drive = drive;
            this.tracks = new List<IAudioSource>();
            this.allocatedHGlobalPointers = new List<IntPtr>();
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

        /// <summary>
        /// Add audio track.
        /// </summary>
        /// <param name="source"></param>
        public void AddAudioTrack(IAudioSource source)
        {
            this.tracks.Add(source);
        }

        /// <summary>
        /// Delete all tracks that are to be burned.
        /// </summary>
        public void ClearAudioTracks()
        {
            this.tracks.Clear();
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Excludes the specified tracks from being burned.
        /// </summary>
        /// <param name="source"></param>
        public void RemoveAudioTrack(IAudioSource source)
        {
            this.tracks.Remove(source);
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Exclude the track at the specified index from being written.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAudioTrackAt(int index)
        {
            this.tracks.RemoveAt(index);
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Exclude all the tracks you want to burn.
        /// </summary>
        public void Clear()
        {
            this.tracks.Clear();
            this.tracks.TrimExcess();
        }

        /// <summary>
        /// Write all the tracks, the thread will be busy until the writing is finished.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void WriteSynchronously()
        {
            // Reset progress.
            UpdateProgress(0);

            // Create instance of writer.
            var writer = new MsftDiscFormat2RawCD();
            writer.Recorder = this.drive.GetRecorder();
            writer.ClientName = Engine.GetIMAPIClientName();

            // Create DAO disc image.
            var image = CreateDiscImageStream();

            // Erase old data if needed.
            EraseOldData(writer);
            UpdateProgress(70);

            // Prepare media.
            writer.PrepareMedia();
            this.drive.AcquireExclusiveAccess(Engine.GetIMAPIClientName());

            // Write disc image.
            writer.RequestedSectorType = IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE.IMAPI_FORMAT2_RAW_CD_SUBCODE_IS_RAW;
            writer.WriteMedia(image);
            UpdateProgress(99);

            // Release media
            writer.ReleaseMedia();
            this.drive.ReleaseExclusiveAccess();

            // Release HGlobal resources.
            foreach (IntPtr ptr in this.allocatedHGlobalPointers)
            {
                Marshal.FreeHGlobal(ptr);
            }

            UpdateProgress(100);
        }

        /// <summary>
        /// Burns tracks to disc in the background.
        /// When writing is finished, the WriteCompleted event is raised.
        /// </summary>
        public void Write()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                WriteSynchronously();
            };
            worker.RunWorkerCompleted += OnWriteCompleted;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Update the progress percentage.
        /// </summary>
        private void UpdateProgress(int progress)
        {
            this.progressPercent = progress;
            this.ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Erase old data.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void EraseOldData(MsftDiscFormat2RawCD writer)
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
                if (isBlankMediaAvailable)
                {
                    throw new Exception("The disc has been used. This disc is non-rewritable and cannot be overwritten.");
                }
            }
        }

        /// <summary>
        /// Allocate HGlobal memory.
        /// </summary>
        /// <param name="cb"></param>
        /// <returns></returns>
        private IntPtr Allocate(int cb)
        {
            var result = Marshal.AllocHGlobal(cb);
            this.allocatedHGlobalPointers.Add(result);

            return result;
        }

        /// <summary>
        /// Cast unsigned 64-bits integer to _ULARGE_INTEGER.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static _ULARGE_INTEGER ConvertToULargeInteger(ulong value)
        {
            return new _ULARGE_INTEGER() { QuadPart = value };
        }

        /// <summary>
        /// Create audio stream as IStream on HGlobal.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private IStream CreateAudioStream(IAudioSource source, out uint size)
        {
            var ptr = Allocate(source.Size);
            source.Read(ptr, source.Size);
            size = (uint)source.Size;

            OleWrapper.CreateStreamOnHGlobal(ptr, false, out var result);
            return result;
        }

        /// <summary>
        /// Create Disc At Once disc image.
        /// </summary>
        /// <returns></returns>
        private IStream CreateDiscImageStream()
        {
            var image = new MsftRawCDImageCreator();
            uint totalSize = 0;
            int cnt = 0;

            foreach (var track in this.tracks)
            {
                if (!Cdda.CheckAudioSourceCompatibleWithCDDA(track))
                {
                    throw new UnsupportedAudioFormatException();
                }

                var istream = CreateAudioStream(track, out uint size);
                totalSize += size;

                // Add audio track.
                image.AddTrack(IMAPI_CD_SECTOR_TYPE.IMAPI_CD_SECTOR_AUDIO, istream);

                // Release resources.
                track.Dispose();

                // Update progress
                int a = (int)((++cnt) / (double)this.tracks.Count) * 100;
                UpdateProgress(a / 2);      // max = 50%
            }

            // Compute file data size on disc.
            ulong fileDataSize = totalSize / 2048;
            fileDataSize = (fileDataSize + 1) * 2048;

            // Allocate memory.
            IntPtr fileData = Allocate((int)fileDataSize - 1);

            // Create instance of IStream on HGlobal.
            OleWrapper.CreateStreamOnHGlobal(fileData, true, out var result);
            result.SetSize(ConvertToULargeInteger(fileDataSize));

            // Create disc image.
            result = image.CreateResultImage();
            UpdateProgress(60);

            return result;
        }

        private void OnWriteCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.WriteCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
