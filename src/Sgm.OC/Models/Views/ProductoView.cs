namespace Sgm.OC.Models.Views
{
    public class ProductoView
    {
        public string Descripcion { get; set; }
        public int IdInterno { get; set; }
        public UnidadMedidaView UnidadMedida { get; set; }
        public int? FactorConversion { get; set; }
        public bool Recurrente { get; set; }
    }
}
