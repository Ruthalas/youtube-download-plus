# Youtube Download Plus
 This tool will provide a GUI with which to leverage youtube-dl to download youtube content, then provide options for formatting to fit KODI's TV show formatting requirements.

 This project is a re-write of an existing HTA+VBS project in C++.

# Current status
Version 1.0 implements basic functionality:
-Download a video, playlist, or Channel
-Choose whether to embed subtitles/closed captioning
-Choose whether to download thumbnail
-Choose whether to download description
-Select save path

The quality is currently unconfigurable and set to max, per the following switches:
```-f bestvideo+bestaudio --youtube-include-dash-manifest```
Output is currently hardcoded to use the mkv container (to best support the various possible encodings).
```--merge-output mkv```

# Screenshots

Version 1.0:

![WPF Abomination](https://i.imgur.com/XXLI9E9.png?raw=true)

The HTA version looks like this:

![HTA Abomination](https://i.imgur.com/jl3wzoY.png?raw=true)