# ASTRedux

A project to reverse engineer, document, and create tooling for Capcom's .ast and *.rSoundSnd audio formats used in Dead Rising through binary analysis of the Windows executable and data files found in the game folder.

## Installation

Go to the releases section, go to the latest version, and download the suitable archive for your OS/CPU architecture.

Alternatively to install the latest development version, clone the repository or download the repository as a .zip, open the .sln in Visual Studio or Rider, and then build it. 
You may also run `dotnet build` with your desired settings in the root directory through a terminal.

## Usage/Examples

ASTRedux uses ManagedBass for audio conversion, and supports most BASS format plugins that don't require overly specific functionality (such as CD or MIDI playback)

ASTRedux ships with the BASS library, but BASS plugins such as BASSZXTune for chiptunes/tracker modules or BASSFLAC for FLACs can be added by downloading the corresponding archive for your OS/CPU architecture,
and extracting the .dll (windows), .so (linux), or .dylib (osx) to the root folder of ASTRedux, where the executable and library DLLs reside. Plugins will automatically be loaded and leave a success message in the console.

ASTRedux is used through the CLI as such:

`ASTRedux.exe --input INPUT --output OUTPUT`

where INPUT is a relative or absolute path to an *.ast, rSoundSnd (not yet implemented) file or *mp3, wav, ogg, etc to be converted, and OUTPUT is the output path for the processed file. AST inputs will always convert to standard audio inputs and vice versa. If the output file already exists, it will be deleted and overwritten.

for example, `ASTRedux.exe --input song.mp3 --output bgm030.ast`

## Roadmap

- Further mapping and documentation of *.ast

- rSound and subtype support (eg *.rSoundSnd, *.rSoundSeg etc)

## Acknowledgements

This project uses [ManagedBass](https://github.com/ManagedBass/ManagedBass) for audio conversion and processing.

Thank you to the developers of Ghidra and HxD for providing excellent tools for analyzing executables and binary formats.

## License

This project is licensed under the [MPL-2.0](https://www.mozilla.org/en-US/MPL/2.0/).
