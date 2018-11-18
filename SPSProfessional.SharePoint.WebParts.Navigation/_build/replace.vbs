Const ForReading = 1
Const ForWriting = 2
Const ForAppending = 8

strFileIn = Wscript.Arguments(0)
strFileOut = Wscript.Arguments(1)
strOldText = Wscript.Arguments(2)
strNewText = Wscript.Arguments(3)
strOldText1 = Wscript.Arguments(4)
strNewText1 = Wscript.Arguments(5)

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile(strFileIn, ForReading)

strText = objFile.ReadAll
objFile.Close
strNewText = Replace(strText, strOldText, strNewText)
strNewText = Replace(strNewText, strOldText1, strNewText1)

Set objFile = objFSO.OpenTextFile(strFileOut, ForWriting, True)

objFile.WriteLine strNewText
objFile.Close