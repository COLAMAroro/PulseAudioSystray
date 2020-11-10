using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PulseAudioSystray
{
    static class Program
    {
		static int Main()
		{
			string pa_env = Environment.GetEnvironmentVariable("pulseaudiopath");
			string pa_path = pa_env != null ? pa_env : Path.Join("C:", "PulseAudio");
			string bin_path = Path.Join(pa_path, "bin", "pulseaudio.exe");
			if (!File.Exists(bin_path))
			{
				MessageBox.Show(
					"Could not find the PulseAudio binary. Install PulseAudio in C:\\PulseAudio or set the \"pulseaudiopath\" environment variable",
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return 1;
			}
			Directory.SetCurrentDirectory(pa_path);
			Application.Run(new notifier(bin_path));
			return 0;
		}
	}

	class notifier : System.Windows.Forms.Form
	{
		public NotifyIcon notify = new NotifyIcon();
		public ContextMenuStrip menu = new ContextMenuStrip();
		public ToolStripMenuItem item = new ToolStripMenuItem();
		public Process pulse;

		public notifier(string pulse_exe)
		{
			//Make the window actually invisible
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.WindowState = FormWindowState.Minimized;
			this.FormClosing += notifier_FormClosing;

			//Set the systray
			notify.Visible = true;
			notify.Text = "PulseAudio";
			notify.ContextMenuStrip = menu;
			notify.Icon = Icon.FromHandle(Properties.Resources.icon.GetHicon());

			//Add the item
			menu.Items.Add(new ToolStripMenuItem("Close PulseAudio", null, new EventHandler(quit_form)));

			//run pulseaudio
			pulse = Process.Start(pulse_exe);
			ChildProcessTracker.AddProcess(pulse);
		}

		void notifier_load(object sender, EventArgs e)
		{
			this.Size = new Size(0, 0);
		}

		void quit_form(object sender, EventArgs e)
		{
			this.Close();
		}

		private void notifier_FormClosing(object sender, FormClosingEventArgs e)
		{
			bool confirmation = MessageBox.Show("Are you sure you want to close PulseAudio ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes;
			if (!confirmation)
            {
				e.Cancel = true;
				return;
            }
			if (pulse != null && !pulse.HasExited)
				pulse.Kill();
		}

		protected override void Dispose(bool disposing)
		{
			menu.Dispose();
			notify.Dispose();
			base.Dispose(disposing);
		}
	}
}
