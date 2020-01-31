rm ./Main.exe
rm ./Output.png
mcs src/*.cs /r:Jurassic.dll /r:AngleSharp.dll /r:System.Drawing.dll -out:Main.exe
clear
mono ./Main.exe -t
#feh Output.png