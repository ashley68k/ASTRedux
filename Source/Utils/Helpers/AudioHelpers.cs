using ASTRedux.Data.Format;
using ManagedBass;

namespace ASTRedux.Utils.Helpers;

internal static class AudioHelpers
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
        // length is casted to double, because dividing int by int results in an int, meaning audio length is always floored.
        return TimeSpan.FromSeconds((double)length / sampleSize / sampleRate);
    }

    public static ManagedBass.WaveFormat WaveFormatFromAudioFormat(Data.Format.SampleFormat fmt)
    {
        return new ManagedBass.WaveFormat(
            fmt.SampleRate,
            16,
            fmt.Channels
        );
    }
}
