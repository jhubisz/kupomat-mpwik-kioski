namespace DbCommunication.Entities
{
    public class Miejscowosc
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameWithoutPl { get; set; }
        public string Gmina { get; set; }
        public int GminaId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
