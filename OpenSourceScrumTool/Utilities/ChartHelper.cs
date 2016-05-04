using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.WebSockets;

namespace OpenSourceScrumTool.Utilities
{
    public static class ChartHelper
    {
        /// <summary>
        /// Generate Minimal Data for Chart.js Bar Chart
        /// </summary>
        /// <param name="fillColor">Colour in format "rgba(r,g,b,a)" Colour of Bar</param>
        /// <param name="strokeColor">Colour in format "rgba(r,g,b,a)" Outer Highlight on Graph</param>
        /// <param name="highlightFill">Colour in format "rgba(r,g,b,a)" Colour when hover over</param>
        /// <param name="highlightStroke">Colour in format "rgba(r,g,b,a)" Outer Highlight on hover over</param>
        /// <param name="labels">Labels for X axis</param>
        /// <param name="datalabel">Label for Dataset</param>
        /// <param name="data">Dataset Values</param>
        /// <returns>Preformatted data to pass to Chart.js Chart(context).Bar()</returns>
        public static ChartData BarChart(string fillColor, string strokeColor, string highlightFill, string highlightStroke, List<string> labels, string datalabel, List<int> data)
        {
            List<object> datasetList = new List<object>();
            datasetList.Add(new ChartBarDataSet()
            {
                label = datalabel,
                fillColor = fillColor,
                strokeColor = strokeColor,
                highlightFill = highlightFill,
                highlightStroke = highlightStroke,
                data = data
            });
            return new ChartData
            {
                labels = labels,
                datasets = datasetList
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fillColor">Colour in format "rgba(r,g,b,a)" Colour of Underside of Graph</param>
        /// <param name="strokeColor">Colour in format "rgba(r,g,b,a)" Colour of Line between data</param>
        /// <param name="pointColor">Colour in format "rgba(r,g,b,a)" Inner Colour of Point</param>
        /// <param name="pointStrokeColor">Colour in format "#ffffff" Outer Colour of Point</param>
        /// <param name="pointHighlightFill">Colour in format "#fff" Inner Colour of Point when Selected</param>
        /// <param name="pointHighlightStroke">Colour in format "rgba(r,g,b,a)" Outer Colour of Point When Selected</param>
        /// <param name="labels">Labels for X axis</param>
        /// <param name="datalabel">Label for Dataset</param>
        /// <param name="data">Dataset Values</param>
        /// <returns>Preformatted data to pass to Chart.js Chart(context).Line()</returns>
        public static ChartData LineGraph(string fillColor, string strokeColor, string pointColor, string pointStrokeColor, string pointHighlightFill, string pointHighlightStroke, List<string> labels, string datalabel, List<int> data)
        {
            List<object> datasetList = new List<object>();
            datasetList.Add(new ChartLineDataSet()
            {
                label = datalabel,
                fillColor = fillColor,
                strokeColor = strokeColor,
                pointColor = pointColor,
                pointStrokeColor = pointStrokeColor,
                pointHighlightFill = pointHighlightFill,
                pointHighlightStroke = pointHighlightStroke,
                data=data
            });
            return new ChartData
            {
                labels = labels,
                datasets = datasetList
            };
        }

        private static string rgbaColourFormatter(int r, int g, int b, double a)
        {
            string result = "rgba(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + "," + a.ToString() +
                            ")";
            return result;
        }

        private static string hexColourFormatter(int r, int g, int b)
        {
            string colorvar = ColorTranslator.FromHtml(String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b)).Name.Remove(0,2);
            return "\"#" + colorvar;

        }

        public static string DarkGreen(string format)
        {
            return GenerateColour(0,200,50,1.0, format);
        }

        public static string ClearWhite(string format)
        {
            return GenerateColour(255,255,255,0.1, format);
        }

        public static string ClearBlue(string format)
        {
            return GenerateColour(0,0,255,0.05,format);
        }

        public static string Black(string format)
        {
            return GenerateColour(0,0,0,1,format);
        }

        /// <summary>
        /// Builds String for Chart JS Colours
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="a">Alpha</param>
        /// <param name="format">"rgba" for RGB+A format or "hex" for colour HexCode</param>
        /// <returns>Formatted String for Chart JS</returns>
        public static string GenerateColour(int r, int g, int b, double a, string format)
        {
            switch (format)
            {
                case "rgba":
                    return rgbaColourFormatter(r, g, b, a);
                case "hex":
                    return hexColourFormatter(r, g, b);
            }
            return "";
        }
    }

    public class ChartData
    {
        public List<String> labels { get; set; }
        public List<object> datasets { get; set; }
    }


    public class ChartDataSet
    {
        public string label { get; set; }
        public string fillColor { get; set; }
        public string strokeColor { get; set; }
        public List<int> data { get; set; }

    }

    public class ChartBarDataSet : ChartDataSet
    {
        public string highlightFill { get; set; }
        public string highlightStroke { get; set; }
    }

    public class ChartLineDataSet : ChartDataSet
    {
        public string pointColor { get; set; }
        public string pointStrokeColor { get; set; }
        public string pointHighlightFill { get; set; }
        public string pointHighlightStroke { get; set; }

    }
}