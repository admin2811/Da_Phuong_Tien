using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAssignment
{
    public class AnimationGifImage
    {
        public bool isChangeColor;
        public void AnimateGif(string hexColors)
        {
            string para;
            if (isChangeColor)
            {
                para = "change_color.py ./elements ./output.gif " + hexColors;
                RunExe("python", para);
            }
        }
        public void FlipImagesFolder(string fileinput)
        {
            string para = "flip_image.py " + fileinput + " ./elements";
            RunExe("python", para);
        }
        public void ZoomImagesFolder(string fileinput)
        {
            string para = "zoom_gif.py " + fileinput + " ./elements";
            RunExe("python", para);
        }
        public void CreateText(string text, string fontFamily, int textSize, string textPosition)
        {
            string para = $"text_background.py output.gif output_text.gif \"{text}\" \"{fontFamily}\" {textSize} \"{textPosition}\"";
            Console.WriteLine("Executing command: python " + para);
            RunExe("python", para);
        }
        private static void RunExe(string filexe, string para)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = false;
            processStartInfo.UseShellExecute = false;
            processStartInfo.Arguments = para;
            processStartInfo.FileName = filexe;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = Process.Start(processStartInfo);
            process.WaitForExit();

        }
    }
}