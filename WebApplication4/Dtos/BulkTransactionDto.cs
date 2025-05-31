using WebApplication4.Dtos.Transactions;

namespace WebApplication4.Dtos
{
    public class BulkTransactionDto
    {
        public List<CreateTransactionDto> Transactions { get; set; } = new();
    }

}