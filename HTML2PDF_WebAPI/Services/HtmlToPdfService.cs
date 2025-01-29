using DinkToPdf.Contracts;
using DinkToPdf;
using PuppeteerSharp;

namespace HTML2PDF_WebAPI.Services
{
    public class HtmlToPdfService
    {
        private readonly IConverter _converter;
        private static readonly HttpClient _httpClient = new HttpClient(); // ♻️ Global HttpClient

        public HtmlToPdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] ConvertHtmlToPdf(string url)
        {
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = { new ObjectSettings() { Page = url } }
            };

            return _converter.Convert(pdf);
        }

        public async Task<byte[]> ConvertHtmlToPdfFromChromeRendererAsync(string url)
        {
            try
            {
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync(BrowserTag.Latest);
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();
                await page.GoToAsync(url);
                var pdf = await page.PdfDataAsync();
                await browser.CloseAsync();
                return pdf;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Xatolik yuz berdi: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ConvertHtmlToPdfFromSelectPdfAsync(string url)
        {
            try
            {
                var selectPdf = new SelectPdf.HtmlToPdf(1500);
                var pdf = selectPdf.ConvertUrl(url);
                // to byte array
                return pdf.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Xatolik yuz berdi: {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ConvertHtmlToPdfAsync(string url)
        {
            try
            {
                // 🌐 HTML-ni yuklash
                var htmlContent = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(htmlContent))
                    throw new Exception("HTML mazmuni bo'sh yoki yuklanmadi!");

                //Console.WriteLine($"✅ HTML yuklandi: {htmlContent.Substring(0, Math.Min(500, htmlContent.Length))}");

                // 📄 PDF hujjatini yaratish
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4
                    },
                    Objects =
                            {
                                new ObjectSettings
                                {
                                    PagesCount = true,
                                    //Page = url,
                                    HtmlContent = htmlContent,
                                    WebSettings =
                                    {
                                        DefaultEncoding = "utf-8",
                                        LoadImages = true,  // 📷 Rasmlar yuklansin
                                        EnableIntelligentShrinking = true,
                                        PrintMediaType = true,
                                        EnableJavascript = true,  // 🟢 JavaScriptni yoqish
                                        Background = true  // 🎨 Orqa fonni chiqarish
                                    }
                                }
                            }
                };

                // 🔥 PDF ni yaratish
                return _converter.Convert(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Xatolik yuz berdi: {ex.Message}");
                throw;
            }
        }
    }
}
