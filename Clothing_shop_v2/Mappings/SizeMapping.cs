using Clothing_shop_v2.Common.Contansts;
using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class SizeMapping
    {
        public static SizeGetVModel EntityToVModel(Size size)
        {
            var model = new SizeGetVModel
            {
                Id = size.Id,
                SizeName = size.SizeName,
                CreatedDate = size.CreatedDate,
                CreatedBy = size.CreatedBy,
                UpdatedDate = size.UpdatedDate,
                UpdatedBy = size.UpdatedBy,
                IsActive = size.IsActive,
            };
            if(size.Variants != null)
            {
                model.Variants = size.Variants.Select(v => new IdNameVModel
                {
                    Id = v.Id,
                    Name = v.Size.SizeName,
                }).ToList();
            }
            return model;
        }
        public static Size VModelToEntity(SizeCreateVModel sizeVModel)
        {
            return new Size
            {
                SizeName = sizeVModel.SizeName,
                //CreatedDate = DateTime.Now,
            };
        }
        public static Size VModelToEntity(SizeUpdateVModel sizeVModel, Size existingSize)
        {
            if (existingSize == null)
            {
                throw new ArgumentNullException(nameof(existingSize));
            }

            existingSize.SizeName = sizeVModel.SizeName;
            //existingSize.UpdatedDate = DateTime.Now;

            return existingSize;
        }
    }
}
