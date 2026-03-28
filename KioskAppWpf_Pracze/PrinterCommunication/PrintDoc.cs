using System.Collections;
using System.Collections.Generic;

namespace PrinterCommunication
{
    public class PrintDoc
    {
        public Queue<PrintLine> Lines { get; set; }

        public int Offset { get; set; }

        public int TopMarginSize { get; set; }
        public int BottomMarginSize { get; set; }
        public int TotalPageHeight
        {
            get
            {
                return PageHeight + TopMarginSize + BottomMarginSize;
            }
        }
        public int PageHeight { get; set; }
        public int TotalHeight { get; set; }
    }
}
