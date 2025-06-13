# ASTRedux

A project to reverse engineer, document, and create tooling for Capcom's .ast and *.rSoundSnd audio formats used in Dead Rising through binary analysis of the Windows executable and data files found in the game folder.

## Installation

Clone the repository or download the repository as a .zip, open the .sln in Visual Studio, and then build it. Alternatively, run `dotnet build` with your desired settings in the root directory through a terminal, especially for Mac and Linux users. GitHub releases coming at an indeterminate date.

## Usage/Examples

ASTRedux is used through the CLI as such:

`ASTRedux.exe --input INPUT --output OUTPUT`

where INPUT is a relative or absolute path to an *.ast, rSoundSnd (not yet implemented) file or *mp3, wav, ogg, etc to be converted, and OUTPUT is the output path for the processed file. AST inputs will always convert to standard audio inputs and vice versa. If the output file already exists, it will be deleted and overwritten.

for example, `ASTRedux.exe --input song.mp3 --output bgm030.ast`

## Roadmap

- Further mapping and documentation of *.ast

- rSound and subtype support (eg *.rSoundSnd, *.rSoundSeg etc)

## Acknowledgements

This project uses the [NAudio](https://github.com/naudio/NAudio) NuGet package for audio conversion and processing.

Thank you to the developers of Ghidra and HxD for providing excellent tools for analyzing executables and binary formats.

## License

This project is licensed under the [MPL-2.0](https://www.mozilla.org/en-US/MPL/2.0/).
