CALL dnu pack ".\src\Haufwerk.Client" --configuration Release
CALL nuget push ".\src\Haufwerk.Client\bin\Release\*.nupkg" %nuget_key%

exit /b 0