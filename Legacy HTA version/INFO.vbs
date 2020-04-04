' ============================================================
' Name: 		Description Converter (INFO subscript)
' Author: 		Ruthalas
' Date: 		2020-04-04
' Description: 	Uses youtube-dl-generated '*.description'
'				files and the Youtube API to compose a proper
'				KODI (and PLEX?) parse-able NFO	file (as well
'				as fetching a thumbnail).
'				[thumbnail fetch disabled!]
versionNumber = "1.7"
'				Generates a NFO and downloads thumbnail.
'				Restructured tag format from CSV to individual
'				tag pairs.
'				Has switch for alternate season structure
'				restructure for KODI
'				MODIFIED TO TAKE PASSED VARIABLES
'				Adding show nfo files!
' ============================================================

' Incoming variables from command line
Set args = Wscript.Arguments

path = args(0) ' incoming path to examine
youtubeAPIkey = args(1) ' Uses mine, pre-baked into the HTA by default, but users can change it to their own
seasonUsesYear = args(2) ' If True, seasons use year (ex: Season 2012, Season 2013)- else all are considered 'Season 1'
actionToPerform = args(3) ' Shall we add episode info ("ep") or show info ("sh")?

' ============================================================

' Global variables
Const ForReading = 1

If actionToPerform = "ep" Then
	processFolderForEp(path)
End If

If actionToPerform = "sh" Then
	processFolderForShow path,youtubeAPIkey,seasonUsesYear
End If

