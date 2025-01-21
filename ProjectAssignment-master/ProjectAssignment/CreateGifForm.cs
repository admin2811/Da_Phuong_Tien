using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxWMPLib;
using Guna.UI2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using WMPLib;
using System.Diagnostics;
using Guna.UI2.WinForms.Suite;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.IO;

namespace ProjectAssignment
{
    public partial class CreateGifForm : Form
    {
        private string videoPath = "";
        private string imagePath = "";
        public AnimationGifImage animationGifImage = null;
        public string hexColors = "Arial";
        public string fontText;
        public int sizeText = 12;
        public string positionText = "top";
        private string finalGifPath = "";
        public CreateGifForm()
        {
            InitializeComponent();
        }

        private void CreateGifForm_Load(object sender, EventArgs e)
        {
            animationGifImage = new AnimationGifImage();
        }

        private void uploadImagebtn_Click(object sender, EventArgs e)
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
                                picUploadImage.Image = (Image)img.Clone();
                                picUploadImage.SizeMode = PictureBoxSizeMode.StretchImage;
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
        private void uploadVideobtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "Video Files|*.mp4;*.avi;*.mkv";
                 
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    videoPath = ofd.FileName;
                    try
                    {
                        axWindowsMediaPlayer1.URL = videoPath;
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

        private void CutVideogunabtn_Click(object sender, EventArgs e)
        {

        }

        private void CutVideo(string inputPath, int start, int end, string outputGifPath)
        {
            string startTime = TimeSpan.FromSeconds(start).ToString(@"hh\:mm\:ss");
            string duration = TimeSpan.FromSeconds(end - start).ToString(@"hh\:mm\:ss");
            string arguments = $"-i \"{inputPath}\" -ss {startTime} -t {duration} -vf \"fps=10,scale=320:-1:flags=lanczos,split[s0][s1];[s0]palettegen[p];[s1][p]paletteuse\" -loop 0 \"{outputGifPath}\"";

            Process ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.FileName = "ffmpeg";
            ffmpegProcess.StartInfo.Arguments = arguments;
            ffmpegProcess.StartInfo.CreateNoWindow = true;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.EnableRaisingEvents = true;

            ffmpegProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            ffmpegProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine("Error: " + e.Data);
                }
            };

