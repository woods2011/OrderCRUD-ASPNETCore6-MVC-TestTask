FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MvcAppTest/MvcAppTest.csproj", "MvcAppTest/"]
RUN dotnet restore "MvcAppTest/MvcAppTest.csproj"
COPY . .
WORKDIR "/src/MvcAppTest"
RUN dotnet build "MvcAppTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MvcAppTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MvcAppTest.dll"]
