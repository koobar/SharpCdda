using IMAPI2;
using SharpCdda.AudioSource;
using SharpCdda.Exceptions;
using SharpCdda.Utils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static SharpCdda.COMErrorCodeDefinition;

namespace SharpCdda.DiscWriter
{
    public class DiscAtOnceWriter : AudioCDWriterBase
    {
        // Constructor
        public DiscAtOnceWriter(DiscDrive drive) : base(drive) { }

        /// <summary>
        /// Write all the tracks, the thread will be busy until the writing is finished.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void WriteSynchronously()
        {
            try
            {
                // Reset progress.
                UpdateProgress(0);

                // Create instance of writer.
                var writer = new MsftDiscFormat2RawCD();
                writer.Recorder = this.drive.GetRecorder();
                writer.ClientName = Engine.GetIMAPIClientName();

                // Create Disc-At-Once disc image.
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
                ReleaseAllocatedHGlobals();

                UpdateProgress(100);
            }
            catch(COMException e)
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
        private void UpdateProgress(int progress)
        {
            this.progressPercent = progress;
            InvokeProgressChangedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create audio stream as IStream on HGlobal.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private IStream CreateAudioStream(IAudioSource source, out uint size)
        {
            var pPCMAudioData = ReadPCMAudioData(source);
            size = (uint)source.Size;

            OleWrapper.CreateStreamOnHGlobal(pPCMAudioData, false, out var result);
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
            InvokeWriteCompletedEvent(this, EventArgs.Empty);
        }
    }
}
