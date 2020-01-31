mkdir temp
cp pkg/anglesharp.0.12.1.nupkg temp
cd temp
unzip anglesharp.0.12.1.nupkg
mv lib/net46/AngleSharp.dll ..
cd ..
rm -rf temp
mkdir temp
cp pkg/jurassic.3.0.0.nupkg temp
cd temp
unzip jurassic.3.0.0.nupkg
mv lib/net45/Jurassic.dll ..
cd ..
rm -rf temp