namespace RefBox
{
	partial class RefBoxGUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.gbTestSettings = new System.Windows.Forms.GroupBox();
			this.nudTestTime = new System.Windows.Forms.NumericUpDown();
			this.cbTeam = new System.Windows.Forms.ComboBox();
			this.cbTest = new System.Windows.Forms.ComboBox();
			this.lblTeamName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblTestName = new System.Windows.Forms.Label();
			this.gbCurrentTest = new System.Windows.Forms.GroupBox();
			this.lblTime = new System.Windows.Forms.Label();
			this.txtContinue = new System.Windows.Forms.TextBox();
			this.lblContinue = new System.Windows.Forms.Label();
			this.btnRestart = new System.Windows.Forms.Button();
			this.btnSendContinue = new System.Windows.Forms.Button();
			this.btnStartStop = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.gbTestSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTestTime)).BeginInit();
			this.gbCurrentTest.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbTestSettings
			// 
			this.gbTestSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbTestSettings.Controls.Add(this.nudTestTime);
			this.gbTestSettings.Controls.Add(this.cbTeam);
			this.gbTestSettings.Controls.Add(this.cbTest);
			this.gbTestSettings.Controls.Add(this.lblTeamName);
			this.gbTestSettings.Controls.Add(this.label3);
			this.gbTestSettings.Controls.Add(this.lblTestName);
			this.gbTestSettings.Location = new System.Drawing.Point(12, 12);
			this.gbTestSettings.Name = "gbTestSettings";
			this.gbTestSettings.Size = new System.Drawing.Size(357, 78);
			this.gbTestSettings.TabIndex = 0;
			this.gbTestSettings.TabStop = false;
			this.gbTestSettings.Text = "Test Settings";
			// 
			// nudTestTime
			// 
			this.nudTestTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudTestTime.DecimalPlaces = 2;
			this.nudTestTime.Location = new System.Drawing.Point(301, 20);
			this.nudTestTime.Name = "nudTestTime";
			this.nudTestTime.Size = new System.Drawing.Size(50, 20);
			this.nudTestTime.TabIndex = 2;
			this.nudTestTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudTestTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// cbTeam
			// 
			this.cbTeam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbTeam.FormattingEnabled = true;
			this.cbTeam.Location = new System.Drawing.Point(78, 46);
			this.cbTeam.Name = "cbTeam";
			this.cbTeam.Size = new System.Drawing.Size(273, 21);
			this.cbTeam.TabIndex = 3;
			// 
			// cbTest
			// 
			this.cbTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbTest.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.cbTest.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbTest.FormattingEnabled = true;
			this.cbTest.Location = new System.Drawing.Point(78, 19);
			this.cbTest.Name = "cbTest";
			this.cbTest.Size = new System.Drawing.Size(158, 21);
			this.cbTest.TabIndex = 1;
			this.cbTest.SelectedIndexChanged += new System.EventHandler(this.cbTest_SelectedIndexChanged);
			// 
			// lblTeamName
			// 
			this.lblTeamName.AutoSize = true;
			this.lblTeamName.Location = new System.Drawing.Point(6, 49);
			this.lblTeamName.Name = "lblTeamName";
			this.lblTeamName.Size = new System.Drawing.Size(66, 13);
			this.lblTeamName.TabIndex = 0;
			this.lblTeamName.Text = "Team name:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(242, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Test time:";
			// 
			// lblTestName
			// 
			this.lblTestName.AutoSize = true;
			this.lblTestName.Location = new System.Drawing.Point(6, 22);
			this.lblTestName.Name = "lblTestName";
			this.lblTestName.Size = new System.Drawing.Size(60, 13);
			this.lblTestName.TabIndex = 0;
			this.lblTestName.Text = "Test name:";
			// 
			// gbCurrentTest
			// 
			this.gbCurrentTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbCurrentTest.Controls.Add(this.lblTime);
			this.gbCurrentTest.Controls.Add(this.txtContinue);
			this.gbCurrentTest.Controls.Add(this.lblContinue);
			this.gbCurrentTest.Controls.Add(this.btnRestart);
			this.gbCurrentTest.Controls.Add(this.btnSendContinue);
			this.gbCurrentTest.Controls.Add(this.btnStartStop);
			this.gbCurrentTest.Location = new System.Drawing.Point(12, 96);
			this.gbCurrentTest.Name = "gbCurrentTest";
			this.gbCurrentTest.Size = new System.Drawing.Size(357, 138);
			this.gbCurrentTest.TabIndex = 1;
			this.gbCurrentTest.TabStop = false;
			this.gbCurrentTest.Text = "Current test";
			// 
			// lblTime
			// 
			this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTime.Enabled = false;
			this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.Location = new System.Drawing.Point(89, 19);
			this.lblTime.Name = "lblTime";
			this.lblTime.Size = new System.Drawing.Size(150, 50);
			this.lblTime.TabIndex = 5;
			this.lblTime.Text = "00:00";
			this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtContinue
			// 
			this.txtContinue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtContinue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtContinue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtContinue.Enabled = false;
			this.txtContinue.Location = new System.Drawing.Point(9, 75);
			this.txtContinue.Multiline = true;
			this.txtContinue.Name = "txtContinue";
			this.txtContinue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtContinue.Size = new System.Drawing.Size(289, 50);
			this.txtContinue.TabIndex = 5;
			this.txtContinue.TextChanged += new System.EventHandler(this.txtContinue_TextChanged);
			this.txtContinue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContinue_KeyDown);
			// 
			// lblContinue
			// 
			this.lblContinue.AutoSize = true;
			this.lblContinue.Enabled = false;
			this.lblContinue.Location = new System.Drawing.Point(6, 59);
			this.lblContinue.Name = "lblContinue";
			this.lblContinue.Size = new System.Drawing.Size(52, 13);
			this.lblContinue.TabIndex = 3;
			this.lblContinue.Text = "Continue:";
			// 
			// btnRestart
			// 
			this.btnRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestart.Enabled = false;
			this.btnRestart.Location = new System.Drawing.Point(245, 19);
			this.btnRestart.Name = "btnRestart";
			this.btnRestart.Image = Resources.refresh32;
			this.btnRestart.Size = new System.Drawing.Size(50, 50);
			this.btnRestart.TabIndex = 1;
			this.btnRestart.TabStop = false;
			this.btnRestart.UseVisualStyleBackColor = true;
			// 
			// btnSendContinue
			// 
			this.btnSendContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSendContinue.Enabled = false;
			this.btnSendContinue.Location = new System.Drawing.Point(301, 75);
			this.btnSendContinue.Name = "btnSendContinue";
			this.btnSendContinue.Image = Resources.injectBlue32;
			this.btnSendContinue.Size = new System.Drawing.Size(50, 50);
			this.btnSendContinue.TabIndex = 6;
			this.btnSendContinue.UseVisualStyleBackColor = true;
			this.btnSendContinue.Click += new System.EventHandler(this.btnSendContinue_Click);
			// 
			// btnStartStop
			// 
			this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartStop.Location = new System.Drawing.Point(301, 19);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Image = Resources.run32;
			this.btnStartStop.Size = new System.Drawing.Size(50, 50);
			this.btnStartStop.TabIndex = 4;
			this.btnStartStop.UseVisualStyleBackColor = true;
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// RefBoxGUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(381, 240);
			this.Controls.Add(this.gbCurrentTest);
			this.Controls.Add(this.gbTestSettings);
			this.Name = "RefBoxGUI";
			this.Text = "RefBox";
			this.gbTestSettings.ResumeLayout(false);
			this.gbTestSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTestTime)).EndInit();
			this.gbCurrentTest.ResumeLayout(false);
			this.gbCurrentTest.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbTestSettings;
		private System.Windows.Forms.NumericUpDown nudTestTime;
		private System.Windows.Forms.ComboBox cbTeam;
		private System.Windows.Forms.ComboBox cbTest;
		private System.Windows.Forms.Label lblTeamName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblTestName;
		private System.Windows.Forms.GroupBox gbCurrentTest;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.TextBox txtContinue;
		private System.Windows.Forms.Label lblContinue;
		private System.Windows.Forms.Button btnRestart;
		private System.Windows.Forms.Button btnSendContinue;
		private System.Windows.Forms.Timer timer;
	}
}