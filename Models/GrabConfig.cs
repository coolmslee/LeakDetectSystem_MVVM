namespace LeakDetectSystem_MVVM.Models
{
    public class GrabConfig
    {
        public int Interval { get; set; } = 100;
        public bool ImageSave { get; set; } = true;
        public string ImageExtension { get; set; } = "BMP";
        public int HddSettingSpace { get; set; } = 10;
    }
}
