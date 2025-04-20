using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

public partial class MainForm : MetroSuite.MetroForm
{
    public MainForm()
    {
        InitializeComponent();

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        saveFileDialog1.InitialDirectory = desktopPath;
        saveFileDialog2.InitialDirectory = desktopPath;
    }

    private void label1_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox1.Focus();
    }

    private void MainForm_MouseClick(object sender, MouseEventArgs e)
    {
        SendKeys.Send("{TAB}");
    }

    private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode.Equals(Keys.Enter))
        {
            guna2GradientButton1.PerformClick();
        }
    }

    private void guna2RadioButton2_MouseClick(object sender, MouseEventArgs e)
    {
        SendKeys.Send("{TAB}");
    }

    private void guna2RadioButton1_MouseClick(object sender, MouseEventArgs e)
    {
        SendKeys.Send("{TAB}");
    }

    private void guna2GradientButton1_Click(object sender, System.EventArgs e)
    {
        string youtubeVideoURI = guna2TextBox1.Text;
        string outputFilePath = "";
        bool isAudioOnly = guna2RadioButton2.Checked;

        guna2GradientButton1.Enabled = false;
        guna2TextBox1.ReadOnly = true;

        if (isAudioOnly)
        {
            guna2RadioButton2.Checked = true;
        }

        if (!Helpers.IsYouTubeURIValid(youtubeVideoURI))
        {
            MessageBox.Show("The inserted YouTube video URI is malformed. Please, type it again correctly and try again.", "RawYouTubeDownloader", MessageBoxButtons.OK, MessageBoxIcon.Error);
            goto enableAgain;
        }

        saveFileDialog1.FileName = "";
        saveFileDialog2.FileName = "";

        if (!isAudioOnly && saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
        {
            outputFilePath = saveFileDialog1.FileName;
        }
        else if (isAudioOnly && saveFileDialog2.ShowDialog().Equals(DialogResult.OK))
        {
            outputFilePath = saveFileDialog2.FileName;
        }

        if (outputFilePath == "")
        {
            goto enableAgain;
        }

        if (File.Exists(outputFilePath))
        {
            try
            {
                File.Delete(outputFilePath);
            }
            catch
            {
                MessageBox.Show("The file already exists and can not be deleted by the program. Please, choose another output file destination path and try again.", "RawYouTubeDownloader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                goto enableAgain;
            }
        }

        Helpers.DownloadYouTubeVideo(youtubeVideoURI, isAudioOnly, outputFilePath);
        MessageBox.Show("Succesfully downloaded your YouTube video in raw format! Enjoy!", "RawYouTubeDownloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        enableAgain: guna2GradientButton1.Enabled = true;
        guna2TextBox1.ReadOnly = false;
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start("https://github.com/ZygoteCode/RawYouTubeDownloader/");
    }

    private void linkLabel1_MouseClick(object sender, MouseEventArgs e)
    {
        linkLabel1.Focus();
    }
}