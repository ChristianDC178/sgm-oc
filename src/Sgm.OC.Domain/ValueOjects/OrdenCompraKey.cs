namespace Sgm.OC.Domain.ValueOjects
{
    /// <summary>
    /// Representa la key de una orden de compra en SGM
    /// </summary>
    public class OrdenCompraKey
    {
        public int? Codigo { get; set; }
        public int? Prefijo { get; set; }
        public int? Sufijo { get; set; }

        public string NroOC
        {
            get
            {

                if (Codigo.HasValue && Prefijo.HasValue && Sufijo.HasValue)
                {
                    return $"{Codigo.Value}-{Prefijo.Value}-{Sufijo.Value}";
                }

                return string.Empty;
            }
        }

    }
}
