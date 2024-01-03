# Youtube Download Plus
This tool will provide a simple GUI with which to leverage [youtube-dlp](https://github.com/yt-dlp/yt-dlp) to download youtube content, then provide options for formatting and organizing.

This tool is partially in service of the Distributed YouTube Archive project (discord.gg rgBHGm9mTC).

(This project is a re-write of an existing HTA+VBS project in C++.)

# Dependencies
Youtube-Download-Plus assumes you have the following tools installed and referenced in PATH:
* youtube-dlp (or youtube-dl, see setting tab)
* ffmpeg

# Current status
 Version 1.3 includes the following functionality:

* Download a video, playlist, or Channel
* Choose whether to embed subtitles/closed captioning
* Choose whether to download thumbnail
* Choose whether to download description
* Select save path
* Toggle creation of a subfolder based on channel name and ID
* Specify your own archive or batch txt
* Toggle between using youtube-dl and youtube-dlp

The quality is currently unconfigurable and set to max, per the following switches:

```-f bestvideo+bestaudio --youtube-include-dash-manifest```

Output is currently hardcoded to use the mkv container (to best support the various possible encodings).

```--merge-output mkv```

# Screenshots

Version 1.3:

![WPF Abomination](https://raw.githubusercontent.com/Ruthalas/youtube-download-plus/master/Screenshot%20v1.3.PNG)

The HTA version looks like this (to provide a sense of where this is going):

![HTA Abomination](https://i.imgur.com/jl3wzoY.png?raw=true)
