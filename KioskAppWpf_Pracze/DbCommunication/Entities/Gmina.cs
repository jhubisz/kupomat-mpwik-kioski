namespace DbCommunication.Entities
{
    public class Gmina
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameWithoutPl { get; set; }
        public string Wojewodztwo { get; set; }
        public string Powiat { get; set; }

        public override string ToString()
        {
            return Name + " / " + Wojewodztwo;
        }
    }
}
