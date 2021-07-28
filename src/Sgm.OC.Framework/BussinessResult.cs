namespace Sgm.OC.Framework
{

    public class BusinessResult<T> 
    {

        public BusinessResult() { }

        public BusinessResult(Validation validation)
        {
            Validation = validation;
        }

        public Validation Validation { get; set; } = new Validation();

        public T Data { get; set; }
    
    }

}
