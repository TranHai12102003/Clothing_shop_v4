using Clothing_shop_v2.Common.Models;
using Clothing_shop_v2.Mappings;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.Services.ISerivce;
using Clothing_shop_v2.VModels;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Services
{
    public class CartService : ICartService
    {
        private readonly ClothingShopV3Context _context;
        public CartService(ClothingShopV3Context context)
        {
            _context = context;
        }

        public async Task<ResponseResult> Create(CartCreateVModel cartVModel)
        {
            var response = new ResponseResult();
            try
            {
                var variant = await _context.Variants.FindAsync(cartVModel.VariantId);
                if (variant == null)
                {
                    return new ErrorResponseResult("Biến thể sản phẩm không tồn tại.");
                }

                //Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(x => x.UserId == cartVModel.UserId && x.VariantId == cartVModel.VariantId);
                if (existingCart != null)
                {
                    existingCart.Quantity += cartVModel.Quantity;
                }
                else
                {
                    var newCart = CartMapping.VModelToEntity(cartVModel);
                    _context.Carts.Add(newCart);
                }
                await _context.SaveChangesAsync();
                return new SuccessResponseResult("Thêm sản phẩm vào giỏ hàng thành công.");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> Delete(int id)
        {
            Console.WriteLine("id:" + id);
            var response = new ResponseResult();
            try
            {
                var cart = _context.Carts.FirstOrDefault(x => x.Id == id);
                if (cart == null)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm trong giỏ hàng");
                }
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return new SuccessResponseResult(cart, "Xóa sản phẩm khỏi giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<ResponseResult> DeleteByUserId(int userId)
        {
            var response = new ResponseResult();
            try
            {
                var carts = _context.Carts.Where(x => x.UserId == userId).ToList();
                if (carts == null || carts.Count == 0)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm trong giỏ hàng");
                }
                _context.Carts.RemoveRange(carts);
                _context.SaveChanges();
                return new SuccessResponseResult(carts, "Xóa sản phẩm khỏi giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }   
        }

        public async Task<ResponseResult> DeleteByVariantId(int variantId)
        {
            var response = new ResponseResult();
            try
            {
                var carts = _context.Carts.Where(x => x.VariantId == variantId).ToList();
                if (carts == null || carts.Count == 0)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm trong giỏ hàng");
                }
                _context.Carts.RemoveRange(carts);
                _context.SaveChanges();
                return new SuccessResponseResult(carts, "Xóa sản phẩm khỏi giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }

        public async Task<List<CartGetVModel>> GetAll(int userId)
        {
            var carts = await _context.Carts
                .Include(x => x.Variant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .Include(x => x.Variant)
                .ThenInclude(x => x.Color)
                .Include(x => x.Variant)
                .ThenInclude(x => x.Size)
                .Where(x => x.UserId == userId)
                .Select(x => CartMapping.EntityToGetVModel(x))
                .ToListAsync();
            return carts;
        }

        public async Task<ResponseResult> Update(CartUpdateVModel cartVModel)
        {
            var response = new ResponseResult();
            try
            {
                var cart = _context.Carts.FirstOrDefault(x => x.Id == cartVModel.Id);
                if (cart == null)
                {
                    return new ErrorResponseResult("Không tìm thấy sản phẩm trong giỏ hàng");
                }
                cart.Quantity = cartVModel.Quantity;
                _context.Carts.Update(cart);
                _context.SaveChanges();
                return new SuccessResponseResult(cart, "Cập nhật sản phẩm trong giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return new ErrorResponseResult(ex.Message);
            }
        }
    }
}
