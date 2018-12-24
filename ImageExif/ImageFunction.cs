using SixLabors.ImageSharp;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageExif
{
    public static class ImageFunction
    {
        public static async Task<MemoryStream> FixExif(Stream stream)
        {
            using (Image<Rgba32> image = Image.Load(stream))
            {
                var x = GetOrientationExif(image);
                await RotateImage(x, image);
                if (image.MetaData != null && image.MetaData.ExifProfile != null)
                {
                    try
                    {
                        image.MetaData.ExifProfile.SetValue(ExifTag.Orientation, (UInt32)0);
                    }
                    catch (Exception e)
                    {
                        image.MetaData.ExifProfile.SetValue(ExifTag.Orientation, (UInt16)0);
                    }
                }

                var newStream = new MemoryStream();
                stream.Dispose();
                image.SaveAsJpeg(newStream);
                newStream.Position = 0;
                return newStream;
            }
        }

        public static int GetOrientationExif(Image<Rgba32> image)
        {
            if (image.MetaData != null && image.MetaData.ExifProfile != null)
            {
                var exifValues = image.MetaData.ExifProfile.Values;
                var orientation = exifValues.Where(x => x.Tag == ExifTag.Orientation);
                if (orientation.Count() == 0) return 0;
                else
                {
                    int orVal = Convert.ToInt32(orientation.First().Value.ToString());
                    return orVal;
                }
            }
            else return 0;
        }

        private static async Task RotateImage(int metaRotation, Image<Rgba32> image)
        {
            switch (metaRotation)
            {
                case 2:
                    image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Horizontal));
                    return;
                case 3:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate180));
                    return;
                case 4:
                    image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Vertical));
                    return;
                case 5:
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));
                    return;
                case 6:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate90));
                    return;
                case 7:
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));
                    return;
                case 8:
                    image.Mutate(x => x.Rotate(RotateMode.Rotate270));
                    return;
                default:
                    return;
            }
        }
    }
}
