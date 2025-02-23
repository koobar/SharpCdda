using System;

namespace SharpCdda.Exceptions
{
    public class UnsupportedAudioFormatException : Exception
    {
        public UnsupportedAudioFormatException() 
            : base("This audio data is in an unsupported format. Only sampling frequency 44100Hz, quantization bit depth 16-bit, stereo audio is supported.")
        {
        }
    }
}
