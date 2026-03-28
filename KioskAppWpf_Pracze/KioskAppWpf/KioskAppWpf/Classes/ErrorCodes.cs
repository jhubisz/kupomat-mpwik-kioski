using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskAppWpf.Classes
{
    public static class ErrorCodes
    {
        public static string _itemLifeBitLabel = "Kod błędu err001: Brak komunikacji ze sterownikiem PLC";
        public static string _item230PowerLabel = "Kod błędu err002: Awaria zasilania 230VAC - Kiosk";
        public static string _item24PowerLabel = "Kod błędu err003: Awaria zasilania 24VDC - Kiosk";
        public static string _itemFlowMeterLabel = "Kod błędu err004: Awaria przepływomierza";
        public static string _itemDumpPathLabel = "Kod błędu err005: Awaria drogi zrzutu";
        public static string _itemScreenLabel = "Kod błędu err006: Awaria sitopiaskownika";
        public static string _itemScreenFullLabel = "Kod błędu err007: Zbiornik sitopiaskownika pełen";
        public static string _itemProbeLabel = "Kod błędu err008: Awaria pobieraka";
        public static string _itemProbeReadyLabel = "Kod błędu err009: Brak gotowości pobieraka";
        public static string _itemPhMeterLabel = "Kod błędu err010: Awaria pomiaru pH";
        // public static string _itemCondMeterLabel = "Kod błędu err011: Awaria pomiaru przewodności";
        public static string _itemTempMeterLabel = "Kod błędu err012: Awaria pomiaru temperatury";
        public static string _itemKioskDoorOpenLabel = "Kod błędu err013: Drzwi kiosku otwarte";
        public static string _itemProbeOutOfBottlesLabel = "Kod błędu err014: Zbyt mała liczba butelek w pobieraku";
        public static string _itemValveMeterOkLabel = "Kod błędu err015: Awaria zaworu wody dla pomiarów";
        // public static string _itemValveCarOkLabel = "Kod błędu err016: Awaria zaworu wody dla wozaka";

        public static string _itemProbeDoorOpenLabel = "Kod błędu err017: Drzwi pobieraka otwarte";
        public static string _itemAirOkLabel = "Kod błędu err018: Awaria sprężonego powietrza";
        public static string _itemDistribution230VACOkLabel = "Kod błędu err019: Awaria zasilania 230VAC - Rozdzielnica SZ";
        public static string _itemRack230VACOkLabel = "Kod błędu err020: Awaria zasilania 230VAC - Szafa SA";
        public static string _itemRack24VDCOkLabel = "Kod błędu err021: Awaria zasilania 24VDC - Szafa SA";
        public static string _itemRack24VDCUPSOkLabel = "Kod błędu err022: Awaria zasilacza UPS dla 24VDC - Szafa SA";
        public static string _itemRack12VDCOkLabel = "Kod błędu err023: Awaria zasilania 12VDC - Szafa SA";

        public static string _itemPumpRoomOkLabel = "Kod błędu err024: Awaria Hydroforni";
        public static string _itemPumpRoomWorkOkLabel = "Kod błędu err025: Brak pracy Hydroforni";
        public static string _itemScreenRoomLeakLabel = "Kod błędu err026: Wyciek w pomieszczeniu sita";
        public static string _itemAugerOkLabel = "Kod błędu err027: Awaria przenośnika ślimakowego";
        public static string _itemPreScreenOkLabel = "Kod błędu err028: Awaria urządzenia grzewczego";

        public static string _itemPressureMeterLabel = "Kod błędu err030: Awaria pomiaru ciśnienia";

        public static string _itemSqlErrorLabel = "Kod błędu err101: Awaria servera SQL";
        public static string _itemOpcErrorLabel = "Kod błędu err102: Awaria servera OPC";
        public static string _itemPrinterErrorLabel = "Kod błędu err103: Awaria drukarki";
        public static string _itemPrinterOutOfPaperErrorLabel = "Kod błędu err104: Brak papieru w drukarce";
        public static string _itemKayboardErrorLabel = "Kod błędu err105: Awaria klawiatury";
        public static string _itemRfidErrorLabel = "Kod błędu err106: Awaria czytnika RFID";
    }
}
