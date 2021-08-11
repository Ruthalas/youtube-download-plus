# Youtube Download Plus
This tool will provide a simple GUI with which to leverage [youtube-dl](https://youtube-dl.org/) to download youtube content, then provide options for formatting and organizing, such that the results fit KODI's TV show formatting requirements.

This tool is partially in service of the Distributed YouTube Archive project (discord.gg rgBHGm9mTC).

(This project is a re-write of an existing HTA+VBS project in C++.)

# Dependencies
Youtube-Download-Plus assumes you have the following tools installed and referenced in PATH:
* youtube-dl
* ffmpeg

# Current status
 Version 1.1 includes the following functionality:

* Download a video, playlist, or Channel
* Choose whether to embed subtitles/closed captioning
* Choose whether to download thumbnail
* Choose whether to download description
* Select save path
* Toggle creation of a subfolder based on channel name and ID

The quality is currently unconfigurable and set to max, per the following switches:

```-f bestvideo+bestaudio --youtube-include-dash-manifest```

Output is currently hardcoded to use the mkv container (to best support the various possible encodings).

```--merge-output mkv```

# Screenshots

Version 1.1:

![WPF Abomination](https://i.imgur.com/rD1D1Y5.png?raw=true)

The HTA version looks like this (to provide a sense of where this is going):

![HTA Abomination](https://i.imgur.com/jl3wzoY.png?raw=true)
