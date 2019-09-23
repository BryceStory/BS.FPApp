%~dp0nuget.exe pack %~dp0FiiiPay.Framework.Cache\FiiiPay.Framework.Cache.csproj -OutputDirectory %~dp0tempPackage -Build -Properties Configuration=Release 
%~dp0nuget.exe push %~dp0tempPackage\*.nupkg -source http://nuget.fiiipay.com/nuget 1234567890
rd /s/q %~dp0tempPackage
pause