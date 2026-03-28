using DbCommunication;
using OpcCommunication;
using System;
using System.ComponentModel;

namespace KioskAppWpf.Classes
{
    public class FlowMeterReadingService
    {
        private int KioskId { get; set; }
        private IDbConnection Connection { get; set; }
        private DbWrite DbWrite { get; set; }
        private DbRead DbRead { get; set; }
        private OpcManager OpcManager { get; set; }
        private DateTime? lastReadDate;
        private System.Timers.Timer checkTimer;
        private static BackgroundWorker worker = new BackgroundWorker();

        public FlowMeterReadingService(int kioskId, IDbConnection connection, DbRead dbRead, DbWrite dbWrite, OpcManager opcManager)
        {
            KioskId = kioskId;
            Connection = connection;
            DbRead = dbRead;
            DbWrite = dbWrite;
            OpcManager = opcManager;

            // Check on startup if today's reading needs to be saved
            CheckAndSaveTodayIfNeeded();

            // Setup timer to check every 15 minutes
            // System.Timers.Timer runs callbacks on thread pool threads, so it won't block the main thread
            checkTimer = new System.Timers.Timer(900000); // Check every 15 minutes (15 * 60 * 1000 ms)
            checkTimer.Elapsed += (sender, e) => CheckAndSaveIfNeeded();
            checkTimer.AutoReset = true;
            checkTimer.Start();
        }

        private void CheckAndSaveTodayIfNeeded()
        {
            try
            {
                DateTime today = DateTime.Today;

                // Only check database if we don't have a cached value
                if (!lastReadDate.HasValue)
                {
                    // Check if reading already exists for today
                    if (DbRead.FlowMeterReadingExistsForDate(today))
                    {
                        lastReadDate = today;
                        return;
                    }
                }
                else if (lastReadDate.Value.Date == today)
                {
                    // Already read today, no need to check database again
                    return;
                }

                // If we're still on the same day, try to read and save
                if (DateTime.Now.Date == today)
                {
                    TryReadAndSave(today);
                }
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService startup check error: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        private void CheckAndSaveIfNeeded()
        {
            try
            {
                DateTime today = DateTime.Today;

                // Check if we already read today - no database query needed if cached
                if (lastReadDate.HasValue && lastReadDate.Value.Date == today)
                {
                    // Already read today, just reset lastReadDate if it's from a previous day
                    if (lastReadDate.Value.Date < today)
                    {
                        lastReadDate = null;
                    }
                    return;
                }

                // We haven't read today yet - check database and try to read
                // Only check database if we don't have a cached value
                if (!lastReadDate.HasValue || lastReadDate.Value.Date != today)
                {
                    // Check if reading already exists in database
                    if (DbRead.FlowMeterReadingExistsForDate(today))
                    {
                        lastReadDate = today;
                        return;
                    }
                }

                // Try to read and save
                TryReadAndSave(today);
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService timer check error: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        private void TryReadAndSave(DateTime date)
        {
            try
            {
                // Check if OPC simulation is enabled (opcOn is false)
                if (OpcManager == null || !OpcManager.OpcOn)
                {
                    TryReadAndSaveSimulated(date);
                }
                else
                {
                    TryReadAndSaveWithOpc(date);
                }
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService error reading/saving: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        private void TryReadAndSaveWithOpc(DateTime date)
        {
            try
            {
                // Check if OPC client is available and connected
                if (OpcManager.Client == null || !OpcManager.Client.IsConnected)
                {
                    return;
                }

                // Get TotalFlow address from configuration
                var config = OpcManager.Client.Configuration;
                if (string.IsNullOrEmpty(config.VarAddressTotalFlow))
                {
                    SimpleLogger sl = new SimpleLogger();
                    sl.Error("FlowMeterReadingService: VarAddressTotalFlow not configured");
                    return;
                }

                // Read TotalFlow value from OPC
                string totalFlowString = OpcManager.Client.ReadValue(config.VarAddressTotalFlow);

                if (string.IsNullOrEmpty(totalFlowString))
                {
                    SimpleLogger sl = new SimpleLogger();
                    sl.Error("FlowMeterReadingService: Failed to read TotalFlow from OPC");
                    return;
                }

                // Parse the value - handle comma as decimal separator (European format)
                // Replace comma with dot for parsing, then convert to int
                string normalizedValue = totalFlowString.Replace(',', '.');
                if (!decimal.TryParse(normalizedValue, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal totalFlowDecimal))
                {
                    SimpleLogger sl = new SimpleLogger();
                    sl.Error($"FlowMeterReadingService: Failed to parse TotalFlow value: {totalFlowString}");
                    return;
                }

                // Convert decimal to int (rounding to nearest integer)
                int totalFlow = (int)Math.Round(totalFlowDecimal);

                // Save to database
                SaveFlowReading(date, totalFlow);
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService OPC read error: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        private void TryReadAndSaveSimulated(DateTime date)
        {
            try
            {
                // Simulate TotalFlow reading when OPC is disabled
                // Use a simulated value based on the day of year for consistency
                int totalFlow = 1000 + (date.DayOfYear * 100);

                // Save to database
                SaveFlowReading(date, totalFlow);
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService simulation error: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        private void SaveFlowReading(DateTime date, int totalFlow)
        {
            try
            {
                bool saved = DbWrite.SaveFlowMeterReading(date, totalFlow);

                if (saved)
                {
                    lastReadDate = date;
                }
                else
                {
                    SimpleLogger sl = new SimpleLogger();
                    sl.Error($"FlowMeterReadingService: Failed to save flow reading for {date:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                SimpleLogger sl = new SimpleLogger();
                sl.Error($"FlowMeterReadingService save error: {ex.Message}");
                sl.Error(ex.StackTrace);
            }
        }

        public void Stop()
        {
            if (checkTimer != null)
            {
                checkTimer.Stop();
                checkTimer.Dispose();
            }
        }
    }
}
