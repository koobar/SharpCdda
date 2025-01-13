using SharpCdda.AudioSource;
using SharpCdda.DiscWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SharpCdda.ExampleApp
{
    public partial class MainWindow : Form
    {
        // Private fields.
        private readonly List<string> tracks;
        private readonly List<DiscDrive> drives;

        // Constructor
        public MainWindow()
        {
            this.tracks = new List<string>();
            this.drives = new List<DiscDrive>();

            InitializeComponent();
        }

        #region Properties

        public bool IsTrackSelected
        {
            get
            {
                if (this.AudioTracksListView.SelectedIndices.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string[] SelectedTracks
        {
            get
            {
                if (!this.IsTrackSelected)
                {
                    return null;
                }

                var result = new string[this.AudioTracksListView.SelectedIndices.Count];

                for (int i = 0; i < this.AudioTracksListView.SelectedIndices.Count; ++i)
                {
                    result[i] = this.AudioTracksListView.Items[this.AudioTracksListView.SelectedIndices[i]].Tag.ToString();
                }

                return result;
            }
        }

        #endregion

        /// <summary>
        /// Write all tracks to disc.
        /// </summary>
        private void Write()
        {
            if (this.AvailableDrivesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select optical disc drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var drive = this.drives[this.AvailableDrivesComboBox.SelectedIndex];
            IAudioCDWriter writer = null;

            // Create writer instance.
            if (this.WriteModesComboBox.SelectedIndex == 0)
            {
                writer = new DiscAtOnceWriter(drive);
            }
            else if (this.WriteModesComboBox.SelectedIndex == 1)
            {
                writer = new TrackAtOnceWriter(drive);
            }
            writer.AutoEraseRewritableMedia = true;
            writer.WriteCompleted += OnWriterWriteCompleted;
            writer.ProgressChanged += OnWriterProgressChanged;

            // Add all tracks to write list.
            foreach (var path in this.tracks)
            {
                var source = new WavAudioSource(path);

                if (source.SampleRate != 44100 || source.BitsPerSample != 16 || source.Channels != 2)
                {
                    MessageBox.Show(
                        $"The track {Path.GetFileName(path)} is in an unsupported format.\r\n" +
                        $"CD-DA supports only PCM (44100Hz, 16-bits, stereo).", 
                        "Unsupported format", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    writer.Clear();
                    return;
                }

                writer.AddAudioTrack(source);
            }

            // Start write.
            SetUIWritingStat(true);
            writer.Write();
        }

        /// <summary>
        /// Switch control Enabled property.
        /// </summary>
        /// <param name="value"></param>
        private void SetUIWritingStat(bool value)
        {
            value = !value;

            this.AvailableDrivesComboBox.Enabled = value;
            this.WriteModesComboBox.Enabled = value;
            this.AudioTracksListView.Enabled = value;
            this.UpdateDriveListButton.Enabled = value;
            this.AddFileButton.Enabled = value;
            this.RemoveFileButton.Enabled = value;
            this.WriteToDiscButton.Enabled = value;
        }

        /// <summary>
        /// Update available disc drives.
        /// </summary>
        private void UpdateDiscDrives()
        {
            this.drives.Clear();

            int cnt = DiscDrive.AvailableDriveCount;
            for (int i = 0; i < cnt; ++i)
            {
                this.drives.Add(new DiscDrive(i));
            }
        }

        /// <summary>
        /// Update available disc drives ComboBox items.
        /// </summary>
        private void UpdateDriveList()
        {
            this.AvailableDrivesComboBox.Items.Clear();

            foreach (var drive in this.drives)
            {
                this.AvailableDrivesComboBox.Items.Add($"{drive.Vendor} {drive.ProductID} ({drive.VolumeName})");
            }
        }

        /// <summary>
        /// Update UI state.
        /// </summary>
        private void UpdateUIState()
        {
            this.RemoveFileButton.Enabled = this.IsTrackSelected;
        }

        /// <summary>
        /// Update tracklist view.
        /// </summary>
        private void UpdateTrackListView()
        {
            var items = new ListViewItem[this.tracks.Count];

            this.AudioTracksListView.Items.Clear();

            for (int i = 0; i < this.tracks.Count; ++i)
            {
                var path = this.tracks[i];
                var item = new ListViewItem();
                item.Text = Path.GetFileName(path);
                item.SubItems.Add((i + 1).ToString());
                item.Tag = path;

                items[i] = item;
            }

            this.AudioTracksListView.Items.AddRange(items);
            this.TitleColumnHeader.Width = -2;
            this.TrackNumberColumnHeader.Width = 50;
        }

        /// <summary>
        /// Add tracks to tracklist.
        /// </summary>
        /// <param name="tracks"></param>
        private void AddTracks(params string[] tracks)
        {
            foreach (var path in tracks)
            {
                if (this.tracks.Contains(path))
                {
                    if (MessageBox.Show(
                        $"Track {Path.GetFileName(path)} has already been added to the tracklist.\r\n Do you want to add a duplicate?",
                        "Duplicate tracks", 
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        continue;
                    }
                }

                this.tracks.Add(path);
            }

            UpdateTrackListView();
        }

        /// <summary>
        /// Remove track from tracklist.
        /// </summary>
        /// <param name="path"></param>
        private void RemoveTrack(string path)
        {
            this.tracks.Remove(path);
            UpdateTrackListView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetUIWritingStat(false);
            UpdateUIState();
            UpdateDiscDrives();
            UpdateDriveList();

            if (this.AvailableDrivesComboBox.Items.Count > 0)
            {
                this.AvailableDrivesComboBox.SelectedIndex = 0;
                this.WriteModesComboBox.SelectedIndex = 0;
            }
        }

        private void OnWriterWriteCompleted(object sender, EventArgs e)
        {
            SetUIWritingStat(false);
            MessageBox.Show("Write completed successfully!", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnWriterProgressChanged(object sender, EventArgs e)
        {
            var writer = (IAudioCDWriter)sender;

            if (this.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => { this.WriteProgressBar.Value = writer.Progress; }));
            }
            else
            {
                this.WriteProgressBar.Value = writer.Progress;
            }
        }

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "WAV(*.wav)|*.wav";
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AddTracks(dialog.FileNames);
                }
            }
        }

        private void RemoveFileButton_Click(object sender, EventArgs e)
        {
            foreach (var path in this.SelectedTracks)
            {
                RemoveTrack(path);
            }
        }

        private void AudioTracksListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        private void WriteToDiscButton_Click(object sender, EventArgs e)
        {
            Write();
        }

        private void UpdateDriveListButton_Click(object sender, EventArgs e)
        {
            UpdateDiscDrives();
            UpdateDriveList();
        }
    }
}
