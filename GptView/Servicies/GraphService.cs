using GptView.Helpers;
using GptView.ViewModels;
using System.Drawing;

namespace GptView.Servicies
{
    public class GraphService
    {
        private readonly IHostEnvironment _env;

        public GraphService(IHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GetPicByte(string option, int partition, int canvasSize, List<PointF> pointF)
        {          
            // 將圖形切割成 N 等份
            Bitmap bitmap = IrregularPolygonSplitter.DrawSplit(pointF, partition, canvasSize);

            // 儲存圖像
            var directoryPath = _env.CreateDirectory("AppData");
            var filePath =  Path.Combine(directoryPath, $"{Guid.NewGuid()}.jpg");
            bitmap.Save(filePath);

            byte[] fileBytes = null;
            if(option == "download")
            {
                fileBytes = File.ReadAllBytes(filePath);
                File.Delete(filePath);
            }
            else
            {
                var watermarkPath = Path.Combine(directoryPath,
                    $"{Path.GetFileName(filePath).Split('.')[0]}watermark.jpg");
                Watermark.AddWatermark(filePath, watermarkPath, "示意圖勿使用在正式報告");
                fileBytes = File.ReadAllBytes(watermarkPath);
                File.Delete(filePath);
                File.Delete(watermarkPath);
            }
            return fileBytes;   
        }
    } 
}
