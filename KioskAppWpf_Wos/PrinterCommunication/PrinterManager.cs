using DbCommunication.Entities;
using DbCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace PrinterCommunication
{
    public class PrinterManager
    {
        public Transaction Transaction { get; set; }
        public KioskConfiguration KioskConf { get; set; }
        public SampleTray SampleTray { get; set; }
        public PrintDoc PrintDoc { get; set; }

        public PrintDoc PrintSampleTray(SampleTray tray)
        {
            SampleTray = tray;
            PrintSampleTray();
            return PrintDoc;
        }
        public PrintDoc PrintRecipt(Transaction transaction, KioskConfiguration kioskConf)
        {
            Transaction = transaction;
            KioskConf = kioskConf;
            Print();
            return PrintDoc;
        }

        private void PrintSampleTray()
        {
            PrintDoc = BuildTrayReciptContent();
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += new PrintPageEventHandler(ProvidePrintDocContent);
            doc.EndPrint += new PrintEventHandler((sender, e) =>
            {
                resetEvent.Set();
            });
            doc.Print();
            resetEvent.WaitOne();
        }
        private void Print()
        {
            PrintDoc = BuildReciptContent();
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += new PrintPageEventHandler(ProvidePrintDocContent);
            doc.EndPrint += new PrintEventHandler((sender, e) =>
            {
                resetEvent.Set();
            });
            doc.Print();
            resetEvent.WaitOne();
        }

        private void ProvidePrintDocContent(object sender, PrintPageEventArgs e)
        {
            var sl = new SimpleLoggerPrinter();
            sl.Debug("start drukowania");

            Graphics graphics = e.Graphics;

            while (PrintDoc.Lines.Count > 0 && (PrintDoc.TotalPageHeight < 210 || PrintDoc.Lines.Peek().OffsetY == 0))
            {
                var line = PrintDoc.Lines.Dequeue();

                sl.Debug("linia: " + line.Text);

                if (line.Type == LineType.Text)
                {
                    PrintDoc.Offset += line.OffsetY;
                    PrintDoc.PageHeight += line.HeightInMm;
                    graphics.DrawString(line.Text, line.Font, line.Brush, line.OffsetX, PrintDoc.Offset);
                }
                else
                {
                    PrintDoc.Offset += line.OffsetY;
                    PrintDoc.PageHeight += line.HeightInMm;

                    var dirPath = Assembly.GetExecutingAssembly().Location;
                    dirPath = Path.GetDirectoryName(dirPath);
                    Image img = Image.FromFile(Path.GetFullPath(dirPath + "\\Assets\\logo_print.png"));
                    Point loc = new Point(line.OffsetX, PrintDoc.Offset);
                    graphics.DrawImage(img, line.OffsetX, PrintDoc.Offset, 156, 48);
                }
            }

            PrintDoc.Offset = 0;
            PrintDoc.TotalHeight += PrintDoc.TotalPageHeight;
            PrintDoc.PageHeight = 0;

            if (PrintDoc.Lines.Count > 0)
            {
                e.HasMorePages = true;
                sl.Debug("koniec ze stroną");
            }
            else
            {
                e.HasMorePages = false;
                sl.Debug("koniec bez strony");
            }
        }

        private PrintDoc BuildTrayReciptContent()
        {
            var printDoc = new PrintDoc() { TopMarginSize = 12, BottomMarginSize = 12, Lines = new Queue<PrintLine>() };
            Font font = new Font("Consolas", 8);

            String underLine = "------------------------------------------";

            if (SampleTray.Bottles != null)
            {
                foreach (SampleBottle bottle in SampleTray.Bottles)
                {
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = underLine,
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Miejsce poboru: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = SampleTray.KioskName + ", " + SampleTray.LocationName,
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Tacka nr: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = SampleTray.TrayNo.ToString(),
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Butelka nr: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = bottle.BottleNo + " (" + bottle.BottleId + ")",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Data poboru: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = bottle.ProbeTakenTime.ToString("yyyy-MM-dd HH:mm"),
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Rodzaj ścieku: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = bottle.SampleType,
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Tryb poboru: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = bottle.ProbeType,
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 0,
                           OffsetX = 100,
                       });

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = "Nazwa dostawcy: ",
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                       });
                    #region nazwa klienta
                    var nazwaLines = BuildMultiLine(bottle.CustomerName, 100);
                    if (nazwaLines != null && nazwaLines.Count > 0)
                    {
                        foreach (var line in nazwaLines)
                        {
                            printDoc.Lines.Enqueue(line);
                        }
                    }
                    #endregion

                    #region adresy wytwórców
                    if (bottle.SampleType == "ROD")
                    {
                        var counter = 1;
                        foreach (CustomerAddress addr in bottle.Addresses)
                        {
                            printDoc.Lines.Enqueue(
                                new PrintLine()
                                {
                                    Text = counter.ToString() + ". ",
                                    Brush = new SolidBrush(Color.Black),
                                    Font = new Font("Consolas", 8),
                                    OffsetY = 28,
                                    OffsetX = 10
                                });

                            printDoc.Lines.Enqueue(
                                new PrintLine()
                                {
                                    Text = addr.RodName + ", dz. " + addr.AddressNumber,
                                    Brush = new SolidBrush(Color.Black),
                                    Font = new Font("Consolas", 8),
                                    OffsetY = 0,
                                    OffsetX = 30
                                });

                            if (addr.ContractNo != null && addr.ContractNo != "")
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = "Umowa: " + addr.ContractNo,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = 20,
                                        OffsetX = 30
                                    });
                            }
                            counter++;
                        }
                    }
                    else if (bottle.SampleType == "TOALETY PRZENOŚNE")
                    {
                        var counter = 1;
                        foreach (CustomerAddress addr in bottle.Addresses)
                        {
                            printDoc.Lines.Enqueue(
                                new PrintLine()
                                {
                                    Text = counter.ToString() + ". ",
                                    Brush = new SolidBrush(Color.Black),
                                    Font = new Font("Consolas", 8),
                                    OffsetY = 28,
                                    OffsetX = 10
                                });

                            if (addr.ContractNo != null && addr.ContractNo != "")
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = "Umowa: " + addr.ContractNo,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = 0,
                                        OffsetX = 30
                                    });
                            }
                            counter++;
                        }
                    }
                    else
                    {
                        var counter = 1;
                        foreach (CustomerAddress addr in bottle.Addresses)
                        {
                            printDoc.Lines.Enqueue(
                                new PrintLine()
                                {
                                    Text = counter.ToString() + ". ",
                                    Brush = new SolidBrush(Color.Black),
                                    Font = new Font("Consolas", 8),
                                    OffsetY = 28,
                                    OffsetX = 10
                                });

                            var nextOffset = 0;
                            if (addr.Company != null)
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = addr.Company.Name,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = 0,
                                        OffsetX = 30
                                    });
                                nextOffset = 20;
                            }
                            if (!string.IsNullOrEmpty(addr.UlicaName))
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = addr.UlicaName + " " + addr.AddressNumber,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = nextOffset,
                                        OffsetX = 30
                                    });

                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = addr.MiejscowoscName,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = 20,
                                        OffsetX = 30
                                    });
                            }
                            else
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = addr.MiejscowoscName + " " + addr.AddressNumber,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = nextOffset,
                                        OffsetX = 30
                                    });
                            }
                            printDoc.Lines.Enqueue(
                                new PrintLine()
                                {
                                    Text = "Gmina " + addr.GminaName,
                                    Brush = new SolidBrush(Color.Black),
                                    Font = new Font("Consolas", 8),
                                    OffsetY = 20,
                                    OffsetX = 30
                                });

                            if (addr.ContractNo != null && addr.ContractNo != "")
                            {
                                printDoc.Lines.Enqueue(
                                    new PrintLine()
                                    {
                                        Text = "Umowa: " + addr.ContractNo,
                                        Brush = new SolidBrush(Color.Black),
                                        Font = new Font("Consolas", 8),
                                        OffsetY = 20,
                                        OffsetX = 30
                                    });
                            }
                            counter++;
                        }
                    }
                    #endregion

                    printDoc.Lines.Enqueue(
                       new PrintLine()
                       {
                           Text = underLine,
                           Brush = new SolidBrush(Color.Black),
                           Font = new Font("Consolas", 8),
                           OffsetY = 20,
                           OffsetX = 0,
                       });
                }
            }

            return printDoc;
        }

        private PrintDoc BuildReciptContent()
        {
            var printDoc = new PrintDoc() { TopMarginSize = 12, BottomMarginSize = 12, Lines = new Queue<PrintLine>() };
            Font font = new Font("Consolas", 8);

            String underLine = "------------------------------------------";


            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 6),
                    OffsetY = 20,
                    OffsetX = 110
                });
            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Type = LineType.Image,
                    Text = "\\Assets\\logo.png",
                    OffsetY = 20,
                    OffsetX = 20
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = underLine,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 60,
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Klient: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            #region Adres klienta

            #region nazwa klienta
            var nazwaLines = BuildMultiLine(Transaction.Address.Name, 30);
            if (nazwaLines != null && nazwaLines.Count > 0)
            {
                foreach (var line in nazwaLines)
                {
                    printDoc.Lines.Enqueue(line);
                }
            }
            #endregion

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Address.AddressLine1,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 30
                });

            if (Transaction.Address.AddressLine2 != "")
            {
                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = Transaction.Address.AddressLine2,
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 30
                    });
            }

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Address.PostCode + " " + Transaction.Address.City,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 30
                });
            #endregion

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = underLine,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Pochodzenie nieczystości: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            #region adresy wytwórców
            if (Transaction.Cargo.Id == 3)
            {
                var counter = 1;
                foreach (CustomerAddress addr in Transaction.CustomerAddresses)
                {
                    printDoc.Lines.Enqueue(
                        new PrintLine()
                        {
                            Text = counter.ToString() + ". ",
                            Brush = new SolidBrush(Color.Black),
                            Font = new Font("Consolas", 8),
                            OffsetY = 28,
                            OffsetX = 10
                        });

                    printDoc.Lines.Enqueue(
                        new PrintLine()
                        {
                            Text = addr.RodName + ", dz. " + addr.AddressNumber,
                            Brush = new SolidBrush(Color.Black),
                            Font = new Font("Consolas", 8),
                            OffsetY = 0,
                            OffsetX = 30
                        });

                    if (addr.ContractNo != null && addr.ContractNo != "")
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = "Umowa: " + addr.ContractNo,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = 20,
                                OffsetX = 30
                            });
                    }
                    counter++;
                }
            }
            else if (Transaction.Cargo.Id == 4)
            {
                var counter = 1;
                foreach (CustomerAddress addr in Transaction.CustomerAddresses)
                {
                    printDoc.Lines.Enqueue(
                        new PrintLine()
                        {
                            Text = counter.ToString() + ". ",
                            Brush = new SolidBrush(Color.Black),
                            Font = new Font("Consolas", 8),
                            OffsetY = 28,
                            OffsetX = 10
                        });

                    if (addr.ContractNo != null && addr.ContractNo != "")
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = "Umowa: " + addr.ContractNo,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = 0,
                                OffsetX = 30
                            });
                    }
                    counter++;
                }
            }
            else
            {
                var counter = 1;
                foreach (CustomerAddress addr in Transaction.CustomerAddresses)
                {
                    printDoc.Lines.Enqueue(
                        new PrintLine()
                        {
                            Text = counter.ToString() + ". ",
                            Brush = new SolidBrush(Color.Black),
                            Font = new Font("Consolas", 8),
                            OffsetY = 28,
                            OffsetX = 10
                        });

                    var nextOffset = 0;
                    if (addr.Company != null)
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = addr.Company.Name,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = 0,
                                OffsetX = 30
                            });
                        nextOffset = 20;
                    }
                    if (!string.IsNullOrEmpty(addr.UlicaName))
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = addr.UlicaName + " " + addr.AddressNumber,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = nextOffset,
                                OffsetX = 30
                            });

                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = addr.MiejscowoscName,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = 20,
                                OffsetX = 30
                            });
                    }
                    else
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = addr.MiejscowoscName + " " + addr.AddressNumber,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = nextOffset,
                                OffsetX = 30
                            });
                    }
                    printDoc.Lines.Enqueue(
                        new PrintLine()
                        {
                            Text = "Gmina " + addr.GminaName,
                            Brush = new SolidBrush(Color.Black),
                            Font = new Font("Consolas", 8),
                            OffsetY = 20,
                            OffsetX = 30
                        });

                    if (addr.ContractNo != null && addr.ContractNo != "")
                    {
                        printDoc.Lines.Enqueue(
                            new PrintLine()
                            {
                                Text = "Umowa: " + addr.ContractNo,
                                Brush = new SolidBrush(Color.Black),
                                Font = new Font("Consolas", 8),
                                OffsetY = 20,
                                OffsetX = 30
                            });
                    }
                    counter++;
                }
            }
            #endregion

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = underLine,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });


            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Ilość ścieków (m3): ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });
            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.ActualAmount.ToString("0.00"),
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Rodzaj: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });
            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Cargo.Name,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Próbka: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });
            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.AlarmSample != null || Transaction.ScheduledSample != null ? "Tak" : "Nie",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8, FontStyle.Bold),
                    OffsetY = 0,
                    OffsetX = 130
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "pH [pH]: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            if (Transaction.Parameters != null && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringPh])
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Parameters.FlowphMed.ToString(),
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }
            else
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }

            //printDoc.Lines.Enqueue(
            //    new PrintLine()
            //    {
            //        Text = "Przewodność [mS/cm]: ",
            //        Brush = new SolidBrush(Color.Black),
            //        Font = new Font("Consolas", 8),
            //        OffsetY = 20,
            //        OffsetX = 0
            //    });

            //if (Transaction.Parameters != null && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringConduction])
            //{
            //    printDoc.Lines.Enqueue(
            //    new PrintLine()
            //    {
            //        Text = Transaction.Parameters.FlowCondMed.ToString(),
            //        Brush = new SolidBrush(Color.Black),
            //        Font = new Font("Consolas", 8),
            //        OffsetY = 0,
            //        OffsetX = 130
            //    });
            //}
            //else
            //{
            //    printDoc.Lines.Enqueue(
            //    new PrintLine()
            //    {
            //        Text = "",
            //        Brush = new SolidBrush(Color.Black),
            //        Font = new Font("Consolas", 8),
            //        OffsetY = 0,
            //        OffsetX = 130
            //    });
            //}

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Temperatura [°C]: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            if (Transaction.Parameters != null && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringTemperature])
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Parameters.FlowTempMed.ToString(),
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }
            else
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Chzt [mg/l]: ",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            if (Transaction.Parameters != null && !KioskConf.KioskBlockages[KioskBlockageType.MeasuringChzt])
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = Transaction.Parameters.FlowChztMed.ToString(),
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }
            else
            {
                printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 0,
                    OffsetX = 130
                });
            }

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = underLine,
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8),
                    OffsetY = 20,
                    OffsetX = 0
                });

            #region koniec licencji na stację zlewną
            if (Transaction.License.DaysTillEnd <= 45)
            {
                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = "Decyzja wygaśnie za " + Transaction.License.DaysTillEnd + " dni.",
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });
                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = "Decyzja obowiązuje do: ",
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });
                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = Transaction.License.DataKoniec.ToString("dd.MM.yyyy HH:mm:ss"),
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });
                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = underLine,
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });
            }
            #endregion

            #region wiadomosc dla kiosku
            var wiadomoscLines = BuildKioskMsg();
            if (wiadomoscLines != null && wiadomoscLines.Count > 0)
            {
                foreach (var line in wiadomoscLines)
                {
                    printDoc.Lines.Enqueue(line);
                }

                printDoc.Lines.Enqueue(
                    new PrintLine()
                    {
                        Text = underLine,
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });
            }
            #endregion

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "Potwierdzam odbiór nieczystości ciekłych",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 6),
                    OffsetY = 20,
                    OffsetX = 0
                });

            printDoc.Lines.Enqueue(
                new PrintLine()
                {
                    Text = "MPWiK S.A.",
                    Brush = new SolidBrush(Color.Black),
                    Font = new Font("Consolas", 8, FontStyle.Bold),
                    OffsetY = 20,
                    OffsetX = 76
                });

            return printDoc;
        }

        private List<PrintLine> BuildMultiLine(string text, int offsetX)
        {
            if (text.Length == 0)
                return null;

            var lines = new List<PrintLine>();

            var msgWords = text.Split(' ');

            var line = msgWords[0];

            for (var i = 1; i < msgWords.Length; i++)
            {
                if (line.Length + msgWords[i].Length < (32 - (offsetX / 6)))
                {
                    line += " " + msgWords[i];
                }
                else
                {
                    lines.Add(new PrintLine()
                    {
                        Text = line,
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = offsetX
                    });

                    line = msgWords[i];
                }
            }

            lines.Add(new PrintLine()
            {
                Text = line,
                Brush = new SolidBrush(Color.Black),
                Font = new Font("Consolas", 8),
                OffsetY = 20,
                OffsetX = offsetX
            });

            return lines;
        }

        private List<PrintLine> BuildKioskMsg()
        {
            if (KioskConf.KioskWiadomosc.Length == 0)
                return null;

            var lines = new List<PrintLine>();

            var msgWords = KioskConf.KioskWiadomosc.Split(' ');

            var line = msgWords[0];

            for (var i = 1; i < msgWords.Length; i++)
            {
                if (line.Length + msgWords[i].Length < 32)
                {
                    line += " " + msgWords[i];
                }
                else
                {
                    lines.Add(new PrintLine()
                    {
                        Text = line,
                        Brush = new SolidBrush(Color.Black),
                        Font = new Font("Consolas", 8),
                        OffsetY = 20,
                        OffsetX = 0
                    });

                    line = msgWords[i];
                }
            }

            lines.Add(new PrintLine()
            {
                Text = line,
                Brush = new SolidBrush(Color.Black),
                Font = new Font("Consolas", 8),
                OffsetY = 20,
                OffsetX = 0
            });

            return lines;
        }
    }
}
