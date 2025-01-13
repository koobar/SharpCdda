using IMAPI2;
using SharpCdda.AudioSource;
using SharpCdda.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpCdda.DiscWriter
{
    public class TrackAtOnceWriter : IAudioCDWriter
    {
        // Private fields.
        private readonly DiscDrive drive;
        private readonly List<IAudioSource> tracks;
        private int numOfTracksWritten;
        private int progressPercent;

        // Events
        public event EventHandler WriteCompleted;
        public event EventHandler ProgressChanged;

        // Constructor
        public TrackAtOnceWriter(DiscDrive drive)
        {
            Engine.ThrowExceptionIfNotStarted();

            this.drive = drive;
            this.tracks = new List<IAudioSource>();
            this.numOfTracksWritten = 0;
        }

        #region Properties

        /// <summary>
        /// Drive
        /// </summary>
        public DiscDrive Drive
        {
            get
            {
                return this.drive;
            }
        }

        /// <summary>
        /// Gets the number of tracks that have been written.
        /// </summary>
        public int NumOfTracksWritten
        {
            get
            {
                return this.numOfTracksWritten;
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
            this.numOfTracksWritten = 0;
            UpdateProgress();

            // Create instance of disc writer.
            var writer = new MsftDiscFormat2TrackAtOnce();
            writer.Recorder = this.drive.GetRecorder();
            writer.ClientName = Engine.GetIMAPIClientName();

            // Erase old data if needed.
            EraseOldData(writer);

            // Prepare media.
            writer.PrepareMedia();
            this.drive.AcquireExclusiveAccess(Engine.GetIMAPIClientName());

            foreach (var track in this.tracks)
            {
                if (!Cdda.CheckAudioSourceCompatibleWithCDDA(track))
                {
                    throw new UnsupportedAudioFormatException();
                }

                int actualSizeOnDisc = Cdda.GetActualSizeOnDisc(track.Size);

                // Allocate HGlobal memory and read PCM data.
                var ptr = Marshal.AllocHGlobal(actualSizeOnDisc);
                track.Read(ptr, track.Size);

                // Create IStream instance.
                OleWrapper.CreateStreamOnHGlobal(ptr, false, out var istream);

                // Add audio track.
                writer.AddAudioTrack(istream);

                // Update progress.
                this.numOfTracksWritten++;
                UpdateProgress();

                // Release resources.
                track.Dispose();
                Marshal.FreeHGlobal(ptr);
                Marshal.ReleaseComObject(istream);
            }

            // Release media
            writer.ReleaseMedia();
            this.drive.ReleaseExclusiveAccess();
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
        private void UpdateProgress()
        {
            this.progressPercent = (int)((double)this.numOfTracksWritten / this.tracks.Count * 100.0);
            this.ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Erase old data.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void EraseOldData(MsftDiscFormat2TrackAtOnce writer)
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

        private void OnWriteCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.WriteCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
