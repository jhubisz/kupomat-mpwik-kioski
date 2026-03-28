using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterCommunication
{
    public enum LineType
    {
        Text,
        Image,
        PageBreak,
    }
    public class PrintLine
    {
        public LineType Type { get; set; }
        public string Text { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public int HeightInMm
        {
            get
            {
                return (int)Math.Ceiling((decimal)OffsetY / 4);
            }
        }

        public Font Font { get; set; }
        public int FontSize { get; set; }
        public Brush Brush { get; set; }
    }
}
