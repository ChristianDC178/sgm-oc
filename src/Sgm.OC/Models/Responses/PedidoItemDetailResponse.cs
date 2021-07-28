namespace Sgm.OC.Models.Responses
{
    public class PedidoItemDetailResponse : PedidoItemResponse
    {

        public int PedidoId { get; set; }
        public string Sucursal { get; set; }
        public string Creacion { get; set; }
        public string Usuario { get; set; }
        public bool Recurrente { get; set; }
    }
}
