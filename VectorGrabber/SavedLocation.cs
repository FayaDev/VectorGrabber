namespace VectorGrabber
{
    public struct SavedLocation
    {
        public float X;
        public float Y;
        public float Z;
        public float Heading;
        public string Title;
        
        
        public SavedLocation(float x, float y, float z, float heading, string title)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
            Title = title;
        }
        
    }
}