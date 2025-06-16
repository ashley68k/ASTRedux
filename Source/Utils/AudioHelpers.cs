using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTRedux.Utils
{
    public static class AudioHelpers
    {
        /// <summary>
        /// A method which calculates the length (in seconds) of audio given some information and length. Doesn't take a buffer so that it is very generic and usable anywhere you have this information.
        /// </summary>
        /// <param name="sampleRate">Sample rate of the input audio</param>
        /// <param name="sampleSize">How many bytes each sample takes in memory (channels * (bit depth / 8))</param>
        /// <param name="length">The length of the buffer to be tested</param>
        /// <returns>A TimeSpan representing the audio length in seconds</returns>
        public static TimeSpan GetAudioLength(int sampleRate, short sampleSize, int length)
        {
            // length / blockSize = samples in buffer
            // thus, samples over sampleRate, for instance 88200 samples / 44100 rate = 2 when simplified
            return TimeSpan.FromSeconds((length / sampleSize) / sampleRate);
        }
    }
}
