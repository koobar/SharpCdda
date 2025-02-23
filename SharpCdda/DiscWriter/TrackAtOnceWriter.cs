using IMAPI2;
using SharpCdda.Exceptions;
using SharpCdda.Utils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static SharpCdda.COMErrorCodeDefinition;

namespace SharpCdda.DiscWriter
{
    public class TrackAtOnceWriter : AudioCDWriterBase
    {
        // Private fields.
        private int numOfTracksWritten;

        // Constructor
        public TrackAtOnceWriter(DiscDrive drive) : base(drive)
        {
            this.numOfTracksWritten = 0;
        }

        /// <summary>
        /// Write all the tracks, the thread will be busy until the writing is finished.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void WriteSynchronously()
        {
            try
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
                    var pPCMAudioData = ReadPCMAudioData(track);

                    // Create IStream instance.
                    OleWrapper.CreateStreamOnHGlobal(pPCMAudioData, false, out var istream);

                    // Add audio track.
                    writer.AddAudioTrack(istream);

                    // Update progress.
                    this.numOfTracksWritten++;
                    UpdateProgress();

                    // Release resources.
                    track.Dispose();
                    Marshal.FreeHGlobal(pPCMAudioData);
                    Marshal.ReleaseComObject(istream);
                }

                // Release media
                writer.ReleaseMedia();
                this.drive.ReleaseExclusiveAccess();
            }
            catch (COMException e)
            {
                switch (e.ErrorCode)
                {
                    case COM_EXCEPTION_ERRORCODE_UNSUPPORTED_MEDIA_TYPE_DETECTED:
                        throw new UnsupportedMediaTypeException();
                    default:
                        throw e;
                }
            }
        }

        /// <summary>
        /// Burns tracks to disc in the background.
        /// When writing is finished, the WriteCompleted event is raised.
        /// </summary>
        public override void Write()
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
            InvokeProgressChangedEvent(this, EventArgs.Empty);
        }

        private void OnWriteCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InvokeWriteCompletedEvent(this, EventArgs.Empty);
        }
    }
}
