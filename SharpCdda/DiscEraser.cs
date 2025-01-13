using IMAPI2;

namespace SharpCdda
{
    public class DiscEraser
    {
        // Private fields.
        private readonly IDiscFormat2Erase eraser;

        // Constructor
        public DiscEraser(DiscDrive drive)
        {
            Engine.ThrowExceptionIfNotStarted();

            this.eraser = new MsftDiscFormat2Erase();
            this.eraser.Recorder = drive.GetRecorder();
            this.eraser.ClientName = Engine.GetIMAPIClientName();
        }

        /// <summary>
        /// Sets or gets a value that indicates whether to write zeros to the entire media before erasing it.
        /// </summary>
        public bool EnableFullErase
        {
            set
            {
                this.eraser.FullErase = value;
            }
            get
            {
                return this.eraser.FullErase;
            }
        }

        /// <summary>
        /// Gets a value indicating whether used media (such as a CD-RW with data written to it) is inserted.
        /// </summary>
        public bool IsUsedMedia
        {
            get
            {
                return !this.eraser.MediaPhysicallyBlank;
            }
        }

        /// <summary>
        /// Erase media.
        /// </summary>
        public void EraseMedia()
        {
            this.eraser.EraseMedia();
        }
    }
}
