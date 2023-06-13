using Microsoft.VisualStudio.TestTools.UnitTesting;

using OzMaps.Core;
using OzMaps.Core.GeoJson;
using OzMaps.Core.TileLayers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzMaps.Core.Tests
{
    [TestClass()]
    public class SvgMapRendererTests
    {
        [TestMethod()]
        public async Task RenderAsSvgAsyncTest()
        {
            Map map = new Map(800, 600);

            map.ShowNorth = true;

            map.AddTileLayer(new OpenStreetMapLayer());

            map.AddMarker(new MapMarker { Coordinates = new() { Latitude = 45, Longitude = 0 } });

            map.AddGeoJson(new LineString
            {
                Coordinates = new()
                {
                     new(45,-0.004),
                    new(45,-0.002),
                    new(45.002,0),
                    new(45,0.002),
                },
                Style = new()
                {
                    StrokeColor = "#0000FF",
                    StrokeWidth = 2
                }
            });


            map.AddGeoJson(new MultiLineString
            {
                Coordinates = new()
                {
                    new LineString
                    {
                         Coordinates = new()
                        {
                            new(45,0.004),
                            new(45,0.002),
                            new(45.002,0),
                            new(45, -0.002),
                        },
                        Style = new()
                        {
                            StrokeColor = "#0000FF",
                            StrokeWidth = 1
                        }
                    },
                    new LineString
                    {
                         Coordinates = new()
                        {
                            new(45,0.006),
                            new(45,0.001),
                            new(45.002,0),
                            new(45,0.003),
                        },
                        Style = new()
                        {
                            StrokeColor = "#FF00FF",
                            StrokeWidth = 1
                        }
                    }
                }

            });

            map.AddGeoJson(new Polygon
            {
                Coordinates = new()
                {
                    new LineString {
                        Coordinates = new()
                        {
                            new(45.003, 0.003),
                            new(45.003, 0.004),
                            new(45.004, 0.004),
                            new(45.004, 0.002),
                        }
                    },
                    new LineString {
                        Coordinates = new()
                        {
                            new(45.0038, 0.0022),
                            new(45.0038, 0.0038),
                            new(45.0032, 0.0038),
                            new(45.0032, 0.0032)
                        }
                    },

                }
            });

            map.AddLegendSection("TEST1", new Legend[]
            {
                new (){
                    Title = "TEST1.a", Pictogram =
                    new (){
                        FillColor = "#FFFF00",
                        FillColor2 = "#FF0000",
                        FillPattern = "<pattern id=\"{0}\" patternUnits=\"userSpaceOnUse\" width=\"8\" height=\"8\" patternTransform=\"rotate(45 2 2)\" viewBox=\"0 0 8 8\">\r\n<rect x=\"0\" y=\"0\" width=\"8\" height=\"8\" fill=\"{2}\" />\r\n  <path d=\"M 0,0 l 8,0\" \r\n        style=\"stroke:{1}; stroke-width:2;\" />\r\n</pattern>"
                    }
                },
                new()
                {
                      Title = "TEST1.b", Pictogram =
                    new (){
                        FillColor = "#FFFF00",
                        FillColor2 = "#FF0000",
                        FillPattern = "<pattern id=\"{0}\" patternUnits=\"userSpaceOnUse\" width=\"8\" height=\"8\" patternTransform=\"rotate(-45 2 2)\" viewBox=\"0 0 8 8\">\r\n<rect x=\"0\" y=\"0\" width=\"8\" height=\"8\" fill=\"{2}\" />\r\n  <path d=\"M 0,0 l 8,0\" \r\n        style=\"stroke:{1}; stroke-width:2;\" />\r\n</pattern>"
                    }
                }
            });

            map.AddLegendSection("TEST2", new Legend[]
        {
                new (){
                    Title = "TEST2.a", Pictogram =
                    new (){
                        FillColor = "#FFFF00",
                        FillColor2 = "#FF0FF0",
                        FillPattern = "<pattern id=\"{0}\" patternUnits=\"userSpaceOnUse\" width=\"8\" height=\"8\" patternTransform=\"rotate(45 2 2)\" viewBox=\"0 0 8 8\">\r\n<rect x=\"0\" y=\"0\" width=\"8\" height=\"8\" fill=\"{2}\" />\r\n  <path d=\"M 0,0 l 8,0\" \r\n        style=\"stroke:{1}; stroke-width:2;\" />\r\n</pattern>"
                    }
                },
                new()
                {
                      Title = "TEST2.b", Pictogram =
                    new (){
                        FillColor = "#FFFF00",
                        FillColor2 = "#FF0FFF",
                        FillPattern = "<pattern id=\"{0}\" patternUnits=\"userSpaceOnUse\" width=\"8\" height=\"8\" patternTransform=\"rotate(-45 2 2)\" viewBox=\"0 0 8 8\">\r\n<rect x=\"0\" y=\"0\" width=\"8\" height=\"8\" fill=\"{2}\" />\r\n  <path d=\"M 0,0 l 8,0\" \r\n        style=\"stroke:{1}; stroke-width:2;\" />\r\n</pattern>"
                    }
                }
        });

            map.Center = new() { Latitude = 45, Longitude = 0 };
            map.Zoom = 15;
            await map.RenderAsSvgAsync(@"c:\temp\svgmaprenderer.svg");
        }
    }
}

