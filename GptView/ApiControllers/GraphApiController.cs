using AutoMapper;
using GptView.Servicies;
using GptView.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Text.Encodings.Web;
using System.Web;

namespace GptView.ApiControllers
{
    [Authorize(Policy = "MenuPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GraphApiController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly GraphService _gService;

        public GraphApiController(IMapper mapper, GraphService gService)
        {
            _mapper = mapper;
            _gService = gService;
        }

        [HttpPost(Name = "PostXY")]
        public IActionResult PostXY(int partition, int canvasSize, [FromBody]List<PointXY> pointXY)
        {
            var option = Request.Headers["option"];

            if (pointXY.Count < 3)
                return BadRequest(new { StatusCode = 400, ErrorMsg = "輸入座標點位點位不能小於三個" });

            var pointfList = _mapper.Map<List<PointXY>, List<PointF>>(pointXY);
            var bytes = _gService.GetPicByte(option ,partition, canvasSize, pointfList);
            var filename = HttpUtility.UrlEncode("等分割圖.jpg");
            Response.Headers?.Add("Content-Disposition", $"attachment; filename=\"{filename}\"");
            return File(fileContents: bytes, contentType: "image/jpg");
        }
    }
}
