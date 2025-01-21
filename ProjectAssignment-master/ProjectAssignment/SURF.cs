using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SURF
{
    private int hessianThreshold;

    public SURF(int hessianThreshold = 700)
    {
        this.hessianThreshold = hessianThreshold;
    }

    public Mat IntegralImage(Mat img)
    {
        Mat integral = new Mat();
        Cv2.Integral(img, integral);
        return integral;
    }

    // Sửa lại hàm Hessian matrix để tính toán chính xác hơn
    public (double Ixx, double Ixy, double Iyy) HessianMatrix(Mat integralImg, int x, int y, int size = 9)
    {
        int halfSize = size / 2;
        var region = integralImg[new Rect(x - halfSize, y - halfSize, size, size)];

        // Tính toán Hessian Matrix
        double Ixx = region.Sum()[0] * region.Sum()[0];
        double Ixy = region.Sum()[0] * region.Sum()[0];
        double Iyy = region.Sum()[0] * region.Sum()[0];

        Ixx = Math.Max(-1e10, Math.Min(1e10, Ixx)); // Giới hạn Ixx
        Iyy = Math.Max(-1e10, Math.Min(1e10, Iyy)); // Giới hạn Iyy

        return (Ixx, Ixy, Iyy);
    }

    public List<Point> DetectKeypoints(Mat integralImg)
    {
        List<Point> keypoints = new List<Point>();
        int height = integralImg.Height;
        int width = integralImg.Width;

        for (int y = 5; y < height - 5; y += 2)  // Bước nhảy lớn hơn để giảm độ phức tạp
        {
            for (int x = 5; x < width - 5; x += 2)
            {
                var (Ixx, Ixy, Iyy) = HessianMatrix(integralImg, x, y);
                if (Ixx + Iyy > hessianThreshold)
                {
                    keypoints.Add(new Point(x, y));
                }
            }
        }
        return keypoints;
    }

    public List<(Point, double, double)> ComputeDescriptors(Mat img, List<Point> keypoints, int windowSize = 9)
    {
        List<(Point, double, double)> descriptors = new List<(Point, double, double)>();
        foreach (var kp in keypoints)
        {
            int x = kp.X;
            int y = kp.Y;
            var window = img.SubMat(new Rect(x - windowSize / 2, y - windowSize / 2, windowSize, windowSize));

            if (window.Width < 2 || window.Height < 2) continue;

            // Tính gradient theo chiều x và y
            var gx = window.Sobel(1, 0, 3); // Sobel theo chiều x
            var gy = window.Sobel(0, 1, 3); // Sobel theo chiều y

            // Tính magnitude và angle từ gradient
            double gxSum = gx.Sum()[0];
            double gySum = gy.Sum()[0];

            double magnitude = Math.Sqrt(gxSum * gxSum + gySum * gySum);
            double angle = Math.Atan2(gySum, gxSum);

            descriptors.Add((new Point(x, y), magnitude, angle));
        }
        return descriptors;
    }

    public List<DMatch> MatchDescriptors(List<(Point, double, double)> descriptors1, List<(Point, double, double)> descriptors2)
    {
        var matDescriptors1 = new Mat(descriptors1.Count, 2, MatType.CV_32F);
        var matDescriptors2 = new Mat(descriptors2.Count, 2, MatType.CV_32F);

        for (int i = 0; i < descriptors1.Count; i++)
        {
            matDescriptors1.Set<float>(i, 0, (float)descriptors1[i].Item2);  // Gán magnitude
            matDescriptors1.Set<float>(i, 1, (float)descriptors1[i].Item3);  // Gán angle
        }

        for (int i = 0; i < descriptors2.Count; i++)
        {
            matDescriptors2.Set<float>(i, 0, (float)descriptors2[i].Item2);  // Gán magnitude
            matDescriptors2.Set<float>(i, 1, (float)descriptors2[i].Item3);  // Gán angle
        }

        var bfMatcher = new BFMatcher(NormTypes.L2, true); // L2 norm cho việc so khớp
        var matches = bfMatcher.Match(matDescriptors1, matDescriptors2); // So khớp các descriptors

        return matches.OrderBy(m => m.Distance).ToList(); // Sắp xếp kết quả theo khoảng cách
    }

    public (List<Point> keypoints, List<(Point, double, double)> descriptors) DetectAndCompute(Mat img)
    {
        var integralImg = IntegralImage(img);
        var keypoints = DetectKeypoints(integralImg);
        if (keypoints.Count == 0) return (new List<Point>(), new List<(Point, double, double)>());
        var descriptors = ComputeDescriptors(img, keypoints);
        return (keypoints, descriptors);
    }

    // Lưu trữ descriptors của GIFs để tránh tính toán lại nhiều lần
    private Dictionary<string, (List<Point> keypoints, List<(Point, double, double)> descriptors)> gifDescriptorsCache = new Dictionary<string, (List<Point> keypoints, List<(Point, double, double)> descriptors)>();

    public async Task FindSimilarGIFsForImageAsync(Mat staticImage, List<string> gifPaths)
    {
        var result = new Dictionary<string, List<string>>();

        // Tiền xử lý các GIF chỉ một lần và lưu trữ trong bộ nhớ đệm
        foreach (var gifPath in gifPaths)
        {
            if (!gifDescriptorsCache.ContainsKey(gifPath))
            {
                Mat gifFrame = Cv2.ImRead(gifPath);
                var gifResult = DetectAndCompute(gifFrame);
                gifDescriptorsCache[gifPath] = gifResult;
            }
        }

        // Xử lý từng ảnh đầu vào một cách song song
        var (keypointsStatic, descriptorsStatic) = DetectAndCompute(staticImage);

        if (descriptorsStatic.Count == 0)
        {
            result["InputImage"] = new List<string>(); // Không có keypoints, không có GIF tương đồng
            return;
        }

        var similarityList = new List<(string gifFile, int matchCount)>();

        // So khớp ảnh đầu vào với các GIF
        var tasks = gifPaths.Select(async gifPath =>
        {
            var (keypointsGif, descriptorsGif) = gifDescriptorsCache[gifPath];
            if (descriptorsGif.Count == 0)
                return (gifPath, 0);

            var matches = MatchDescriptors(descriptorsStatic, descriptorsGif);
            return (gifPath, matches.Count);
        });

        // Chờ các tác vụ song song
        var results = await Task.WhenAll(tasks);

        // Sắp xếp GIF theo độ tương đồng
        similarityList.AddRange(results);
        similarityList = similarityList.OrderByDescending(x => x.matchCount).ToList();

        // Lưu kết quả vào dictionary
        result["InputImage"] = similarityList.Select(x => x.gifFile).ToList();

        // Hiển thị kết quả
        foreach (var gifFile in result["InputImage"])
        {
            Console.WriteLine($"Tìm thấy GIF tương đồng: {gifFile}");
        }
    }
}