Function processFolderForShow (path,youtubeAPIkey,sampleID)
	Set objFSO = CreateObject("Scripting.FileSystemObject")
	WScript.Echo "&#x25AA; Fetching details for a sample episode, ID: " & sampleID
	
	currentFileName = "SampleEpisodeInfo"
	' Grab the json file for the sample video ID from the youtube API
	videoJSONURL = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" & sampleID & "&key=" & youtubeAPIkey
	localJSONcache = path & "\" & currentFileName & ".json"
	downloadFile videoJSONURL,currentFileName & ".json",path
	Set objTextFile = objFSO.OpenTextFile(localJSONcache, ForReading)
	episodeJsonText = objTextFile.ReadAll
	objTextFile.Close
	objFSO.DeleteFile localJSONcache
	
	WScript.Echo "&emsp;&#x25AA; Parsing sample episode json for channel ID..."
	' Now that we have the json file for a random episode, let's find the channel ID
	' Break apart that JSON file we just grabbed by quote marks
	splitEpJsonText = Split (episodeJsonText,"""")
	currentIndex = 0
	' Loop through the JSON chunks looking for the bits we are interested in
	For each stringPart in splitEpJsonText
		currentIndex = currentIndex + 1
		' Look for the Channel ID
		If stringPart = "channelId" Then
			discoveredChannelId = splitEpJsonText(currentIndex + 1)
		End If
	Next
	
	WScript.Echo "&emsp;&#x25AA; Channel ID found: " & discoveredChannelId
	WScript.Echo "&#x25AA; Using channel ID to get fetch channel details..."
	' Now that we have the channel ID, lets get the json for that channel
	
	currentFileName = "tvshow"
	' Grab the json file for the channel from the youtube API
	channelInfoURL = "https://www.googleapis.com/youtube/v3/channels?part=snippet&id=" & discoveredChannelId & "&key=" & youtubeAPIkey
	localJSONcache = path & "\" & currentFileName & ".json"
	downloadFile channelInfoURL,currentFileName & ".json",path
	Set objTextFile = objFSO.OpenTextFile(localJSONcache, ForReading)
	channelJsonText = objTextFile.ReadAll
	objTextFile.Close
	objFSO.DeleteFile localJSONcache
	
	WScript.Echo "&emsp;&#x25AA; Channel details downloaded."
	WScript.Echo "&emsp;&#x25AA; Parsing json for channel information..."
	' Now that we have the json file for a random episode, let's find the channel ID
	' Break apart that JSON file we just grabbed by quote marks
	splitShowJsonText = Split (channelJsonText,"""")
	currentIndex = 0
	' Loop through the JSON chunks looking for the bits we are interested in
	For each stringPart in splitShowJsonText
		currentIndex = currentIndex + 1
		' Look for the Channel ID
		If stringPart = "publishedAt" Then
			channelPublishedAt = splitShowJsonText(currentIndex + 1)
			shortChannelStart = Left(channelPublishedAt,10)
			yearChannelStart = left(channelPublishedAt,4)
		End If
		If stringPart = "title" Then
			channelTitle = splitShowJsonText(currentIndex + 1)
		End If
		If stringPart = "description" Then
			channelDescription = splitShowJsonText(currentIndex + 1)
			channelDescription = Replace(channelDescription,"\n",vbcrlf)
			' (If the channel description is longer than 70 characters...)
			' Select the first first 70 char as a shortened description
			If Len(channelDescription) > 70 Then
				shortChannelDescription = Left(channelDescription,70) & "..."
			Else
				shortChannelDescription = channelDescription
			End If
		End If
	Next
	
	WScript.Echo "&emsp;&#x25AA; Channel start date found: " & shortChannelStart
	WScript.Echo "&emsp;&#x25AA; Channel title found: " & channelTitle
	WScript.Echo "&emsp;&#x25AA; Channel description found: " & shortChannelDescription
	
	WScript.Echo "&#x25AA; Generating show NFO file..."
	' Generate a nicely-formatted time code for the NFO header
	timeString = Year(Date) & "-" & Right("00" & Month(Date), 2) & "-" & Right("00" & Day(Date), 2) & " " & FormatDateTime(Now,vbShortTime)
	
	nfoString = "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>"							& vbCrLf &_
	"<!-- created on " & timeString & " - by Conversion Script " & versionNumber & " (Ruthalas) -->" 	& vbCrLf &_
	"<tvshow>"																							& vbCrLf &_
	"  <title>" & channelTitle & "</title>"																& vbCrLf &_
	"  <showtitle>" & channelTitle & "</showtitle>"														& vbCrLf &_
	"  <year>" & yearChannelStart & "</year>"															& vbCrLf &_
	"  <outline>" & shortChannelDescription & "</outline>"												& vbCrLf &_
	"  <plot>" & channelDescription & "</plot>"															& vbCrLf &_
	"  <premiered>" & shortChannelStart & "</premiered>"												& vbCrLf &_
	"  <studio>" & channelTitle & "</studio>"															& vbCrLf &_
	"</tvshow>"
	
	WScript.Echo "&emsp;&#x25AA; Writing show NFO file..."
	' Compose the appropriate path+filename for the new NFO file
	nfoFilePath = path & "\" & currentFileName & ".nfo"
	' Write the NFO file (in UTF-8, per KODI guidelines)
	makeUTF8File nfoString,nfoFilePath
	
	WScript.Echo "&emsp;&#x25AA; Show NFO file created!"
	
End Function
	
Function processFolderForEp (path)
	Set objFSO = CreateObject("Scripting.FileSystemObject")
	
	' Grab the folder and files from Path
	Set objFolder = objFSO.GetFolder(path)
	Set colFiles = objFolder.Files
	
	WScript.Echo " &#x25AA; Processing Folder: " & objFolder.Name
	
	' Look at every file in the provided folder
	For Each objFile in colFiles
	
		' Break each current file into it's file name and extension
		currentFileExt = LCase(objFSO.GetExtensionName(objFile.Name))
		currentFileName = objFSO.GetBaseName(objFile.Name)
		
		' For any description files we find...
		If currentFileExt = "description" Then
			' Determine what an mp4/mkv video file should look like
			mp4FileName = path & "\" & currentFileName & ".mp4"
			mkvFileName = path & "\" & currentFileName & ".mkv"
			webmFileName = path & "\" & currentFileName & ".webm"
			
			nfoFileName = path & "\" & currentFileName & ".nfo"
			
			' If there is an accompanying video file, let's go to town!
			If (objFSO.FileExists(mp4FileName) OR objFSO.FileExists(mkvFileName) OR objFSO.FileExists(webmFileName)) And (NOT objFSO.FileExists(nfoFileName)) Then
				
				WScript.Echo "&emsp;&#x25AA; " & currentFileName
				
				' Start by parsing all the useful info out of the filename
				' Get the date code by grabbing the first characters of the string
				rawDateCode = Left(currentFileName,8)
				dateCode = Left(rawDateCode,4) & "-" & Mid(rawDateCode,5,2) & "-" & Right(rawDateCode,2)
				yearCode = Left(rawDateCode,4)
				monthCode = Mid(rawDateCode,5,2)
				dayCode = Right(rawDateCode,2)
				
				' Get the duration by looking for the end of the "(50s)" notation and looking back for the start 
				secondsIndex = InStr (currentFileName,"s) [")
				indexOffset = countBack (currentFileName,"(",secondsIndex)
				startOfTime = secondsIndex - indexOffset +1
				clipDuration = Clng(Mid(currentFileName,startOfTime,indexOffset-1))
				
				' Invert the filename to make searching back through it simpler
				foundID = False
				For i=1 To Len(currentFileName)
					invertedIndex = Len(currentFileName)-i+1
					currentChar = Mid(currentFileName,invertedIndex,1)
					' Starting from the right, find the first bracket close
					If currentChar = "]" Then
						' Hop back til we find the opening of this bracket pair and dump the contents
						indexOffset = countBack (currentFileName,"[",invertedIndex)
						
						If (foundID = False) Then
							clipID = Mid(currentFileName,invertedIndex - indexOffset +1,indexOffset-1)
							foundID = True
						Else
							'clipRes = Mid(currentFileName,invertedIndex - indexOffset +1,indexOffset-1)
						End If
					End If
				Next
				'WScript.Echo ("ClipID: " & clipID  & vbCrLf & "clipDat: " & dateCode & vbCrLf & "clipDur: " & clipDuration)
				
				' Use the rest of the file name as the video title
				clipTitle = Mid(currentFileName,12,startOfTime-16)
				
				' Pull in contents of the description file generated by youtube-dl
				descriptionFilePath = path & "\" & currentFileName & ".description"
				Set objTextFile = objFSO.OpenTextFile(descriptionFilePath, ForReading)
				Set objFile = objFSO.GetFile(descriptionFilePath)
				
				If objFile.Size <> 0 Then
					descriptionText = objTextFile.ReadAll
					objTextFile.Close
				Else
					descriptionText = "No description available."
				End If
				
				' Let's pull in tags and genre and such from the youtube API
				videoJSONURL = "https://www.googleapis.com/youtube/v3/videos?part=snippet&id=" & clipID & "&key=" & youtubeAPIkey
				localJSONcache = path & "\" & currentFileName & ".json"
				downloadFile videoJSONURL,currentFileName & ".json",path
				Set objTextFile = objFSO.OpenTextFile(localJSONcache, ForReading)
				jsonText = objTextFile.ReadAll
				objTextFile.Close
				objFSO.DeleteFile localJSONcache
				
				' Break apart that JSON file we just grabbed by quote marks
				splitJsonText = Split (jsonText,"""")
				currentIndex = 0
				' Loop through the JSON chunks looking for the bits we are interested in
				For each stringPart in splitJsonText
					currentIndex = currentIndex + 1
					
					' Look for the Channel Title
					If stringPart = "channelTitle" Then
						channelTitle = splitJsonText(currentIndex + 1)
					End If
					
					' When we find the start of the tag cloud...
					If stringPart = "tags" Then
						' Make note of the start of the tag index
						tagStartIndex = currentIndex
						tagEndFound = False
						tempCurrentIndex = currentIndex
						Do While (tagEndFound = False)
							' Loop through the JSON until we find the start of the category /after/ tags, indicating the end of the tags
							tempCurrentIndex = tempCurrentIndex + 1
							If splitJsonText(tempCurrentIndex) = "categoryId" Then
								tagEndFound = True
								' Make note of the index at the end of the tags
								tagEndIndex = tempCurrentIndex
								firstTag = tagStartIndex+1
								lastTag = tagEndIndex-2
								
								' Start composing comma separated list of tags
								tagCloudText = "<tag>" & splitJsonText(firstTag) & "</tag>"
								tagsEnumerated = False
								currentTag = firstTag + 2
								Do While tagsEnumerated = False
									' Loop through the next few indices until we hit the last tag, recording each
									If currentTag >= lastTag Then
										tagCloudText = tagCloudText & "<tag>" & splitJsonText(lastTag) & "</tag>"
										tagsEnumerated = True
									Else
										tagCloudText = tagCloudText & "<tag>" & splitJsonText(currentTag) & "</tag>"
										currentTag = currentTag + 2
									End If
								Loop
							End If
						Loop
					End If
					
					' Look for the Category ID
					If stringPart = "categoryId" Then
						categoryId = CInt(splitJsonText(currentIndex + 1))
					End If
					' Convert category ID into category text (big lookup table in function)
					clipCategory = matchCategory(categoryId)
				Next
				
				' Generate a nicely-formatted time code for the NFO header
				timeString = Year(Date) & "-" & Right("00" & Month(Date), 2) & "-" & Right("00" & Day(Date), 2) & " " & FormatDateTime(Now,vbShortTime)
				timeEpisodeString = monthCode & dayCode
				
				' Check whether the user wanted to use years for seasons, then compose season line
				If seasonUsesYear = "True" Then
					seasonString = "<season>" & yearCode & "</season>"
				Else
					seasonString = "<season>1</season>"
				End If
				
				nfoString = "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>"			& vbCrLf &_
							"<!-- created on " & timeString & " - by Conversion Script " & versionNumber & " (Ruthalas) -->" & vbCrLf &_
							"<episodedetails>"																& vbCrLf &_
							"<title>" & clipTitle & "</title>"										& vbCrLf &_
							"<sorttitle>" & dateCode & " " & clipTitle & "</sorttitle>"				& vbCrLf &_
							seasonString															& vbCrLf &_
							"<episode>" & timeEpisodeString & "</episode>"							& vbCrLf &_
							"<plot>" & descriptionText & "</plot>"									& vbCrLf &_
							"<aired>" & dateCode & "</aired>"										& vbCrLf &_
							"<studio>" & channelTitle & "</studio>"									& vbCrLf &_
							tagCloudText															& vbCrLf &_
							"<year>" & yearCode & "</year>"											& vbCrLf &_
							"<runtime>" & Round(clipDuration/60) & "</runtime>"						& vbCrLf &_
							"<genre>" & clipCategory & "</genre>"									& vbCrLf &_
							"</episodedetails>"
				
				' Compose the appropriate path+filename for the new NFO file
				nfoFilePath = path & "\" & currentFileName & ".nfo"
				' Write the NFO file (in UTF-8, per KODI guidelines)
				makeUTF8File nfoString,nfoFilePath
				
				' Download the thumbnail for this video
				downloadThumbnail clipID,currentFileName,path
				
			End If
        End If
	Next
	
End Function

Function makeUTF8File (stringToWrite,pathToWrite)
	Dim objStream
	Set objStream = CreateObject("ADODB.Stream")
	objStream.CharSet = "utf-8"
	objStream.Open
	objStream.WriteText stringToWrite
	objStream.SaveToFile pathToWrite, 2
End Function

Function matchCategory (categoryID)
	categoryName = ""
	' Source for list: https://gist.github.com/dgp/1b24bf2961521bd75d6c
	Select Case categoryID
		Case 1
			categoryName = "Film & Animation"
		Case 2
			categoryName = "Autos & Vehicles"
		Case 10
			categoryName = "Music"
		Case 15
			categoryName = "Pets & Animals"
		Case 17
			categoryName = "Sports"
		Case 18
			categoryName = "Short Movies"
		Case 19
			categoryName = "Travel & Events"
		Case 20
			categoryName = "Gaming"
		Case 21
			categoryName = "Videoblogging"
		Case 22
			categoryName = "People & Blogs"
		Case 23
			categoryName = "Comedy"
		Case 24
			categoryName = "Entertainment"
		Case 25
			categoryName = "News & Politics"
		Case 26
			categoryName = "Howto & Style"
		Case 27
			categoryName = "Education"
		Case 28
			categoryName = "Science & Technology"
		Case 29
			categoryName = "Nonprofits & Activism"
		Case 30
			categoryName = "Movies"
		Case 31
			categoryName = "Anime/Animation"
		Case 32
			categoryName = "Action/Adventure"
		Case 33
			categoryName = "Classics"
		Case 34
			categoryName = "Comedy"
		Case 35
			categoryName = "Documentary"
		Case 36
			categoryName = "Drama"
		Case 37
			categoryName = "Family"
		Case 38
			categoryName = "Foreign"
		Case 39
			categoryName = "Horror"
		Case 40
			categoryName = "Sci-Fi/Fantasy"
		Case 41
			categoryName = "Thriller"
		Case 42
			categoryName = "Shorts"
		Case 43
			categoryName = "Shows"
		Case 44
			categoryName = "Trailers"
	End Select
	matchCategory = categoryName
End Function

Function countBack (stringToCount,charToFind,rightBound)
	indexOffset = 0
	looking = True
	Do While (looking = True)
		currentChar = Mid(stringToCount,rightBound - indexOffset,1)
		If currentChar = charToFind Then
			looking = False
		Else
			indexOffset = indexOffset + 1
		End If
	Loop
	countBack = indexOffset
End Function

Function downloadThumbnail (videoID,videoFileName,path)
	videoThumbnailURL = "https://img.youtube.com/vi/" & videoID & "/mqdefault.jpg"
	videoThumbnailName = videoFileName & ".tbn"
	
	'downloadFile videoThumbnailURL,videoThumbnailName,path	
End Function

Function downloadFile (fileURL,name,path)
	fullPath = path & "\" & name
	Dim xHttp: Set xHttp = createobject("Microsoft.XMLHTTP")
	Dim bStrm: Set bStrm = createobject("Adodb.Stream")
	xHttp.Open "GET", fileURL, False
	xHttp.Send
	With bStrm
		.type = 1 '//binary
		.open
		.write xHttp.responseBody
		.savetofile fullPath, 2 '//overwrite
	End with
End Function