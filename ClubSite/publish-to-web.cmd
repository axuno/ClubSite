@ECHO OFF
dotnet --version
REM Single-file deployments are not supported in IIS. 
REM /p:Platform=AnyCPU | x64 | x86
dotnet publish --no-self-contained -v minimal -c release -f net5.0 -o ..\..\Web\ -p:Platform=x64 -p:EnvironmentName=Production
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\CorFlags.exe" ..\..\Web\ClubSite.dll
REM CPU Architecture           PE      32BITREQ   32BITPREF
REM ------------------------   -----   --------   ---------
REM x86 (32-bit)               PE32           1           0
REM x64 (64-bit)               PE32+          0           0
REM Any CPU                    PE32           0           0
REM Any CPU 32-Bit Preferred   PE32           0           1
copy ..\..\Secrets\* ..\..\Web\Configuration\ /Y
pause

