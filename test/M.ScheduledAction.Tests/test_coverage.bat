@echo off

SET dotnet="C:/Program Files/dotnet/dotnet.exe"  
SET opencover=%USERPROFILE%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe  
SET reportgenerator=%USERPROFILE%\.nuget\packages\ReportGenerator\2.5.6\tools\ReportGenerator.exe

SET targetargs="test -f netcoreapp1.1"  
SET filter="+[*]M.ScheduledAction.* -[*.Tests]* -[xunit.*]* -[FakeItEasy*]*"  
SET coveragefile=Coverage.xml  
SET coveragedir=Coverage

REM Run code coverage analysis  
%opencover% -oldStyle -register:user -target:%dotnet% -output:%coveragefile% -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All

REM Generate the report  
%reportgenerator% -targetdir:%coveragedir% -reporttypes:Html;Badges -reports:%coveragefile% -verbosity:Error

REM Open the report  
start "report" "%coveragedir%\index.htm"  