using Clothing_shop_v2.Models;
using Clothing_shop_v2.VModels;

namespace Clothing_shop_v2.Mappings
{
    public static class ColorMapping
    {
        public static ColorGetVModel EntityToVModel(Color color)
        {
            var model = new ColorGetVModel
            {
                Id = color.Id,
                ColorName = color.ColorName,
                ColorCode = color.ColorCode,
                IsActive = color.IsActive,
                CreatedDate = color.CreatedDate,
                UpdatedDate = color.UpdatedDate,
            };
            if(color.Variants != null)
            {
                model.Variants = color.Variants.Select(v => VariantMapping.EntityGetVModel(v)).ToList();
            }
            return model;
        }
        public static Color VModelToEntity(ColorCreateVModel colorVModel)
        {
            return new Color
            {
                ColorName = colorVModel.ColorName,
                ColorCode = colorVModel.ColorCode,
                IsActive = colorVModel.IsActive,
            };
        }
        public static Color VModelToEntity(ColorUpdateVModel colorVModel, Color existingColor)
        {
            if (existingColor == null)
            {
                throw new ArgumentNullException(nameof(existingColor));
            }
            existingColor.ColorName = colorVModel.ColorName;
            existingColor.ColorCode = colorVModel.ColorCode;
            existingColor.IsActive = colorVModel.IsActive;
            return existingColor;
        }
    }
}
