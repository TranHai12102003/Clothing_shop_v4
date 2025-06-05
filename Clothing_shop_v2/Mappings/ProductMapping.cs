using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;
using Shopapp.Mappings;

namespace Clothing_shop_v2.Mappings
{
    public static class ProductMapping
    {
        public static ProductGetVModel EntityToVModel(Product product)
        {
            var vmodel = new ProductGetVModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
            };
            if (product.Category != null)
            {
                vmodel.Category = CategoryMapping.EntityToVModel(product.Category);
            }
            if (product.ProductImages != null && product.ProductImages.Count > 0)
            {
                vmodel.PrimaryImageUrl = product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl
                         ?? product.ProductImages?.FirstOrDefault()?.ImageUrl;//Tìm ảnh chính nếu không có sẽ lấy ảnh đầu tiên tìm thấy
            }
            if (product.Variants != null && product.Variants.Count > 0)
            {
                vmodel.Price = product.Variants
                    .Where(v => v.Price > 0)
                    .Min(v => v.Price);
            }
            else
            {
                vmodel.Price = 0;
            }
            return vmodel;
        }
        public static Product VModelToEntity(ProductCreateVModel productVModel)
        {
            return new Product
            {
                ProductName = productVModel.ProductName,
                Description = productVModel.Description,
                CategoryId = productVModel.CategoryId,
                //CreatedDate = DateTime.Now,
            };
        }
        public static Product VModelToEntity(ProductUpdateVModel productVModel, Product existingProduct)
        {
            if (existingProduct == null)
            {
                throw new ArgumentNullException(nameof(existingProduct));
            }
            existingProduct.ProductName = productVModel.ProductName;
            existingProduct.Description = productVModel.Description;
            existingProduct.CategoryId = productVModel.CategoryId;
            //existingProduct.UpdatedDate = DateTime.Now;
            return existingProduct;
        }
        public static ProductDetailVModel EntityToDetailVModel(Product product)
        {
            var vmodel = new ProductDetailVModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
            };
            if (product.Category != null)
            {
                vmodel.Category = CategoryMapping.EntityToVModel(product.Category);
            }
            // Map ProductImages
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                vmodel.ProductImages = product.ProductImages.Select(pi => new ProductImageGetVModel
                {
                    Id = pi.Id,
                    ImageUrl = pi.ImageUrl,
                    IsPrimary = pi.IsPrimary
                }).ToList();

                vmodel.PrimaryImageUrl = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl
                    ?? product.ProductImages.FirstOrDefault()?.ImageUrl;
            }
            // Map Variants and Price
            if (product.Variants != null && product.Variants.Any())
            {
                vmodel.Variants = product.Variants
                    .Where(v => v.Size != null && v.Color != null)
                    .Select(v => new VariantGetVModel
                    {
                        Id = v.Id,
                        Price = v.Price,
                        Size = new IdNameVModel
                        {
                            Id = v.Size.Id,
                            Name = v.Size.SizeName
                        },
                        Color = new IdNameVModel
                        {
                            Id = v.Color.Id,
                            Name = v.Color.ColorName
                        }
                    }).ToList();

                vmodel.Price = product.Variants
                    .Where(v => v.Price > 0)
                    .Select(v => v.Price)
                    .DefaultIfEmpty(0)
                    .Min();
            }
            else
            {
                vmodel.Price = 0;
            }
            return vmodel;
        }
    }
}