            ffmpegProcess.Exited += (sender, e) =>
            {
                ffmpegProcess.WaitForExit(); // Chờ cho đến khi quá trình hoàn toàn kết thúc
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Tạo GIF thành công");
                    DisplayGif(outputGifPath);
                });
            };

            ffmpegProcess.Start();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.BeginErrorReadLine();
        }


        private void DisplayGif(string gifPath)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(DisplayGif), gifPath);
                return;
            }

            if (System.IO.File.Exists(gifPath)) // Kiểm tra tệp tồn tại
            {
                try
                {
                    // Tải ảnh GIF từ tệp
                    Image gifImage = Image.FromFile(gifPath);
                    picDisplay.Image = gifImage; // Hiển thị ảnh GIF
                    picDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (ExternalException ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh GIF: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Tệp GIF không tồn tại hoặc không thể đọc.");
            }
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (picDisplay.Image != null)
            {
                string folderPath = @"D:\Da_phuong_tien\ProjectAssignment-master\ProjectAssignment\data";
                string filePath = System.IO.Path.Combine(folderPath, "output.gif");

                try
                {
                    // Đảm bảo rằng thư mục tồn tại
                    if (!System.IO.Directory.Exists(folderPath))
                    {
                        System.IO.Directory.CreateDirectory(folderPath);
                    }
                    // Lưu ảnh GIF từ picDisplay.Image
                    picDisplay.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Gif);
                    MessageBox.Show($"GIF đã được tải xuống thành công vào {filePath}.");

                    // Xóa ảnh GIF trong picDisplay
                    picDisplay.Image = null; // Thiết lập lại Image để xóa ảnh hiện tại
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu ảnh GIF: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Không có ảnh GIF nào để tải xuống.");
            }
        }

        private void btnFontSelect_Click(object sender, EventArgs e)
        {
            using (FontFamilySelector fontFamilySelector = new FontFamilySelector())
            {
                if (fontFamilySelector.ShowDialog() == DialogResult.OK)
                {
                    fontText = fontFamilySelector.SelectedFont;
                    sizeText = fontFamilySelector.SelectedSize;
                    positionText = fontFamilySelector.SelectedPosition;

                    MessageBox.Show($"Font: {fontText}, Size: {sizeText}, Position: {positionText}");
                }
            }
        }

        private void btnOptionColor_Click(object sender, EventArgs e)
        {
            using (ColorSelector colorSelector = new ColorSelector())
            {
                if (colorSelector.ShowDialog() == DialogResult.OK)
                {
                    // Nhận giá trị hex của cả 3 selector
                    string[] selectedHexColors = (string[])colorSelector.Tag;

                    // Hiển thị hoặc sử dụng
                    MessageBox.Show($"Mã màu đã chọn:\n" +
                                    $"Selector 1: {selectedHexColors[0]}\n" +
                                    $"Selector 2: {selectedHexColors[1]}\n" +
                                    $"Selector 3: {selectedHexColors[2]}",
                                    "Thông tin màu",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                    hexColors = selectedHexColors[0] + "," + selectedHexColors[1] + "," + selectedHexColors[2];
                }
            }
        }

        private void GifCreateImagegunabtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkColorChange.Checked)
                {
                    if (checkFlipImage.Checked)
                    {
                        animationGifImage.FlipImagesFolder(imagePath);
                    }
                    else if (checkZoomImage.Checked)
                    {
                        animationGifImage.ZoomImagesFolder(imagePath);
                    }
                    animationGifImage.isChangeColor = true;
                    animationGifImage.AnimateGif(hexColors);
                }

                if (!string.IsNullOrWhiteSpace(guna2TextBox1.Text))
                {
                    animationGifImage.CreateText(guna2TextBox1.Text, fontText, sizeText, positionText);
                    finalGifPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output_text.gif");
                }
                else
                {
                    finalGifPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.gif");
                }

                if (File.Exists(finalGifPath))
                {
                    picDisplay.Image = Image.FromFile(finalGifPath);
                    picDisplay.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy file GIF đầu ra!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tạo GIF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void zipLZWgunabtn_Click(object sender, EventArgs e)
        {
            if (picDisplay.Image != null)
            {
                try
                {
                    // Lưu hình ảnh từ PictureBox thành tệp tạm thời dưới định dạng GIF
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_image.gif");
                    picDisplay.Image.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Gif);

                    // Đường dẫn đến file Python script
                    string scriptPath = "lzw.py";  // Thay bằng đường dẫn thực tế đến script Python của bạn
                    string outputDirectory = @"D:\Da_phuong_tien\ProjectAssignment-master\ProjectAssignment\data";
                    string outputImagePath = Path.Combine(outputDirectory, "compressed_image.gif");

                    // Đảm bảo đường dẫn file không bị lỗi mã hóa
                    tempImagePath = Path.GetFullPath(tempImagePath);
                    outputImagePath = Path.GetFullPath(outputImagePath);
                    string arguments = $"\"{tempImagePath}\" \"{outputImagePath}\"";  // Truyền cả input và output path

                    // Chạy script Python và truyền tham số
                    RunExe("python", $"{scriptPath} {arguments}");

                    // Xóa tệp tạm thời sau khi sử dụng
                    if (File.Exists(tempImagePath))
                    {
                        File.Delete(tempImagePath);
                    }

                    // Kiểm tra nếu file output tồn tại và hiển thị kết quả
                    if (File.Exists(outputImagePath))
                    {
                        MessageBox.Show($"File hình ảnh đã được nén và lưu tại: {outputImagePath}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hình ảnh trước khi nén.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static void RunExe(string filexe, string para)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = filexe,
                Arguments = para,  // Truyền tham số input và output cho Python script
                CreateNoWindow = false,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,  // Đảm bảo mã hóa UTF-8 cho đầu ra
                StandardErrorEncoding = System.Text.Encoding.UTF8   // Đảm bảo mã hóa UTF-8 cho lỗi
            };

            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    MessageBox.Show(output, "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CreateGifVideo_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtSoGiayDau.Text, out int start) && int.TryParse(txtSoGiayCuoi.Text, out int end))
            {
                String outputGifPath = "output.gif";
                //MessageBox.Show($"VideoPath: {videoPath}, Start: {start}, End: {end}, OutputGifPath: {outputGifPath}");
                CutVideo(videoPath, start, end, outputGifPath);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập giá trị bắt đầu và kết thúc");
            }
        }

        private void zipDeltagunabtn_Click(object sender, EventArgs e)
        {
            if (picDisplay.Image != null)
            {
                try
                {
                    // Lưu hình ảnh từ PictureBox thành tệp tạm thời dưới định dạng GIF
                    string tempImagePath = Path.Combine(Path.GetTempPath(), "temp_image.gif");
                    picDisplay.Image.Save(tempImagePath, System.Drawing.Imaging.ImageFormat.Gif);

                    // Đường dẫn đến file Python script
                    string scriptPath = "Delta.py";  // Thay bằng đường dẫn thực tế đến script Python của bạn
                    string outputDirectory = @"D:\Da_phuong_tien\ProjectAssignment-master\ProjectAssignment\data";
                    string outputImagePath = Path.Combine(outputDirectory, "compressed_image.gif");

                    // Đảm bảo đường dẫn file không bị lỗi mã hóa
                    tempImagePath = Path.GetFullPath(tempImagePath);
                    outputImagePath = Path.GetFullPath(outputImagePath);
                    string arguments = $"\"{tempImagePath}\" \"{outputImagePath}\"";  // Truyền cả input và output path

                    // Chạy script Python và truyền tham số
                    RunExe("python", $"{scriptPath} {arguments}");

                    // Xóa tệp tạm thời sau khi sử dụng
                    if (File.Exists(tempImagePath))
                    {
                        File.Delete(tempImagePath);
                    }

                    // Kiểm tra nếu file output tồn tại và hiển thị kết quả
                    if (File.Exists(outputImagePath))
                    {
                        MessageBox.Show($"File hình ảnh đã được nén và lưu tại: {outputImagePath}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

            }
        }
    }
}
