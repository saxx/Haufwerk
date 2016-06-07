CALL dotnet pack ".\src\Haufwerk.Client" --configuration Release
CALL nuget push ".\src\Haufwerk.Client\bin\Release\*.nupkg" %nuget_key% -Source https://www.nuget.org/api/v2/package

exit /b 0