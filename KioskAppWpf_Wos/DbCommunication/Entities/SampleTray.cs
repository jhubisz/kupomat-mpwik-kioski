using System;
using System.Collections.Generic;

namespace DbCommunication.Entities
{
    public class SampleTray
    {
        public int TrayId { get; set; }
        public int TrayNo { get; set; }
        public DateTime TrayStartDate { get; set; }
        public DateTime TrayEndDate { get; set; }

        public string KioskName { get; set; }
        public string LocationName { get; set; }

        public List<SampleBottle> Bottles { get; set; }

        //        SELECT pt.Id as TackaId, pt.DataStart as TackaStart, pt.DataKoniec as TackaKoniec, typ.Nazwa as TypProby
        //    , pb.Id as PoborButelkaId, pb.NumerButelki as NumerButelki, pb.ProbaData as DataPoboru
        //    , ki.Nazwa as KioskNazwa, l.Nazwa as LokalizacjaNazwa
        //    , t.Id as TransakcjaId, s.Rejestracja as SamochodRejestracja, k.KartaId
        //    , kl.Nazwa as KlientNazwa, kl.AdresLinia1 as KlientAdres1, kl.AdresLinia2 as KlientAdres2, kl.KodPocztowy as KlientKodPocztowy, kl.Miejscowosc as KlientMiejscowosc
    }
}
