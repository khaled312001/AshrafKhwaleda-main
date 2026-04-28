using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Models
{
    // جدول الألوان
    public class ProductColor
    {
        public int Id { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }

    // جدول الأنواع
    public class ProductType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
