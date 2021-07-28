namespace Sgm.OC.Domain
{
    public class ConsolidacionPendiente
    {
        public int RubroIdInterno { get; private set; }
        public int ProductoId { get; private set; }
        public int PedidoItemId { get; private set; }
        public int Cantidad { get; private set; }
        public bool Recurrente { get; private set; }

        public override string ToString()
        {
            return $"{RubroIdInterno} {ProductoId} {PedidoItemId} {Cantidad} {Recurrente}";
        }
    }
}
