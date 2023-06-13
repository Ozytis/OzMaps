using Microsoft.AspNetCore.Mvc;

using OzMaps.Core;
using OzMaps.Web.Models;

using Ozytis.PdfEngine.WKHtmlToPdf;

using System.Globalization;

using WkHtmlToPdfDotNet;

using SkiaSharp;
using Svg.Skia;

namespace OzMaps.Web.Controllers
{
    [Route("api/print")]
    public class PrintController : ControllerBase
    {
        public PrintController(IHostEnvironment hostEnvironment)
        {
            this.HostEnvironment = hostEnvironment;
        }

        public IHostEnvironment HostEnvironment { get; }

        [HttpGet]
        public string Get()
        {
            return "print";
        }

        [HttpPost, ValidateModel]
        public async Task<IActionResult> PrintAsync([FromBody] PrintModel model)
        {
            if (model == null)
            {
                return new EmptyResult();
            }

            Map map = new Map((int)(model.Width * model.Dpi / 25.4m), (int)(model.Height * model.Dpi / 25.4m));
            map.Center = model.Center;
            map.Zoom = model.Zoom;

            if (model.TileLayers != null)
            {
                foreach (var layer in model.TileLayers)
                {
                    map.AddTileLayer(layer);
                }
            }

            if (model.GeoJsons != null)
            {
                foreach (var shape in model.GeoJsons)
                {
                    map.AddGeoJson(shape.GeoJson);
                }
            }

            if (model.Markers != null)
            {
                foreach (var marker in model.Markers)
                {
                    map.AddMarker(marker);
                }
            }

            if (model.Legends != null)
            {
                foreach (var section in model.Legends)
                {
                    map.AddLegendSection(section.Name, section.Legends);
                }
            }

            if (model.ShowNorth)
            {
                map.ShowNorth = true;
            }


            if (model.Type == PrintType.Svg)
            {
                var svgBytes = await map.RenderAsSvgAsync();
                return File(svgBytes, "text/svg+xml");
            }

            byte[] pdfBytes = this.ConvertToPdf(map.RenderAsSvgString(), model);

            return File(pdfBytes, "application/pdf");
        }

        private byte[] ConvertToPdf(string svg, PrintModel model)
        {

            string output = Path.Combine(this.HostEnvironment.ContentRootPath, "Temp");

            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }

            string svgFile = Path.Combine(output, $"{Guid.NewGuid()}.svg");

            System.IO.File.WriteAllText(svgFile, svg);

            var skSvg = new SKSvg();
            skSvg.Load(svgFile);


            MemoryStream memoryStream = new MemoryStream();
            

            output = Path.Combine(output, $"{Guid.NewGuid()}.pdf");

            skSvg.Picture.ToPdf(output, SKColor.Parse("ffffff"), (float)model.Width, (float)model.Height);


            return System.IO.File.ReadAllBytes(output);
        }
    }
}
