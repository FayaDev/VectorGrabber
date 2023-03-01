namespace VectorGrabber
{
    public struct SavedLocation
    {
        public float x;
        public float y;
        public float z;
        public float heading;
        public string title;
        
        
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