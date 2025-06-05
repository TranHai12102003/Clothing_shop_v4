using System.ComponentModel.DataAnnotations;

namespace Clothing_shop_v2.VModels
{
    public class CheckoutVModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; }

        public bool CreateAccount { get; set; }

        public bool ShipToDifferentAddress { get; set; }

        public string? ShippingFullName { get; set; }
        public string? ShippingEmail { get; set; }
        public string? ShippingPhoneNumber { get; set; }
        public string? ShippingAddress { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShipToDifferentAddress)
            {
                if (string.IsNullOrWhiteSpace(ShippingFullName))
                {
                    yield return new ValidationResult("Vui lòng nhập họ và tên giao hàng.", new[] { nameof(ShippingFullName) });
                }
                if (string.IsNullOrWhiteSpace(ShippingAddress))
                {
                    yield return new ValidationResult("Vui lòng nhập địa chỉ giao hàng.", new[] { nameof(ShippingAddress) });
                }
            }
        }
    }
}

