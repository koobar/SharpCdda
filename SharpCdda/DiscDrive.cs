using IMAPI2;

namespace SharpCdda
{
    public class DiscDrive
    {
        // Private fields.
        private readonly MsftDiscRecorder2 recorder;
        private readonly string volumeName;
        private readonly string productID;
        private readonly string productRevision;

        #region Constructors

        public DiscDrive(char driveLetter)
        {
            Engine.ThrowExceptionIfNotStarted();

            var discMaster = new MsftDiscMaster2();

            for (int i = 0; i < discMaster.Count; ++i)
            {
                var recorder = new MsftDiscRecorder2();
                recorder.InitializeDiscRecorder(discMaster[i]);

                var driveRootPath = recorder.VolumePathNames.GetValue(0).ToString();

                if (driveRootPath[0] == driveLetter)
                {
                    this.recorder = recorder;
                    this.volumeName = recorder.VolumePathNames.GetValue(0).ToString();
                    this.productID = recorder.ProductId;
                    this.productRevision = recorder.ProductRevision;
                }
            }
        }

        public DiscDrive(int index)
        {
            Engine.ThrowExceptionIfNotStarted();

            var discMaster = new MsftDiscMaster2();
            this.recorder = new MsftDiscRecorder2();
            this.recorder.InitializeDiscRecorder(discMaster[index]);

            this.volumeName = this.recorder.VolumePathNames.GetValue(0).ToString();
            this.productID = this.recorder.ProductId;
            this.productRevision = this.recorder.ProductRevision;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of disk drives available on the computer.
        /// </summary>
        public static int AvailableDriveCount
        {
            get
            {
                var discMaster = new MsftDiscMaster2();
                return discMaster.Count;
            }
        }

        /// <summary>
        /// Gets the volume name.
        /// </summary>
        public string VolumeName
        {
            get
            {
                return this.volumeName;
            }
        }

        /// <summary>
        /// Gets the vendor name of the drive.
        /// </summary>
        public string Vendor
        {
            get
            {
                return this.recorder.VendorId;
            }
        }

        /// <summary>
        /// Gets the product ID of the drive.
        /// </summary>
        public string ProductID
        {
            get
            {
                return this.productID;
            }
        }

        /// <summary>
        /// Gets the revision of the drive.
        /// </summary>
        public string ProductRevision
        {
            get
            {
                return this.productRevision;
            }
        }

        #endregion

        /// <summary>
        /// Get this recorder as an instance of IMAPI's MsftDiscRecorder2.
        /// </summary>
        /// <returns></returns>
        internal MsftDiscRecorder2 GetRecorder()
        {
            return this.recorder;
        }

        /// <summary>
        /// Get exclusive access to the drive.
        /// </summary>
        /// <param name="clientName"></param>
        public void AcquireExclusiveAccess(string clientName)
        {
            this.recorder.AcquireExclusiveAccess(true, clientName);
        }

        /// <summary>
        /// Release exclusive access to the drive.
        /// </summary>
        public void ReleaseExclusiveAccess()
        {
            this.recorder.ReleaseExclusiveAccess();
        }

        /// <summary>
        /// Eject
        /// </summary>
        public void Eject()
        {
            this.recorder.EjectMedia();
        }
    }
}
