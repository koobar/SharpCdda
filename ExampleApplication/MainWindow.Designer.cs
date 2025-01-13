namespace SharpCdda.ExampleApp
{
    partial class MainWindow
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.WriteToDiscButton = new System.Windows.Forms.Button();
            this.WriteProgressBar = new System.Windows.Forms.ProgressBar();
            this.AudioTracksListView = new System.Windows.Forms.ListView();
            this.TitleColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TrackNumberColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.TracklistLabel = new System.Windows.Forms.Label();
            this.AddFileButton = new System.Windows.Forms.Button();
            this.RemoveFileButton = new System.Windows.Forms.Button();
            this.DrivesLabel = new System.Windows.Forms.Label();
            this.AvailableDrivesComboBox = new System.Windows.Forms.ComboBox();
            this.UpdateDriveListButton = new System.Windows.Forms.Button();
            this.ModeLabel = new System.Windows.Forms.Label();
            this.WriteModesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // WriteToDiscButton
            // 
            this.WriteToDiscButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WriteToDiscButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.WriteToDiscButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold);
            this.WriteToDiscButton.Location = new System.Drawing.Point(12, 297);
            this.WriteToDiscButton.Name = "WriteToDiscButton";
            this.WriteToDiscButton.Size = new System.Drawing.Size(460, 56);
            this.WriteToDiscButton.TabIndex = 9;
            this.WriteToDiscButton.Text = "WRITE TO DISC";
            this.WriteToDiscButton.UseVisualStyleBackColor = true;
            this.WriteToDiscButton.Click += new System.EventHandler(this.WriteToDiscButton_Click);
            // 
            // WriteProgressBar
            // 
            this.WriteProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WriteProgressBar.Location = new System.Drawing.Point(12, 374);
            this.WriteProgressBar.Name = "WriteProgressBar";
            this.WriteProgressBar.Size = new System.Drawing.Size(460, 25);
            this.WriteProgressBar.TabIndex = 11;
            // 
            // AudioTracksListView
            // 
            this.AudioTracksListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AudioTracksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TitleColumnHeader,
            this.TrackNumberColumnHeader});
            this.AudioTracksListView.FullRowSelect = true;
            this.AudioTracksListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.AudioTracksListView.HideSelection = false;
            this.AudioTracksListView.Location = new System.Drawing.Point(12, 71);
            this.AudioTracksListView.Name = "AudioTracksListView";
            this.AudioTracksListView.Size = new System.Drawing.Size(349, 182);
            this.AudioTracksListView.TabIndex = 4;
            this.AudioTracksListView.UseCompatibleStateImageBehavior = false;
            this.AudioTracksListView.View = System.Windows.Forms.View.Details;
            this.AudioTracksListView.SelectedIndexChanged += new System.EventHandler(this.AudioTracksListView_SelectedIndexChanged);
            // 
            // TitleColumnHeader
            // 
            this.TitleColumnHeader.Text = "Title";
            this.TitleColumnHeader.Width = 48;
            // 
            // TrackNumberColumnHeader
            // 
            this.TrackNumberColumnHeader.Text = "Track";
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(12, 357);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(50, 12);
            this.ProgressLabel.TabIndex = 10;
            this.ProgressLabel.Text = "Progress";
            // 
            // TracklistLabel
            // 
            this.TracklistLabel.AutoSize = true;
            this.TracklistLabel.Location = new System.Drawing.Point(12, 56);
            this.TracklistLabel.Name = "TracklistLabel";
            this.TracklistLabel.Size = new System.Drawing.Size(50, 12);
            this.TracklistLabel.TabIndex = 3;
            this.TracklistLabel.Text = "Tracklist";
            // 
            // AddFileButton
            // 
            this.AddFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddFileButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AddFileButton.Location = new System.Drawing.Point(367, 71);
            this.AddFileButton.Name = "AddFileButton";
            this.AddFileButton.Size = new System.Drawing.Size(105, 25);
            this.AddFileButton.TabIndex = 5;
            this.AddFileButton.Text = "Add Files...";
            this.AddFileButton.UseVisualStyleBackColor = true;
            this.AddFileButton.Click += new System.EventHandler(this.AddFileButton_Click);
            // 
            // RemoveFileButton
            // 
            this.RemoveFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveFileButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RemoveFileButton.Location = new System.Drawing.Point(367, 102);
            this.RemoveFileButton.Name = "RemoveFileButton";
            this.RemoveFileButton.Size = new System.Drawing.Size(105, 25);
            this.RemoveFileButton.TabIndex = 6;
            this.RemoveFileButton.Text = "Remove Files";
            this.RemoveFileButton.UseVisualStyleBackColor = true;
            this.RemoveFileButton.Click += new System.EventHandler(this.RemoveFileButton_Click);
            // 
            // DrivesLabel
            // 
            this.DrivesLabel.AutoSize = true;
            this.DrivesLabel.Location = new System.Drawing.Point(12, 9);
            this.DrivesLabel.Name = "DrivesLabel";
            this.DrivesLabel.Size = new System.Drawing.Size(170, 12);
            this.DrivesLabel.TabIndex = 0;
            this.DrivesLabel.Text = "The disk drive to use for writing";
            // 
            // AvailableDrivesComboBox
            // 
            this.AvailableDrivesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AvailableDrivesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AvailableDrivesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.AvailableDrivesComboBox.FormattingEnabled = true;
            this.AvailableDrivesComboBox.Location = new System.Drawing.Point(12, 27);
            this.AvailableDrivesComboBox.Name = "AvailableDrivesComboBox";
            this.AvailableDrivesComboBox.Size = new System.Drawing.Size(349, 20);
            this.AvailableDrivesComboBox.TabIndex = 1;
            // 
            // UpdateDriveListButton
            // 
            this.UpdateDriveListButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateDriveListButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.UpdateDriveListButton.Location = new System.Drawing.Point(367, 27);
            this.UpdateDriveListButton.Name = "UpdateDriveListButton";
            this.UpdateDriveListButton.Size = new System.Drawing.Size(105, 25);
            this.UpdateDriveListButton.TabIndex = 2;
            this.UpdateDriveListButton.Text = "Refresh drives";
            this.UpdateDriveListButton.UseVisualStyleBackColor = true;
            this.UpdateDriveListButton.Click += new System.EventHandler(this.UpdateDriveListButton_Click);
            // 
            // ModeLabel
            // 
            this.ModeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ModeLabel.AutoSize = true;
            this.ModeLabel.Location = new System.Drawing.Point(12, 256);
            this.ModeLabel.Name = "ModeLabel";
            this.ModeLabel.Size = new System.Drawing.Size(32, 12);
            this.ModeLabel.TabIndex = 7;
            this.ModeLabel.Text = "Mode";
            // 
            // WriteModesComboBox
            // 
            this.WriteModesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WriteModesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WriteModesComboBox.FormattingEnabled = true;
            this.WriteModesComboBox.Items.AddRange(new object[] {
            "Disc-At-Once",
            "Track-At-Once"});
            this.WriteModesComboBox.Location = new System.Drawing.Point(12, 271);
            this.WriteModesComboBox.Name = "WriteModesComboBox";
            this.WriteModesComboBox.Size = new System.Drawing.Size(150, 20);
            this.WriteModesComboBox.TabIndex = 8;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(484, 411);
            this.Controls.Add(this.WriteModesComboBox);
            this.Controls.Add(this.ModeLabel);
            this.Controls.Add(this.UpdateDriveListButton);
            this.Controls.Add(this.AvailableDrivesComboBox);
            this.Controls.Add(this.DrivesLabel);
            this.Controls.Add(this.RemoveFileButton);
            this.Controls.Add(this.AddFileButton);
            this.Controls.Add(this.TracklistLabel);
            this.Controls.Add(this.AudioTracksListView);
            this.Controls.Add(this.WriteProgressBar);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.WriteToDiscButton);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SharpCdda Example Writer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button WriteToDiscButton;
        private System.Windows.Forms.ProgressBar WriteProgressBar;
        private System.Windows.Forms.ListView AudioTracksListView;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Label TracklistLabel;
        private System.Windows.Forms.Button AddFileButton;
        private System.Windows.Forms.Button RemoveFileButton;
        private System.Windows.Forms.ColumnHeader TitleColumnHeader;
        private System.Windows.Forms.ColumnHeader TrackNumberColumnHeader;
        private System.Windows.Forms.Label DrivesLabel;
        private System.Windows.Forms.ComboBox AvailableDrivesComboBox;
        private System.Windows.Forms.Button UpdateDriveListButton;
        private System.Windows.Forms.Label ModeLabel;
        private System.Windows.Forms.ComboBox WriteModesComboBox;
    }
}

