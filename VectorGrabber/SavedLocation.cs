namespace VectorGrabber
{
    public struct SavedLocation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float heading { get; set; }
        public string title { get; set; }
        
        
        public SavedLocation(float x, float y, float z, float heading, string title)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.heading = heading;
            this.title = title;
        }
        
    }
}