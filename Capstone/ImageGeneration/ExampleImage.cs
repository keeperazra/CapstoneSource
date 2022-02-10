
namespace ImageGeneration
{
    public class ExampleImage
    {
        public string filePath;
        
        public ExampleImage(string path = "demo.png")
        {
            filePath = path;
        }

        public void Render()
        {
            int width = 256;
            int height = 256;
            using FileStream pngStream = new(filePath, FileMode.Open, FileAccess.Write);
            _ = width * height;
        }
    }
}