using AsyncBatchProcessor.Interfaces;
using AsyncBatchProcessor.Models;

namespace AsyncBatchProcessor.Handler.ProductValidation
{
    public abstract class ProductValidationHandler : IProductValidationHandler
    {
        private IProductValidationHandler? _next;


        public  void Handle(Product product,int quantity) {

            Validate(product, quantity);
            if(_next is not null)
            {
                 _next.Handle(product, quantity);
            }
        }

        public void SetNextHandler(IProductValidationHandler next)
        {
            _next = next;
        }

        

        protected abstract void Validate(Product product,int quantity);
    }
}
