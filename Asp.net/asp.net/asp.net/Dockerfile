FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY asp.net/asp.net.csproj asp.net/
RUN dotnet restore asp.net/asp.net.csproj
COPY asp.net/ asp.net/
RUN dotnet publish asp.net/asp.net.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/Storage/Resumes
EXPOSE 8080
ENTRYPOINT ["dotnet", "SkillMatch.API.dll"]
