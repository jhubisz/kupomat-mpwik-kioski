namespace DbCommunication.Entities
{
    public class Ulica
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameWithoutPl { get; set; }
        public string Miejscowosc { get; set; }
        public int MiejscowoscId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
