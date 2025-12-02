namespace eInvoice.domain.Models
{
    public class AdapterResponse<T>
    {
        public T? Value { get; set; }

        public IReadOnlyList<Exception> Errors => _errors.AsReadOnly();
        private List<Exception> _errors = new List<Exception>();

        protected AdapterResponse(T? value, Exception err)
        {
            this.Value = value;
            if (err != null)
                _errors.AddRange(err);
        }

        public static AdapterResponse<T> Success(T value)
        {
            return new AdapterResponse<T>(value, null!);
        }

        public static AdapterResponse<T> Failure(Exception err)
        {
            return new AdapterResponse<T>(default, err);
        }
    }
}
