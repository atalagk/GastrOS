@echo off

for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set datestamp=%%c-%%b-%%a)

set VS=C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE
set Proj=Wrapper
set OutName=GastrOS_%datestamp%
set DeployDir=..\dist
set Knowl=..\..\Knowledge
set Outzip=%OutName%.zip
set ProjDeployDir=%DeployDir%\%OutName%

"%VS%\devenv" GastrOS.sln /Rebuild "Release|x86"
if not exist %DeployDir% mkdir %DeployDir%
if exist %ProjDeployDir% rd /s /q %ProjDeployDir%
mkdir %ProjDeployDir%
mkdir %ProjDeployDir%\data
copy %Proj%\bin\Release\GastrOS.mdb %ProjDeployDir%\data
copy %Proj%\bin\Release\gastros.db %ProjDeployDir%\data
robocopy /E %Proj%\bin\Release %ProjDeployDir% /XF *vshost* *.pdb *.mdb *.db *.log *.log.* Microsoft.*.xml log4net.xml NHibernate.xml *.1
robocopy /E %Knowl% %ProjDeployDir%\Knowledge /XD .svn
copy /Y %Proj%\configs\app.config.1 %ProjDeployDir%\GastrOs.Wrapper.exe.config
rem pushd %DeployDir%
rem if exist %Outzip% del %Outzip%
rem 7z a -tzip -r %Outzip% %OutName%
rem popd
rem rd /s /q %ProjDeployDir%