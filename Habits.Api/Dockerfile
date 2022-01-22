FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Habits.Api/Habits.Api.csproj", "Habits.Api/"]
RUN dotnet restore "Habits.Api/Habits.Api.csproj"
COPY . .
WORKDIR "/src/Habits.Api"
RUN dotnet build "Habits.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Habits.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Habits.Api.dll"]
