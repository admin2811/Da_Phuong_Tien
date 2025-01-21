using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Net;
using System.Net.Http;
namespace ProjectAssignment
{
    public partial class SearchingGif : Form
    {
        String imagePath = "";
        public SearchingGif()
        {
            InitializeComponent();
            LoadImage();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SearchingGif_Load(object sender, EventArgs e)
        {

        }
        private void LoadImage()
        {
            string gifFolderPath = @"D:\Da_phuong_tien\CuoiKy\gif";
            if (Directory.Exists(gifFolderPath))
            {
                flowLayoutPanel1.Controls.Clear();

                string[] gifFiles = Directory.GetFiles(gifFolderPath, "*.gif");
                foreach (string gifFile in gifFiles)
                {
                    PictureBox pictureBox = new PictureBox
                    {
                        Width = 150,
                        Height = 150,
                        ImageLocation = gifFile,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Margin = new Padding(10)
                    };
                    flowLayoutPanel1.Controls.Add(pictureBox);
                }
            }
            else
            {
                MessageBox.Show("Thư mục không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imagePath = ofd.FileName;
                    try
                    {
                        using (var img = Image.FromFile(imagePath))
                        {
                            if (img != null)
                            {
                                picImageSearch.Image = (Image)img.Clone();
                                picImageSearch.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đây không phải là một file ảnh hợp lệ: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một file ảnh.");
                }
            }
        }

        private async void btnSURF_Click(object sender, EventArgs e)
        {
            if (picImageSearch.Image != null)
            {
                string tempImagePath = Path.Combine(Path.GetTempPath(), "tempImage.jpg");
                picImageSearch.Image.Save(tempImagePath);
                string pythonScriptPath = "surf.py";
                string gifFolderPath = @"D:\Da_phuong_tien\CuoiKy\gif";
                string result = RunPythonScript(pythonScriptPath, tempImagePath, gifFolderPath);
                if (!string.IsNullOrEmpty(result))
                {
                    DisplayGifs(result);
                }
            }
            else
            {
                MessageBox.Show("Please select an image first.");
            }
        }
        private string RunPythonScript(string scriptPath, string imagePath, string gifFolderPath)
        {
            try
            {
                var start = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{imagePath}\" \"{gifFolderPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(start))
                {
                    using (var reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        string errors = process.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show($"Python script error: {errors}");
                            return null;
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
        private void DisplayGifs(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("No results from Python script.");
                return;
            }

            try
            {
                var json = JArray.Parse(result);
                flowLayoutPanel1.Controls.Clear();

                foreach (var gifInfo in json)
                {
                    string gifPath = Path.Combine(@"D:\Da_phuong_tien\CuoiKy\gif", gifInfo["gif_file"].ToString());
                    int matchCount = gifInfo["match_count"].ToObject<int>();

                    if (File.Exists(gifPath))
                    {
                        PictureBox pictureBox = new PictureBox
                        {
                            Width = 150,
                            Height = 150,
                            ImageLocation = gifPath,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Margin = new Padding(10)
                        };

                        Label label = new Label
                        {
                            Text = $"Matches: {matchCount}",
                            AutoSize = true
                        };

                        flowLayoutPanel1.Controls.Add(pictureBox);
                        flowLayoutPanel1.Controls.Add(label);
                    }
                    else
                    {
                        MessageBox.Show($"GIF file not found: {gifPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}");
            }
        }
        private void DisplayGifsSift(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("No results from Python script.");
                return;
            }

            try
            {
                var json = JArray.Parse(result);
                flowLayoutPanel1.Controls.Clear();

                foreach (var gifInfo in json)
                {
                    string gifPath = Path.Combine(@"D:\Da_phuong_tien\CuoiKy\gif", gifInfo["gif_file"].ToString());
                    float matchCount = gifInfo["similarity_score"].ToObject <float>();

                    if (File.Exists(gifPath))
                    {
                        PictureBox pictureBox = new PictureBox
                        {
                            Width = 150,
                            Height = 150,
                            ImageLocation = gifPath,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Margin = new Padding(10)
                        };

                        Label label = new Label
                        {
                            Text = $"Similary: {matchCount}",
                            AutoSize = true
                        };

                        flowLayoutPanel1.Controls.Add(pictureBox);
                        flowLayoutPanel1.Controls.Add(label);
                    }
                    else
                    {
                        MessageBox.Show($"GIF file not found: {gifPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}");
            }
        }

        private void DisplayGifsWithText(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("No results from Python script.");
                return;
            }

            try
            {
                var json = JArray.Parse(result);
                flowLayoutPanel1.Controls.Clear();

                foreach (var gifInfo in json)
                {
                    string gifPath = Path.Combine(@"D:\Da_phuong_tien\CuoiKy\gif", gifInfo["gif_file"].ToString());
                    float matchCount = gifInfo["similarity_score"].ToObject<float>();

                    if (File.Exists(gifPath))
                    {
                        PictureBox pictureBox = new PictureBox
                        {
                            Width = 150,
                            Height = 150,
                            ImageLocation = gifPath,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Margin = new Padding(10)
                        };

                        Label label = new Label
                        {
                            Text = $"Similary: {matchCount}",
                            AutoSize = true
                        };

                        flowLayoutPanel1.Controls.Add(pictureBox);
                        flowLayoutPanel1.Controls.Add(label);
                    }
                    else
                    {
                        MessageBox.Show($"GIF file not found: {gifPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}");
            }
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (picImageSearch.Image != null)
            {
                string tempImagePath = Path.Combine(Path.GetTempPath(), "tempImage.jpg");
                picImageSearch.Image.Save(tempImagePath);
                string pythonScriptPath = "sift.py";
                string gifFolderPath = @"D:\Da_phuong_tien\CuoiKy\gif";
                string result = RunPythonScript(pythonScriptPath, tempImagePath, gifFolderPath);
                if (!string.IsNullOrEmpty(result))
                {
                    DisplayGifsSift(result);
                }
            }
            else
            {
                MessageBox.Show("Please select an image first.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtSearchText.Text != "")
            {
                String text = txtSearchText.Text;
                string pythonScriptPath = "TFIDF.py";
                string gifFolderPath = @"D:\Da_phuong_tien\CuoiKy\gif";
                string result = RunPythonScript(pythonScriptPath, text, gifFolderPath);
                if (!string.IsNullOrEmpty(result))
                {
                    DisplayGifsWithText(result);
                }
            }
            else
            {
                MessageBox.Show("Please enter a text to search.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtSearchText.Text != "")
            {
                String text = txtSearchText.Text;
                string pythonScriptPath = "searchClipImage.py";
                string gifFolderPath = @"D:\Da_phuong_tien\CuoiKy\gif";
                string result = RunPythonScript(pythonScriptPath, text, gifFolderPath);
                if (!string.IsNullOrEmpty(result))
                {
                    DisplayGifsWithText(result);
                }
            }
            else
            {
                MessageBox.Show("Please enter a text to search.");
            }
        }
    }
}
