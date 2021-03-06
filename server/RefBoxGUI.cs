﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RefBox
{
	public partial class RefBoxGUI : Form
	{
		public RefBoxGUI()
		{
			InitializeComponent();
			LoadData();
			this.cbTest.SelectedIndex = 0;
			this.cbTeam.SelectedIndex = 0;
		}

		public Refbox Refbox { get; set; }

		private void LoadData()
		{
			LoadTeams();
			LoadTests();
		}

		private void LoadTeams()
		{
			this.cbTeam.Items.AddRange(Loader.LoadTeams().ToArray());
		}

		private void LoadTests()
		{
			this.cbTest.Items.AddRange(Loader.LoadTests().ToArray());
		}

		private void UpdateControls()
		{
			gbTestSettings.Enabled = !Refbox.TestStarted;
			btnRestart.Enabled = Refbox.TestStarted;
			lblTime.Enabled = Refbox.TestStarted;
			txtContinue.Enabled = Refbox.TestStarted;
			timer.Enabled = Refbox.TestStarted;
			btnStartStop.Image = Refbox.TestStarted ? Resources.stop32 : Resources.run32;
			btnSendContinue.Enabled = Refbox.TestStarted && (txtContinue.Text.Length > 0);
			UpdateRemainingTime();
			if (Refbox.TestStarted)
				txtContinue.Focus();
			else
				cbTeam.Focus();
		}

		private void UpdateRemainingTime()
		{
			if (!Refbox.TestStarted)
			{
				lblTime.Text = "--:--";
				return;
			}
			int min = Refbox.RemainingTime / 60;
			int sec = Refbox.RemainingTime % 60;
			string smin = min.ToString().PadLeft(2, '0');
			string ssec = sec.ToString().PadLeft(2, '0');
			lblTime.Text = smin + ":" + ssec;
		}

		private void txtContinue_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode == Keys.Enter) || (e.KeyValue == '\r') || (e.KeyValue == '\n'))
			{
				e.SuppressKeyPress = true;
				btnSendContinue.PerformClick();
			}
		}

		private void btnSendContinue_Click(object sender, EventArgs e)
		{
			Refbox.SendContinueText(txtContinue.Text);
			txtContinue.AutoCompleteCustomSource.Add(txtContinue.Text);
			txtContinue.Clear();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			UpdateRemainingTime();
		}

		private void txtContinue_TextChanged(object sender, EventArgs e)
		{
			btnSendContinue.Enabled = Refbox.TestStarted && (txtContinue.Text.Length > 0);
		}

		private void btnRestart_Click(object sender, EventArgs e)
		{
			Refbox.RestartTest();
		}

		private void cbTest_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbTest.SelectedIndex == -1)
				return;
			TestInfo ti = (TestInfo)cbTest.SelectedItem;
			nudTestTime.Value = (decimal)ti.Duration.TotalSeconds / 60;
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			if ((cbTest.SelectedIndex == -1) || (cbTeam.SelectedIndex == -1))
				return;
			TestInfo ti = (TestInfo)cbTest.SelectedItem;
			if (!Refbox.TestStarted)
			{
				Refbox.PrepareTest(ti.Name, (int)ti.Duration.TotalSeconds, cbTeam.Text);
				Refbox.StartTest();
			}
			else
				Refbox.StopTest();
			UpdateControls();
		}
	}
}
