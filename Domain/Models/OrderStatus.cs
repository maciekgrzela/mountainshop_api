using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    [NotMapped]
    public static class OrderStatus
    {
        public static string Created => "Created";
        public static string Paid => "Paid";
    }
}