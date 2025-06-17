using ASTRedux.Structs;
using ASTRedux.Utils;
using ManagedBass;

namespace ASTRedux;

internal static class Processing
{
    public static void ProcessAST(FileInfo input, FileInfo output)
    {
        using BinaryReader reader = new(input.OpenRead());
        using FileStream outStream = output.Create();

        if (!ASTFile.ValidateMagic(reader))
        {
            Console.WriteLine("Header doesn't match AST file!");
            return;
        }

        ASTFile ast = new(reader);

        reader.BaseStream.Position = ast.AudioInfo.StartOffset;
        byte[] outputBuffer = reader.ReadBytes(ast.AudioInfo.Length);

        using WaveFileWriter memWriter = new(outStream, AudioHelpers.WaveFormatFromAudioFormat(ast.AudioInfo.Format));
        memWriter.Write(outputBuffer, outputBuffer.Length);

        Bass.Free();
    }

    public static void ProcessAudio(FileInfo input, FileInfo output)
    {
        using BinaryWriter writer = new(File.OpenWrite(output.FullName));

        int streamHnd = Bass.CreateStream(input.FullName, 0, 0, BassFlags.Decode | BassFlags.Prescan);

        if (streamHnd != 0)
        {
            ChannelInfo ch = Bass.ChannelGetInfo(streamHnd);
            int totalLength = Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) <= int.MaxValue ? (int)Bass.ChannelGetLength(streamHnd, PositionFlags.Bytes) : 0;

            if (totalLength == 0)
            {
                Console.WriteLine("Invalid PCM stream!");
                Bass.Free();
                return;
            }

            byte[] pcmBuffer = new byte[totalLength];
            int bytesRead = Bass.ChannelGetData(streamHnd, pcmBuffer, totalLength);
            if (bytesRead > 0)
            {
                ASTFile ast = new
                (
                    new WaveFormat
                    (
                        ch.Frequency,
                        16,
                        ch.Channels
                    ),
                    totalLength
                );

                // TODO: Wrap in a serialize function
                writer.Write(ast.Header.ToByteArray());
                writer.BaseStream.Position = ast.AudioInfo.StartOffset;
                writer.Write(pcmBuffer);

                //Console.WriteLine($"Audio conversion to .ast finished! Audio duration is {AudioHelpers.GetAudioLength(ast.AudioInfo.Format.SampleRate, ast.AudioInfo.Format.BlockSize, ast.AudioInfo.Length):mm\\:ss\\.ff}.");
            }
            else
            {
                Errors err = Bass.LastError;
                switch(err)
                {
                    // add more as needed/relevant
                    case Errors.FileFormat:
                        Console.WriteLine($"Invalid input extension {input.Extension}. Are you missing a BASS plugin for this format?");
                        break;
                    default:
                        Console.WriteLine($"BASS Error! Code #{err}");
                        break;
                }
                Bass.Free();
                return;
            }
        }

        Bass.Free();
    }
}
