using SharpCdda.AudioSource;
using System;

namespace SharpCdda.DiscWriter
{
    public interface IAudioCDWriter : IDisposable
    {
        // Events
        event EventHandler WriteCompleted;
        event EventHandler ProgressChanged;

        #region Properties

        /// <summary>
        /// Disc drive
        /// </summary>
        DiscDrive Drive { get; }

        /// <summary>
        /// Gets the progress in percentage.
        /// </summary>
        int Progress { get; }

        /// <summary>
        /// Sets or gets whether, if a used CD-RW is inserted, the media will be automatically erased before writing tracks to it.
        /// </summary>
        bool AutoEraseRewritableMedia { set; get; }

        #endregion

        /// <summary>
        /// Add audio track.
        /// </summary>
        /// <param name="source"></param>
        void AddAudioTrack(IAudioSource source);

        /// <summary>
        /// Delete all tracks that are to be burned.
        /// </summary>
        void ClearAudioTracks();

        /// <summary>
        /// Excludes the specified tracks from being burned.
        /// </summary>
        /// <param name="source"></param>
        void RemoveAudioTrack(IAudioSource source);

        /// <summary>
        /// Exclude the track at the specified index from being written.
        /// </summary>
        /// <param name="index"></param>
        void RemoveAudioTrackAt(int index);

        /// <summary>
        /// Exclude all the tracks you want to burn.
        /// </summary>
        void Clear();

        /// <summary>
        /// Write all the tracks, the thread will be busy until the writing is finished.
        /// </summary>
        void WriteSynchronously();

        /// <summary>
        /// Burns tracks to disc in the background.
        /// When writing is finished, the WriteCompleted event is raised.
        /// </summary>
        void Write();
    }
}
