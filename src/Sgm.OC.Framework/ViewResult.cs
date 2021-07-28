using System.Collections.Generic;

namespace Sgm.OC.Framework
{
    public class ViewResult<T> where T : class
    {
        public ViewResult() { }
        public ViewResult(List<T> items, int pageCount, int rowCount)
        {
            Items = items;
            PageCount = pageCount;
            RowCount = rowCount;
        }


        public List<T> Items { get;  set; }
        public int RowCount { get;  set; }
        public int PageCount { get;  set; }

        public Validation Validation { get; private set; } = new Validation();
    }
}
