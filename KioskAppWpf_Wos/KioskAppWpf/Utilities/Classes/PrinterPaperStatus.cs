using System;

namespace Utilities.Classes
{
    public class PrinterPaperStatus
    {
        public int ReciptLenghtInMilimeters { get; set; }
        public int ReciptPaperLenghtInMilimeters { get; set; }
        public int ReciptPrintedLenghtInMilimeters { get; set; }

        public int NoOfReciptPrinted
        {
            get
            {
                return ReciptPrintedLenghtInMilimeters / ReciptLenghtInMilimeters;
            }
        }
        public int NoOfReciptLeft
        {
            get
            {
                decimal reciptsLeft = (ReciptPaperLenghtInMilimeters - ReciptPrintedLenghtInMilimeters) / ReciptLenghtInMilimeters;
                return Convert.ToInt32(Math.Floor(reciptsLeft));
            }
        }

        public int ReciptPaperLeftInMilimeters
        {
            get
            {
                return ReciptPaperLenghtInMilimeters - ReciptPrintedLenghtInMilimeters;
            }
        }

        public decimal ReciptPaperLeftInMeters
        {
            get
            {
                return ReciptPaperLeftInMilimeters / 1000;
            }
        }

        public decimal ReciptPrintedLenthInMeters
        {
            get
            {
                return ReciptPrintedLenghtInMilimeters / 1000;
            }
        }

        public bool OutOfPaper
        {
            get
            {
                return (ReciptPaperLenghtInMilimeters - ReciptPrintedLenghtInMilimeters) <= ReciptLenghtInMilimeters;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PrinterPaperStatus)
            {
                return ReciptLenghtInMilimeters == ((PrinterPaperStatus)obj).ReciptLenghtInMilimeters 
                    && ReciptPaperLenghtInMilimeters == ((PrinterPaperStatus)obj).ReciptPaperLenghtInMilimeters
                    && ReciptPrintedLenghtInMilimeters == ((PrinterPaperStatus)obj).ReciptPrintedLenghtInMilimeters;
            }
            return false;
        }
    }
}
