using System.Drawing;
using System.Drawing.Imaging;

namespace GptView.Helpers
{
    // 圖形分割
    public class IrregularPolygonSplitter
    {
        // 計算多邊形的重心
        private static PointF GetCentroid(List<PointF> points)
        {
            float cx = 0, cy = 0;
            float signedArea = 0;

            for (int i = 0; i < points.Count; i++)
            {
                int j = (i + 1) % points.Count;
                float x0 = points[i].X;
                float y0 = points[i].Y;
                float x1 = points[j].X;
                float y1 = points[j].Y;

                float a = x0 * y1 - x1 * y0;
                signedArea += a;
                cx += (x0 + x1) * a;
                cy += (y0 + y1) * a;
            }

            signedArea /= 2;
            cx /= (6 * signedArea);
            cy /= (6 * signedArea);

            return new PointF(cx, cy);
        }

        // 計算縮放後的點集，使其適應 500x500 畫布並置中
        private static List<PointF> NormalizePolygon(List<PointF> points, int canvasSize)
        {
            float minX = points.Min(p => p.X);
            float minY = points.Min(p => p.Y);
            float maxX = points.Max(p => p.X);
            float maxY = points.Max(p => p.Y);

            float width = maxX - minX;
            float height = maxY - minY;
            float scale = Math.Min((canvasSize * 0.8f) / width, (canvasSize * 0.8f) / height);

            List<PointF> scaledPoints = points.Select(p => new PointF(
                (p.X - minX) * scale + canvasSize / 2 - (width * scale) / 2,
                (p.Y - minY) * scale + canvasSize / 2 - (height * scale) / 2
            )).ToList();

            return scaledPoints;
        }

        // 繪製圖形並切割，輸出 500x500 的圖片
        public static Bitmap DrawSplit(List<PointF> polygon, int n, int canvasSize = 500)
        {
            List<PointF> normalizedPolygon = NormalizePolygon(polygon, canvasSize);
            PointF centroid = GetCentroid(normalizedPolygon);
            float maxRadius = (canvasSize * 0.65f);

            Bitmap bitmap = new Bitmap(canvasSize, canvasSize);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 繪製多邊形
                g.FillPolygon(Brushes.LightBlue, normalizedPolygon.ToArray());
                g.DrawPolygon(Pens.Black, normalizedPolygon.ToArray());

                // 計算每條切割線的角度並繪製
                float angleStep = 360f / n;
                for (int i = 0; i < n; i++)
                {
                    float angle = i * angleStep;
                    float xEnd = centroid.X + maxRadius * (float)Math.Cos(angle * Math.PI / 180f);
                    float yEnd = centroid.Y + maxRadius * (float)Math.Sin(angle * Math.PI / 180f);
                    g.DrawLine(Pens.Red, centroid, new PointF(xEnd, yEnd));
                }
            }

            return bitmap;
        }
    }

    // 浮水印
    public class Watermark
    {
        public static void AddWatermark(string inputPath, string outputPath, string text)
        {
            using (Image image = Image.FromFile(inputPath))
            using (Bitmap bitmap = new Bitmap(image))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Font font = new Font("Arial", 25, FontStyle.Bold); // 加大字體
                Brush brush = new SolidBrush(Color.FromArgb(200, 255, 100, 100)); // 深灰色，較為明顯
                SizeF textSize = graphics.MeasureString(text, font);

                // 計算文字放置位置 (圖片中央)
                float x = (bitmap.Width - textSize.Width) / 2;
                float y = (bitmap.Height - textSize.Height) / 2;

                graphics.TranslateTransform(x + textSize.Width / 2, y + textSize.Height / 2); // 移動原點到文字中心
                graphics.RotateTransform(-45); // 向下傾斜 45 度
                graphics.DrawString(text, font, brush, -textSize.Width / 2, -textSize.Height / 2);
                graphics.ResetTransform(); // 重置變換矩陣

                bitmap.Save(outputPath, ImageFormat.Jpeg); // 儲存圖片
            }
        }
    }
}
