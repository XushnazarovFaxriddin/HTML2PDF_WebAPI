using HTML2PDF_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HTML2PDF_WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly HtmlToPdfService _htmlToPdfService;

        public PdfController(HtmlToPdfService htmlToPdfService)
        {
            _htmlToPdfService = htmlToPdfService;
        }

        [HttpGet("generate")]
        public IActionResult GeneratePdf([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL berilishi shart.");
            }

            var pdfBytes = _htmlToPdfService.ConvertHtmlToPdf(url);
            return File(pdfBytes, "application/pdf", "output.pdf");
        }
        [HttpGet("generate-async")]
        public async Task<IActionResult> GeneratePdfAsync([FromQuery] string url, [FromQuery] string fileName = "output.pdf")
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL berilishi shart.");
            }

            var pdfBytes = await _htmlToPdfService.ConvertHtmlToPdfAsync(url);
            return File(pdfBytes, "application/pdf", fileName);
        }
        [HttpGet("generate-chrome")]
        public async Task<IActionResult> GeneratePdfFromChrome([FromQuery] string url, [FromQuery] string fileName = "output.pdf")
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL berilishi shart.");
            }
            var pdfBytes = await _htmlToPdfService.ConvertHtmlToPdfFromChromeRendererAsync(url);
            return File(pdfBytes, "application/pdf", fileName);
        }
        [HttpGet("generate-selectpdf")]
        public async Task<IActionResult> GeneratePdfFromSelectPdf([FromQuery] string url, [FromQuery] string fileName = "output.pdf")
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("URL berilishi shart.");
            }
            var pdfBytes = await _htmlToPdfService.ConvertHtmlToPdfFromSelectPdfAsync(url);
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
