using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class VariantMapping
    {
        public static VariantGetVModel EntityGetVModel(Variant variant)
        {
            var vModel = new VariantGetVModel
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                SizeId = variant.SizeId,
                ColorId = variant.ColorId,
                Price = variant.Price,
                SalePrice = variant.SalePrice,
                QuantityInStock = variant.QuantityInStock,
                CreatedDate = variant.CreatedDate,
                UpdatedDate = variant.UpdatedDate
            };
            if(variant.Size != null)
            {
                vModel.Size = new IdNameVModel
                {
                    Id = variant.Size.Id,
                    Name = variant.Size.SizeName
                };
            }
            if (variant.Color != null)
            {
                vModel.Color = new IdNameVModel
                {
                    Id = variant.Color.Id,
                    Name = variant.Color.ColorName
                };
            }
            if (variant.Product != null)
            {
                vModel.Product = ProductMapping.EntityToVModel(variant.Product);
            }
            return vModel;
        }
        public static Variant VModelToEntity(VariantCreateVModel variantVModel)
        {
            return new Variant
            {
                ProductId = variantVModel.ProductId,
                SizeId = variantVModel.SizeId,
                ColorId = variantVModel.ColorId,
                Price = variantVModel.Price,
                SalePrice = variantVModel.SalePrice,
                QuantityInStock = variantVModel.QuantityInStock,
                CreatedDate = DateTime.Now
            };
        }
        public static Variant VModelToEntity(VariantUpdateVModel variantVModel, Variant existingVariant)
        {
            if (existingVariant == null)
            {
                throw new ArgumentNullException(nameof(existingVariant));
            }
            existingVariant.ProductId = variantVModel.ProductId;
            existingVariant.SizeId = variantVModel.SizeId;
            existingVariant.ColorId = variantVModel.ColorId;
            existingVariant.Price = variantVModel.Price;
            existingVariant.SalePrice = variantVModel.SalePrice;
            existingVariant.QuantityInStock = variantVModel.QuantityInStock;
            existingVariant.UpdatedDate = DateTime.Now;
            return existingVariant;
        }
    }
}
